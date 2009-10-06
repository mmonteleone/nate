using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Nate.Core;
using Nate.Fluent;
using Moq;

namespace Nate.Tests.Integration
{
    public class FluentStateMachineBuilderTests
    {
        [Fact]
        public void FluentStateMachineBuilder_GloballyTransitionsTo_NullState_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FluentStateMachine<StubStateModel>.Describe()
                    .State("s1")
                    .GloballyTransitionsTo(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_GloballyTransitionsTo_AddsAGlobalTransition()
        {
            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                .State("s2").Initiates()
                .GloballyTransitionsTo("s1").On("t1")
                .GloballyTransitionsTo("s2").On("t2")
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            Assert.Equal("s2",model.CurrentState.ToString());
            machine.Trigger("t1", model);
            Assert.Equal("s1", model.CurrentState.ToString());
            machine.Trigger("t2", model);
            Assert.Equal("s2", model.CurrentState.ToString());
        }

        [Fact]
        public void FluentStateMachineBuilder_Initiates_SetsCurrentStateToInitial()
        {
            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                .State("s2").Initiates()
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;
            
            Assert.Equal("s2", model.CurrentState.ToString());
        }

        [Fact]
        public void FluentStateMachineBuilder_Initiates_NotCurrentlyDefiningState_ThrowsTriggerEx()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            Assert.Throws<FluentSyntaxException>(() =>
                builder.Initiates());
        }

        [Fact]
        public void FluentStateMachineBuilder_On_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).On(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_On_SetsTriggerForATransition()
        {
            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .TransitionsTo("s2").On("t")
                    .Initiates()
                .State("s2")
                    .TransitionsTo("s1").On("t")
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            Assert.Equal("s1", model.CurrentState.ToString());
            machine.Trigger("t", model);
            Assert.Equal("s2", model.CurrentState.ToString());
        }

        [Fact]
        public void FluentStateMachineBuilder_On_NotCurrentlyDefiningTransition_ThrowsFluentSyntaxException()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            Assert.Throws<FluentSyntaxException>(() =>
                builder.On("s"));
        }

