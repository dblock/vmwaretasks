using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Vestris.VMWareLibUnitTests;
using Vestris.VMWareLib.Tools.Windows;
using System.IO;

namespace Vestris.VMWareToolsUnitTests
{
    [TestFixture]
    public class WindowsMappedNetworkDriveTests : VMWareTestSetup
    {
        [Test]
        public void TestMapCDrive()
        {
            MappedNetworkDriveInfo mappedNetworkDriveInfo = new MappedNetworkDriveInfo();
            mappedNetworkDriveInfo.RemotePath = @"C:\";
            mappedNetworkDriveInfo.Username = VMWareTest.Instance.Username;
            mappedNetworkDriveInfo.Password = VMWareTest.Instance.Password;
            using (MappedNetworkDrive mappedNetworkDrive = new MappedNetworkDrive(
                VMWareTest.Instance.PoweredVirtualMachine, mappedNetworkDriveInfo))
            {
                string guestWindowsPath = mappedNetworkDrive.GuestPathToNetworkPath(@"C:\Windows");
                Console.WriteLine("Mapped windows directory: {0}", guestWindowsPath);
                Shell guestShell = new Shell(VMWareTest.Instance.PoweredVirtualMachine);
                Assert.AreEqual(string.Format(@"\\{0}\C$\Windows", guestShell.IpAddress), guestWindowsPath);
                Console.WriteLine(mappedNetworkDrive.NetworkPath);
                Assert.IsTrue(Directory.Exists(guestWindowsPath));
            }
        }
    }
}
