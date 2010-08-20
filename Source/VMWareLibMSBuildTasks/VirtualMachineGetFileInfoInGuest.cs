using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;
using Interop.VixCOM;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Get file information from a guest operating system.
    /// </summary>
    public class VirtualMachineGetFileInfoInGuest : VirtualMachineLoginGuest
    {
        private int _getFileInfoTimeout = VMWareInterop.Timeouts.GetFileInfoTimeout;
        private string _guestPathName;
        private VMWareVirtualMachine.GuestFileInfo _fileInfo = null;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int GetFileInfoTimeout
        {
            get
            {
                return _getFileInfoTimeout;
            }
            set
            {
                _getFileInfoTimeout = value;
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
        /// File size.
        /// </summary>
        [Output]
        public long FileSize
        {
            get
            {
                return _fileInfo.FileSize;
            }
        }

        /// <summary>
        /// True if the path belongs to a directory.
        /// </summary>
        [Output]
        public bool IsDirectory
        {
            get
            {
                return _fileInfo.IsDirectory;
            }
        }

        /// <summary>
        /// True if the path belongs to a symbolic link.
        /// </summary>
        [Output]
        public bool IsSymLink
        {
            get
            {
                return _fileInfo.IsSymLink;
            }
        }

        /// <summary>
        /// Date/time when the file was last modified.
        /// </summary>
        [Output]
        public string LastModified
        {
            get
            {
                return _fileInfo.LastModified.HasValue
                    ? _fileInfo.LastModified.Value.ToString()
                    : string.Empty;
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
                    Log.LogMessage(string.Format("Retreiving file information for '{0}' from guest os", _guestPathName));
                    _fileInfo = virtualMachine.GetFileInfoInGuest(_guestPathName, _getFileInfoTimeout);
                }
            }

            return true;
        }
    }
}
