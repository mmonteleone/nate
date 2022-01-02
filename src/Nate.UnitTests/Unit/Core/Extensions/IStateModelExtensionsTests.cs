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
using Moq;
using Nate.Core;
using Nate.Core.Extensions;
using Xunit;

namespace Nate.Tests.Unit.Core.Extensions
{
    public class IStateModelExtensionsTests
    {
        [Fact]
        public void IStateModelExtensions_AvailableStates_NullMachine_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();
            Assert.Throws<ArgumentNullException>(() =>
                mockModel.AvailableStates((IStateMachine<StubStateModel>)null));
        }

        [Fact]
        public void IStateModelExtensions_AvailableStates_NullModel_ThrowsNullEx()
        {
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.AvailableStates(null, mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_AvailableStates_ValidParms_CallsReturnsMachineAvailable()
        {
            var mockModel = new StubStateModel();
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            var mockStates = new Mock<IEnumerable<State<StubStateModel>>>().Object;
            mockMachine.Setup(m => m.AvailableStates(mockModel)).Returns(mockStates).Verifiable();

            var result = mockModel.AvailableStates(mockMachine.Object);

            mockMachine.VerifyAll();
            Assert.Same(mockStates, result);
        }

        [Fact]
        public void IStateModelExtensions_AvailableTriggers_NullMachine_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();

            Assert.Throws<ArgumentNullException>(() =>
                mockModel.AvailableTriggers((IStateMachine<StubStateModel>)null));
        }

        [Fact]
        public void IStateModelExtensions_AvailableTriggers_NullModel_ThrowsNullEx()
        {
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();

            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.AvailableTriggers(null, mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_AvailableTriggers_ValidParms_CallsReturnsTriggersAvailable()
        {
            var mockModel = new StubStateModel();
            var mockTriggers = new Mock<IEnumerable<Trigger>>().Object;
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            mockMachine.Setup(m => m.AvailableTriggers(mockModel)).Returns(mockTriggers).Verifiable();

            var result = mockModel.AvailableTriggers(mockMachine.Object);

            mockMachine.VerifyAll();
            Assert.Same(mockTriggers, result);
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullMachine_ObjTrigger_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();
            var trigger = new Trigger("trigger");
            Assert.Throws<ArgumentNullException>(() =>
                mockModel.Trigger(trigger, (IStateMachine<StubStateModel>)null));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullMachine_StringTrigger_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();

            Assert.Throws<ArgumentNullException>(() =>
                mockModel.Trigger("triggerName", (IStateMachine<StubStateModel>)null));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullModel_ObjTrigger_ThrowsNullEx()
        {
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            var trigger = new Trigger("trigger");

            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.Trigger(null, trigger, mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullModel_StringTrigger_ThrowsNullEx()
        {
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();

            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.Trigger(null, "someTrigger", mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullTrigger_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();

            Assert.Throws<ArgumentNullException>(() =>
                mockModel.Trigger((Trigger)null, mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullTriggerName_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();

            Assert.Throws<ArgumentNullException>(() =>
                mockModel.Trigger((string)null, mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_ValidParms_ObjTrigger_CallsTriggerOnMachine()
        {
            var mockModel = new StubStateModel();
            var trigger = new Trigger("trigger");
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            mockMachine.Setup(m => m.Trigger(trigger, mockModel)).Verifiable();
            mockModel.Trigger(trigger, mockMachine.Object);
            mockMachine.VerifyAll();
        }

        [Fact]
        public void IStateModelExtensions_Trigger_ValidParms_StringTrigger_CallsTriggerOnMachine()
        {
            var mockModel = new StubStateModel();
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            mockMachine.Setup(m => m.Trigger(It.Is<Trigger>(t => t.Name == "triggerName"), mockModel)).Verifiable();

            mockModel.Trigger("triggerName", mockMachine.Object);

            mockMachine.VerifyAll();
        }
    }
}