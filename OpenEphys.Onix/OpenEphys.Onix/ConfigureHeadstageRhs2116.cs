﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using Bonsai;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace OpenEphys.Onix
{
    [Editor("OpenEphys.Onix.Design.HeadstageRhs2116Editor, OpenEphys.Onix.Design", typeof(ComponentEditor))]
    public class ConfigureHeadstageRhs2116 : HubDeviceFactory
    {
        PortName port;
        Rhs2116ProbeGroup probeGroup = new();
        readonly ConfigureHeadstageRhs2116LinkController LinkController = new();

        public ConfigureHeadstageRhs2116()
        {
            // TODO: The issue with this headstage is that its locking voltage is far, far lower than the voltage required for full
            // functionality. Locking occurs at around 2V on the headstage (enough to turn 1.8V on). Full functionality is at 5.0 volts.
            // Whats worse: the port voltage can only go down to 3.3V, which means that its very hard to find the true lowest voltage
            // for a lock and then add a large offset to that.
            Port = PortName.PortA;
            ChannelConfiguration = probeGroup;
            LinkController.HubConfiguration = HubConfiguration.Standard;
        }

        [Category(ConfigurationCategory)]
        [TypeConverter(typeof(HubDeviceConverter))]
        public ConfigureRhs2116 Rhs2116A { get; set; } = new();

        [Category(ConfigurationCategory)]
        [TypeConverter(typeof(HubDeviceConverter))]
        public ConfigureRhs2116 Rhs2116B { get; set; } = new();

        [Category(ConfigurationCategory)]
        [TypeConverter(typeof(HubDeviceConverter))]
        public ConfigureRhs2116Trigger StimulusTrigger { get; set; } = new();

        internal override void UpdateDeviceNames()
        {
            LinkController.DeviceName = GetFullDeviceName(nameof(LinkController));
            Rhs2116A.DeviceName = GetFullDeviceName(nameof(Rhs2116A));
            Rhs2116B.DeviceName = GetFullDeviceName(nameof(Rhs2116B));
            StimulusTrigger.DeviceName = GetFullDeviceName(nameof(StimulusTrigger));
        }

        public PortName Port
        {
            get { return port; }
            set
            {
                port = value;
                var offset = (uint)port << 8;
                LinkController.DeviceAddress = (uint)port;
                Rhs2116A.DeviceAddress = offset + 0;
                Rhs2116B.DeviceAddress = offset + 1;
                StimulusTrigger.DeviceAddress = offset + 2;
            }
        }

        [XmlIgnore]
        [Category(ConfigurationCategory)]
        [Description("Defines the physical channel configuration")]
        public Rhs2116ProbeGroup ChannelConfiguration
        {
            get { return probeGroup; }
            set { probeGroup = value; }
        }

        [Browsable(false)]
        [Externalizable(false)]
        [XmlElement(nameof(ChannelConfiguration))]
        public string ChannelConfigurationString
        {
            get
            {
                var jsonString = JsonConvert.SerializeObject(ChannelConfiguration);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
            }
            set
            {
                var jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(value));
                ChannelConfiguration = JsonConvert.DeserializeObject<Rhs2116ProbeGroup>(jsonString);
            }
        }

        [Description("If defined, it will override automated voltage discovery and apply the specified voltage" +
                     "to the headstage. Warning: this device requires 3.4V to 4.4V for proper operation." +
                     "Supplying higher voltages may result in damage to the headstage.")]
        public double? PortVoltage
        {
            get => LinkController.PortVoltage;
            set => LinkController.PortVoltage = value;
        }

        internal override IEnumerable<IDeviceConfiguration> GetDevices()
        {
            yield return LinkController;
            yield return Rhs2116A;
            yield return Rhs2116B;
            yield return StimulusTrigger;
        }

        class ConfigureHeadstageRhs2116LinkController : ConfigureFmcLinkController
        {
            protected override bool ConfigurePortVoltage(DeviceContext device)
            {
                const double MinVoltage = 3.3;
                const double MaxVoltage = 4.4;
                const double VoltageOffset = 2.0;
                const double VoltageIncrement = 0.2;

                for (var voltage = MinVoltage; voltage <= MaxVoltage; voltage += VoltageIncrement)
                {
                    SetPortVoltage(device, voltage);
                    if (base.CheckLinkState(device))
                    {
                        SetPortVoltage(device, voltage + VoltageOffset);
                        return CheckLinkState(device);
                    }
                }

                return false;
            }

            private void SetPortVoltage(DeviceContext device, double voltage)
            {
                device.WriteRegister(FmcLinkController.PORTVOLTAGE, 0);
                Thread.Sleep(500);
                device.WriteRegister(FmcLinkController.PORTVOLTAGE, (uint)(10 * voltage));
                Thread.Sleep(500);
            }

            protected override bool CheckLinkState(DeviceContext device)
            {
                // NB: The RHS2116 headstage needs an additional reset after power on to provide its device table.
                device.Context.Reset();
                var linkState = device.ReadRegister(FmcLinkController.LINKSTATE);
                return (linkState & FmcLinkController.LINKSTATE_SL) != 0;
            }
        }
    }
}