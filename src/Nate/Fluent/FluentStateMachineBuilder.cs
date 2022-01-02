using System;
using System.Collections.Generic;
using System.Linq;
using Nate.Core;
using Nate.Core.Extensions;

namespace Nate.Fluent
{
    public class FluentStateMachineBuilder<TStateModel> : IStateModel, IFluentStateMachineBuilder<TStateModel>
        where TStateModel : IStateModel
    {
        #region model manipulation

        /// <summary>
        ///     Triggers an event on the internal fluent state machine builder, also
        ///     traps trigger exceptions and exposes them as fluent syntax errors
        /// </summary>
        /// <param name="trigger"></param>
        private void Trigger(Trigger trigger)
        {
            try
            {
                builderStateMachine.Trigger(trigger, this);
            }
            catch (InvalidTriggerException trigEx)
            {
                var message =
                    $"A syntax error occurred in the definition of a FluentStateMachine when attempting to define '{trigger.Name}'.  The only allowed definitions at this point are: {string.Join(", ", trigEx.AvailableTriggers.ToList().ConvertAll(t => t.Name).ToArray())}";
                throw new FluentSyntaxException(message, trigEx);
            }
        }

        public void State(string name, int? code)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Trigger(_stateTrigger);
            _workingState = FindOrCreateState(name, code);
        }

        public void Initiates()
        {
            Trigger(_initiatesTrigger);
        }

        public void TransitionsTo(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Trigger(_transitionsToTrigger);
            _workingTransitionTarget = FindOrCreateState(name, null);
        }

        public void On(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            _workingTransitionTrigger = FindOrCreateTrigger(name);
            Trigger(_onTrigger);
        }

        public void When(Func<TStateModel, bool> guard)
        {
            _workingTransitionGuard = guard ?? throw new ArgumentNullException(nameof(guard));
            Trigger(_whenTrigger);
        }

        public void BeforeEntry(Action<TransitionEventArgs<TStateModel>> callback)
        {
            _workingBeforeEntry = callback ?? throw new ArgumentNullException(nameof(callback));
            Trigger(_beforeEntryTrigger);
        }

        public void AfterEntry(Action<TransitionEventArgs<TStateModel>> callback)
        {
            _workingAfterEntry = callback ?? throw new ArgumentNullException(nameof(callback));
            Trigger(_afterEntryTrigger);
        }

        public void BeforeExit(Action<TransitionEventArgs<TStateModel>> callback)
        {
            _workingBeforeExit = callback ?? throw new ArgumentNullException(nameof(callback));
            Trigger(_beforeExitTrigger);
        }

        public void AfterExit(Action<TransitionEventArgs<TStateModel>> callback)
        {
            _workingAfterExit = callback ?? throw new ArgumentNullException(nameof(callback));
            Trigger(_afterExitTrigger);
        }

        public void BeforeTransition(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _workingGlobalTransitionings.Add(callback);
            Trigger(_beforeTransitionTrigger);
        }

        public void AfterTransition(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _workingGlobalTransitioneds.Add(callback);
            Trigger(_afterTransitionTrigger);
        }

        public void GloballyTransitionsTo(string state)
        {
            if (string.IsNullOrEmpty(state)) throw new ArgumentNullException(nameof(state));

            Trigger(_globallyTransitionsToTrigger);
            _workingTransitionTarget = FindOrCreateState(state, null);
        }

        public IFluentStateMachine<TStateModel> Compile(StateMachineConfiguration configuration)
        {
            if (Equals(CurrentState, initial))
                throw new FluentSyntaxException(
                    "Cannot compile an empty state machine.  No statements have been made to define this state machine");
            Trigger(_compileTrigger);
            var stateMachine = new StateMachine<TStateModel>(configuration);
            lock (_statesLock)
            {
                return new FluentStateMachine<TStateModel>(
                    stateMachine,
                    _states.Values.ToList(),
                    _workingInitialState,
                    _workingGlobalTransitionings,
                    _workingGlobalTransitioneds,
                    _workingGlobalTransitions);
            }
        }

        public IFluentStateMachine<TStateModel> Compile()
        {
            return Compile(new StateMachineConfiguration());
        }

