using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;
using Interop.VixCOM;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Create a temporary file in the guest operating system.
    /// </summary>
    public class VirtualMachineCreateTempFileInGuest : VirtualMachineLoginGuest
    {
        private int _createTempFileTimeout = VMWareInterop.Timeouts.CreateTempFileTimeout;
        private string _guestPathName;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int CreateTempFileTimeout
        {
            get
            {
                return _createTempFileTimeout;
            }
            set
            {
                _createTempFileTimeout = value;
            }
        }

        /// <summary>
        /// Temporary file path on the guest operating system.
        /// </summary>
        [Output]
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
                    _guestPathName = virtualMachine.CreateTempFileInGuest(_createTempFileTimeout);
                    Log.LogMessage(string.Format("Created temporary file '{0}' in guest os", _guestPathName));
                }
            }

            return true;
        }
    }
}
