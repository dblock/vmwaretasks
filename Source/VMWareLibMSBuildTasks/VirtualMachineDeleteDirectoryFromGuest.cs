using System;
using System.Collections.Generic;
using System.Text;
using Interop.VixCOM;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Virtual Machine power on.
    /// </summary>
    public class VirtualMachineDeleteDirectoryFromGuest : VirtualMachineLoginGuest
    {
        private int _deleteDirectoryTimeout = VMWareInterop.Timeouts.DeleteDirectoryTimeout;
        private string _guestPathName = null;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int DeleteDirectoryTimeout
        {
            get
            {
                return _deleteDirectoryTimeout;
            }
            set
            {
                _deleteDirectoryTimeout = value;
            }
        }

        /// <summary>
        /// Path on the guest operating system to delete.
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
                    Log.LogMessage(string.Format("Deleting guest directory '{0}'", _guestPathName));
                    virtualMachine.DeleteDirectoryFromGuest(_guestPathName, _deleteDirectoryTimeout);
                }
            }

            return true;
        }
    }
}
