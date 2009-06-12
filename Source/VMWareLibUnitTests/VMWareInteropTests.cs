using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareInteropTests
    {
        [Test]
        public void TestThrowOperationWasSuccessfulException()
        {
            VMWareInterop.Check(0);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(VMWareException), ExpectedMessage = "Unknown error")]
        public void TestThrowUnknownErrorException()
        {
            VMWareInterop.Check(1);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(VMWareException), ExpectedMessage = "This login type is not supported")]
        public void TestThrowLoginTypeNotSupportedException()
        {
            VMWareInterop.Check(3032);
        }
    }
}
