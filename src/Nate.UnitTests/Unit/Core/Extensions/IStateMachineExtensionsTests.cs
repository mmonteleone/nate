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
using Moq;
using Nate.Core;
using Nate.Core.Extensions;
using Xunit;

namespace Nate.Tests.Unit.Core.Extensions
{
    public class IStateMachineExtensionsTests
    {
        [Fact]
        public void IStateMachineExtensinos_ValidParms_CallsTriggerOnMachineWithParms()
        {
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            var mockModel = new StubStateModel();

            mockMachine.Setup(m =>
                m.Trigger(It.Is<Trigger>(t => t.Name == "trigger"), mockModel)).Verifiable();

            mockMachine.Object.Trigger("trigger", mockModel);

            mockMachine.VerifyAll();
        }

        [Fact]
        public void IStateMachineExtensions_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Mock<IStateMachine<StubStateModel>>().Object.Trigger("trigger", null));
        }

        [Fact]
        public void IStateMachineExtensions_NullTriggerName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                StateMachineExtensions.Trigger(new Mock<IStateMachine<StubStateModel>>().Object, null,
                    new StubStateModel()));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullGuard_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = null;
            Assert.Throws<ArgumentNullException>(() =>
                machine.AddGlobalTransition(trigger, null, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullStateMachine_ThrowsNullex()
        {
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Assert.Throws<ArgumentNullException>(() =>
                ((IStateMachine<StubStateModel>)null).AddGlobalTransition(trigger, toState));
        }


        [Fact]
        public void StateExtensions_AddTransition_NullStateMachine_WithGuard_ThrowsNullex()
        {
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = s => true;
            Assert.Throws<ArgumentNullException>(() =>
                ((IStateMachine<StubStateModel>)null).AddGlobalTransition(trigger, toState, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTo_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var trigger = new Trigger("trigger");
            Assert.Throws<ArgumentNullException>(() =>
                machine.AddGlobalTransition(trigger, null));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTo_WithGuard_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var trigger = new Trigger("trigger");
            Func<StubStateModel, bool> guard = s => true;
            Assert.Throws<ArgumentNullException>(() =>
                machine.AddGlobalTransition(trigger, null, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTrigger_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var toState = new State<StubStateModel>("to");
            Assert.Throws<ArgumentNullException>(() =>
                machine.AddGlobalTransition(null, toState));
        }

        [Fact]
        public void StateExtensions_AddTransition_NullTrigger_WithGuard_ThrowsNullex()
        {
            var machine = new Mock<IStateMachine<StubStateModel>>().Object;
            var toState = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = s => true;
            Assert.Throws<ArgumentNullException>(() =>
                machine.AddGlobalTransition(null, toState, guard));
        }

        [Fact]
        public void StateExtensions_AddTransition_ValidParms_CallsAddOnFromState()
        {
            var machineMock = new Mock<IStateMachine<StubStateModel>>();
            var trigger = new Trigger("trigger");
            var toState = new State<StubStateModel>("to");
            machineMock.Setup(s => s.AddGlobalTransition(It.Is<Transition<StubStateModel>>(
                t => t.Target.Name == "to" && t.Trigger.Name == "trigger"))).Verifiable();
            machineMock.Object.AddGlobalTransition(trigger, toState);
            machineMock.VerifyAll();
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
            machineMock.Object.AddGlobalTransition(trigger, toState, guard);
            machineMock.VerifyAll();
        }
    }
}