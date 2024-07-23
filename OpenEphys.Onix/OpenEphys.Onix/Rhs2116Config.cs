﻿using System.Collections.Generic;

namespace OpenEphys.Onix
{
    public static class Rhs2116Config
    {
        public static readonly IReadOnlyDictionary<Rhs2116AnalogLowCutoff, IReadOnlyList<uint>> AnalogLowCutoffToRegisters =
            new Dictionary<Rhs2116AnalogLowCutoff, IReadOnlyList<uint>>()
        {
            { Rhs2116AnalogLowCutoff.Low1000Hz, new uint[] { 10, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low500Hz, new uint[] { 13, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low300Hz, new uint[] { 15, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low250Hz, new uint[] { 17, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low200Hz, new uint[] { 18, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low150Hz, new uint[] { 21, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low100Hz, new uint[] { 25, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low75Hz, new uint[] { 28, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low50Hz, new uint[] { 34, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low30Hz, new uint[] { 44, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low25Hz, new uint[] { 48, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low20Hz, new uint[] { 54, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low15Hz, new uint[] { 62, 0, 0 } },
            { Rhs2116AnalogLowCutoff.Low10Hz, new uint[] { 5, 1, 0 } },
            { Rhs2116AnalogLowCutoff.Low7500mHz, new uint[] { 18, 1, 0 } },
            { Rhs2116AnalogLowCutoff.Low5000mHz, new uint[] { 40, 1, 0 } },
            { Rhs2116AnalogLowCutoff.Low3090mHz, new uint[] { 20, 2, 0 } },
            { Rhs2116AnalogLowCutoff.Low2500mHz, new uint[] { 42, 2, 0 } },
            { Rhs2116AnalogLowCutoff.Low2000mHz, new uint[] { 8, 3, 0 } },
            { Rhs2116AnalogLowCutoff.Low1500mHz, new uint[] { 9, 4, 0 } },
            { Rhs2116AnalogLowCutoff.Low1000mHz, new uint[] { 44, 6, 0 } },
            { Rhs2116AnalogLowCutoff.Low750mHz, new uint[] { 49, 9, 0 } },
            { Rhs2116AnalogLowCutoff.Low500mHz, new uint[] { 35, 17, 0 } },
            { Rhs2116AnalogLowCutoff.Low300mHz, new uint[] { 1, 40, 0 } },
            { Rhs2116AnalogLowCutoff.Low250mHz, new uint[] { 56, 54, 0 } },
            { Rhs2116AnalogLowCutoff.Low100mHz, new uint[] { 16, 60, 1 } },
        };

        public static readonly IReadOnlyDictionary<Rhs2116AnalogHighCutoff, IReadOnlyList<uint>> AnalogHighCutoffToRegisters =
            new Dictionary<Rhs2116AnalogHighCutoff, IReadOnlyList<uint>>()
        {
            { Rhs2116AnalogHighCutoff.High20000Hz, new uint[] { 8, 0, 4, 0 } },
            { Rhs2116AnalogHighCutoff.High15000Hz, new uint[] { 11, 0, 8, 0 } },
            { Rhs2116AnalogHighCutoff.High10000Hz, new uint[] { 17, 0, 16, 0 } },
            { Rhs2116AnalogHighCutoff.High7500Hz, new uint[] { 22, 0, 23, 0 } },
            { Rhs2116AnalogHighCutoff.High5000Hz, new uint[] { 33, 0, 37, 0 } },
            { Rhs2116AnalogHighCutoff.High3000Hz, new uint[] { 3, 1, 13, 1 } },
            { Rhs2116AnalogHighCutoff.High2500Hz, new uint[] { 13, 1, 25, 1 } },
            { Rhs2116AnalogHighCutoff.High2000Hz, new uint[] { 27, 1, 44, 1 } },
            { Rhs2116AnalogHighCutoff.High1500Hz, new uint[] { 1, 2, 23, 2 } },
            { Rhs2116AnalogHighCutoff.High1000Hz, new uint[] { 46, 2, 30, 3 } },
            { Rhs2116AnalogHighCutoff.High750Hz, new uint[] { 41, 3, 36, 4 } },
            { Rhs2116AnalogHighCutoff.High500Hz, new uint[] { 30, 5, 43, 6 } },
            { Rhs2116AnalogHighCutoff.High300Hz, new uint[] { 6, 9, 2, 11 } },
            { Rhs2116AnalogHighCutoff.High250Hz, new uint[] { 42, 10, 5, 13 } },
            { Rhs2116AnalogHighCutoff.High200Hz, new uint[] { 24, 13, 7, 16 } },
            { Rhs2116AnalogHighCutoff.High150Hz, new uint[] { 44, 17, 8, 21 } },
            { Rhs2116AnalogHighCutoff.High100Hz, new uint[] { 38, 26, 5, 31 } },
        };

        public static readonly IReadOnlyDictionary<Rhs2116AnalogHighCutoff, uint> AnalogHighCutoffToFastSettleSamples =
            new Dictionary<Rhs2116AnalogHighCutoff, uint>()
        {
            { Rhs2116AnalogHighCutoff.High20000Hz, 4 },
            { Rhs2116AnalogHighCutoff.High15000Hz, 5 },
            { Rhs2116AnalogHighCutoff.High10000Hz, 8 },
            { Rhs2116AnalogHighCutoff.High7500Hz, 10 },
            { Rhs2116AnalogHighCutoff.High5000Hz, 15 },
            { Rhs2116AnalogHighCutoff.High3000Hz, 25 },
            { Rhs2116AnalogHighCutoff.High2500Hz, 30 },
            { Rhs2116AnalogHighCutoff.High2000Hz, 30 },
            { Rhs2116AnalogHighCutoff.High1500Hz, 30 },
            { Rhs2116AnalogHighCutoff.High1000Hz, 30 },
            { Rhs2116AnalogHighCutoff.High750Hz, 30 },
            { Rhs2116AnalogHighCutoff.High500Hz, 30 },
            { Rhs2116AnalogHighCutoff.High300Hz, 30 },
            { Rhs2116AnalogHighCutoff.High250Hz, 30 },
            { Rhs2116AnalogHighCutoff.High200Hz, 30 },
            { Rhs2116AnalogHighCutoff.High150Hz, 30 },
            { Rhs2116AnalogHighCutoff.High100Hz, 30 },
        };

        public static readonly IReadOnlyDictionary<Rhs2116StepSize, IReadOnlyList<uint>> StimulatorStepSizeToRegisters =
            new Dictionary<Rhs2116StepSize, IReadOnlyList<uint>>()
        {
            { Rhs2116StepSize.Step10nA, new uint[] { 64, 19, 3 } },
            { Rhs2116StepSize.Step20nA, new uint[] { 40, 40, 1 } },
            { Rhs2116StepSize.Step50nA, new uint[] { 64, 40, 0 } },
            { Rhs2116StepSize.Step100nA, new uint[] { 30, 20, 0 } },
            { Rhs2116StepSize.Step200nA, new uint[] { 25, 10, 0 } },
            { Rhs2116StepSize.Step500nA, new uint[] { 101, 3, 0 } },
            { Rhs2116StepSize.Step1000nA, new uint[] { 98, 1, 0 } },
            { Rhs2116StepSize.Step2000nA, new uint[] { 94, 0, 0 } },
            { Rhs2116StepSize.Step5000nA, new uint[] { 38, 0, 0 } },
            {  Rhs2116StepSize.Step10000nA, new uint[] { 15, 0, 0 } },
        };
    }

    public enum Rhs2116AnalogLowCutoff
    {
        Low1000Hz,
        Low500Hz,
        Low300Hz,
        Low250Hz,
        Low200Hz,
        Low150Hz,
        Low100Hz,
        Low75Hz,
        Low50Hz,
        Low30Hz,
        Low25Hz,
        Low20Hz,
        Low15Hz,
        Low10Hz,
        Low7500mHz,
        Low5000mHz,
        Low3090mHz,
        Low2500mHz,
        Low2000mHz,
        Low1500mHz,
        Low1000mHz,
        Low750mHz,
        Low500mHz,
        Low300mHz,
        Low250mHz,
        Low100mHz,
    }

    public enum Rhs2116AnalogHighCutoff
    {
        High20000Hz,
        High15000Hz,
        High10000Hz,
        High7500Hz,
        High5000Hz,
        High3000Hz,
        High2500Hz,
        High2000Hz,
        High1500Hz,
        High1000Hz,
        High750Hz,
        High500Hz,
        High300Hz,
        High250Hz,
        High200Hz,
        High150Hz,
        High100Hz,
    }

    public enum Rhs2116DspCutoff
    {
        /// <summary>
        /// out = samp[n] - samp[n-1]
        /// </summary>
        Differential = 0,

        /// <summary>
        /// 3309 Hz
        /// </summary>
        Dsp3309Hz,

        /// <summary>
        /// 1370 Hz
        /// </summary>
        Dsp1374Hz,
        
        /// <summary>
        /// 638 Hz
        /// </summary>
        Dsp638Hz,
        
        /// <summary>
        /// 308 Hz
        /// </summary>
        Dsp308Hz,
        
        /// <summary>
        /// 152 Hz
        /// </summary>
        Dsp152Hz,
        
        /// <summary>
        /// 75.2 Hz
        /// </summary>
        Dsp75Hz,
        
        /// <summary>
        /// 37.4 Hz
        /// </summary>
        Dsp37Hz,
        
        /// <summary>
        /// 18.7 Hz
        /// </summary>
        Dsp19Hz,
        
        /// <summary>
        /// 9.34 Hz
        /// </summary>
        Dsp9336mHz,
        
        /// <summary>
        /// 4.67 Hz
        /// </summary>
        Dsp4665mHz,
        
        /// <summary>
        /// 2.33 Hz
        /// </summary>
        Dsp2332mHz,
        
        /// <summary>
        /// 1.17 Hz
        /// </summary>
        Dsp1166mHz,
        
        /// <summary>
        /// 0.583 Hz
        /// </summary>
        Dsp583mHz,
        
        /// <summary>
        /// 0.291 Hz
        /// </summary>
        Dsp291mHz,
        
        /// <summary>
        /// 0.146 Hz
        /// </summary>
        Dsp146mHz,

        /// <summary>
        /// 
        /// </summary>
        Off
    }

    public enum Rhs2116StepSize
    {
        Step10nA,
        Step20nA,
        Step50nA,
        Step100nA,
        Step200nA,
        Step500nA,
        Step1000nA,
        Step2000nA,
        Step5000nA,
        Step10000nA
    }
}