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
        private List<string> GetSnapshotPaths(IEnumerable<VMWareSnapshot> snapshots, int level)
        {
            List<string> result = new List<string>();
            foreach (VMWareSnapshot snapshot in snapshots)
            {
                string snapshotPath = snapshot.Path;
                result.Add(snapshotPath);
                result.AddRange(GetSnapshotPaths(snapshot.ChildSnapshots, level + 1));
            }
            return result;
        }

        [Test]
        public void TestWorkstationEnumerateSnapshots()
        {
            VMWareVirtualMachine virtualMachine = VMWareTestVirtualMachine.VM.VirtualMachine;
            List<string> snapshotPaths = GetSnapshotPaths(virtualMachine.Snapshots, 0);
            foreach (string snapshotPath in snapshotPaths)
            {
                VMWareSnapshot snapshot = virtualMachine.Snapshots.FindSnapshot(snapshotPath);
                Assert.IsNotNull(snapshot);
                Console.WriteLine("{0}: {1}, power state={2}, replayable={3}",
                    snapshot.DisplayName, snapshotPath, snapshot.PowerState, snapshot.IsReplayable);
            }
        }

        [Test]
        public void TestWorkstationCreateRemoveSnapshot()
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
