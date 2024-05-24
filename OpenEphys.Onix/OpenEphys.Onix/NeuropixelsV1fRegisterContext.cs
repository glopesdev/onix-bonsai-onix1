﻿using System;
using System.Collections;
using System.Linq;
using Bonsai;

namespace OpenEphys.Onix
{
    class NeuropixelsV1fRegisterContext : I2CRegisterContext
    {
        readonly double[] ApGainCorrection = new double[NeuropixelsV1.ChannelCount];
        readonly double[] LfpGainCorrection = new double[NeuropixelsV1.ChannelCount];
        readonly NeuropixelsV1Adc[] Adcs = new NeuropixelsV1Adc[NeuropixelsV1.AdcCount];

        // TODO: Shank configuration consts
        const int ShankConfigurationBitCount = 968;
        const int BaseConfigurationBitCount = 2448;
        const int BaseConfigurationConfigOffset = 576;
        const int NumberOfGains = 8;
        const uint ShiftRegisterSuccess = 1 << 7;
        const int ShankBitExt1 = 965;
        const int ShankBitExt2 = 2;
        const int ShankBitTip1 = 484;
        const int ShankBitTip2 = 483;
        const int InternalReferenceChannel = 191;

        readonly DeviceContext device;
        readonly BitArray ShankConfig = new(ShankConfigurationBitCount, false);
        readonly BitArray[] BaseConfigs = { new BitArray(BaseConfigurationBitCount, false),   // Ch 0, 2, 4, ...
                                            new BitArray(BaseConfigurationBitCount, false) }; // Ch 1, 3, 5, ...

