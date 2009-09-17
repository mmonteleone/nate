using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;
using Xunit;
using Moq;

namespace Nate.Tests.Unit.Core
{
    public class InvalidTriggerExceptionTests
    {
        [Fact]
        public void InvalidTriggerException_Message_VerifyMessage()
        {
            var ex = new InvalidTriggerException("message");
            Assert.Equal("message", ex.Message);
        }

        [Fact]
        public void InvalidTriggerException_MessageEx_VerifyMessageEx()
        {
            var inner = new Exception();
            var ex = new InvalidTriggerException("message", inner);
            Assert.Equal("message", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }
    }
}
