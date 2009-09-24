using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;
using Xunit;

namespace Nate.Tests.Unit.Core
{
    public class StateMachineConfigurationTests
    {
        [Fact]
        public void StateMachineConfiguration_RaiseExceptionOnTriggerMatchingNoTransitions_DefaultsTrue()
        {
            Assert.True((new StateMachineConfiguration()).RaiseExceptionOnTriggerMatchingNoTransition);
        }

        [Fact]
        public void StateMachineConfiguration_RaiseExceptionOnTriggerMatchingNoPassingTransitions_DefaultsFalse()
        {
            Assert.False((new StateMachineConfiguration()).RaiseExceptionOnTriggerMatchingNoPassingTransition);
        }

        [Fact]
        public void StateMachineConfiguration_RaiseExceptionOnTransitioningToSameState_DefaultsFalse()
        {
            Assert.False((new StateMachineConfiguration()).RaiseExceptionBeforeTransitionToSameState);
        }
    }
}
