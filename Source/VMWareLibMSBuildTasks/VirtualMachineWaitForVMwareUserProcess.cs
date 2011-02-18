using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Virtual Machine power on.
    /// </summary>
    public class VirtualMachineWaitForVMwareUserProcessInGuest : VirtualMachineOpen
    {
        private string _guestUsername = null;
        private string _guestPassword = null;
        private int _waitForVMwareUserProcessTimeout = VMWareInterop.Timeouts.WaitForToolsTimeout;

        /// <summary>
        /// Guest operating system username.
        /// </summary>
        public string GuestUsername
        {
            get
            {
                return _guestUsername;
            }
            set
            {
                _guestUsername = value;
            }
        }

        /// <summary>
        /// Guest operating system password.
        /// </summary>
        public string GuestPassword
        {
            get
            {
                return _guestPassword;
            }
            set
            {
                _guestPassword = value;
            }
        }

        /// <summary>
        /// Timeout to wait for the vmware user process.
        /// </summary>
        public int WaitForVMwareUserProcessTimeout
        {
            get
            {
                return _waitForVMwareUserProcessTimeout;
            }
            set
            {
                _waitForVMwareUserProcessTimeout = value;
            }
        }

        /// <summary>
        /// Waits for vmware user process in the guest
        /// </summary>
        /// <param name="virtualMachine"></param>
        protected void WaitForVMwareUserProcessInGuest(VMWareVirtualMachine virtualMachine)
        {
            Log.LogMessage(string.Format("Waiting for vmware user process in {0}", Filename));
            virtualMachine.WaitForVMWareUserProcessInGuest(GuestUsername, GuestPassword, WaitForVMwareUserProcessTimeout);
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
                    WaitForVMwareUserProcessInGuest(virtualMachine);
                }
            }

            return true;
        }
    }
}
