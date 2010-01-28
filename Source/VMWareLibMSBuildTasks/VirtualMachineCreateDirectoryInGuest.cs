using System;
using System.Collections.Generic;
using System.Text;
using Interop.VixCOM;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Create a directory in the guest operating system.
    /// </summary>
    public class VirtualMachineCreateDirectoryInGuest : VirtualMachineLoginGuest
    {
        private int _createDirectoryTimeout = VMWareInterop.Timeouts.CreateDirectoryTimeout;
        private string _guestPathName = null;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int CreateDirectoryTimeout
        {
            get
            {
                return _createDirectoryTimeout;
            }
            set
            {
                _createDirectoryTimeout = value;
            }
        }

        /// <summary>
        /// Source path on the guest operating system.
        /// </summary>
        public string GuestPathName
        {
            get
            {
                return _guestPathName;
            }
            set
            {
                _guestPathName = value;
            }
        }

        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                using (VMWareVirtualMachine virtualMachine = OpenVirtualMachine(host))
                {
                    LoginGuest(virtualMachine);
                    Log.LogMessage(string.Format("Creating guest directory '{0}'", _guestPathName));
                    virtualMachine.CreateDirectoryInGuest(_guestPathName, _createDirectoryTimeout);
                }
            }

            return true;
        }
    }
}
