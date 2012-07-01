using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;
using System.Configuration;
using System.IO;
using Interop.VixCOM;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareVirtualHostTests : VMWareUnitTest
    {
        [Test]
        public void TestWorkstationIDisposable()
        {
            if (! _test.Config.RunWorkstationTests)
                Assert.Ignore("Skipping, Workstation tests disabled.");

            using (VMWareVirtualHost virtualHost = new VMWareVirtualHost())
            {
                virtualHost.ConnectToVMWareWorkstation();
                Assert.AreEqual(VMWareVirtualHost.ServiceProviderType.Workstation, virtualHost.ConnectionType);
            }
        }

        [Test]
        public void TestWorkstationSynchronousConnect()
        {
            if (! _test.Config.RunWorkstationTests)
                Assert.Ignore("Skipping, Workstation tests disabled.");

            VixLib vix = new VixLib();
            IJob job = vix.Connect(
                Constants.VIX_API_VERSION,
                Constants.VIX_SERVICEPROVIDER_VMWARE_WORKSTATION,
                null, 0, null, null, 0, null, null);
            ulong rc = job.WaitWithoutResults();
            Assert.IsFalse(vix.ErrorIndicatesFailure(rc));
            Assert.IsTrue(vix.ErrorIndicatesSuccess(rc));
        }

        [Test]
        public void TestWorkstationConnectDisconnect()
        {
            if (! _test.Config.RunWorkstationTests)
                Assert.Ignore("Skipping, Workstation tests disabled.");

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
            if (!_test.Config.RunWorkstationTests)
                Assert.Ignore("Skipping, Workstation tests disabled.");

            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            virtualHost.ConnectToVMWareWorkstation();
            virtualHost.Disconnect();
            virtualHost.Disconnect();
        }

        [Test]
        public void ShowRunningVirtualMachines()
        {
            foreach (VMWareVirtualHost virtualHost in _test.ConnectedVirtualHosts)
            {
                foreach (VMWareVirtualMachine virtualMachine in virtualHost.RunningVirtualMachines)
                {
                    ConsoleOutput.WriteLine("{0}: running={1}, memory={2}, CPUs={3}",
                        virtualMachine.PathName, virtualMachine.IsRunning,
                        virtualMachine.MemorySize, virtualMachine.CPUCount);
                }
            }
        }

        [Test]
        public void ShowVIRegisteredVirtualMachines()
        {
            if (! _test.Config.RunVITests)
                Assert.Ignore("Skipping, VI tests disabled.");

            foreach (VMWareVirtualHost virtualHost in _test.ConnectedVirtualHosts)
            {
                foreach (VMWareVirtualMachine virtualMachine in virtualHost.RegisteredVirtualMachines)
                {
                    ConsoleOutput.WriteLine("{0}: running={1}, memory={2}, CPUs={3}",
                        virtualMachine.PathName, virtualMachine.IsRunning,
                        virtualMachine.MemorySize, virtualMachine.CPUCount);
                }
            }
        }

        [Test]
        protected void TestRegisterUnregisterVirtualMachine()
        {
            // todo: test on ESX, requires admin privileges
        }
        
        [Test]
        public void TestGetPropertyByName()
        {
            if (!_test.Config.RunWorkstationTests)
                Assert.Ignore("Skipping, Workstation tests disabled.");

            foreach (VMWareVirtualHost virtualHost in _test.ConnectedVirtualHosts)
            {
                if (virtualHost.ConnectionType == VMWareVirtualHost.ServiceProviderType.Workstation)
                {
                    virtualHost.ConnectToVMWareWorkstation();
 
                    // Should return default is prporty does not exist
                    var result = virtualHost.GetProperty<int>("VIX_DUMMY_PROPERTY", 99);
                    Assert.AreEqual(99, result, "Default value incorrect for property that does not exist");

                    // Should return property value for existing property
                    result = virtualHost.GetProperty<int>("VIX_PROPERTY_HOST_HOSTTYPE", 33);
                    Assert.AreEqual(3, result, "Incorrect value returned for existing property");
                }
            }
        }  
    }
}
