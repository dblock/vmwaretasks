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
    public class GuestOSReadFileUnitTests : VMWareTestSetup
    {
        private VMWareVirtualMachine _vm;
        private string _tempHostFilename;
        private string _tempGuestFilename;

        public override void SetUp()
        {
            base.SetUp();
            // create a temp file
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Guid.NewGuid().ToString());
            sb.AppendLine(Guid.NewGuid().ToString());
            _tempHostFilename = Path.GetTempFileName();
            File.WriteAllText(_tempHostFilename, sb.ToString());
            // copy the file to the guest OS
            _vm = VMWareTest.Instance.PoweredVirtualMachine;
            _tempGuestFilename = Path.Combine(_vm.GuestEnvironmentVariables["tmp"], Guid.NewGuid().ToString());
            _vm.CopyFileFromHostToGuest(_tempHostFilename, _tempGuestFilename);
        }

        public override void TearDown()
        {
            if (!string.IsNullOrEmpty(_tempGuestFilename)) _vm.DeleteFileFromGuest(_tempGuestFilename);
            if (!string.IsNullOrEmpty(_tempHostFilename)) File.Delete(_tempHostFilename);
            base.TearDown();
        }

        [Test]
        public void TestReadFile()
        {
            GuestOS guestOS = new GuestOS(_vm);
            string tempGuestFileContents = guestOS.ReadFile(_tempGuestFilename);
            Console.WriteLine("Read {0} bytes.", tempGuestFileContents.Length);
            Assert.AreEqual(tempGuestFileContents, File.ReadAllText(_tempHostFilename));
        }

        [Test]
        public void TestReadFileLines()
        {
            GuestOS guestOS = new GuestOS(_vm);
            string[] tempGuestFileLines = guestOS.ReadFileLines(_tempGuestFilename);
            Console.WriteLine("Read {0} lines.", tempGuestFileLines.Length);
            Assert.AreEqual(2, tempGuestFileLines.Length);
            Assert.AreEqual(tempGuestFileLines, File.ReadAllLines(_tempHostFilename));
        }

        [Test]
        public void TestReadFileBytes()
        {
            GuestOS guestOS = new GuestOS(_vm);
            byte[] tempGuestFileBytes = guestOS.ReadFileBytes(_tempGuestFilename);
            Console.WriteLine("Read {0} bytes.", tempGuestFileBytes.Length);
            Assert.AreEqual(tempGuestFileBytes, File.ReadAllBytes(_tempHostFilename));
        }
    }
}
