﻿using System;
using Nate.Core;
using Nate.Fluent;
using Xunit;

namespace Nate.Tests.Unit.Fluent;

public class BeforeExitFluentBuilderApiTests : FluentTestBase
{
    [Fact]
    public void BeforeExitFluentBuilderApi_AfterEntry_NullCallback_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeExitFluentBuilderApi<StubStateModel>(Builder).AfterEntry(null));
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_AfterEntry_ValidParms_CallsAfterEntryOnBuilder_ReturnsApiWithBuilder()
    {
        Action<TransitionEventArgs<StubStateModel>> callback = e => { };
        MockBuilder.Setup(b => b.AfterEntry(callback)).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.AfterEntry(callback);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_AfterTransition_NullCallback_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeExitFluentBuilderApi<StubStateModel>(Builder).AfterTransition(null));
    }

    [Fact]
    public void
        BeforeExitFluentBuilderApi_AfterTransition_ValidParms_CallsAfterTransitionOnBuilder_ReturnsApiWithBuilder()
    {
        Action<TransitionEventArgs<StubStateModel>> callback = e => { };
        MockBuilder.Setup(b => b.AfterTransition(callback)).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.AfterTransition(callback);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_BeforeTransition_NullCallBack_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeExitFluentBuilderApi<StubStateModel>(Builder).BeforeTransition(null));
    }

    [Fact]
    public void
        BeforeExitFluentBuilderApi_BeforeTransition_ValidParms_CallsBeforeTransitionOnBuilder_ReturnsApiWithBuilder()
    {
        Action<TransitionEventArgs<StubStateModel>> callback = e => { };
        MockBuilder.Setup(b => b.BeforeTransition(callback)).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.BeforeTransition(callback);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_Compile_NullConfiguration_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeExitFluentBuilderApi<StubStateModel>(Builder).Compile(null));
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_Compile_ValidParms_CallsCompileOnBuilder_ReturnsStateMachine()
    {
        var machine = FluentStateMachine;
        MockBuilder.Setup(b => b.Compile()).Returns(machine);
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.Compile();
        Assert.Same(machine, result);
    }

    [Fact]
    public void
        BeforeExitFluentBuilderApi_GloballyTransitionsTo_CallsGloballyTransitionsToOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.GloballyTransitionsTo("s")).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.GloballyTransitionsTo("s");
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_GloballyTransitionsTo_NullState_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeExitFluentBuilderApi<StubStateModel>(Builder).GloballyTransitionsTo(null));
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_Initiates_CallsInitiatesOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.Initiates()).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.Initiates();
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_State_NullName_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeExitFluentBuilderApi<StubStateModel>(Builder).State(null));
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_State_ValidParms_CallsStateOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.State("s", null)).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.State("s");
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_State_ValidParmsWithCode_CallsStateOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.State("s", 2)).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.State("s", 2);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void BeforeExitFluentBuilderApi_TransitionsTo_NullName_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeExitFluentBuilderApi<StubStateModel>(Builder).TransitionsTo(null));
    }

    [Fact]
    public void
        BeforeExitFluentBuilderApi_TransitionsTo_ValidParms_CallsTransitionsToOnBuilder_ReturnsApiWithBuilder()
    {
        MockBuilder.Setup(b => b.TransitionsTo("t")).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.TransitionsTo("t");
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }

    [Fact]
    public void StateFluentBuilderApi_AfterExit_NullCallback_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeExitFluentBuilderApi<StubStateModel>(Builder).AfterExit(null));
    }

    [Fact]
    public void StateFluentBuilderApi_AfterExit_ValidParms_CallsAfterExitOnBuilder_ReturnsApiWithBuilder()
    {
        Action<TransitionEventArgs<StubStateModel>> callback = e => { };
        MockBuilder.Setup(b => b.AfterExit(callback)).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.AfterExit(callback);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }


    [Fact]
    public void StateFluentBuilderApi_BeforeEntry_NullCallback_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BeforeExitFluentBuilderApi<StubStateModel>(Builder).BeforeEntry(null));
    }

    [Fact]
    public void StateFluentBuilderApi_BeforeEntry_ValidParms_CallsBeforeEntryOnBuilder_ReturnsApiWithBuilder()
    {
        Action<TransitionEventArgs<StubStateModel>> callback = e => { };
        MockBuilder.Setup(b => b.BeforeEntry(callback)).Verifiable();
        var target = new BeforeExitFluentBuilderApi<StubStateModel>(Builder);
        var result = target.BeforeEntry(callback);
        Assert.NotNull(result);
        MockBuilder.VerifyAll();
    }
}