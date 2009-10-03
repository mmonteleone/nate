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
    public class BeforeEntryFluentBuilderApiTests : FluentTestBase
    {
        [Fact]
        public void BeforeEntryFluentBuilderApi_BeforeExit_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new BeforeEntryFluentBuilderApi<StubStateModel>(Builder)).BeforeExit(null));
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_BeforeExit_ValidParms_CallsBeforeExitOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.BeforeExit(callback)).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.BeforeExit(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_TransitionsTo_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new BeforeEntryFluentBuilderApi<StubStateModel>(Builder)).TransitionsTo(null));
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_TransitionsTo_ValidParms_CallsTransitionsToOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.TransitionsTo("t")).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.TransitionsTo("t");
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_Initiates_CallsInitiatesOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.Initiates()).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.Initiates();
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_State_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new BeforeEntryFluentBuilderApi<StubStateModel>(Builder)).State(null));
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_State_ValidParms_CallsStateOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.State("s", null)).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.State("s");
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_State_ValidParmsWithCode_CallsStateOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.State("s", 2)).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.State("s", 2);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_Compile_NullConfiguration_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new BeforeEntryFluentBuilderApi<StubStateModel>(Builder)).Compile(null));
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_Compile_ValidParms_CallsCompileOnBuilder_ReturnsStateMachine()
        {
            var machine = FluentStateMachine;
            MockBuilder.Setup(b => b.Compile()).Returns(machine);
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.Compile();
            Assert.Same(machine, result);
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_BeforeTransition_NullCallBack_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new BeforeEntryFluentBuilderApi<StubStateModel>(Builder)).BeforeTransition(null));
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_BeforeTransition_ValidParms_CallsBeforeTransitionOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.BeforeTransition(callback)).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.BeforeTransition(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_AfterTransition_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new BeforeEntryFluentBuilderApi<StubStateModel>(Builder)).AfterTransition(null));
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_AfterTransition_ValidParms_CallsAfterTransitionOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.AfterTransition(callback)).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.AfterTransition(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_GloballyTransitionsTo_NullState_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new BeforeEntryFluentBuilderApi<StubStateModel>(Builder)).GloballyTransitionsTo(null));
        }

        [Fact]
        public void BeforeEntryFluentBuilderApi_GloballyTransitionsTo_CallsGloballyTransitionsToOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.GloballyTransitionsTo("s")).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.GloballyTransitionsTo("s");
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }


        [Fact]
        public void StateFluentBuilderApi_AfterEntry_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new BeforeEntryFluentBuilderApi<StubStateModel>(Builder)).AfterEntry(null));

        }

        [Fact]
        public void StateFluentBuilderApi_AfterEntry_ValidParms_CallsAfterEntryOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.AfterEntry(callback)).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.AfterEntry(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void StateFluentBuilderApi_AfterExit_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new BeforeEntryFluentBuilderApi<StubStateModel>(Builder)).AfterExit(null));
        }

        [Fact]
        public void StateFluentBuilderApi_AfterExit_ValidParms_CallsAfterExitOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.AfterExit(callback)).Verifiable();
            var target = new BeforeEntryFluentBuilderApi<StubStateModel>(Builder);
            var result = target.AfterExit(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

    }
}