        #endregion

        #region implementation

        #region state-building trackers/methods/helpers

        // holder for all constructed states in the state model
        private readonly object _statesLock = new object();
        private readonly object _triggersLock = new object();
        private readonly Dictionary<string, State<TStateModel>> _states = new Dictionary<string, State<TStateModel>>();
        private readonly Dictionary<string, Trigger> _triggers = new Dictionary<string, Trigger>();

        // working placeholders for items as the model is built
        private State<TStateModel> _workingInitialState;
        private State<TStateModel> _workingState;
        private State<TStateModel> _workingTransitionTarget;
        private Trigger _workingTransitionTrigger;
        private Func<TStateModel, bool> _workingTransitionGuard;
        private Action<TransitionEventArgs<TStateModel>> _workingBeforeEntry;
        private Action<TransitionEventArgs<TStateModel>> _workingAfterEntry;
        private Action<TransitionEventArgs<TStateModel>> _workingBeforeExit;
        private Action<TransitionEventArgs<TStateModel>> _workingAfterExit;

        private readonly List<Action<TransitionEventArgs<TStateModel>>> _workingGlobalTransitionings =
            new List<Action<TransitionEventArgs<TStateModel>>>();

        private readonly List<Action<TransitionEventArgs<TStateModel>>> _workingGlobalTransitioneds =
            new List<Action<TransitionEventArgs<TStateModel>>>();

        private readonly List<Transition<TStateModel>> _workingGlobalTransitions = new List<Transition<TStateModel>>();

        private State<TStateModel> FindOrCreateState(string stateName, int? code)
        {
            lock (_statesLock)
            {
                if (!_states.TryGetValue(stateName, out var state))
                {
                    state = new State<TStateModel>(stateName, code);
                    _states[stateName] = state;
                }

                if (code.HasValue && !state.Code.HasValue)
                    state.Code = code;
                return state;
            }
        }

        private Trigger FindOrCreateTrigger(string triggerName)
        {
            lock (_triggersLock)
            {
                if (!_triggers.TryGetValue(triggerName, out var trigger))
                {
                    trigger = new Trigger(triggerName);
                    _triggers[triggerName] = trigger;
                }

                return trigger;
            }
        }

        private void FinalizeTransition()
        {
            if (_workingTransitionTrigger != null)
            {
                // this should be a transition for the current state being built
                if (_workingState != null)
                {
                    if (_workingTransitionGuard != null)
                        _workingState.AddTransition(_workingTransitionTrigger, _workingTransitionTarget,
                            _workingTransitionGuard);
                    else
                        _workingState.AddTransition(_workingTransitionTrigger, _workingTransitionTarget);
                }
                // should be a global transition
                else
                {
                    if (_workingTransitionGuard != null)
                        _workingGlobalTransitions.Add(new Transition<TStateModel>(_workingTransitionTrigger, null,
                            _workingTransitionTarget, _workingTransitionGuard));
                    else
                        _workingGlobalTransitions.Add(new Transition<TStateModel>(_workingTransitionTrigger, null,
                            _workingTransitionTarget));
                }

                _workingTransitionTrigger = null;
                _workingTransitionTarget = null;
                _workingTransitionGuard = null;
            }
        }

        private void FinalizeState()
        {
            _workingState = null;
        }

        #endregion

        #region state-building internal state machine

        public FluentStateMachineBuilder()
        {
            CurrentState = initial;
        }

