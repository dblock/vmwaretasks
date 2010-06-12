using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Vestris.VMWareLibUnitTests
{
    public class VMWareUnitTest
    {
        protected VMWareTest _test = new VMWareTest();

        [SetUp]
        public virtual void SetUp()
        {
            _test.SetUp();
        }

        [TearDown]
        public virtual void TearDown()
        {
            _test.TearDown();
        }
    }
}
