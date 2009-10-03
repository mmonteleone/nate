using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;
using Xunit;
using Moq;
using Nate.Fluent;

namespace Nate.Tests.Unit.Fluent
{
    public class GloballyTransitionsToBuilderApiTests : FluentTestBase
    {
        [Fact]
        public void GloballyTransitionsToBuilderApi_On_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new GloballyTransitionsToBuilderApi<StubStateModel>(Builder)).On(null));
        }

        [Fact]
        public void GloballyTransitionsToBuilderApi_On_ValidParms_CallsTransitionsToBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.On("t")).Verifiable();
            var target = new GloballyTransitionsToBuilderApi<StubStateModel>(Builder);
            var result = target.On("t");
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

    }
}
