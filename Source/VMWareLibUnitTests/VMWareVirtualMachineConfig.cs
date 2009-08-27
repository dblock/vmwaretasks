using System;
using System.Collections.Generic;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    /// <summary>
    /// Types of supported virtual machines
    /// </summary>
    public enum VMWareVirtualMachineType
    {
        Workstation,
        ESX
    };

    /// <summary>
    /// Virtual Machine configuration
    /// </summary>
    public class VMWareVirtualMachineConfig : ConfigurationElement
    {
        private IVMWareTestProvider _provider = null;

        public VMWareVirtualMachineConfig()
        {

        }

        /// <summary>
        /// Virtual machine type
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true, DefaultValue = VMWareVirtualMachineType.Workstation)]
        public VMWareVirtualMachineType Type
        {
            get
            {
                return (VMWareVirtualMachineType)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        /// <summary>
        /// VirtualMachine vmx storage file
        /// </summary>
        [ConfigurationProperty("file", IsRequired = true)]
        public string File
        {
            get
            {
                return (string)this["file"];
            }
            set
            {
                this["file"] = value;
            }
        }

        /// <summary>
        /// Host which the VM lives on
        /// </summary>
        [ConfigurationProperty("host", IsRequired = false)]
        public string Host
        {
            get
            {
                return (string)this["host"];
            }
            set
            {
                this["host"] = value;
            }
        }

        /// <summary>
        /// Username used to login to the VM
        /// </summary>
        [ConfigurationProperty("guestUsername", IsRequired = false)]
        public string GuestUsername
        {
            get
            {
                return (string)this["guestUsername"];
            }
            set
            {
                this["guestUsername"] = value;
            }
        }

        /// <summary>
        /// Password used to login to the VM
        /// </summary>
        [ConfigurationProperty("guestPassword", IsRequired = false)]
        public string GuestPassword
        {
            get
            {
                return (string)this["guestPassword"];
            }
            set
            {
                this["guestPassword"] = value;
            }
        }

        /// <summary>
        /// Username used to login to the VM host.
        /// </summary>
        [ConfigurationProperty("hostUsername", IsRequired = false)]
        public string HostUsername
        {
            get
            {
                return (string)this["hostUsername"];
            }
            set
            {
                this["hostUsername"] = value;
            }
        }

        /// <summary>
        /// Password used to login to the VM host.
        /// </summary>
        [ConfigurationProperty("hostPassword", IsRequired = false)]
        public string HostPassword
        {
            get
            {
                return (string)this["hostPassword"];
            }
            set
            {
                this["hostPassword"] = value;
            }
        }

        /// <summary>
        /// User friendly string to used to title the VM
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        /// <summary>
        /// A delay after power-on to enable services to startup.
        /// </summary>
        [ConfigurationProperty("powerDelay")]
        public int PowerDelay
        {
            get
            {
                return (int)this["powerDelay"];
            }
            set
            {
                this["powerDelay"] = value;
            }
        }

        public IVMWareTestProvider Provider
        {
            get
            {
                lock (this)
                {
                    if (_provider == null)
                    {
                        switch (Type)
                        {
                            case VMWareVirtualMachineType.ESX:
                                _provider = new TestVI(this);
                                break;
                            case VMWareVirtualMachineType.Workstation:
                                _provider = new TestWorkstation(this);
                                break;
                        }
                    }

                    return _provider;
                }
            }
        }
    }
}
