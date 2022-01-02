using System;
using Nate.Core;
using Nate.Fluent;
using Xunit;

namespace Nate.Tests.Unit.Fluent;

public class AfterTransitionBuilderApiTests : FluentTestBase
{
    [Fact]
    public void AfterTransitionBuilderApi_BeforeTransition_NullCallBack_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AfterTransitionBuilderApi<StubStateModel>(Builder).BeforeTransition(null));
    }

    [Fact]
    public void
        AfterTransitionBuilderApi_BeforeTransition_ValidParms_CallsBeforeTransitionOnBuilder_ReturnsApiWithBuilder()
    {
        Action<TransitionEventArgs<StubStateModel>> callback = e => { };
        MockBuilder.Setup(b => b.BeforeTransition(callback)).Verifiable();
        var target = new AfterTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.BeforeTransition(callback);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void AfterTransitionBuilderApi_Compile_NullConfiguration_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AfterTransitionBuilderApi<StubStateModel>(Builder).Compile(null));
    }

    [Fact]
    public void AfterTransitionBuilderApi_Compile_ValidParms_CallsCompileOnBuilder_ReturnsStateMachine()
    {
        var machine = FluentStateMachine;
        MockBuilder.Setup(b => b.Compile()).Returns(machine);
        var target = new AfterTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.Compile();
        Assert.Same(machine, result);
    }

    [Fact]
    public void
        AfterTransitionBuilderApi_GloballyTransitionsTo_CallsGloballyTransitionsToOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.GloballyTransitionsTo("s")).Verifiable();
        var target = new AfterTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.GloballyTransitionsTo("s");
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void AfterTransitionBuilderApi_GloballyTransitionsTo_NullState_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AfterTransitionBuilderApi<StubStateModel>(Builder).GloballyTransitionsTo(null));
    }

    [Fact]
    public void AfterTransitionBuilderApi_State_NullName_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new AfterTransitionBuilderApi<StubStateModel>(Builder).State(null));
    }

    [Fact]
    public void AfterTransitionBuilderApi_State_ValidParms_CallsStateOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.State("s", null)).Verifiable();
        var target = new AfterTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.State("s");
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void AfterTransitionBuilderApi_State_ValidParmsWithCode_CallsStateOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.State("s", 2)).Verifiable();
        var target = new AfterTransitionBuilderApi<StubStateModel>(Builder);
        var result = target.State("s", 2);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }
}