using System;
using Nate.Core;
using Nate.Fluent;
using Xunit;

namespace Nate.Tests.Unit.Fluent;

public class BeforeTransitionBuilderApiTests : FluentTestBase
{
    [Fact]
    public void BeforeTransitionBuilderApi_AfterTransition_NullCallback_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeTransitionBuilderApi<StubStateModel>(Builder).AfterTransition(null));
    }

    [Fact]
    public void
        BeforeTransitionBuilderApi_AfterTransition_ValidParms_CallsAfterTransitionOnBuilder_ReturnsApiWithBuilder()
    {
        Action<TransitionEventArgs<StubStateModel>> callback = e => { };
        MockBuilder.Setup(b => b.AfterTransition(callback)).Verifiable();
        var target = new BeforeTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.AfterTransition(callback);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeTransitionBuilderApi_Compile_NullConfiguration_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeTransitionBuilderApi<StubStateModel>(Builder).Compile(null));
    }

    [Fact]
    public void BeforeTransitionBuilderApi_Compile_ValidParms_CallsCompileOnBuilder_ReturnsStateMachine()
    {
        var machine = FluentStateMachine;
        MockBuilder.Setup(b => b.Compile()).Returns(machine);
        var target = new BeforeTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.Compile();
        Assert.Same(machine, result);
    }

    [Fact]
    public void
        BeforeTransitionBuilderApi_GloballyTransitionsTo_CallsGloballyTransitionsToOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.GloballyTransitionsTo("s")).Verifiable();
        var target = new BeforeTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.GloballyTransitionsTo("s");
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeTransitionBuilderApi_GloballyTransitionsTo_NullState_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeTransitionBuilderApi<StubStateModel>(Builder).GloballyTransitionsTo(null));
    }

    [Fact]
    public void BeforeTransitionBuilderApi_State_NullName_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeTransitionBuilderApi<StubStateModel>(Builder).State(null));
    }

    [Fact]
    public void BeforeTransitionBuilderApi_State_ValidParms_CallsStateOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.State("s", null)).Verifiable();
        var target = new BeforeTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.State("s");
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeTransitionBuilderApi_State_ValidParmsWithCode_CallsStateOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.State("s", 2)).Verifiable();
        var target = new BeforeTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.State("s", 2);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }
}