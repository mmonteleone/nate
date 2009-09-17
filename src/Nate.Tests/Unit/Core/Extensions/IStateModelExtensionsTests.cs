using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq;
using Nate.Core;

namespace Nate.Tests.Unit.Core.Extensions
{
    public class IStateModelExtensionsTests
    {
        [Fact]
        public void IStateModelExtensions_AvailableStates_NullModel_ThrowsNullEx()
        {
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.AvailableStates(null, mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_AvailableStates_NullMachine_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();
            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.AvailableStates(mockModel, (IStateMachine<StubStateModel>)null));
        }

        [Fact]
        public void IStateModelExtensions_AvailableStates_ValidParms_CallsReturnsMachineAvailable()
        {
            var mockModel = new StubStateModel();
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            var mockStates = new Mock<IEnumerable<State<StubStateModel>>>().Object;
            mockMachine.Setup(m => m.AvailableStates(mockModel)).Returns(mockStates).Verifiable();

            var result = IStateModelExtensions.AvailableStates(mockModel, mockMachine.Object);

            mockMachine.VerifyAll();
            Assert.Same(mockStates, result);
        }

        [Fact]
        public void IStateModelExtensions_AvailableTriggers_NullModel_ThrowsNullEx()
        {
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();

            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.AvailableTriggers(null, mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_AvailableTriggers_NullMachine_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();

            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.AvailableTriggers(mockModel, (IStateMachine<StubStateModel>)null));
        }

        [Fact]
        public void IStateModelExtensions_AvailableTriggers_ValidParms_CallsReturnsTriggersAvailable()
        {
            var mockModel = new StubStateModel();
            var mockTriggers = new Mock<IEnumerable<Trigger>>().Object;
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            mockMachine.Setup(m => m.AvailableTriggers(mockModel)).Returns(mockTriggers).Verifiable();

            var result = IStateModelExtensions.AvailableTriggers(mockModel, mockMachine.Object);

            mockMachine.VerifyAll();
            Assert.Same(mockTriggers, result);
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullModel_StringTrigger_ThrowsNullEx()
        {
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();

            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.Trigger(null, "someTrigger", mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullTriggerName_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();

            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.Trigger(mockModel, (string)null, mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullMachine_StringTrigger_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();

            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.Trigger(mockModel, "triggerName", (IStateMachine<StubStateModel>)null));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_ValidParms_StringTrigger_CallsTriggerOnMachine()
        {
            var mockModel = new StubStateModel();
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            mockMachine.Setup(m => m.Trigger(It.Is<Trigger>(t => t.Name == "triggerName"), mockModel)).Verifiable();

            IStateModelExtensions.Trigger(mockModel, "triggerName", mockMachine.Object);

            mockMachine.VerifyAll();
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
        public void IStateModelExtensions_Trigger_NullTrigger_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();

            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.Trigger(mockModel, (Trigger)null, mockMachine.Object));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_NullMachine_ObjTrigger_ThrowsNullEx()
        {
            var mockModel = new StubStateModel();
            var trigger = new Trigger("trigger");
            Assert.Throws<ArgumentNullException>(() =>
                IStateModelExtensions.Trigger(mockModel, trigger, (IStateMachine<StubStateModel>)null));
        }

        [Fact]
        public void IStateModelExtensions_Trigger_ValidParms_ObjTrigger_CallsTriggerOnMachine()
        {
            var mockModel = new StubStateModel();
            var trigger = new Trigger("trigger");
            var mockMachine = new Mock<IStateMachine<StubStateModel>>();
            mockMachine.Setup(m => m.Trigger(trigger, mockModel)).Verifiable();
            IStateModelExtensions.Trigger(mockModel, trigger, mockMachine.Object);
            mockMachine.VerifyAll();
        }
    }
}
