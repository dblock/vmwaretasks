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
    public class WindowsMappedNetworkDriveTests : VMWareUnitTest
    {
        [Test]
        public void TestMapCDrive()
        {
            foreach (IVMWareTestProvider provider in _test.Providers)
            {
                MappedNetworkDriveInfo mappedNetworkDriveInfo = new MappedNetworkDriveInfo();
                mappedNetworkDriveInfo.RemotePath = @"C:\";
                mappedNetworkDriveInfo.Username = provider.Username;
                mappedNetworkDriveInfo.Password = provider.Password;
                Console.WriteLine("Remote IP: {0}", provider.VirtualMachine.GuestVariables["ip"]);
                using (MappedNetworkDrive mappedNetworkDrive = new MappedNetworkDrive(
                    provider.PoweredVirtualMachine, mappedNetworkDriveInfo))
                {
                    string guestWindowsPath = mappedNetworkDrive.GuestPathToNetworkPath(@"C:\Windows");
                    Console.WriteLine("Mapped windows directory: {0}", guestWindowsPath);
                    Shell guestShell = new Shell(provider.PoweredVirtualMachine);
                    Assert.AreEqual(string.Format(@"\\{0}\C$\Windows", guestShell.IpAddress), guestWindowsPath);
                    Console.WriteLine(mappedNetworkDrive.NetworkPath);
                    Assert.IsTrue(Directory.Exists(guestWindowsPath));
                }
            }
        }
    }
}
