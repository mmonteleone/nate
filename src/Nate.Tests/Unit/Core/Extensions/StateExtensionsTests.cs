using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Xunit;
using Nate.Core;

namespace Nate.Tests.Unit.Core.Extensions
{
    public class StateExtensionsTests
    {
        [Fact]
        public void StateExtensions_AddTransition_NullState_ThrowsNullex()
        {
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Assert.Throws<ArgumentNullException>(() =>
                StateExtensions.AddTransition((State<StubStateModel>)null, trigger, toState));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTrigger_ThrowsNullex()
        {
            var fromState = new State<StubStateModel>("from");
            var toState = new State<StubStateModel>("to");
            Assert.Throws<ArgumentNullException>(() =>
                StateExtensions.AddTransition(fromState, (Trigger)null, toState));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTo_ThrowsNullex()
        {
            var fromState = new State<StubStateModel>("from");
            var trigger = new Trigger("trigger");
            Assert.Throws<ArgumentNullException>(() =>
                StateExtensions.AddTransition(fromState, trigger, (State<StubStateModel>)null));
        }

        [Fact]
        public void StateExtensions_AddTransition_ValidParms_CallsAddOnFromState()
        {
            var fromStateMock = new Mock<State<StubStateModel>>("from");
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            fromStateMock.Setup(s => s.AddTransition(It.Is<Transition<StubStateModel>>(
                t => t.Source.Name == "from" && t.Target.Name == "to" && t.Trigger.Name == "trigger"))).Verifiable();
            StateExtensions.AddTransition(fromStateMock.Object, trigger, toState);
            fromStateMock.VerifyAll();
        }


        [Fact]
        public void StateExtensions_AddTransition_NullState_WithGuard_ThrowsNullex()
        {
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = s => true;
            Assert.Throws<ArgumentNullException>(() =>
                StateExtensions.AddTransition((State<StubStateModel>)null, trigger, toState, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTrigger_WithGuard_ThrowsNullex()
        {
            var fromState = new State<StubStateModel>("from");
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = s => true;
            Assert.Throws<ArgumentNullException>(() =>
                StateExtensions.AddTransition(fromState, (Trigger)null, toState, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTo_WithGuard_ThrowsNullex()
        {
            var fromState = new State<StubStateModel>("from");
            var trigger = new Trigger("trigger");
            Func<StubStateModel, bool> guard = s => true;
            Assert.Throws<ArgumentNullException>(() =>
                StateExtensions.AddTransition(fromState, trigger, (State<StubStateModel>)null, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullGuard_ThrowsNullex()
        {
            var fromState = new State<StubStateModel>("from");
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = null;
            Assert.Throws<ArgumentNullException>(() =>
                StateExtensions.AddTransition(fromState, trigger, (State<StubStateModel>)null, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_ValidParms_WithGuard_CallsAddOnFromState()
        {
            var fromStateMock = new Mock<State<StubStateModel>>("from");
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = s => true;
            fromStateMock.Setup(s => s.AddTransition(It.Is<Transition<StubStateModel>>(
                t => t.Source.Name == "from" && t.Target.Name == "to" && t.Trigger.Name == "trigger" && t.Guard == guard))).Verifiable();
            StateExtensions.AddTransition(fromStateMock.Object, trigger, toState, guard);
            fromStateMock.VerifyAll();
        }
    }
}
