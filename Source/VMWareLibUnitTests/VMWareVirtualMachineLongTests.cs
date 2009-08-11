using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Text;
using VixCOM;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareVirtualMachineLongTests
    {
        [Test]
        public void TestRecordingBeginEnd()
        {
            if (!VMWareTest.RunWorkstationTests)
                Assert.Ignore("Skipping, Workstation tests disabled.");

            VMWareVirtualMachine virtualMachine = TestWorkstation.Instance.PoweredVirtualMachine;
            Assert.IsFalse(virtualMachine.IsRecording);
            string snapshotName = Guid.NewGuid().ToString();
            Console.WriteLine("Begin recording ...");
            VMWareSnapshot snapshot = virtualMachine.BeginRecording(snapshotName, Guid.NewGuid().ToString());
            Assert.IsNotNull(snapshot);
            Assert.IsTrue(virtualMachine.IsRecording);
            Assert.IsFalse(virtualMachine.IsReplaying);
            virtualMachine.WaitForToolsInGuest();
            Console.WriteLine("Snapshot: {0}", snapshot.DisplayName);
            VMWareVirtualMachine.Process cmdProcess = virtualMachine.RunProgramInGuest("cmd.exe", "/C dir");
            Assert.IsNotNull(cmdProcess);
            Console.WriteLine("Process: {0}", cmdProcess.Id);
            Console.WriteLine("End recording ...");
            virtualMachine.EndRecording();
            Assert.IsFalse(virtualMachine.IsRecording);
            Assert.IsFalse(virtualMachine.IsReplaying);
            Console.WriteLine("Begin replay ...");
            snapshot.BeginReplay(VixCOM.Constants.VIX_VMPOWEROP_LAUNCH_GUI, VMWareInterop.Timeouts.ReplayTimeout);
            Assert.IsTrue(virtualMachine.IsReplaying);
            Thread.Sleep(10000);
            snapshot.EndReplay();
            Assert.IsFalse(virtualMachine.IsReplaying);
            Console.WriteLine("Removing snapshot ...");
            snapshot.RemoveSnapshot();
        }

        [Test]
        public void TestReset()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
                // hardware reset
                Console.WriteLine("Reset ...");
                virtualMachine.Reset();
                Assert.AreEqual(true, virtualMachine.IsRunning);
                Console.WriteLine("Wait ...");
                virtualMachine.WaitForToolsInGuest();
            }
        }

        [Test]
        public void TestSuspend()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.PoweredVirtualMachines)
            {
                Console.WriteLine("Suspend ...");
                virtualMachine.Suspend();
                Assert.AreEqual(false, virtualMachine.IsPaused);
                Assert.AreEqual(true, virtualMachine.IsSuspended);
                Console.WriteLine("Power ...");
                virtualMachine.PowerOn();
                Console.WriteLine("Wait ...");
                virtualMachine.WaitForToolsInGuest();
            }
        }
    }
}
