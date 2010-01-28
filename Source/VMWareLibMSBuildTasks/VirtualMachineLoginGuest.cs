using System;
using System.Collections.Generic;
using System.Text;
using Interop.VixCOM;

namespace Vestris.VMWareLib.MSBuildTasks
{
    /// <summary>
    /// Virtual Machine power on.
    /// </summary>
    public class VirtualMachineLoginGuest : VirtualMachinePowerOn
    {
        private int _loginTimeout = VMWareInterop.Timeouts.LoginTimeout;
        private bool _interactive = false;
        private string _guestUsername = null;
        private string _guestPassword = null;

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int LoginTimeout
        {
            get
            {
                return _loginTimeout;
            }
            set
            {
                _loginTimeout = value;
            }
        }

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
        /// Perform an interactive login.
        /// </summary>
        public bool Interactive
        {
            get
            {
                return _interactive;
            }
            set
            {
                _interactive = value;
            }
        }

        protected void LoginGuest(VMWareVirtualMachine virtualMachine)
        {
            PowerOnVirtualMachine(virtualMachine);
            Log.LogMessage(string.Format("Logging in {0}", Filename));
            virtualMachine.LoginInGuest(GuestUsername, GuestPassword,
                _interactive ? Constants.VIX_LOGIN_IN_GUEST_REQUIRE_INTERACTIVE_ENVIRONMENT : 0,
                _loginTimeout);
        }

        public override bool Execute()
        {
            using (VMWareVirtualHost host = GetConnectedHost())
            {
                using (VMWareVirtualMachine virtualMachine = OpenVirtualMachine(host))
                {
                    LoginGuest(virtualMachine);
                }
            }

            return true;
        }
    }
}
