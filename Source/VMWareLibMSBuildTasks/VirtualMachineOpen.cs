using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Open a virtual machine.
    /// </summary>
    public class VirtualMachineOpen : VirtualHostConnect
    {
        private string _filename;

        private int _openTimeout = VMWareInterop.Timeouts.OpenVMTimeout;

        /// <summary>
        /// Virtual machine file.
        /// </summary>
        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
            }
        }

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int OpenVMTimeout
        {
            get
            {
                return _openTimeout;
            }
            set
            {
                _openTimeout = value;
            }
        }

        /// <summary>
        /// Opened virtual machine.
        /// </summary>
        protected VMWareVirtualMachine OpenVirtualMachine(VMWareVirtualHost virtualHost)
        {
            Log.LogMessage(string.Format("Opening {0} ...", _filename));
            return virtualHost.Open(_filename, OpenVMTimeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                using (VMWareVirtualMachine virtualMachine = OpenVirtualMachine(host))
                {
                    // opened virtual machine
                }
            }

            return true;
        }
    }
}
