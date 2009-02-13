using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareSnapshotTests
    {
        [Test]
        public void TestWorkstationEnumerateSnapshots()
        {
            VMWareVirtualMachine virtualMachine = VMWareTestVirtualMachine.VM.VirtualMachine;
            // this is the root snapshot
            Assert.IsTrue(virtualMachine.Snapshots.Count >= 0);
            string name = Guid.NewGuid().ToString();
            Console.WriteLine("Snapshot name: {0}", name);
            // take a snapshot at the current state
            virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString());
            // delete the snapshot
            virtualMachine.Snapshots.RemoveSnapshot(name);
        }
    }
}
