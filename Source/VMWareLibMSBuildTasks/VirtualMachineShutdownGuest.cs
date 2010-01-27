using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Shutdown guest operating system.
    /// </summary>
    public class VirtualMachineShutdownGuest : VirtualMachineOpen
    {
        private int _shutdownTimeout = VMWareInterop.Timeouts.PowerOffTimeout;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int ShutdownTimeout
        {
            get
            {
                return _shutdownTimeout;
            }
            set
            {
                _shutdownTimeout = value;
            }
        }

        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                using (VMWareVirtualMachine virtualMachine = OpenVirtualMachine(host))
                {
                    Log.LogMessage(string.Format("Shutting down {0}", Filename));
                    virtualMachine.ShutdownGuest(ShutdownTimeout);
                }
            }

            return true;
        }
    }
}
