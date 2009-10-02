using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareSnapshotTests : VMWareUnitTest
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
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.Instance.VirtualMachines)
            {
                List<string> snapshotPaths = GetSnapshotPaths(virtualMachine.Snapshots, 0);
                foreach (string snapshotPath in snapshotPaths)
                {
                    VMWareSnapshot snapshot = virtualMachine.Snapshots.FindSnapshot(snapshotPath);
                    Assert.IsNotNull(snapshot);
                    ConsoleOutput.WriteLine("{0}: {1}, power state={2}",
                        snapshot.DisplayName, snapshotPath, snapshot.PowerState);
                }
            }
        }

        [Test]
        public void TestCreateRemoveSnapshot()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.Instance.VirtualMachines)
            {
                // this is the root snapshot
                Assert.IsTrue(virtualMachine.Snapshots.Count >= 0);
                string name = Guid.NewGuid().ToString();
                ConsoleOutput.WriteLine("Snapshot name: {0}", name);
                // take a snapshot at the current state
                using (VMWareSnapshot snapshot = virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString()))
                {
                    ConsoleOutput.WriteLine("Created snapshot: {0}", name);
                }
                // check whether the snapshot was created
                Assert.IsNotNull(virtualMachine.Snapshots.GetNamedSnapshot(name));
                // delete the snapshot via VM interface
                virtualMachine.Snapshots.RemoveSnapshot(name);
                // check whether the snapshot was deleted
                Assert.IsNull(virtualMachine.Snapshots.FindSnapshotByName(name));
            }
        }

        [Test]
        public void TestCreateRevertRemoveSnapshot()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.Instance.VirtualMachines)
            {
                // this is the root snapshot
                Assert.IsTrue(virtualMachine.Snapshots.Count >= 0);
                string name = Guid.NewGuid().ToString();
                ConsoleOutput.WriteLine("Creating snapshot: {0}", name);
                // take a snapshot at the current state
                using (VMWareSnapshot snapshot = virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString()))
                {

                }
                // revert to the newly created snapshot
                ConsoleOutput.WriteLine("Locating snapshot: {0}", name);
                using (VMWareSnapshot snapshot = virtualMachine.Snapshots.GetNamedSnapshot(name))
                {
                    Assert.IsNotNull(snapshot);
                    ConsoleOutput.WriteLine("Reverting snapshot: {0}", name);
                    snapshot.RevertToSnapshot();
                    ConsoleOutput.WriteLine("Removing snapshot: {0}", name);
                    snapshot.RemoveSnapshot();
                }
            }
        }

        [Test]
        public void TestCreateSnapshotSameName()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.Instance.VirtualMachines)
            {
                // this is the root snapshot
                Assert.IsTrue(virtualMachine.Snapshots.Count >= 0);
                string name = Guid.NewGuid().ToString();
                // take a snapshot at the current state
                ConsoleOutput.WriteLine("Creating snapshot 1: {0}", name);
                using (VMWareSnapshot snapshot = virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString()))
                {
                    // needs to be disposed
                }
                ConsoleOutput.WriteLine("Creating snapshot 2: {0}", name);
                using (VMWareSnapshot snapshot = virtualMachine.Snapshots.CreateSnapshot(name, Guid.NewGuid().ToString()))
                {
                    // needs to be disposed
                }
                int count = 0;
                IEnumerable<VMWareSnapshot> snapshots = virtualMachine.Snapshots.FindSnapshotsByName(name);
                foreach (VMWareSnapshot snapshot in snapshots)
                {
                    ConsoleOutput.WriteLine("Removing snapshot: {0}", snapshot.Path);
                    snapshot.RemoveSnapshot();
                    count++;
                }
                Assert.AreEqual(2, count);
            }
        }

        [Test]
        public void TestFindByName()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.Instance.VirtualMachines)
            {
                // this is the root snapshot
                string name = Guid.NewGuid().ToString();
                // take two snapshots at the current state
                ConsoleOutput.WriteLine("Creating snapshot 1: {0}", name);
                using (VMWareSnapshot snapshot = virtualMachine.Snapshots.CreateSnapshot(
                    name, Guid.NewGuid().ToString()))
                {
                    Console.WriteLine("Created snapshot: {0}", snapshot.DisplayName);
                }

                ConsoleOutput.WriteLine("Creating snapshot 2: {0}", name);
                using (VMWareSnapshot snapshot = virtualMachine.Snapshots.CreateSnapshot(
                    name, Guid.NewGuid().ToString()))
                {
                    Console.WriteLine("Created snapshot: {0}", snapshot.DisplayName);
                }

                ConsoleOutput.WriteLine("Locating snapshot ...");
                Assert.IsNotNull(virtualMachine.Snapshots.FindSnapshotByName(name));
                ConsoleOutput.WriteLine("Locating snapshots ...");
                IEnumerable<VMWareSnapshot> snapshots = virtualMachine.Snapshots.FindSnapshotsByName(name);
                int count = 0;
                foreach (VMWareSnapshot snapshot in snapshots)
                {
                    count++;
                    Assert.IsNotNull(virtualMachine.Snapshots.FindSnapshotByName(name));
                    ConsoleOutput.WriteLine("Removing {0}: {1}", snapshot.Path, snapshot.Description);
                    snapshot.RemoveSnapshot();
                    snapshot.Close();
                }
                Assert.AreEqual(2, count);
                Assert.IsNull(virtualMachine.Snapshots.FindSnapshotByName(name));
            }
        }

        [Test]
        public void TestRevertToRoot()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.Instance.VirtualMachines)
            {
                foreach (VMWareSnapshot snapshot in virtualMachine.Snapshots)
                {
                    snapshot.RevertToSnapshot();
                    break;
                }
            }
        }
    }
}
