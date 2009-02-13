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
        public void TestEnumerateSnapshots()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.VirtualMachines)
            {
                List<string> snapshotPaths = GetSnapshotPaths(virtualMachine.Snapshots, 0);
                foreach (string snapshotPath in snapshotPaths)
                {
                    VMWareSnapshot snapshot = virtualMachine.Snapshots.FindSnapshot(snapshotPath);
                    Assert.IsNotNull(snapshot);
                    Console.WriteLine("{0}: {1}, power state={2}",
                        snapshot.DisplayName, snapshotPath, snapshot.PowerState);
                }
            }
        }

        [Test]
        public void TestCreateRemoveSnapshot()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.VirtualMachines)
            {
                // this is the root snapshot
                Assert.IsTrue(virtualMachine.Snapshots.Count >= 0);
                string name = Guid.NewGuid().ToString();
                Console.WriteLine("Snapshot name: {0}", name);
                // take a snapshot at the current state
                virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString());
                // check whether the snapshot was created
                Assert.IsNotNull(virtualMachine.Snapshots.GetNamedSnapshot(name));
                // delete the snapshot via VM interface
                virtualMachine.Snapshots.RemoveSnapshot(name);
                // check whether the snapshot was deleted
                Assert.IsNull(virtualMachine.Snapshots.GetNamedSnapshot(name));
            }
        }

        [Test]
        public void TestCreateRevertRemoveSnapshot()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.VirtualMachines)
            {
                // this is the root snapshot
                Assert.IsTrue(virtualMachine.Snapshots.Count >= 0);
                string name = Guid.NewGuid().ToString();
                Console.WriteLine("Snapshot name: {0}", name);
                // take a snapshot at the current state
                virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString());
                // revert to the newly created snapshot
                VMWareSnapshot snapshot = virtualMachine.Snapshots.GetNamedSnapshot(name);
                Assert.IsNotNull(snapshot);
                snapshot.RevertToSnapshot();
                snapshot.RemoveSnapshot();
            }
        }

        [Test, ExpectedException(typeof(VMWareException))]
        public void TestCreateSnapshotSameName()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.VirtualMachines)
            {
                // this is the root snapshot
                Assert.IsTrue(virtualMachine.Snapshots.Count >= 0);
                string name = Guid.NewGuid().ToString();
                Console.WriteLine("Snapshot name: {0}", name);
                // take a snapshot at the current state
                try
                {
                    virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString());
                    virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString());
                    // throws an error that the snapshot cannot be unique identified
                    VMWareSnapshot snapshot = virtualMachine.Snapshots.GetNamedSnapshot(name);
                }
                finally
                {
                    IEnumerable<VMWareSnapshot> snapshots = virtualMachine.Snapshots.FindSnapshotsByName(name);
                    foreach (VMWareSnapshot snapshot in snapshots)
                    {
                        Console.WriteLine("Removing {0}", snapshot.Path);
                        snapshot.RemoveSnapshot();
                    }
                }
            }
        }

        [Test]
        public void TestFindByName()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.VirtualMachines)
            {
                // this is the root snapshot
                string name = Guid.NewGuid().ToString();
                Console.WriteLine("Snapshot name: {0}", name);
                // take two snapshots at the current state
                virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString());
                virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString());
                Assert.IsNotNull(virtualMachine.Snapshots.FindSnapshotByName(name));
                IEnumerable<VMWareSnapshot> snapshots = virtualMachine.Snapshots.FindSnapshotsByName(name);
                int count = 0;
                foreach (VMWareSnapshot snapshot in snapshots)
                {
                    count++;
                    Assert.IsNotNull(virtualMachine.Snapshots.FindSnapshotByName(name));
                    Console.WriteLine("Removing {0}: {1}", snapshot.Path, snapshot.Description);
                    snapshot.RemoveSnapshot();
                }
                Assert.AreEqual(2, count);
                Assert.IsNull(virtualMachine.Snapshots.FindSnapshotByName(name));
            }
        }
    }
}