        // api states
        private static readonly State<FluentStateMachineBuilder<TStateModel>> initial =
            new State<FluentStateMachineBuilder<TStateModel>>("Initial", 0);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingState =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningState", 1);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingStateInitiation =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningStateInitiation", 2);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingTransition =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningTransition", 3);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingTransitionTrigger =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningTransitionTrigger", 4);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingTransitionGuard =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningTransitionGuard", 5);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingBeforeEntry =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningBeforeEntry", 6);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingAfterEntry =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningAfterEntry", 7);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingBeforeExit =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningBeforeExit", 8);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingAfterExit =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningAfterExit", 9);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> compiling =
            new State<FluentStateMachineBuilder<TStateModel>>("Compiling", 10);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingGlobalBeforeTransition =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningGlobalTransitioning", 11);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingGlobalAfterTransition =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningGlobalTransitioned", 12);

        private static readonly State<FluentStateMachineBuilder<TStateModel>> definingGlobalTransition =
            new State<FluentStateMachineBuilder<TStateModel>>("DefiningGloballyTransition", 13);

        // api triggers
        private static readonly Trigger _stateTrigger = new Trigger("State");
        private static readonly Trigger _initiatesTrigger = new Trigger("Initiates");
        private static readonly Trigger _transitionsToTrigger = new Trigger("TransitionsTo");
        private static readonly Trigger _onTrigger = new Trigger("On");
        private static readonly Trigger _whenTrigger = new Trigger("When");
        private static readonly Trigger _beforeEntryTrigger = new Trigger("BeforeEntry");
        private static readonly Trigger _afterEntryTrigger = new Trigger("AfterEntry");
        private static readonly Trigger _beforeExitTrigger = new Trigger("BeforeExit");
        private static readonly Trigger _afterExitTrigger = new Trigger("AfterExit");
        private static readonly Trigger _compileTrigger = new Trigger("Compile");
        private static readonly Trigger _beforeTransitionTrigger = new Trigger("BeforeTransition");
        private static readonly Trigger _afterTransitionTrigger = new Trigger("AfterTransition");
        private static readonly Trigger _globallyTransitionsToTrigger = new Trigger("GloballyTransitionsTo");

        // stateful model stuff
        public object CurrentState { get; set; }
        private static readonly IStateMachine<FluentStateMachineBuilder<TStateModel>> builderStateMachine;

        static FluentStateMachineBuilder()
        {
            builderStateMachine = new StateMachine<FluentStateMachineBuilder<TStateModel>>();

            // wire transitions

            initial.AddTransition(_stateTrigger, definingState);

            definingState.AddTransition(_afterEntryTrigger, definingAfterEntry);
            definingState.AddTransition(_beforeExitTrigger, definingBeforeExit);
            definingState.AddTransition(_transitionsToTrigger, definingTransition);
            definingState.AddTransition(_initiatesTrigger, definingStateInitiation);
            definingState.AddTransition(_stateTrigger, definingState);
            definingState.AddTransition(_compileTrigger, compiling);
            definingState.AddTransition(_beforeTransitionTrigger, definingGlobalBeforeTransition);
            definingState.AddTransition(_afterTransitionTrigger, definingGlobalAfterTransition);
            definingState.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);
            definingState.AddTransition(_beforeEntryTrigger, definingBeforeEntry);
            definingState.AddTransition(_afterExitTrigger, definingAfterExit);

            definingStateInitiation.AddTransition(_afterEntryTrigger, definingAfterEntry);
            definingStateInitiation.AddTransition(_beforeExitTrigger, definingBeforeExit);
            definingStateInitiation.AddTransition(_transitionsToTrigger, definingTransition);
            definingStateInitiation.AddTransition(_stateTrigger, definingState);
            definingStateInitiation.AddTransition(_compileTrigger, compiling);
            definingStateInitiation.AddTransition(_beforeTransitionTrigger, definingGlobalBeforeTransition);
            definingStateInitiation.AddTransition(_afterTransitionTrigger, definingGlobalAfterTransition);
            definingStateInitiation.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);
            definingStateInitiation.AddTransition(_beforeEntryTrigger, definingBeforeEntry);
            definingStateInitiation.AddTransition(_afterExitTrigger, definingAfterExit);

            definingTransition.AddTransition(_onTrigger, definingTransitionTrigger);

            definingTransitionTrigger.AddTransition(_whenTrigger, definingTransitionGuard);
            definingTransitionTrigger.AddTransition(_afterEntryTrigger, definingAfterEntry);
            definingTransitionTrigger.AddTransition(_beforeExitTrigger, definingBeforeExit);
            definingTransitionTrigger.AddTransition(_transitionsToTrigger, definingTransition);
            definingTransitionTrigger.AddTransition(_initiatesTrigger, definingStateInitiation);
            definingTransitionTrigger.AddTransition(_stateTrigger, definingState);
            definingTransitionTrigger.AddTransition(_compileTrigger, compiling);
            definingTransitionTrigger.AddTransition(_beforeTransitionTrigger, definingGlobalBeforeTransition);
            definingTransitionTrigger.AddTransition(_afterTransitionTrigger, definingGlobalAfterTransition);
            definingTransitionTrigger.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);
            definingTransitionTrigger.AddTransition(_beforeEntryTrigger, definingBeforeEntry);
            definingTransitionTrigger.AddTransition(_afterExitTrigger, definingAfterExit);

            definingTransitionGuard.AddTransition(_afterEntryTrigger, definingAfterEntry);
            definingTransitionGuard.AddTransition(_beforeExitTrigger, definingBeforeExit);
            definingTransitionGuard.AddTransition(_transitionsToTrigger, definingTransition);
            definingTransitionGuard.AddTransition(_initiatesTrigger, definingStateInitiation);
            definingTransitionGuard.AddTransition(_stateTrigger, definingState);
            definingTransitionGuard.AddTransition(_compileTrigger, compiling);
            definingTransitionGuard.AddTransition(_beforeTransitionTrigger, definingGlobalBeforeTransition);
            definingTransitionGuard.AddTransition(_afterTransitionTrigger, definingGlobalAfterTransition);
            definingTransitionGuard.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);
            definingTransitionGuard.AddTransition(_beforeEntryTrigger, definingBeforeEntry);
            definingTransitionGuard.AddTransition(_afterExitTrigger, definingAfterExit);

            definingAfterEntry.AddTransition(_beforeExitTrigger, definingBeforeExit);
            definingAfterEntry.AddTransition(_transitionsToTrigger, definingTransition);
            definingAfterEntry.AddTransition(_initiatesTrigger, definingStateInitiation);
            definingAfterEntry.AddTransition(_stateTrigger, definingState);
            definingAfterEntry.AddTransition(_compileTrigger, compiling);
            definingAfterEntry.AddTransition(_beforeTransitionTrigger, definingGlobalBeforeTransition);
            definingAfterEntry.AddTransition(_afterTransitionTrigger, definingGlobalAfterTransition);
            definingAfterEntry.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);
            definingAfterEntry.AddTransition(_beforeEntryTrigger, definingBeforeEntry);
            definingAfterEntry.AddTransition(_afterExitTrigger, definingAfterExit);

            definingBeforeExit.AddTransition(_afterEntryTrigger, definingBeforeExit);
            definingBeforeExit.AddTransition(_transitionsToTrigger, definingTransition);
            definingBeforeExit.AddTransition(_initiatesTrigger, definingStateInitiation);
            definingBeforeExit.AddTransition(_stateTrigger, definingState);
            definingBeforeExit.AddTransition(_compileTrigger, compiling);
            definingBeforeExit.AddTransition(_beforeTransitionTrigger, definingGlobalBeforeTransition);
            definingBeforeExit.AddTransition(_afterTransitionTrigger, definingGlobalAfterTransition);
            definingBeforeExit.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);
            definingBeforeExit.AddTransition(_beforeEntryTrigger, definingBeforeEntry);
            definingBeforeExit.AddTransition(_afterExitTrigger, definingAfterExit);

            definingBeforeEntry.AddTransition(_beforeExitTrigger, definingBeforeExit);
            definingBeforeEntry.AddTransition(_transitionsToTrigger, definingTransition);
            definingBeforeEntry.AddTransition(_initiatesTrigger, definingStateInitiation);
            definingBeforeEntry.AddTransition(_stateTrigger, definingState);
            definingBeforeEntry.AddTransition(_compileTrigger, compiling);
            definingBeforeEntry.AddTransition(_beforeTransitionTrigger, definingGlobalBeforeTransition);
            definingBeforeEntry.AddTransition(_afterTransitionTrigger, definingGlobalAfterTransition);
            definingBeforeEntry.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);
            definingBeforeEntry.AddTransition(_afterEntryTrigger, definingAfterEntry);
            definingBeforeEntry.AddTransition(_afterExitTrigger, definingAfterExit);

            definingAfterExit.AddTransition(_afterEntryTrigger, definingBeforeExit);
            definingAfterExit.AddTransition(_transitionsToTrigger, definingTransition);
            definingAfterExit.AddTransition(_initiatesTrigger, definingStateInitiation);
            definingAfterExit.AddTransition(_stateTrigger, definingState);
            definingAfterExit.AddTransition(_compileTrigger, compiling);
            definingAfterExit.AddTransition(_beforeTransitionTrigger, definingGlobalBeforeTransition);
            definingAfterExit.AddTransition(_afterTransitionTrigger, definingGlobalAfterTransition);
            definingAfterExit.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);
            definingAfterExit.AddTransition(_beforeEntryTrigger, definingBeforeEntry);
            definingAfterExit.AddTransition(_beforeExitTrigger, definingBeforeExit);

            definingGlobalBeforeTransition.AddTransition(_stateTrigger, definingState);
            definingGlobalBeforeTransition.AddTransition(_afterTransitionTrigger, definingGlobalAfterTransition);
            definingGlobalBeforeTransition.AddTransition(_compileTrigger, compiling);
            definingGlobalBeforeTransition.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);

            definingGlobalAfterTransition.AddTransition(_stateTrigger, definingState);
            definingGlobalAfterTransition.AddTransition(_beforeTransitionTrigger, definingGlobalBeforeTransition);
            definingGlobalAfterTransition.AddTransition(_compileTrigger, compiling);
            definingGlobalAfterTransition.AddTransition(_globallyTransitionsToTrigger, definingGlobalTransition);

            definingGlobalTransition.AddTransition(_onTrigger, definingTransitionTrigger);

            // wire transition callbacks

            definingState.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
                e.Model.FinalizeState();
            };

            definingStateInitiation.Entered += (s, e) => { e.Model.FinalizeTransition(); };
            definingStateInitiation.Exiting += (s, e) => { e.Model._workingInitialState = e.Model._workingState; };

            definingTransition.Entered += (s, e) => { e.Model.FinalizeTransition(); };

            definingTransitionGuard.Exiting += (s, e) => { e.Model.FinalizeTransition(); };

            definingBeforeEntry.Entered += (s, e) => { e.Model.FinalizeTransition(); };
            definingBeforeEntry.Exited += (s, e) =>
            {
                // grab an immediate reference to working entry.
                // will have changed by the time the lambda is called
                var entry = e.Model._workingBeforeEntry;
                e.Model._workingState.Entering += (es, ee) => entry(ee);
            };

            definingAfterEntry.Entered += (s, e) => { e.Model.FinalizeTransition(); };

            definingAfterEntry.Exiting += (s, e) =>
            {
                // grab an immediate reference to working entry.
                // will have changed by the time the lambda is called
                var entry = e.Model._workingAfterEntry;
                e.Model._workingState.Entered += (es, ee) => entry(ee);
            };

            definingBeforeExit.Entered += (s, e) => { e.Model.FinalizeTransition(); };
            definingBeforeExit.Exiting += (s, e) =>
            {
                // grab an immediate reference to working exit.
                // will have changed by the time the lambda is called
                var exit = e.Model._workingBeforeExit;
                e.Model._workingState.Exiting += (es, ee) => exit(ee);
            };

            definingAfterExit.Entered += (s, e) => { e.Model.FinalizeTransition(); };
            definingAfterExit.Exiting += (s, e) =>
            {
                // grab an immediate reference to working exit.
                // will have changed by the time the lambda is called
                var exit = e.Model._workingAfterExit;
                e.Model._workingState.Exited += (es, ee) => exit(ee);
            };

            definingGlobalBeforeTransition.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
                e.Model.FinalizeState();
            };

            definingGlobalAfterTransition.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
                e.Model.FinalizeState();
            };

            definingGlobalTransition.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
                e.Model.FinalizeState();
            };

            compiling.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
                e.Model.FinalizeState();
            };
        }

        #endregion

        #endregion
    }
}