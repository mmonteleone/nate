using System;
using Nate.Core;
using Nate.Fluent;
using Xunit;

namespace Nate.Tests.Unit.Fluent
{
    public class OnFluentBuilderApiTests : FluentTestBase
    {
        [Fact]
        public void OnFluentBuilderApi_AfterEntry_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).AfterEntry(null));
        }

        [Fact]
        public void OnFluentBuilderApi_AfterEntry_ValidParms_CallsAfterEntryOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.AfterEntry(callback)).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.AfterEntry(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void OnFluentBuilderApi_AfterTransition_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).AfterTransition(null));
        }

        [Fact]
        public void OnFluentBuilderApi_AfterTransition_ValidParms_CallsAfterTransitionOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.AfterTransition(callback)).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.AfterTransition(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void OnFluentBuilderApi_BeforeExit_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).BeforeExit(null));
        }

        [Fact]
        public void OnFluentBuilderApi_BeforeExit_ValidParms_CallsBeforeExitOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.BeforeExit(callback)).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.BeforeExit(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void OnFluentBuilderApi_BeforeTransition_NullCallBack_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).BeforeTransition(null));
        }

        [Fact]
        public void
            OnFluentBuilderApi_BeforeTransition_ValidParms_CallsBeforeTransitionOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.BeforeTransition(callback)).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.BeforeTransition(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void OnFluentBuilderApi_GloballyTransitionsTo_CallsGloballyTransitionsToOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.GloballyTransitionsTo("s")).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.GloballyTransitionsTo("s");
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void OnFluentBuilderApi_GloballyTransitionsTo_NullState_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).GloballyTransitionsTo(null));
        }

        [Fact]
        public void OnFluentBuilderApi_Initiates_CallsInitiatesOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.Initiates()).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.Initiates();
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void OnFluentBuilderApi_State_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).State(null));
        }

        [Fact]
        public void OnFluentBuilderApi_State_ValidParms_CallsStateOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.State("s", null)).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.State("s");
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void OnFluentBuilderApi_State_ValidParmsWithCode_CallsStateOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.State("s", 2)).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.State("s", 2);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void OnFluentBuilderApi_TransitionsTo_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).TransitionsTo(null));
        }

        [Fact]
        public void OnFluentBuilderApi_TransitionsTo_ValidParms_CallsTransitionsToOnBuilder_ReturnsApiWithBuilder()
        {
            MockBuilder.Setup(b => b.TransitionsTo("t")).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.TransitionsTo("t");
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void OnFluentBuilderApi_When_NullGuard_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).When(null));
        }

        [Fact]
        public void OnFluentBuilderApi_When_ValidParms_CallsWhenOnBuilder_ReturnsApiWithBuilder()
        {
            Func<StubStateModel, bool> guard = s => true;
            MockBuilder.Setup(b => b.When(guard)).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.When(guard);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }

        [Fact]
        public void StateFluentBuilderApi_AfterExit_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).AfterExit(null));
        }

        [Fact]
        public void StateFluentBuilderApi_AfterExit_ValidParms_CallsAfterExitOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.AfterExit(callback)).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.AfterExit(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }


        [Fact]
        public void StateFluentBuilderApi_BeforeEntry_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OnFluentBuilderApi<StubStateModel>(Builder).BeforeEntry(null));
        }

        [Fact]
        public void StateFluentBuilderApi_BeforeEntry_ValidParms_CallsBeforeEntryOnBuilder_ReturnsApiWithBuilder()
        {
            Action<TransitionEventArgs<StubStateModel>> callback = e => { };
            MockBuilder.Setup(b => b.BeforeEntry(callback)).Verifiable();
            var target = new OnFluentBuilderApi<StubStateModel>(Builder);
            var result = target.BeforeEntry(callback);
            Assert.NotNull(result);
            MockBuilder.VerifyAll();
        }
    }
}