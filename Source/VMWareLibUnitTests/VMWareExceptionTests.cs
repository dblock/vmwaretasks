using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vestris.VMWareLib;

namespace Vestris.VMWareLibUnitTests
{
    [TestFixture]
    public class VMWareExceptionTests
    {
        [Test]
        [ExpectedException(ExceptionType = typeof(VMWareException), ExpectedMessage = "The operation was successful")]
        public void TestThrowOperationWasSuccessfulException()
        {
            throw new VMWareException(0);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(VMWareException), ExpectedMessage = "Unknown error")]
        public void TestThrowUnknownErrorException()
        {
            throw new VMWareException(1);
        }

        [Test]
        [ExpectedException(ExceptionType = typeof(VMWareException), ExpectedMessage = "This login type is not supported")]
        public void TestThrowLoginTypeNotSupportedException()
        {
            throw new VMWareException(3032);
        }
    }
}