        [Fact]
        public void FluentStateMachineBuilder_State_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).State(null, null));
        }

        [Fact]
        public void FluentStateMachineBuilder_State_ValidParms_CreatesNewState()
        {
            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                .Initiates()
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            Assert.Equal("s1", model.CurrentState.ToString());
        }

        [Fact]
        public void FluentStateMachineBuilder_State_DefiningTransition_ThrowsFluentSyntaxException()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.TransitionsTo("t1");
            // shouldn't allow new state definitions while in middle of defining a transition
            Assert.Throws<FluentSyntaxException>(() =>
                builder.State("s2", null));
        }

        [Fact]
        public void FluentStateMachineBuilder_TransitionsTo_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).TransitionsTo(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_TransitionsTo_Valid_CreatesNewTransition()
        {
            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .Initiates()
                    .TransitionsTo("s2").On("t")
                .State("s2")
                    .TransitionsTo("s1").On("t")
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            Assert.Equal("s1", model.CurrentState.ToString());
            machine.Trigger("t", model);
            Assert.Equal("s2", model.CurrentState.ToString());
            machine.Trigger("t", model);
            Assert.Equal("s1", model.CurrentState.ToString());
        }

        [Fact]
        public void FluentStateMachineBuilder_TransitionsTo_WhileAlreadyBuildingTransition_ThrowsFluentSyntaxEx()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1",null);
            builder.TransitionsTo("s2");
            // shouldn't allow another definition of transition while in middle of transition definition
            Assert.Throws<FluentSyntaxException>(() =>
                builder.TransitionsTo("s3"));
        }

        [Fact]
        public void FluentStateMachineBuilder_When_NullGuard_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).When(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_When_WhileDefTrans_AddsGuardToTransition()
        {
            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .Initiates()
                    .TransitionsTo("s2").On("t").When(m => false)
                    .TransitionsTo("s3").On("t").When(m => true)
                .State("s2")
                .State("s3")
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            Assert.Equal("s1", model.CurrentState.ToString());
            machine.Trigger("t", model);
            Assert.Equal("s3", model.CurrentState.ToString());
        }

        [Fact]
        public void FluentStateMachineBuilder_When_WhileNotDefTrans_ThrowsFluentSyntaxEx()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            // shouldn't allow guard definition while not even defining a transition
            Assert.Throws<FluentSyntaxException>(() =>
                builder.When(m => false));
        }

        [Fact]
        public void FluentStateMachineBuilder_Compile_NoStatements_ThrowsFluentSyntaxEx()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            Assert.Throws<FluentSyntaxException>(() =>
                builder.Compile());
        }

        [Fact]
        public void FluentStateMachineBuilder_Compile_ReturnsNewStateMachineWithDefinedParts()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.Initiates();
            var machine = builder.Compile();
            Assert.NotNull(machine);
            Assert.Equal("s1", machine.InitialState.ToString());
        }

        [Fact]
        public void FluentStateMachineBuilder_CompileConfig_NullConfig_ThrowsNullEx()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.Initiates();
            Assert.Throws<ArgumentNullException>(() =>
                builder.Compile(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_CompileConfig_ReturnsNewStateMachineWithDefinedParts_WithConfig()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.Initiates();
            var config = new StateMachineConfiguration { RaiseExceptionBeforeTransitionToSameState = true };
            var machine = builder.Compile(config);

            Assert.NotNull(machine);
            Assert.Same(config, machine.Configuration);
            Assert.Equal(true, machine.Configuration.RaiseExceptionBeforeTransitionToSameState);
        }

        [Fact]
        public void FluentStateMachineBuilder_DefiningCallbacksOnAll_AllCallbacksCalledInOrder()
        {
            var loggedEvents = new List<string>();
            Action<string, TransitionEventArgs<StubStateModel>> logEvent = (message, e) =>
            {
                loggedEvents.Add(String.Format("{0} from {1} to {2} on {3}",
                        message, e.From, e.To, e.Trigger));
            };

            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .BeforeEntry(e => logEvent("beforeEntry", e))
                    .AfterEntry(e => logEvent("afterEntry", e))
                    .BeforeExit(e => logEvent("beforeExit", e))
                    .AfterExit(e => logEvent("afterExit", e))
                    .TransitionsTo("s2").On("t")
                    .Initiates()
                .State("s2")
                    .BeforeEntry(e => logEvent("beforeEntry", e))
                    .AfterEntry(e => logEvent("afterEntry", e))
                    .BeforeExit(e => logEvent("beforeExit", e))
                    .AfterExit(e => logEvent("afterExit", e))
                    .TransitionsTo("s1").On("t")
                .BeforeTransition(e => logEvent("beforeTransition", e))
                .AfterTransition(e => logEvent("afterTransition", e))
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            machine.Trigger("t", model);
            Assert.Equal("s2", model.CurrentState.ToString());
            Assert.Equal(6, loggedEvents.Count());
            Assert.Equal("beforeTransition from s1 to s2 on t", loggedEvents[0]);
            Assert.Equal("beforeExit from s1 to s2 on t", loggedEvents[1]);
            Assert.Equal("beforeEntry from s1 to s2 on t", loggedEvents[2]);
            Assert.Equal("afterExit from s1 to s2 on t", loggedEvents[3]);
            Assert.Equal("afterEntry from s1 to s2 on t", loggedEvents[4]);
            Assert.Equal("afterTransition from s1 to s2 on t", loggedEvents[5]);
        }


        [Fact]
        public void FluentStateMachineBuilder_BeforeEntry_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).BeforeEntry(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_BeforeEntry_DefiningState_AddsEvent()
        {
            var loggedEvents = new List<string>();
            Action<string, TransitionEventArgs<StubStateModel>> logEvent = (message, e) =>
            {
                loggedEvents.Add(String.Format("{0} from {1} to {2} on {3}",
                        message, e.From, e.To, e.Trigger));
            };

            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .BeforeEntry(e => logEvent("beforeEntry", e))
                    .TransitionsTo("s2").On("t")
                    .Initiates()
                .State("s2")
                    .BeforeEntry(e => logEvent("beforeEntry", e))
                    .TransitionsTo("s1").On("t")
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            machine.Trigger("t", model);
            Assert.Equal("s2", model.CurrentState.ToString());
            Assert.Equal(1, loggedEvents.Count());
            Assert.Equal("beforeEntry from s1 to s2 on t", loggedEvents[0]);
        }

        [Fact]
        public void FluentStateMachineBuilder_BeforeEntry_NotDefiningState_ThrowsFluentSyntaxException()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.TransitionsTo("s2");
            // should nto allow before entry definition while in middle of defining a transition
            Assert.Throws<FluentSyntaxException>(() =>
                builder.BeforeEntry(e => { }));
        }


        [Fact]
        public void FluentStateMachineBuilder_BeforeExit_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).BeforeExit(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_BeforeExit_DefiningState_AddsEvent()
        {
            var loggedEvents = new List<string>();
            Action<string, TransitionEventArgs<StubStateModel>> logEvent = (message, e) =>
            {
                loggedEvents.Add(String.Format("{0} from {1} to {2} on {3}",
                        message, e.From, e.To, e.Trigger));
            };

            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .BeforeExit(e => logEvent("beforeExit", e))
                    .TransitionsTo("s2").On("t")
                    .Initiates()
                .State("s2")
                    .BeforeExit(e => logEvent("beforeExit", e))
                    .TransitionsTo("s1").On("t")
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            machine.Trigger("t", model);
            Assert.Equal("s2", model.CurrentState.ToString());
            Assert.Equal(1, loggedEvents.Count());
            Assert.Equal("beforeExit from s1 to s2 on t", loggedEvents[0]);
        }

        [Fact]
        public void FluentStateMachineBuilder_BeforeExit_NotDefiningState_ThrowsFluentSyntaxException()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.TransitionsTo("s2");
            // should nto allow before Exit definition while in middle of defining a transition
            Assert.Throws<FluentSyntaxException>(() =>
                builder.BeforeExit(e => { }));
        }


        [Fact]
        public void FluentStateMachineBuilder_AfterExit_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).AfterExit(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_AfterExit_DefiningState_AddsEvent()
        {
            var loggedEvents = new List<string>();
            Action<string, TransitionEventArgs<StubStateModel>> logEvent = (message, e) =>
            {
                loggedEvents.Add(String.Format("{0} from {1} to {2} on {3}",
                        message, e.From, e.To, e.Trigger));
            };

            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .AfterExit(e => logEvent("AfterExit", e))
                    .TransitionsTo("s2").On("t")
                    .Initiates()
                .State("s2")
                    .AfterExit(e => logEvent("AfterExit", e))
                    .TransitionsTo("s1").On("t")
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            machine.Trigger("t", model);
            Assert.Equal("s2", model.CurrentState.ToString());
            Assert.Equal(1, loggedEvents.Count());
            Assert.Equal("AfterExit from s1 to s2 on t", loggedEvents[0]);
        }

        [Fact]
        public void FluentStateMachineBuilder_AfterExit_NotDefiningState_ThrowsFluentSyntaxException()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.TransitionsTo("s2");
            // should nto allow After Exit definition while in middle of defining a transition
            Assert.Throws<FluentSyntaxException>(() =>
                builder.AfterExit(e => { }));
        }


        [Fact]
        public void FluentStateMachineBuilder_AfterEntry_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).AfterEntry(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_AfterEntry_DefiningState_AddsEvent()
        {
            var loggedEvents = new List<string>();
            Action<string, TransitionEventArgs<StubStateModel>> logEvent = (message, e) =>
            {
                loggedEvents.Add(String.Format("{0} from {1} to {2} on {3}",
                        message, e.From, e.To, e.Trigger));
            };

            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .AfterEntry(e => logEvent("AfterEntry", e))
                    .TransitionsTo("s2").On("t")
                    .Initiates()
                .State("s2")
                    .AfterEntry(e => logEvent("AfterEntry", e))
                    .TransitionsTo("s1").On("t")
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            machine.Trigger("t", model);
            Assert.Equal("s2", model.CurrentState.ToString());
            Assert.Equal(1, loggedEvents.Count());
            Assert.Equal("AfterEntry from s1 to s2 on t", loggedEvents[0]);
        }

        [Fact]
        public void FluentStateMachineBuilder_AfterEntry_NotDefiningState_ThrowsFluentSyntaxException()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.TransitionsTo("s2");
            // should nto allow After Exit definition while in middle of defining a transition
            Assert.Throws<FluentSyntaxException>(() =>
                builder.AfterEntry(e => { }));
        }


        [Fact]
        public void FluentStateMachineBuilder_BeforeTransition_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).BeforeTransition(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_BeforeTransition_DefiningState_AddsEvent()
        {
            var loggedEvents = new List<string>();
            Action<string, TransitionEventArgs<StubStateModel>> logEvent = (message, e) =>
            {
                loggedEvents.Add(String.Format("{0} from {1} to {2} on {3}",
                        message, e.From, e.To, e.Trigger));
            };

            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .TransitionsTo("s2").On("t")
                    .Initiates()
                .State("s2")
                    .TransitionsTo("s1").On("t")
                .BeforeTransition(e => logEvent("BeforeTransition", e))
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            machine.Trigger("t", model);
            Assert.Equal("s2", model.CurrentState.ToString());
            Assert.Equal(1, loggedEvents.Count());
            Assert.Equal("BeforeTransition from s1 to s2 on t", loggedEvents[0]);
        }

        [Fact]
        public void FluentStateMachineBuilder_BeforeTransition_NotDefiningState_ThrowsFluentSyntaxException()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.TransitionsTo("s2");
            // should nto allow After Exit definition while in middle of defining a transition
            Assert.Throws<FluentSyntaxException>(() =>
                builder.BeforeTransition(e => { }));
        }


        [Fact]
        public void FluentStateMachineBuilder_AfterTransition_NullCallback_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                (new FluentStateMachineBuilder<StubStateModel>()).AfterTransition(null));
        }

        [Fact]
        public void FluentStateMachineBuilder_AfterTransition_DefiningState_AddsEvent()
        {
            var loggedEvents = new List<string>();
            Action<string, TransitionEventArgs<StubStateModel>> logEvent = (message, e) =>
            {
                loggedEvents.Add(String.Format("{0} from {1} to {2} on {3}",
                        message, e.From, e.To, e.Trigger));
            };

            var machine = FluentStateMachine<StubStateModel>.Describe()
                .State("s1")
                    .TransitionsTo("s2").On("t")
                    .Initiates()
                .State("s2")
                    .TransitionsTo("s1").On("t")
                .AfterTransition(e => logEvent("AfterTransition", e))
                .Compile();

            var model = new StubStateModel();
            model.CurrentState = machine.InitialState;

            machine.Trigger("t", model);
            Assert.Equal("s2", model.CurrentState.ToString());
            Assert.Equal(1, loggedEvents.Count());
            Assert.Equal("AfterTransition from s1 to s2 on t", loggedEvents[0]);
        }

        [Fact]
        public void FluentStateMachineBuilder_AfterTransition_NotDefiningState_ThrowsFluentSyntaxException()
        {
            var builder = new FluentStateMachineBuilder<StubStateModel>();
            builder.State("s1", null);
            builder.TransitionsTo("s2");
            // should nto allow After Exit definition while in middle of defining a transition
            Assert.Throws<FluentSyntaxException>(() =>
                builder.AfterTransition(e => { }));
        }


    }
}
