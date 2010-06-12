using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Vestris.VMWareLib;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareLibSTATest : VMWareUnitTest
    {
        private void STAThreadFunction()
        {
            try
            {
                _test.SetUp();
                foreach (VMWareVirtualMachine virtualMachine in _test.VirtualMachines)
                {
                    ConsoleOutput.WriteLine(virtualMachine.PathName);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                _test.TearDown();
            }
        }

        [Test]
        [STAThread]
        public void TestSTA()
        {
            ThreadStart ts = new ThreadStart(STAThreadFunction);
            Thread t1 = new Thread(ts);
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();
            t1.Join();
            Thread t2 = new Thread(ts);
            t2.SetApartmentState(ApartmentState.STA);
            t2.Start();
            t2.Join();
        }
    }
}
