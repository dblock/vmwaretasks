using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareVirtualHostTests
    {
        [Test]
        public void TestIDisposable()
        {
            using (VMWareVirtualHost virtualHost = new VMWareVirtualHost())
            {
                virtualHost.ConnectToVMWareWorkstation();
                Assert.AreEqual(VMWareVirtualHost.ServiceProviderType.Workstation, virtualHost.ConnectionType);
            }
        }

        [Test]
        public void TestConnectDisconnect()
        {
            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            Assert.AreEqual(VMWareVirtualHost.ServiceProviderType.None, virtualHost.ConnectionType);
            virtualHost.ConnectToVMWareWorkstation();
            Assert.AreEqual(VMWareVirtualHost.ServiceProviderType.Workstation, virtualHost.ConnectionType);
            virtualHost.Disconnect();
            Assert.AreEqual(VMWareVirtualHost.ServiceProviderType.None, virtualHost.ConnectionType);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestConnectDisconnectTwice()
        {
            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            virtualHost.ConnectToVMWareWorkstation();
            virtualHost.Disconnect();
            virtualHost.Disconnect();
        }

        [Test]
        public void TestRunningVirtualMachines()
        {
            using (VMWareVirtualHost virtualHost = new VMWareVirtualHost())
            {
                virtualHost.ConnectToVMWareWorkstation();
                foreach (VMWareVirtualMachine virtualMachine in virtualHost.RunningVirtualMachines)
                {
                    Console.WriteLine("{0}: running={1}, memory={2}, CPUs={3}",
                        virtualMachine.PathName, virtualMachine.IsRunning, 
                        virtualMachine.MemorySize, virtualMachine.CPUCount);
                }
            }
        }
    }
}
