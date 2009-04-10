using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;
using System.IO;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareVirtualHostTests : VMWareTestSetup
    {
        [Test]
        public void TestWorkstationIDisposable()
        {
            if (VMWareTest.Instance.TestType != VMWareTestType.Workstation)
                Assert.Ignore("Skipping, test applies to Workstation only.");

            using (VMWareVirtualHost virtualHost = new VMWareVirtualHost())
            {
                virtualHost.ConnectToVMWareWorkstation();
                Assert.AreEqual(VMWareVirtualHost.ServiceProviderType.Workstation, virtualHost.ConnectionType);
            }
        }

        [Test]
        public void TestWorkstationSynchronousConnect()
        {
            if (VMWareTest.Instance.TestType != VMWareTestType.Workstation)
                Assert.Ignore("Skipping, test applies to Workstation only.");

            VixCOM.VixLib vix = new VixCOM.VixLib();
            VixCOM.IJob job = vix.Connect(
                VixCOM.Constants.VIX_API_VERSION,
                VixCOM.Constants.VIX_SERVICEPROVIDER_VMWARE_WORKSTATION, 
                null, 0, null, null, 0, null, null);
            ulong rc = job.WaitWithoutResults();
            Console.WriteLine("rc: {0}", rc);
            Assert.IsFalse(vix.ErrorIndicatesFailure(rc));
            Assert.IsTrue(vix.ErrorIndicatesSuccess(rc));
        }

        [Test]
        public void TestWorkstationConnectDisconnect()
        {

            if (VMWareTest.Instance.TestType != VMWareTestType.Workstation)
                Assert.Ignore("Skipping, test applies to Workstation only.");

            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            Assert.IsFalse(virtualHost.IsConnected);
            Assert.AreEqual(VMWareVirtualHost.ServiceProviderType.None, virtualHost.ConnectionType);
            virtualHost.ConnectToVMWareWorkstation();
            Assert.IsTrue(virtualHost.IsConnected);
            Assert.AreEqual(VMWareVirtualHost.ServiceProviderType.Workstation, virtualHost.ConnectionType);
            virtualHost.Disconnect();
            Assert.IsFalse(virtualHost.IsConnected);
            Assert.AreEqual(VMWareVirtualHost.ServiceProviderType.None, virtualHost.ConnectionType);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestWorkstationConnectDisconnectTwice()
        {
            if (VMWareTest.Instance.TestType != VMWareTestType.Workstation)
                Assert.Ignore("Skipping, test applies to Workstation only.");

            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            virtualHost.ConnectToVMWareWorkstation();
            virtualHost.Disconnect();
            virtualHost.Disconnect();
        }

        [Test]
        public void ShowRunningVirtualMachines()
        {
            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.Instance.VirtualHost.RunningVirtualMachines)
            {
                Console.WriteLine("{0}: running={1}, memory={2}, CPUs={3}",
                    virtualMachine.PathName, virtualMachine.IsRunning,
                    virtualMachine.MemorySize, virtualMachine.CPUCount);
            }
        }

        [Test]
        public void ShowVIRegisteredVirtualMachines()
        {
            if (VMWareTest.Instance.TestType != VMWareTestType.VI)
                Assert.Ignore("Skipping, test applies to VI only.");

            foreach (VMWareVirtualMachine virtualMachine in VMWareTest.Instance.VirtualHost.RegisteredVirtualMachines)
            {
                Console.WriteLine("{0}: running={1}, memory={2}, CPUs={3}",
                    virtualMachine.PathName, virtualMachine.IsRunning,
                    virtualMachine.MemorySize, virtualMachine.CPUCount);
            }
        }

        [Test]
        protected void TestRegisterUnregisterVirtualMachine()
        {
            // todo: test on ESX, requires admin privileges
        }
    }
}
