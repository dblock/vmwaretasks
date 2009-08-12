using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Text;
using Interop.VixCOM;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareVirtualMachinePoweredOffTests
    {
        [SetUp]
        public void SetUp()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.VirtualMachines)
            {
                if (virtualMachine.IsRunning)
                {
                    virtualMachine.PowerOff();
                }
            }
        }

        [Test]
        protected void TestUpgradeVirtualHardware()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.VirtualMachines)
            {
                // upgrading virtual hardware should always succeed
                Console.WriteLine("Upgrading virtual hardware ...");
                virtualMachine.UpgradeVirtualHardware();
            }
        }

        [Test]
        public void TestCloneVirtualMachine()
        {
            if (!VMWareTest.RunWorkstationTests)
                Assert.Ignore("Skipping test, Workstation tests disabled.");

            VMWareVirtualMachine virtualMachine = TestWorkstation.Instance.VirtualMachine;
            string vmxPathName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Console.WriteLine("Creating linked clone: {0}", vmxPathName);
            Directory.CreateDirectory(vmxPathName);
            string vmxFileName = Path.Combine(vmxPathName, "Clone.vmx");
            virtualMachine.Clone(VMWareVirtualMachineCloneType.Linked, vmxFileName);
            Assert.IsTrue(File.Exists(vmxFileName));
            Directory.Delete(vmxPathName, true);
        }

        [Test]
        public void TestDeleteVirtualMachine()
        {
            if (!VMWareTest.RunWorkstationTests)
                Assert.Ignore("Skipping, test requires server admin privileges for ESX, Workstation tests disabled.");

            VMWareVirtualMachine virtualMachine = TestWorkstation.Instance.VirtualMachine;
            string vmxPathName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Console.WriteLine("Creating linked clone: {0}", vmxPathName);
            Directory.CreateDirectory(vmxPathName);
            string vmxFileName = Path.Combine(vmxPathName, "Clone.vmx");
            virtualMachine.Clone(VMWareVirtualMachineCloneType.Linked, vmxFileName);
            Assert.IsTrue(File.Exists(vmxFileName));

            VMWareVirtualMachine virtualMachineClone = TestWorkstation.Instance.VirtualHost.Open(vmxFileName);
            virtualMachineClone.Delete(Constants.VIX_VMDELETE_DISK_FILES);
            Assert.IsFalse(File.Exists(vmxFileName));
            Assert.IsFalse(Directory.Exists(vmxPathName));
        }

        [Test]
        public void TestCloneVirtualMachineSnapshot()
        {
            if (!VMWareTest.RunWorkstationTests)
                Assert.Ignore("Skipping, test requires server admin privileges for ESX, Workstation tests disabled.");

            VMWareVirtualMachine virtualMachine = TestWorkstation.Instance.VirtualMachine;
            string vmxPathName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Console.WriteLine("Creating linked clone of root snapshot: {0}", vmxPathName);
            Directory.CreateDirectory(vmxPathName);
            string vmxFileName = Path.Combine(vmxPathName, "Clone.vmx");
            virtualMachine.Snapshots.GetCurrentSnapshot().Clone(VMWareVirtualMachineCloneType.Linked, vmxFileName);
            Assert.IsTrue(File.Exists(vmxFileName));
            Directory.Delete(vmxPathName, true);
        }
    }
}
