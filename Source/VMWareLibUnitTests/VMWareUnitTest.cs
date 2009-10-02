using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Vestris.VMWareLibUnitTests
{
    public class VMWareUnitTest
    {
        [SetUp]
        public virtual void SetUp()
        {
            VMWareTest.SetUp();
        }

        [TearDown]
        public virtual void TearDown()
        {
            VMWareTest.TearDown();
        }
    }
}
