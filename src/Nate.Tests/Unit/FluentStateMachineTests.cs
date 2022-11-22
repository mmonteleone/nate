using System;
using System.Collections.Generic;
using Moq;
using Nate.Core;
using Xunit;

namespace Nate.Tests.Unit
{
    public class FluentStateMachineTests
    {
        [Fact]
        public void FluentStateMachine_AvailableStates_NullModel_ThrowsNullEx()
        {
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);

            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machineMock.Object,
                allStates,
                state2,
                null,
                null,
                null);

            Assert.Throws<ArgumentNullException>(() =>
                fluentMachine.AvailableStates(null));
        }

        [Fact]
        public void FluentStateMachine_AvailableStates_ValidParms_CallsReturnsMachineAvailable()
        {
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);

            var model = new StubStateModel();


            machineMock.Setup(m => m.AvailableStates(model)).Returns(allStates).Verifiable();

            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machineMock.Object,
                allStates,
                state2,
                null,
                null,
                null);

            var result = fluentMachine.AvailableStates(model);

            Assert.Same(allStates, result);
            machineMock.VerifyAll();
        }

        [Fact]
        public void FluentStateMachine_Describe_ReturnsNewBuilderApi()
        {
            var result = FluentStateMachine<StubStateModel>.Describe();
            var result2 = FluentStateMachine<StubStateModel>.Describe();
            Assert.NotNull(result);
            Assert.NotNull(result);
            Assert.NotSame(result, result2);
        }

        [Fact]
        public void FluentStateMachine_GlobalTransition_AddsEachOneToMachine()
        {
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");
            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);

            // create some global trans
            var transition1 = new Transition<StubStateModel>(trigger1, null, state2);
            var transition2 = new Transition<StubStateModel>(trigger2, null, state3);
            // put them in an ienum
            var transitions = new List<Transition<StubStateModel>>();
            transitions.Add(transition1);
            transitions.Add(transition2);

            // set up verifications for items being added to machine mock
            var globalTransitionsAdded = new List<Transition<StubStateModel>>();
            machineMock.Setup(s => s.AddGlobalTransition(It.IsAny<Transition<StubStateModel>>()))
                .Callback<Transition<StubStateModel>>(t => globalTransitionsAdded.Add(t));

            // create a fluent machine, and validate each was added to machine

            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machineMock.Object,
                allStates,
                null,
                null,
                null,
                transitions);

            Assert.Equal(2, globalTransitionsAdded.Count);
            Assert.Equal(transition1, globalTransitionsAdded[0]);
            Assert.Equal(transition2, globalTransitionsAdded[1]);
        }

        [Fact]
        public void FluentStateMachine_GlobalTransitioneds_AddsEachOneToMachine()
        {
            var machine = new StateMachine<StubStateModel>();
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");
            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);

            // transition arg
            var arg = new TransitionEventArgs<StubStateModel>(new StubStateModel(),
                state1, state2, trigger1);

            // create some global transitionings
            Action<TransitionEventArgs<StubStateModel>> callback1 = e => { };
            Action<TransitionEventArgs<StubStateModel>> callback2 = e => { };
            Action<TransitionEventArgs<StubStateModel>> callback3 = e => { };

            // put them in an ienum
            var callbacks = new List<Action<TransitionEventArgs<StubStateModel>>>();
            callbacks.Add(callback1);
            callbacks.Add(callback2);
            callbacks.Add(callback3);


            // create a fluent machine, and validate each was added to machine
            //machineMock.Raise(m => m.Transitioning += null, arg);
            //machineMock.SetupAllProperties();

            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machineMock.Object,
                allStates,
                null,
                null,
                callbacks,
                null);

            //machineMock.VerifyAll();
        }

        [Fact]
        public void FluentStateMachine_GlobalTransitionings_AddsEachOneToMachine()
        {
            var machine = new StateMachine<StubStateModel>();
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");
            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);

            // transition arg
            var arg = new TransitionEventArgs<StubStateModel>(new StubStateModel(),
                state1, state2, trigger1);

            // create some global transitionings
            Action<TransitionEventArgs<StubStateModel>> callback1 = e => { };
            Action<TransitionEventArgs<StubStateModel>> callback2 = e => { };
            Action<TransitionEventArgs<StubStateModel>> callback3 = e => { };

            // put them in an ienum
            var callbacks = new List<Action<TransitionEventArgs<StubStateModel>>>();
            callbacks.Add(callback1);
            callbacks.Add(callback2);
            callbacks.Add(callback3);


            // create a fluent machine, and validate each was added to machine
            //machineMock.Raise(m => m.Transitioning += null, arg);
            machineMock.SetupAllProperties();

            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machineMock.Object,
                allStates,
                null,
                callbacks,
                null,
                null);

            //machineMock.VerifyAll();
        }

        [Fact]
        public void FluentStateMachine_InitialState_SameValueAsConstructor()
        {
            var machine = new StateMachine<StubStateModel>();
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");
            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);


            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machineMock.Object,
                allStates,
                state2,
                null,
                null,
                null);

            Assert.Same(state2, fluentMachine.InitialState);
        }

        [Fact]
        public void FluentStateMachine_NullMachine_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FluentStateMachine<StubStateModel>(
                    null,
                    new Mock<IEnumerable<State<StubStateModel>>>().Object,
                    new State<StubStateModel>("state"),
                    new Mock<IEnumerable<Action<TransitionEventArgs<StubStateModel>>>>().Object,
                    new Mock<IEnumerable<Action<TransitionEventArgs<StubStateModel>>>>().Object,
                    new Mock<IEnumerable<Transition<StubStateModel>>>().Object));
        }

        [Fact]
        public void FluentStateMachine_NullStates_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FluentStateMachine<StubStateModel>(
                    new Mock<IStateMachine<StubStateModel>>().Object,
                    null,
                    new State<StubStateModel>("state"),
                    new Mock<IEnumerable<Action<TransitionEventArgs<StubStateModel>>>>().Object,
                    new Mock<IEnumerable<Action<TransitionEventArgs<StubStateModel>>>>().Object,
                    new Mock<IEnumerable<Transition<StubStateModel>>>().Object));
        }

        [Fact]
        public void FluentStateMachine_StateCoded_ValidParms_ReturnsMatchingStateFromConstructor()
        {
            var machine = new StateMachine<StubStateModel>();
            var state1 = new State<StubStateModel>("s1", 1);
            var state2 = new State<StubStateModel>("s2", 2);
            var state3 = new State<StubStateModel>("s3", 3);

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);

            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machine,
                allStates,
                state2,
                null,
                null,
                null);

            Assert.Equal(state1, fluentMachine.StateCoded(1));
            Assert.Equal(state2, fluentMachine.StateCoded(2));
            Assert.Equal(state3, fluentMachine.StateCoded(3));
        }

        [Fact]
        public void FluentStateMachine_StateNamed_NullName_ThrowsNullEx()
        {
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);

            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machineMock.Object,
                allStates,
                state2,
                null,
                null,
                null);

            Assert.Throws<ArgumentNullException>(() =>
                fluentMachine.StateNamed(null));
        }

        [Fact]
        public void FluentStateMachine_StateNamed_ValidParms_ReturnsMatchingStateFromConstructor()
        {
            var machine = new StateMachine<StubStateModel>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);

            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machine,
                allStates,
                state2,
                null,
                null,
                null);

            Assert.Equal(state1, fluentMachine.StateNamed("s1"));
            Assert.Equal(state2, fluentMachine.StateNamed("s2"));
            Assert.Equal(state3, fluentMachine.StateNamed("s3"));
        }

        [Fact]
        public void FluentStateMachine_Trigger_NullModel_ThrowsNullEx()
        {
            var machine = new StateMachine<StubStateModel>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);


            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machine,
                allStates,
                state2,
                null,
                null,
                null);

            Assert.Throws<ArgumentNullException>(() =>
                fluentMachine.Trigger("trigger", null));
        }

        [Fact]
        public void FluentStateMachine_Trigger_NullName_ThrowsNullEx()
        {
            var machine = new StateMachine<StubStateModel>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);


            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machine,
                allStates,
                state2,
                null,
                null,
                null);

            Assert.Throws<ArgumentNullException>(() =>
                fluentMachine.Trigger(null, new StubStateModel()));
        }

        [Fact]
        public void FluentStateMachine_Trigger_ValidParms_CallsReturnsMachineTrigger()
        {
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");

            var allStates = new List<State<StubStateModel>>();
            allStates.Add(state1);
            allStates.Add(state2);
            allStates.Add(state3);

            var model = new StubStateModel();


            var fluentMachine = new FluentStateMachine<StubStateModel>(
                machineMock.Object,
                allStates,
                state2,
                null,
                null,
                null);

            machineMock.Setup(m => m.Trigger(It.Is<Trigger>(t => t.Name == "trigger1"), model)).Verifiable();

            fluentMachine.Trigger("trigger1", model);

            machineMock.VerifyAll();
        }
    }
}