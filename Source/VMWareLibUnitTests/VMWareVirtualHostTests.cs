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
            }
        }

        [Test]
        public void TestConnectDisconnect()
        {
            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            virtualHost.ConnectToVMWareWorkstation();
            virtualHost.Disconnect();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestConnectDisconnectTwice()
        {
            VMWareVirtualHost virtualHost = new VMWareVirtualHost();
            virtualHost.ConnectToVMWareWorkstation();
            virtualHost.Disconnect();
            virtualHost.Disconnect();
        }
    }
}
