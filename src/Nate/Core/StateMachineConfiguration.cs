using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nate.Core
{
    /// <summary>
    /// Holds configuration for a state machine
    /// </summary>
    public class StateMachineConfiguration
    {
        public bool RaiseExceptionOnTriggerMatchingNoTransition { get; set; }
        public bool RaiseExceptionOnTriggerMatchingNoPassingTransition { get; set; }
        public bool RaiseExceptionBeforeTransitionToSameState { get; set; }

        public StateMachineConfiguration()
        {
            // default options
            RaiseExceptionOnTriggerMatchingNoTransition = true;
            RaiseExceptionOnTriggerMatchingNoPassingTransition = false;
            RaiseExceptionBeforeTransitionToSameState = false;
        }
    }
}
