using System;
using System.Collections.Generic;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    /// <summary>
    /// Section to include multiple virtual machines to be tested.
    /// </summary>
    public class VMWareVirtualMachinesConfig : ConfigurationElementCollection
    {
        public VMWareVirtualMachinesConfig()
        {

        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new VMWareVirtualMachineConfig();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((VMWareVirtualMachineConfig)element).File;
        }

        public void Add(VMWareVirtualMachineConfig value)
        {
            BaseAdd(value);
        }

        public VMWareVirtualMachineConfig this[int index]
        {
            get
            {
                return (VMWareVirtualMachineConfig)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }


        public bool HasType(VMWareVirtualMachineType type)
        {
            foreach (VMWareVirtualMachineConfig virtualMachineConfig in this)
            {
                if (virtualMachineConfig.Type == type)
                    return true;
            }

            return false;
        }
    }
}
