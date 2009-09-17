using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;
using Xunit;
using Moq;

namespace Nate.Tests.Unit.Core
{
    public class InvalidStateModelExceptionTests
    {
        [Fact]
        public void InvalidStateModelException_Message_VerifyMessage()
        {
            var ex = new InvalidStateModelException("message");
            Assert.Equal("message", ex.Message);
        }

        [Fact]
        public void InvalidStateModelException_MessageEx_VerifyMessageEx()
        {
            var inner = new Exception();
            var ex = new InvalidStateModelException("message", inner);
            Assert.Equal("message", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }
    }
}