        public NeuropixelsV1fRegisterContext(DeviceContext deviceContext, NeuropixelsV1Gain apGain, NeuropixelsV1Gain lfpGain, NeuropixelsV1Reference refSource, bool apFilter, string gainCalibrationFile, string adcCalibrationFile)
            : base(deviceContext, NeuropixelsV1f.I2cAddress)
        {
            device = deviceContext;

            if (gainCalibrationFile == null || adcCalibrationFile == null)
            {
                throw new ArgumentException("Calibraiton files must be specified.");
            }

            System.IO.StreamReader gainFile = new(gainCalibrationFile);
            var sn = UInt64.Parse(gainFile.ReadLine());

            System.IO.StreamReader adcFile = new(adcCalibrationFile);
            if (sn != UInt64.Parse(adcFile.ReadLine()))
                throw new ArgumentException("Calibraiton file serial numbers do not match.");

            // parse gain correction file
            for (int i = 0; i < NeuropixelsV1.ChannelCount; i++)
            {
                var gainCorrections = gainFile.ReadLine().Split(',').Skip(1);

                if (gainCorrections.Count() != 2 * NumberOfGains)
                    throw new ArgumentException("Incorrectly formmatted gain correction calibration file.");

                ApGainCorrection[i] = double.Parse(gainCorrections.ElementAt(Array.IndexOf(Enum.GetValues(typeof(NeuropixelsV1Gain)), apGain)));
                LfpGainCorrection[i] = double.Parse(gainCorrections.ElementAt(Array.IndexOf(Enum.GetValues(typeof(NeuropixelsV1Gain)), lfpGain) + 8));
            }

            // parse ADC calibration file
            for (var i = 0; i < NeuropixelsV1.AdcCount; i++)
            {
                var adcCal = adcFile.ReadLine().Split(',').Skip(1);
                if (adcCal.Count() != NumberOfGains)
                {
                    throw new ArgumentException("Incorrectly formmatted ADC calibration file.");
                }

                Adcs[i] = new NeuropixelsV1Adc
                {
                    CompP = int.Parse(adcCal.ElementAt(0)),
                    CompN = int.Parse(adcCal.ElementAt(1)),
                    Slope = int.Parse(adcCal.ElementAt(2)),
                    Coarse = int.Parse(adcCal.ElementAt(3)),
                    Fine = int.Parse(adcCal.ElementAt(4)),
                    Cfix = int.Parse(adcCal.ElementAt(5)),
                    Offset = int.Parse(adcCal.ElementAt(6)),
                    Threshold = int.Parse(adcCal.ElementAt(7))
                };
            }

            // create shift-register bit arrays
            for (int i = 0; i < NeuropixelsV1.ChannelCount; i++)
            {
                var configIdx = i % 2;

                // References
                var refIdx = configIdx == 0 ?
                    (382 - i) / 2 * 3 :
                    (383 - i) / 2 * 3;

                BaseConfigs[configIdx][refIdx + 0] = ((byte)refSource >> 0 & 0x1) == 1;
                BaseConfigs[configIdx][refIdx + 1] = ((byte)refSource >> 1 & 0x1) == 1;
                BaseConfigs[configIdx][refIdx + 2] = ((byte)refSource >> 2 & 0x1) == 1;

                var chanOptsIdx = BaseConfigurationConfigOffset + ((i - configIdx) * 4);

                // MSB [Full, standby, LFPGain(3 downto 0), APGain(3 downto0)] LSB

                BaseConfigs[configIdx][chanOptsIdx + 0] = ((byte)apGain >> 0 & 0x1) == 1;
                BaseConfigs[configIdx][chanOptsIdx + 1] = ((byte)apGain >> 1 & 0x1) == 1;
                BaseConfigs[configIdx][chanOptsIdx + 2] = ((byte)apGain >> 2 & 0x1) == 1;

                BaseConfigs[configIdx][chanOptsIdx + 3] = ((byte)lfpGain >> 0 & 0x1) == 1;
                BaseConfigs[configIdx][chanOptsIdx + 4] = ((byte)lfpGain >> 1 & 0x1) == 1;
                BaseConfigs[configIdx][chanOptsIdx + 5] = ((byte)lfpGain >> 2 & 0x1) == 1;

                BaseConfigs[configIdx][chanOptsIdx + 6] = false;
                BaseConfigs[configIdx][chanOptsIdx + 7] = !apFilter; // Full bandwidth = 1, filter on = 0

            }

            int k = 0;
            foreach (var adc in Adcs)
            {
                if (adc.CompP < 0 || adc.CompP > 0x1F)
                {
                    throw new ArgumentOutOfRangeException($"ADC calibration parameter CompP value of {adc.CompP} is invalid.");
                }

                if (adc.CompN < 0 || adc.CompN > 0x1F)
                {
                    throw new ArgumentOutOfRangeException($"ADC calibration parameter CompN value of {adc.CompN} is invalid.");
                }

                if (adc.Cfix < 0 || adc.Cfix > 0xF)
                {
                    throw new ArgumentOutOfRangeException($"ADC calibration parameter Cfix value of {adc.Cfix} is invalid.");
                }

                if (adc.Slope < 0 || adc.Slope > 0x7)
                {
                    throw new ArgumentOutOfRangeException($"ADC calibration parameter Slope value of {adc.Slope} is invalid.");
                }

                if (adc.Coarse < 0 || adc.Coarse > 0x3)
                {
                    throw new ArgumentOutOfRangeException($"ADC calibration parameter Coarse value of {adc.Coarse} is invalid.");
                }

                if (adc.Fine < 0 || adc.Fine > 0x3)
                {
                    throw new ArgumentOutOfRangeException($"ADC calibration parameter Fine value of {adc.Fine} is invalid.");
                }

                var configIdx = k % 2;
                int d = k++ / 2;

                int compOffset = 2406 - 42 * (d / 2) + (d % 2) * 10;
                int slopeOffset = compOffset + 20 + (d % 2);

                var compP = new BitArray(new byte[] { (byte)adc.CompP });
                var compN = new BitArray(new byte[] { (byte)adc.CompN });
                var cfix = new BitArray(new byte[] { (byte)adc.Cfix });
                var slope = new BitArray(new byte[] { (byte)adc.Slope });
                var coarse = (new BitArray(new byte[] { (byte)adc.Coarse }));
                var fine = new BitArray(new byte[] { (byte)adc.Fine });

                BaseConfigs[configIdx][compOffset + 0] = compP[0];
                BaseConfigs[configIdx][compOffset + 1] = compP[1];
                BaseConfigs[configIdx][compOffset + 2] = compP[2];
                BaseConfigs[configIdx][compOffset + 3] = compP[3];
                BaseConfigs[configIdx][compOffset + 4] = compP[4];

                BaseConfigs[configIdx][compOffset + 5] = compN[0];
                BaseConfigs[configIdx][compOffset + 6] = compN[1];
                BaseConfigs[configIdx][compOffset + 7] = compN[2];
                BaseConfigs[configIdx][compOffset + 8] = compN[3];
                BaseConfigs[configIdx][compOffset + 9] = compN[4];

                BaseConfigs[configIdx][slopeOffset + 0] = slope[0];
                BaseConfigs[configIdx][slopeOffset + 1] = slope[1];
                BaseConfigs[configIdx][slopeOffset + 2] = slope[2];

                BaseConfigs[configIdx][slopeOffset + 3] = fine[0];
                BaseConfigs[configIdx][slopeOffset + 4] = fine[1];

                BaseConfigs[configIdx][slopeOffset + 5] = coarse[0];
                BaseConfigs[configIdx][slopeOffset + 6] = coarse[1];

                BaseConfigs[configIdx][slopeOffset + 7] = cfix[0];
                BaseConfigs[configIdx][slopeOffset + 8] = cfix[1];
                BaseConfigs[configIdx][slopeOffset + 9] = cfix[2];
                BaseConfigs[configIdx][slopeOffset + 10] = cfix[3];

            }
        }

