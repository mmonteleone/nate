using System;
using Nate.Fluent;
using Xunit;

namespace Nate.Tests.Unit.Fluent
{
    public class TransitionsToFluentBuilderApiTests : FluentTestBase
    {
        [Fact]
        public void TransitionsToFluentBuilderApi_On_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TransitionsToFluentBuilderApi<StubStateModel>(Builder).On(null));
        }

        [Fact]
        public void TransitionsToFluentBuilderApi_On_ValidParms_CallsTransitionsToBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.On("t")).Verifiable();
            var target = new TransitionsToFluentBuilderApi<StubStateModel>(Builder);
            var result = target.On("t");
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }
    }
}