﻿#region license

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

namespace Nate.Core
{
    /// <summary>
    ///     Represents a state machine which can process a state to the next, raising all necessary events.
    ///     Also contains optional global transitions that are state-independent.
    /// </summary>
    /// <typeparam name="TStateModel"></typeparam>
    public class StateMachine<TStateModel> : IStateMachine<TStateModel> where TStateModel : IStateModel
    {
        private readonly object _globalTransitionsLock = new object();

        private readonly Dictionary<Trigger, List<Transition<TStateModel>>> _globalTransitions =
            new Dictionary<Trigger, List<Transition<TStateModel>>>();

        public StateMachine()
            : this(new StateMachineConfiguration())
        {
        }

        public StateMachine(StateMachineConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public event EventHandler<TransitionEventArgs<TStateModel>> Transitioning;
        public event EventHandler<TransitionEventArgs<TStateModel>> Transitioned;

        public StateMachineConfiguration Configuration { get; }

        #region public

        public void AddGlobalTransition(Transition<TStateModel> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));
            // global transitions are by definition not from state to state.
            if (transition.Source != null)
                throw new InvalidTransitionException(
                    "Global transitions cannot have pre-defined source states as they are, by definition, from any state to a defined target.");

            lock (_globalTransitionsLock)
            {
                if (!_globalTransitions.ContainsKey(transition.Trigger))
                    _globalTransitions[transition.Trigger] = new List<Transition<TStateModel>>();
                if (_globalTransitions[transition.Trigger].Contains(transition))
                    // don't allow effectively duplicate transitions
                    throw new InvalidTransitionException(
                        $"State Machine already has a defined global transition to target '{transition.Target}' on trigger '{transition.Trigger}'");

                _globalTransitions[transition.Trigger].Add(transition);
            }
        }

        public void Trigger(Trigger trigger, TStateModel model)
        {
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (!(model.CurrentState is State<TStateModel> currentState))
                throw new InvalidStateModelException(
                    "State model's CurrentState object property must be of type State<TStateModel>");

            // find all possible transitions (state+global) from current state with given trigger
            // doesn't yet take into account the passing of those transitions' guards lambdas
            var possibleTransitions = currentState
                .TransitionsOn(trigger)
                .Concat(GlobalTransitionsOn(trigger)).ToList();
            // if no possible transitions, throw exception about it
            if (Configuration.RaiseExceptionOnTriggerMatchingNoTransition && !possibleTransitions.Any())
            {
                var availableTriggers = AvailableTriggers(model).ToList();
                var ex = new InvalidTriggerException(
                    $"State Model's CurrentState '{currentState}' does not define a Transition for trigger '{trigger}', nor does the state machine provide any global transitions on this trigger.  Available Triggers from this state: {string.Join(", ", availableTriggers.ConvertAll(t => t.Name).ToArray())}.  This exception can be suppressed via the state machine's Configuration.RaiseExceptionOnTriggerMatchingNoTransitions property")
                {
                    AvailableTriggers = availableTriggers
                };
                throw ex;
            }


            // out of the possible transitions, find the first whose guard passes
            var firstTransitionWithPassingGuard = possibleTransitions
                .FirstOrDefault(t => t.Guard(model));
            // if no possible transition had a passing guard, throw exception about it.
            if (Configuration.RaiseExceptionOnTriggerMatchingNoPassingTransition &&
                (firstTransitionWithPassingGuard == null || firstTransitionWithPassingGuard.Target == null))
                throw new InvalidTriggerException(
                    $"State Model's CurrentState '{currentState}' and the state machine's global transitions define at least {possibleTransitions.Count} transition(s) for the trigger '{trigger.Name}', but none of their guard lambdas returned true.  This exception can be suppressed via the state machine's Configuration.RaiseExceptionOnTriggerMatchingNoPassingTransitions property");


            if (firstTransitionWithPassingGuard != null && firstTransitionWithPassingGuard.Target != null)
            {
                var nextState = firstTransitionWithPassingGuard.Target;

                // No transitions should happen if not effectively changing states, even if there's a matching transition
                if (!Equals(nextState, currentState))
                {
                    // run transition callbacks and set the new state on model
                    if (Transitioning != null)
                        Transitioning(this,
                            new TransitionEventArgs<TStateModel>(model, currentState, nextState, trigger));

                    // raise events that occur before actual state change
                    currentState.RaiseExiting(model, nextState, trigger);
                    nextState.RaiseEntering(model, currentState, trigger);

                    // change state
                    model.CurrentState = nextState;

                    // raise events that occur after actual state change
                    currentState.RaiseExited(model, nextState, trigger);
                    nextState.RaiseEntered(model, currentState, trigger);

                    // run transition callbacks and set the new state on model
                    if (Transitioned != null)
                        Transitioned(this,
                            new TransitionEventArgs<TStateModel>(model, currentState, nextState, trigger));
                }
                // else if the next state is same as current, and machine is configured to 
                // raise exception when that happens, go ahead and raise it
                else if (Configuration.RaiseExceptionBeforeTransitionToSameState)
                {
                    throw new InvalidTriggerException(
                        $"Trigger '{trigger.Name}' would effectively transition State Model from its current state of '{model.CurrentState}' to the same state of '{nextState}', and the machine is currently configured to raise an exception when this happens.  This exception can be suppressed via the state machine's Configuration.RaiseExceptionBeforeTransitionToSameState property.");
                }
            }
        }

        public IEnumerable<Trigger> AvailableTriggers(TStateModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (!(model.CurrentState is State<TStateModel> currentState))
                throw new InvalidStateModelException(
                    "State model's CurrentState object property must be of type State<TStateModel>");

            // get current state

            lock (_globalTransitionsLock)
            {
                return currentState.AvailableTransitions
                    .Concat(_globalTransitions.Values.SelectMany(t => t))
                    .Select(t => t.Trigger)
                    .Distinct()
                    .OrderBy(t => t.Name);
            }
        }

        public IEnumerable<State<TStateModel>> AvailableStates(TStateModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (!(model.CurrentState is State<TStateModel> currentState))
                throw new InvalidStateModelException(
                    "State model's CurrentState object property must be of type State<TStateModel>");

            // get current state

            lock (_globalTransitionsLock)
            {
                return currentState.AvailableTransitions
                    .Concat(_globalTransitions.Values.SelectMany(t => t))
                    .Select(t => t.Target)
                    .Distinct()
                    .OrderBy(s => s.Name);
            }
        }

        public List<Transition<TStateModel>> GlobalTransitionsOn(Trigger trigger)
        {
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));

            lock (_globalTransitionsLock)
            {
                if (!_globalTransitions.ContainsKey(trigger))
                    return new List<Transition<TStateModel>>();
                return _globalTransitions[trigger];
            }
        }

        #endregion
    }
}