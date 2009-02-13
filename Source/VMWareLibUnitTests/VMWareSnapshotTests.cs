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
            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            virtualHost.ConnectToVMWareWorkstation();
            string testWorkstationFilename = ConfigurationManager.AppSettings["testWorkstationFilename"];
            VMWareVirtualMachine virtualMachine = virtualHost.Open(testWorkstationFilename);
            List<VMWareSnapshot> snapshots = virtualMachine.GetSnapshots();
            Assert.IsTrue(snapshots.Count >= 0);
            // \todo: enumerate child snapshots, get snapshot name
        }
    }
}
