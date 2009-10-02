using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Threading;
using Vestris.VMWareLib;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareMultiThreadedTests : VMWareUnitTest
    {
        const int _threadCount = 3;

        private void RunThreadTest(ParameterizedThreadStart threadStart)
        {
            List<Thread> threads = new List<Thread>();
            foreach (IVMWareTestProvider provider in VMWareTest.Instance.Providers)
            {
                ConsoleOutput.WriteLine("Starting thread {0} ...", threads.Count + 1);
                Thread thread = new Thread(threadStart);
                thread.Start(provider);
                threads.Add(thread);
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        #region TestVMWareHostConnectDisconnect
        
        private void TestVMWareHostConnectDisconnectThreadProc(object o)
        {
            IVMWareTestProvider provider = (IVMWareTestProvider) o;
            Assert.IsTrue(provider.VirtualHost.IsConnected);
            for (int i = 0; i < _threadCount; i++)
            {
                provider.Reconnect();
            }
        }

        [Test]
        public void TestVMWareHostConnectDisconnect()
        {
            RunThreadTest(new ParameterizedThreadStart(TestVMWareHostConnectDisconnectThreadProc));
        }

        #endregion

        #region TestVMWareVirtualMachinePower

        private void TestVMWareVirtualMachinePowerThreadProc(object o)
        {
            IVMWareTestProvider provider = (IVMWareTestProvider)o;
            Assert.IsTrue(provider.VirtualHost.IsConnected);
            for (int i = 0; i < _threadCount; i++)
            {
                ConsoleOutput.WriteLine("CPUs: {0}", provider.VirtualMachine.CPUCount);
                ConsoleOutput.WriteLine("Memory: {0}", provider.PoweredVirtualMachine.MemorySize);
                ConsoleOutput.WriteLine("Powering off ...");
                provider.PoweredVirtualMachine.PowerOff();
                provider.Reconnect();
            }
        }

        [Test]
        public void TestVMWareVirtualMachinePower()
        {
            RunThreadTest(new ParameterizedThreadStart(TestVMWareVirtualMachinePowerThreadProc));
        }

        #endregion
    }
}
