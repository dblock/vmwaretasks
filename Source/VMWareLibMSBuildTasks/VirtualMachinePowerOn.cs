using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Virtual Machine power on.
    /// </summary>
    public class VirtualMachinePowerOn : VirtualMachineOpen
    {
        private int _powerOnTimeout = VMWareInterop.Timeouts.PowerOnTimeout;
        private int _waitForToolsTimeout = VMWareInterop.Timeouts.WaitForToolsTimeout;
        private bool _waitForTools = true;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int PowerOnTimeout
        {
            get
            {
                return _powerOnTimeout;
            }
            set
            {
                _powerOnTimeout = value;
            }
        }

        /// <summary>
        /// Wait for the virtual machine to boot to a ready state.
        /// </summary>
        public bool WaitForTools
        {
            get
            {
                return _waitForTools;
            }
            set
            {
                _waitForTools = value;
            }
        }

        /// <summary>
        /// Timeout to wait for the virtual machine to boot.
        /// </summary>
        public int WaitForToolsTimeout
        {
            get
            {
                return _waitForToolsTimeout;
            }
            set
            {
                _waitForToolsTimeout = value;
            }
        }

        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                using (VMWareVirtualMachine virtualMachine = OpenVirtualMachine(host))
                {
                    Log.LogMessage(string.Format("Powering on {0}", Filename));
                    virtualMachine.PowerOn(PowerOnTimeout);

                    if (WaitForTools)
                    {
                        virtualMachine.WaitForToolsInGuest(WaitForToolsTimeout);
                    }
                }
            }

            return true;
        }
    }
}
