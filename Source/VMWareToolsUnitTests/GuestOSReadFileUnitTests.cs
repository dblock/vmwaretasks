using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Vestris.VMWareLib;
using Vestris.VMWareLib.Tools;
using Vestris.VMWareLibUnitTests;
using System.IO;

namespace Vestris.VMWareToolsUnitTests
{
    [TestFixture]
    public class GuestOSReadFileUnitTests
    {
        [Test]
        public void TestReadFile()
        {
            foreach (VMWareVirtualMachine vm in VMWareTest.PoweredVirtualMachines)
            {
                // create a temp file
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Guid.NewGuid().ToString());
                sb.AppendLine(Guid.NewGuid().ToString());
                string tempHostFilename = Path.GetTempFileName();
                File.WriteAllText(tempHostFilename, sb.ToString());
                // copy the file to the guest OS
                string tempGuestFilename = vm.CreateTempFileInGuest();
                vm.CopyFileFromHostToGuest(tempHostFilename, tempGuestFilename);
                GuestOS guestOS = new GuestOS(vm);
                // read file
                string tempGuestFileContents = guestOS.ReadFile(tempGuestFilename);
                Console.WriteLine("Read {0} bytes.", tempGuestFileContents.Length);
                Assert.AreEqual(tempGuestFileContents, File.ReadAllText(tempHostFilename));
                // read file lines
                string[] tempGuestFileLines = guestOS.ReadFileLines(tempGuestFilename);
                Console.WriteLine("Read {0} lines.", tempGuestFileLines.Length);
                Assert.AreEqual(2, tempGuestFileLines.Length);
                Assert.AreEqual(tempGuestFileLines, File.ReadAllLines(tempHostFilename));
                // read file bytes
                byte[] tempGuestFileBytes = guestOS.ReadFileBytes(tempGuestFilename);
                Console.WriteLine("Read {0} bytes.", tempGuestFileBytes.Length);
                Assert.AreEqual(tempGuestFileBytes, File.ReadAllBytes(tempHostFilename));
                // cleanup
                vm.DeleteFileFromGuest(tempGuestFilename);
                File.Delete(tempHostFilename);
            }
        }
    }
}
