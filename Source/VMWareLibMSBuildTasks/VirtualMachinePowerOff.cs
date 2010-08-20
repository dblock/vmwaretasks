using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Power off a virtual machine.
    /// </summary>
    public class VirtualMachinePowerOff : VirtualMachineOpen
    {
        private int _powerOffTimeout = VMWareInterop.Timeouts.PowerOffTimeout;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int PowerOffTimeout
        {
            get
            {
                return _powerOffTimeout;
            }
            set
            {
                _powerOffTimeout = value;
            }
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
                    Log.LogMessage(string.Format("Powering off {0}", Filename));
                    virtualMachine.PowerOff(0, PowerOffTimeout);
                }
            }

            return true;
        }
    }
}
