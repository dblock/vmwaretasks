using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Vestris.VMWareLib;
using Vestris.VMWareLib.Tools.Windows;

namespace Vestris.VMWareToolsSamples
{
    [TestFixture]
    public class WindowsShellSamples
    {
        [Test]
        public void GetEnvironmentVariablesSample()
        {
            #region Example: Enumerating Environment Variables on the GuestOS without VixCOM
            // connect to a local virtual machine and power it on
            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            virtualHost.ConnectToVMWareWorkstation();
            VMWareVirtualMachine virtualMachine = virtualHost.Open(@"C:\Users\dblock\Virtual Machines\Windows XP Pro SP3 25GB\WinXP Pro SP3 25GB.vmx");
            virtualMachine.PowerOn();
            virtualMachine.WaitForToolsInGuest();
            virtualMachine.LoginInGuest("Administrator", "admin123");
            Shell guestShell = new Shell(virtualMachine);
            Dictionary<string, string> guestEnvironmentVariables = guestShell.GetEnvironmentVariables();
            Console.WriteLine(guestEnvironmentVariables["ProgramFiles"]);
            #endregion
        }
    }
}
