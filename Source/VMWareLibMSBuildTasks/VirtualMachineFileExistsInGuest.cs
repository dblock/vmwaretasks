using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;
using Interop.VixCOM;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Check whether a file exists in a guest operating system.
    /// </summary>
    public class VirtualMachineFileExistsInGuest : VirtualMachineLoginGuest
    {
        private int _fileExistsTimeout = VMWareInterop.Timeouts.FileExistsTimeout;
        private string _guestPathName;
        private bool _fileExists = false;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int FileExistsTimeout
        {
            get
            {
                return _fileExistsTimeout;
            }
            set
            {
                _fileExistsTimeout = value;
            }
        }

        /// <summary>
        /// File path on the guest operating system.
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
        /// True if file exists.
        /// </summary>
        [Output]
        public bool FileExists
        {
            get
            {
                return _fileExists;
            }
        }

        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                using (VMWareVirtualMachine virtualMachine = OpenVirtualMachine(host))
                {
                    LoginGuest(virtualMachine);
                    Log.LogMessage(string.Format("Checking file '{0}' in guest os", _guestPathName));
                    _fileExists = virtualMachine.FileExistsInGuest(_guestPathName, _fileExistsTimeout);
                }
            }

            return true;
        }
    }
}
