#region license
/* Nate
 * http://github.com/mmonteleone/nate
 * 
 * Copyright (C) 2009 Michael Monteleone (http://michaelmonteleone.net)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 */ 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;
using Moq;
using Xunit;

namespace Nate.Tests.Unit.Core.Extensions
{
    public class IStateMachineExtensionsTests
    {
        [Fact]
        public void StateExtensions_AddTransition_NullStateMachine_ThrowsNullex()
        {
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.AddGlobalTransition((IStateMachine<StubStateModel>)null, trigger, toState));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTrigger_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var toState = new State<StubStateModel>("to");
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.AddGlobalTransition(machine, (Trigger)null, toState));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTo_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var trigger = new Trigger("trigger");
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.AddGlobalTransition(machine, trigger, (State<StubStateModel>)null));
        }

        [Fact]
        public void StateExtensions_AddTransition_ValidParms_CallsAddOnFromState()
        {
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            machineMock.Setup(s => s.AddGlobalTransition(It.Is<Transition<StubStateModel>>(
                t => t.Target.Name == "to" && t.Trigger.Name == "trigger"))).Verifiable();
            IStateMachineExtensions.AddGlobalTransition(machineMock.Object, trigger, toState);
            machineMock.VerifyAll();
        }


        [Fact]
        public void StateExtensions_AddTransition_NullStateMachine_WithGuard_ThrowsNullex()
        {
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = s => true;
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.AddGlobalTransition((IStateMachine<StubStateModel>)null, trigger, toState, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTrigger_WithGuard_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = s => true;
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.AddGlobalTransition(machine, (Trigger)null, toState, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTo_WithGuard_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var trigger = new Trigger("trigger");
            Func<StubStateModel, bool> guard = s => true;
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.AddGlobalTransition(machine, trigger, (State<StubStateModel>)null, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullGuard_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = null;
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.AddGlobalTransition(machine, trigger, (State<StubStateModel>)null, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_ValidParms_WithGuard_CallsAddOnFromState()
        {
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = s => true;
            machineMock.Setup(s => s.AddGlobalTransition(It.Is<Transition<StubStateModel>>(
                t => t.Target.Name == "to" && t.Trigger.Name == "trigger" && t.Guard == guard))).Verifiable();
            IStateMachineExtensions.AddGlobalTransition(machineMock.Object, trigger, toState, guard);
            machineMock.VerifyAll();
        }

        [Fact]
        public void IStateMachineExtensions_NullMachine_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.Trigger((IStateMachine<StubStateModel>)null, "trigger", new StubStateModel()));
        }

        [Fact]
        public void IStateMachineExtensions_NullTriggerName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.Trigger(new Mock<IStateMachine<StubStateModel>>().Object, null, new StubStateModel()));
        }

        [Fact]
        public void IStateMachineExtensions_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                IStateMachineExtensions.Trigger(new Mock<IStateMachine<StubStateModel>>().Object, "trigger", null));
        }

        [Fact]
        public void IStateMachineExtensinos_ValidParms_CallsTriggerOnMachineWithParms()
        {
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            var mockModel = new StubStateModel();

            mockMachine.Setup(m =>
                m.Trigger(It.Is<Trigger>(t => t.Name == "trigger"), mockModel)).Verifiable();

            IStateMachineExtensions.Trigger(mockMachine.Object, "trigger", mockModel);

            mockMachine.VerifyAll();
        }
    }
}
