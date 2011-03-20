using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareArgumentExceptionTests : VMWareUnitTest
    {
        [Test]
        public void Test_VI_Validate_Host_String_On_Connect()
        {
            if (!_test.Config.RunVITests)
                Assert.Ignore("Skipping, VI tests disabled.");

            foreach (VMWareVirtualHost virtualHost in _test.VirtualHosts)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    virtualHost.ConnectToVMWareVIServer(null, "username", "password");
                });
            }
        }

        [Test]
        public void Test_VI_Validate_Username_On_Connect()
        {
            if (!_test.Config.RunVITests)
                Assert.Ignore("Skipping, VI tests disabled.");

            foreach (VMWareVirtualHost virtualHost in _test.VirtualHosts)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    virtualHost.ConnectToVMWareVIServer("host", null, "password");
                });
            }
        }
    }
}
