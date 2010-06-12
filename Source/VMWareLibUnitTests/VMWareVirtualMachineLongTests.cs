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
    public class VMWareVirtualMachineLongTests : VMWareUnitTest
    {
        public override void SetUp()
        {
            if (!_test.Config.RunLongTests)
                Assert.Ignore("Skipping, long tests disabled.");
        }

        [Test]
        public void TestRecordingBeginEnd()
        {
            if (! _test.Config.RunWorkstationTests)
                Assert.Ignore("Skipping, Workstation tests disabled.");

            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                Assert.IsFalse(virtualMachine.IsRecording);
                string snapshotName = Guid.NewGuid().ToString();
                ConsoleOutput.WriteLine("Begin recording ...");
                VMWareSnapshot snapshot = virtualMachine.BeginRecording(snapshotName, Guid.NewGuid().ToString());
                Assert.IsNotNull(snapshot);
                Assert.IsTrue(virtualMachine.IsRecording);
                Assert.IsFalse(virtualMachine.IsReplaying);
                virtualMachine.WaitForToolsInGuest();
                ConsoleOutput.WriteLine("Snapshot: {0}", snapshot.DisplayName);
                VMWareVirtualMachine.Process cmdProcess = virtualMachine.RunProgramInGuest("cmd.exe", "/C dir");
                Assert.IsNotNull(cmdProcess);
                ConsoleOutput.WriteLine("Process: {0}", cmdProcess.Id);
                ConsoleOutput.WriteLine("End recording ...");
                virtualMachine.EndRecording();
                Assert.IsFalse(virtualMachine.IsRecording);
                Assert.IsFalse(virtualMachine.IsReplaying);
                ConsoleOutput.WriteLine("Begin replay ...");
                snapshot.BeginReplay(Constants.VIX_VMPOWEROP_LAUNCH_GUI, VMWareInterop.Timeouts.ReplayTimeout);
                Assert.IsTrue(virtualMachine.IsReplaying);
                Thread.Sleep(10000);
                snapshot.EndReplay();
                Assert.IsFalse(virtualMachine.IsReplaying);
                ConsoleOutput.WriteLine("Removing snapshot ...");
                snapshot.RemoveSnapshot();
            }
        }

        [Test]
        public void TestReset()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                // hardware reset
                ConsoleOutput.WriteLine("Reset ...");
                virtualMachine.Reset();
                Assert.AreEqual(true, virtualMachine.IsRunning);
                ConsoleOutput.WriteLine("Wait ...");
                virtualMachine.WaitForToolsInGuest();
            }
        }

        [Test]
        public void TestSuspend()
        {
            foreach (VMWareVirtualMachine virtualMachine in _test.PoweredVirtualMachines)
            {
                ConsoleOutput.WriteLine("Suspend ...");
                virtualMachine.Suspend();
                Assert.AreEqual(false, virtualMachine.IsPaused);
                Assert.AreEqual(true, virtualMachine.IsSuspended);
                ConsoleOutput.WriteLine("Power ...");
                virtualMachine.PowerOn();
                ConsoleOutput.WriteLine("Wait ...");
                virtualMachine.WaitForToolsInGuest();
            }
        }
    }
}
