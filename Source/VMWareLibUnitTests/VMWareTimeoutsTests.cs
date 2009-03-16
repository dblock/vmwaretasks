using System;
using System.Collections.Generic;
using System.Text;
using Vestris.VMWareLib;
using NUnit.Framework;
using System.Reflection;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareTimeoutsTests
    {
        [Test]
        public void TestZeroReflectionConstructor()
        {
            VMWareTimeouts timeouts = new VMWareTimeouts(0);
            FieldInfo[] timeoutFieldInfo = timeouts.GetType().GetFields();
            long timeoutsSum = 0;
            foreach (FieldInfo timeout in timeoutFieldInfo)
            {
                object[] timeoutAttributes = timeout.GetCustomAttributes(typeof(VMWareTimeoutAttribute), false);
                if (timeoutAttributes == null || timeoutAttributes.Length == 0)
                    continue;

                int timeoutValue = (int) timeout.GetValue(timeouts);
                Assert.AreEqual(0, timeoutValue);
                timeoutsSum += timeoutValue;
            }

            Assert.AreEqual(0, timeoutsSum);
        }

        [Test]
        public void TestReflectionConstructor()
        {
            int baseTimeout = 10;
            VMWareTimeouts timeouts = new VMWareTimeouts(baseTimeout);
            FieldInfo[] timeoutFieldInfo = timeouts.GetType().GetFields();
            long timeoutsSum = 0;
            int timeoutsCount = 0;
            foreach (FieldInfo timeout in timeoutFieldInfo)
            {
                object[] timeoutAttributes = timeout.GetCustomAttributes(typeof(VMWareTimeoutAttribute), false);
                if (timeoutAttributes == null || timeoutAttributes.Length == 0)
                    continue;

                int timeoutValue = (int) timeout.GetValue(timeouts);
                Assert.AreNotEqual(0, timeoutValue);
                Assert.IsTrue(timeoutValue >= baseTimeout);
                Assert.IsTrue(timeoutValue % baseTimeout == 0);
                timeoutsSum += timeoutValue;
                timeoutsCount++;
            }

            Assert.AreNotEqual(0, timeoutsSum);
            Assert.IsTrue(timeoutsSum > timeoutsCount * baseTimeout, "There're no multiplied timeouts.");
        }
    }
}
