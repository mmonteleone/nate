using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;
using Xunit;
using Moq;

namespace Nate.Tests.Unit.Core
{
    public class InvalidTransitionExceptionTests
    {
        [Fact]
        public void InvalidTransitionException_Message_VerifyMessage()
        {
            var ex = new InvalidTransitionException("message");
            Assert.Equal("message", ex.Message);
        }

        [Fact]
        public void InvalidTransitionException_MessageEx_VerifyMessageEx()
        {
            var inner = new Exception();
            var ex = new InvalidTransitionException("message", inner);
            Assert.Equal("message", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }
    }
}
