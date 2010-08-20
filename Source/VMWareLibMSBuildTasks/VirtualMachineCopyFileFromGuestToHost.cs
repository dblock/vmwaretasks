using System;
using System.Collections.Generic;
using System.Text;
using Interop.VixCOM;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Virtual Machine power on.
    /// </summary>
    public class VirtualMachineCopyFileFromGuestToHost : VirtualMachineLoginGuest
    {
        private int _copyFileTimeout = VMWareInterop.Timeouts.CopyFileTimeout;
        private string _hostPathName = null;
        private string _guestPathName = null;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int CopyFileTimeout
        {
            get
            {
                return _copyFileTimeout;
            }
            set
            {
                _copyFileTimeout = value;
            }
        }

        /// <summary>
        /// Destination path on the host operating system.
        /// </summary>
        public string HostPathName
        {
            get
            {
                return _hostPathName;
            }
            set
            {
                _hostPathName = value;
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
                    Log.LogMessage(string.Format("Copying guest '{0}' to host '{1}'", _guestPathName, _hostPathName));
                    virtualMachine.CopyFileFromGuestToHost(_guestPathName, _hostPathName, _copyFileTimeout);
                }
            }

            return true;
        }
    }
}
