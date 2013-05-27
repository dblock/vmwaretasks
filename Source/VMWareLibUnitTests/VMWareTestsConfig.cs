using System;
using System.Collections.Generic;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    public class VMWareTestsConfig : ConfigurationSection, IDisposable
    {
        public VMWareTestsConfig()
        {

        }

        /// <summary>
        /// Section to include virtual machines to be tested.
        /// </summary>
        [ConfigurationProperty("virtualmachines", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(VMWareVirtualMachinesConfig), AddItemName = "virtualmachine")]
        public VMWareVirtualMachinesConfig VirtualMachines
        {
            get
            {
                return (VMWareVirtualMachinesConfig)this["virtualmachines"];
            }
            set
            {
                this["virtualmachines"] = value;
            }
        }

        [ConfigurationProperty("runWorkstationTests", DefaultValue = true)]
        public bool RunWorkstationTests
        {
            get
            {
                return (bool)this["runWorkstationTests"]
                    && ((VirtualMachines.HasType(VMWareVirtualMachineType.Workstation))
                    || (VirtualMachines.HasType(VMWareVirtualMachineType.WorkstationShared)));
            }
            set
            {
                this["runWorkstationTests"] = value;
            }
        }

        [ConfigurationProperty("runVITests", DefaultValue = true)]
        public bool RunVITests
        {
            get
            {
                return (bool)this["runVITests"] 
                    && VirtualMachines.HasType(VMWareVirtualMachineType.ESX);
            }
            set
            {
                this["runVITests"] = value;
            }
        }

        [ConfigurationProperty("runLongTests", DefaultValue = true)]
        public bool RunLongTests
        {
            get
            {
                return (bool)this["runLongTests"];
            }
            set
            {
                this["runLongTests"] = value;
            }
        }

        [ConfigurationProperty("runPoweredOffTests", DefaultValue = true)]
        public bool RunPoweredOffTests
        {
            get
            {
                return (bool)this["runPoweredOffTests"];
            }
            set
            {
                this["runPoweredOffTests"] = value;
            }
        }

        public void Dispose()
        {
            foreach (VMWareVirtualMachineConfig virtualMachineConfig in VirtualMachines)
            {
                virtualMachineConfig.Dispose();
            }
        }
    }
}