        internal void InitializeProbe()
        {
            // turn off calibration mode
            WriteByte(NeuropixelsV1f.CAL_MOD, (uint)NeuropixelsV1CalibrationRegisterValues.CAL_OFF);
            WriteByte(NeuropixelsV1f.SYNC, 0);

            // perform digital and channel reset
            WriteByte(NeuropixelsV1f.REC_MOD, (uint)NeuropixelsV1RecordRegisterValues.DIG_CH_RESET);

            // change operation state to Recording
            WriteByte(NeuropixelsV1f.OP_MODE, (uint)NeuropixelsV1OperationRegisterValues.RECORD);

            // start acquisition
            WriteByte(NeuropixelsV1f.REC_MOD, (uint)NeuropixelsV1RecordRegisterValues.ACTIVE);
        }

        internal void WriteShiftRegisters()
        {
            // TODO: Shank configuration
            // shank
            // NB: no read check because of ASIC bug
            // var shankBytes = BitArrayToBytes(ShankConfig);
            //
            // WriteByte(NeuropixelsV1f.SR_LENGTH1, (uint)shankBytes.Length % 0x100);
            // WriteByte(NeuropixelsV1f.SR_LENGTH2, (uint)shankBytes.Length / 0x100);
            //
            // foreach (var b in shankBytes)
            // {
            //     WriteByte(NeuropixelsV1f.SR_CHAIN1, b);
            // }


            // base
            for (int i = 0; i < BaseConfigs.Length; i++)
            {
                var srAddress = i == 0 ? NeuropixelsV1f.SR_CHAIN2 : NeuropixelsV1f.SR_CHAIN3;

                for (int j = 0; j < 2; j++)
                {
                    var baseBytes = BitArrayToBytes(BaseConfigs[i]);

                    WriteByte(NeuropixelsV1f.SR_LENGTH1, (uint)baseBytes.Length % 0x100);
                    WriteByte(NeuropixelsV1f.SR_LENGTH2, (uint)baseBytes.Length / 0x100);

                    foreach (var b in baseBytes)
                    {
                        WriteByte(srAddress, b);
                    }
                }

                if (ReadByte(NeuropixelsV1f.STATUS) != ShiftRegisterSuccess)
                {
                    throw new WorkflowException($"Shift register {srAddress} status check failed.");
                }
            }

            // Adc correction parameters
            for (uint i = 0; i < Adcs.Length; i+=2)
            {
                device.WriteRegister(NeuropixelsV1f.ADC01_00_OFF_THRESH + i, (uint)(Adcs[i+1].Offset << 26 | Adcs[i+1].Threshold << 16 | Adcs[i].Offset << 10 | Adcs[i].Threshold));
            }
            for (uint i = 0; i < NeuropixelsV1.ChannelCount; i++)
            {
                device.WriteRegister(NeuropixelsV1f.CHAN001_000_LFPGAIN, (uint)(LfpGainCorrection[i] * (1 << 14)));
                device.WriteRegister(NeuropixelsV1f.CHAN001_000_APGAIN, (uint)(ApGainCorrection[i] * (1 << 14)));
            }
        }

        // Bits go into the shift registers MSB first
        // This creates a *bit-reversed* byte array from a bit array
        private static byte[] BitArrayToBytes(BitArray bits)
        {
            if (bits.Length == 0)
            {
                throw new ArgumentException("Shift register data is empty", nameof(bits));
            }

            var bytes = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(bytes, 0);

            for (int i = 0; i < bytes.Length; i++)
            {
                // NB: http://graphics.stanford.edu/~seander/bithacks.html
                bytes[i] = (byte)((bytes[i] * 0x0202020202ul & 0x010884422010ul) % 1023);
            }

            return bytes;
        }
    }
}