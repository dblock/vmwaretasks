using System;
using System.Collections.Generic;
using System.Text;
using Interop.VixCOM;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Virtual Machine power on.
    /// </summary>
    public class VirtualMachineDeleteFileFromGuest : VirtualMachineLoginGuest
    {
        private int _deleteFileTimeout = VMWareInterop.Timeouts.DeleteFileTimeout;
        private string _guestPathName = null;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int DeleteFileTimeout
        {
            get
            {
                return _deleteFileTimeout;
            }
            set
            {
                _deleteFileTimeout = value;
            }
        }

        /// <summary>
        /// Path to the file to delete on the guest operating system.
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
                    LoginGuest(virtualMachine);
                    Log.LogMessage(string.Format("Deleting guest file '{0}'", _guestPathName));
                    virtualMachine.DeleteFileFromGuest(_guestPathName, _deleteFileTimeout);
                }
            }

            return true;
        }
    }
}
