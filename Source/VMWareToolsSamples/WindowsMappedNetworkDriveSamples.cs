using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib.Tools.Windows;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.IO;

namespace Vestris.VMWareToolsSamples
{
    [TestFixture]
    public class WindowsMappedNetworkDriveSamples
    {
        [Test]
        public void CopyFromHostToGuestWithoutVixCOMSample()
        {
            #region Example: Copying Files to/from the Guest Operating System Without VixCOM
            // connect to a local virtual machine and power it on
            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            virtualHost.ConnectToVMWareWorkstation();
            VMWareVirtualMachine virtualMachine = virtualHost.Open(@"C:\Users\dblock\Virtual Machines\Windows XP Pro SP3 25GB\WinXP Pro SP3 25GB.vmx");
            virtualMachine.PowerOn();
            virtualMachine.WaitForToolsInGuest();
            // map the C:\ drive (\\guest\c$)
            MappedNetworkDriveInfo info = new MappedNetworkDriveInfo();
            info.RemotePath = @"C:\";
            info.Username = @"dblock-blue\Administrator";
            info.Password = @"admin123";
            MappedNetworkDrive mappedNetworkDrive = new MappedNetworkDrive(virtualMachine, info);
            string hostFile = @"C:\WINDOWS\win.ini";
            string guestFile = mappedNetworkDrive.GuestPathToNetworkPath(@"C:\host-win.ini");
            Console.WriteLine(guestFile);
            File.Copy(hostFile, guestFile);
            #endregion
        }
    }
}
