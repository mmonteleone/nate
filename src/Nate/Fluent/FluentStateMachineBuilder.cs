using System;
using System.Collections.Generic;
using System.Linq;
using Nate.Core;

namespace Nate.Fluent
{
    public class FluentStateMachineBuilder<TStateModel> : IStateModel, Nate.Fluent.IFluentStateMachineBuilder<TStateModel> where TStateModel : IStateModel
    {
        #region model manipulation


        /// <summary>
        /// Triggers an event on the internal fluent state machine builder, also
        /// traps trigger exceptions and exposes them as fluent syntax errors
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
                var message = String.Format(
                    "A syntax error occurred in the definition of a FluentStateMachine when attempting to define '{0}'.  The only allowed definitions at this point are: {1}",
                    trigger.Name,
                    String.Join(", ", trigEx.AvailableTriggers.ToList().ConvertAll(t => t.Name).ToArray()));
                throw new FluentSyntaxException(message, trigEx);
            }
        }

        public void State(string name, int? code)
        {
            if (String.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

            Trigger(stateTrigger);
            workingState = FindOrCreateState(name, code);
        }

        public void Initiates()
        {
            Trigger(initiatesTrigger);
        }

        public void TransitionsTo(string name)
        {
            if (String.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

            Trigger(transitionsToTrigger);
            workingTransitionTarget = FindOrCreateState(name, null);
        }

        public void On(string name)
        {
            if (String.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

            workingTransitionTrigger = FindOrCreateTrigger(name);
            Trigger(onTrigger);
        }

        public void When(Func<TStateModel, bool> guard)
        {
            if (guard == null) { throw new ArgumentNullException("guard"); }

            workingTransitionGuard = guard;
            Trigger(whenTrigger);
        }

        public void BeforeEntry(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            this.workingBeforeEntry = callback;
            Trigger(beforeEntryTrigger);
        }
        
        public void AfterEntry(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            this.workingAfterEntry = callback;
            Trigger(afterEntryTrigger);
        }

        public void BeforeExit(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            workingBeforeExit = callback;
            Trigger(beforeExitTrigger);
        }

        public void AfterExit(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            this.workingAfterExit = callback;
            Trigger(afterExitTrigger);
        }

        public void BeforeTransition(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            workingGlobalTransitionings.Add(callback);
            Trigger(BeforeTransitionTrigger);
        }

        public void AfterTransition(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            workingGlobalTransitioneds.Add(callback);
            Trigger(AfterTranasitionTrigger);
        }

        public void GloballyTransitionsTo(string state)
        {
            if (String.IsNullOrEmpty(state)) { throw new ArgumentNullException("state"); }

            Trigger(globallyTransitionsToTrigger);
            workingTransitionTarget = FindOrCreateState(state, null);
        }

        public IFluentStateMachine<TStateModel> Compile(StateMachineConfiguration configuration)
        {
            if (CurrentState == initial)
            {
                throw new FluentSyntaxException("Cannot compile an empty state machine.  No statements have been made to define this state machine");
            }
            Trigger(compileTrigger);
            var stateMachine = new StateMachine<TStateModel>(configuration);
            return new FluentStateMachine<TStateModel>(
                stateMachine,
                states.Values.ToList(),
                workingInitialState,
                workingGlobalTransitionings,
                workingGlobalTransitioneds,
                workingGlobalTransitions);
        }

        public IFluentStateMachine<TStateModel> Compile()
        {
            return Compile(new StateMachineConfiguration());
        }


        #endregion

        #region implementation

        #region state-building trackers/methods/helpers

        // holder for all constructed states in the state model
        private static readonly object statesLock = new object();
        private static readonly object triggersLock = new object();
        private Dictionary<string, State<TStateModel>> states = new Dictionary<string, State<TStateModel>>();
        private Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();

        // working placeholders for items as the model is built
        private State<TStateModel> workingInitialState;
        private State<TStateModel> workingState;
        private State<TStateModel> workingTransitionTarget;
        private Trigger workingTransitionTrigger;
        private Func<TStateModel, bool> workingTransitionGuard;
        private Action<TransitionEventArgs<TStateModel>> workingBeforeEntry;
        private Action<TransitionEventArgs<TStateModel>> workingAfterEntry;
        private Action<TransitionEventArgs<TStateModel>> workingBeforeExit;
        private Action<TransitionEventArgs<TStateModel>> workingAfterExit;
        private List<Action<TransitionEventArgs<TStateModel>>> workingGlobalTransitionings = new List<Action<TransitionEventArgs<TStateModel>>>();
        private List<Action<TransitionEventArgs<TStateModel>>> workingGlobalTransitioneds = new List<Action<TransitionEventArgs<TStateModel>>>();
        private List<Transition<TStateModel>> workingGlobalTransitions = new List<Transition<TStateModel>>();

        private State<TStateModel> FindOrCreateState(string stateName, int? code)
        {
            lock (statesLock)
            {
                State<TStateModel> state;
                if (!states.TryGetValue(stateName, out state))
                {
                    state = new State<TStateModel>(stateName, code);
                    states[stateName] = state;
                }
                if (code.HasValue && !state.Code.HasValue)
                    state.Code = code;
                return state;
            }
        }

        private Trigger FindOrCreateTrigger(string triggerName)
        {
            lock (triggersLock)
            {
                Trigger trigger;
                if (!triggers.TryGetValue(triggerName, out trigger))
                {
                    trigger = new Trigger(triggerName);
                    triggers[triggerName] = trigger;
                }
                return trigger;
            }
        }

        private void FinalizeTransition()
        {
            if (workingTransitionTrigger != null)
            {
                // this should be a transition for the current state being built
                if (workingState != null)
                {
                    if (workingTransitionGuard != null)
                    {
                        workingState.AddTransition(workingTransitionTrigger, workingTransitionTarget, workingTransitionGuard);
                    }
                    else
                    {
                        workingState.AddTransition(workingTransitionTrigger, workingTransitionTarget);
                    }
                }
                // should be a global transition
                else
                {
                    if (workingTransitionGuard != null)
                    {
                        workingGlobalTransitions.Add(new Transition<TStateModel>(workingTransitionTrigger, null, workingTransitionTarget, workingTransitionGuard));
                    }
                    else
                    {
                        workingGlobalTransitions.Add(new Transition<TStateModel>(workingTransitionTrigger, null, workingTransitionTarget));
                    }
                }
                workingTransitionTrigger = null;
                workingTransitionTarget = null;
                workingTransitionGuard = null;
            }
        }

        private void FinalizeState()
        {
            workingState = null;
        }

        #endregion

        #region state-building internal state machine

        public FluentStateMachineBuilder()
        {
            this.CurrentState = initial;
        }

        // api states
        private static State<FluentStateMachineBuilder<TStateModel>> initial = new State<FluentStateMachineBuilder<TStateModel>>("Initial", 0);
        private static State<FluentStateMachineBuilder<TStateModel>> definingState = new State<FluentStateMachineBuilder<TStateModel>>("DefiningState", 1);
        private static State<FluentStateMachineBuilder<TStateModel>> definingStateInitiation = new State<FluentStateMachineBuilder<TStateModel>>("DefiningStateInitiation", 2);
        private static State<FluentStateMachineBuilder<TStateModel>> definingTransition = new State<FluentStateMachineBuilder<TStateModel>>("DefiningTransition", 3);
        private static State<FluentStateMachineBuilder<TStateModel>> definingTransitionTrigger = new State<FluentStateMachineBuilder<TStateModel>>("DefiningTransitionTrigger", 4);
        private static State<FluentStateMachineBuilder<TStateModel>> definingTransitionGuard = new State<FluentStateMachineBuilder<TStateModel>>("DefiningTransitionGuard", 5);
        private static State<FluentStateMachineBuilder<TStateModel>> definingBeforeEntry = new State<FluentStateMachineBuilder<TStateModel>>("DefiningBeforeEntry", 6);
        private static State<FluentStateMachineBuilder<TStateModel>> definingAfterEntry = new State<FluentStateMachineBuilder<TStateModel>>("DefiningAfterEntry", 7);
        private static State<FluentStateMachineBuilder<TStateModel>> definingBeforeExit = new State<FluentStateMachineBuilder<TStateModel>>("DefiningBeforeExit", 8);
        private static State<FluentStateMachineBuilder<TStateModel>> definingAfterExit = new State<FluentStateMachineBuilder<TStateModel>>("DefiningAfterExit", 9);
        private static State<FluentStateMachineBuilder<TStateModel>> compiling = new State<FluentStateMachineBuilder<TStateModel>>("Compiling", 10);
        private static State<FluentStateMachineBuilder<TStateModel>> definingGlobalBeforeTransition = new State<FluentStateMachineBuilder<TStateModel>>("DefiningGlobalTransitioning", 11);
        private static State<FluentStateMachineBuilder<TStateModel>> definingGlobalAfterTranasition = new State<FluentStateMachineBuilder<TStateModel>>("DefiningGlobalTransitioned", 12);
        private static State<FluentStateMachineBuilder<TStateModel>> definingGlobalTransition = new State<FluentStateMachineBuilder<TStateModel>>("DefiningGloballyTransition", 13);

        // api triggers
        private static Trigger stateTrigger = new Trigger("State");
        private static Trigger initiatesTrigger = new Trigger("Initiates");
        private static Trigger transitionsToTrigger = new Trigger("TransitionsTo");
        private static Trigger onTrigger = new Trigger("On");
        private static Trigger whenTrigger = new Trigger("When");
        private static Trigger beforeEntryTrigger = new Trigger("BeforeEntry");
        private static Trigger afterEntryTrigger = new Trigger("AfterEntry");
        private static Trigger beforeExitTrigger = new Trigger("BeforeExit");
        private static Trigger afterExitTrigger = new Trigger("AfterExit");
        private static Trigger compileTrigger = new Trigger("Compile");
        private static Trigger BeforeTransitionTrigger = new Trigger("BeforeTransition");
        private static Trigger AfterTranasitionTrigger = new Trigger("AfterTranasition");
        private static Trigger globallyTransitionsToTrigger = new Trigger("GloballyTransitionsTo");

        // istatefulmodel stuff
        public object CurrentState { get; set; }
        private static IStateMachine<FluentStateMachineBuilder<TStateModel>> builderStateMachine;

        static FluentStateMachineBuilder()
        {

            builderStateMachine = new StateMachine<FluentStateMachineBuilder<TStateModel>>();

            // wire transitions

            initial.AddTransition(stateTrigger, definingState);

            definingState.AddTransition(afterEntryTrigger, definingAfterEntry);
            definingState.AddTransition(beforeExitTrigger, definingBeforeExit);
            definingState.AddTransition(transitionsToTrigger, definingTransition);
            definingState.AddTransition(initiatesTrigger, definingStateInitiation);
            definingState.AddTransition(stateTrigger, definingState);
            definingState.AddTransition(compileTrigger, compiling);
            definingState.AddTransition(BeforeTransitionTrigger, definingGlobalBeforeTransition);
            definingState.AddTransition(AfterTranasitionTrigger, definingGlobalAfterTranasition);
            definingState.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);
            definingState.AddTransition(beforeEntryTrigger, definingBeforeEntry);
            definingState.AddTransition(afterExitTrigger, definingAfterExit);

            definingStateInitiation.AddTransition(afterEntryTrigger, definingAfterEntry);
            definingStateInitiation.AddTransition(beforeExitTrigger, definingBeforeExit);
            definingStateInitiation.AddTransition(transitionsToTrigger, definingTransition);
            definingStateInitiation.AddTransition(stateTrigger, definingState);
            definingStateInitiation.AddTransition(compileTrigger, compiling);
            definingStateInitiation.AddTransition(BeforeTransitionTrigger, definingGlobalBeforeTransition);
            definingStateInitiation.AddTransition(AfterTranasitionTrigger, definingGlobalAfterTranasition);
            definingStateInitiation.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);
            definingStateInitiation.AddTransition(beforeEntryTrigger, definingBeforeEntry);
            definingStateInitiation.AddTransition(afterExitTrigger, definingAfterExit);

            definingTransition.AddTransition(onTrigger, definingTransitionTrigger);

            definingTransitionTrigger.AddTransition(whenTrigger, definingTransitionGuard);
            definingTransitionTrigger.AddTransition(afterEntryTrigger, definingAfterEntry);
            definingTransitionTrigger.AddTransition(beforeExitTrigger, definingBeforeExit);
            definingTransitionTrigger.AddTransition(transitionsToTrigger, definingTransition);
            definingTransitionTrigger.AddTransition(initiatesTrigger, definingStateInitiation);
            definingTransitionTrigger.AddTransition(stateTrigger, definingState);
            definingTransitionTrigger.AddTransition(compileTrigger, compiling);
            definingTransitionTrigger.AddTransition(BeforeTransitionTrigger, definingGlobalBeforeTransition);
            definingTransitionTrigger.AddTransition(AfterTranasitionTrigger, definingGlobalAfterTranasition);
            definingTransitionTrigger.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);
            definingTransitionTrigger.AddTransition(beforeEntryTrigger, definingBeforeEntry);
            definingTransitionTrigger.AddTransition(afterExitTrigger, definingAfterExit);

            definingTransitionGuard.AddTransition(afterEntryTrigger, definingAfterEntry);
            definingTransitionGuard.AddTransition(beforeExitTrigger, definingBeforeExit);
            definingTransitionGuard.AddTransition(transitionsToTrigger, definingTransition);
            definingTransitionGuard.AddTransition(initiatesTrigger, definingStateInitiation);
            definingTransitionGuard.AddTransition(stateTrigger, definingState);
            definingTransitionGuard.AddTransition(compileTrigger, compiling);
            definingTransitionGuard.AddTransition(BeforeTransitionTrigger, definingGlobalBeforeTransition);
            definingTransitionGuard.AddTransition(AfterTranasitionTrigger, definingGlobalAfterTranasition);
            definingTransitionGuard.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);
            definingTransitionGuard.AddTransition(beforeEntryTrigger, definingBeforeEntry);
            definingTransitionGuard.AddTransition(afterExitTrigger, definingAfterExit);

            definingAfterEntry.AddTransition(beforeExitTrigger, definingBeforeExit);
            definingAfterEntry.AddTransition(transitionsToTrigger, definingTransition);
            definingAfterEntry.AddTransition(initiatesTrigger, definingStateInitiation);
            definingAfterEntry.AddTransition(stateTrigger, definingState);
            definingAfterEntry.AddTransition(compileTrigger, compiling);
            definingAfterEntry.AddTransition(BeforeTransitionTrigger, definingGlobalBeforeTransition);
            definingAfterEntry.AddTransition(AfterTranasitionTrigger, definingGlobalAfterTranasition);
            definingAfterEntry.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);
            definingAfterEntry.AddTransition(beforeEntryTrigger, definingBeforeEntry);
            definingAfterEntry.AddTransition(afterExitTrigger, definingAfterExit);

            definingBeforeExit.AddTransition(afterEntryTrigger, definingBeforeExit);
            definingBeforeExit.AddTransition(transitionsToTrigger, definingTransition);
            definingBeforeExit.AddTransition(initiatesTrigger, definingStateInitiation);
            definingBeforeExit.AddTransition(stateTrigger, definingState);
            definingBeforeExit.AddTransition(compileTrigger, compiling);
            definingBeforeExit.AddTransition(BeforeTransitionTrigger, definingGlobalBeforeTransition);
            definingBeforeExit.AddTransition(AfterTranasitionTrigger, definingGlobalAfterTranasition);
            definingBeforeExit.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);
            definingBeforeExit.AddTransition(beforeEntryTrigger, definingBeforeEntry);
            definingBeforeExit.AddTransition(afterExitTrigger, definingAfterExit);

            definingBeforeEntry.AddTransition(beforeExitTrigger, definingBeforeExit);
            definingBeforeEntry.AddTransition(transitionsToTrigger, definingTransition);
            definingBeforeEntry.AddTransition(initiatesTrigger, definingStateInitiation);
            definingBeforeEntry.AddTransition(stateTrigger, definingState);
            definingBeforeEntry.AddTransition(compileTrigger, compiling);
            definingBeforeEntry.AddTransition(BeforeTransitionTrigger, definingGlobalBeforeTransition);
            definingBeforeEntry.AddTransition(AfterTranasitionTrigger, definingGlobalAfterTranasition);
            definingBeforeEntry.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);
            definingBeforeEntry.AddTransition(afterEntryTrigger, definingAfterEntry);
            definingBeforeEntry.AddTransition(afterExitTrigger, definingAfterExit);

            definingAfterExit.AddTransition(afterEntryTrigger, definingBeforeExit);
            definingAfterExit.AddTransition(transitionsToTrigger, definingTransition);
            definingAfterExit.AddTransition(initiatesTrigger, definingStateInitiation);
            definingAfterExit.AddTransition(stateTrigger, definingState);
            definingAfterExit.AddTransition(compileTrigger, compiling);
            definingAfterExit.AddTransition(BeforeTransitionTrigger, definingGlobalBeforeTransition);
            definingAfterExit.AddTransition(AfterTranasitionTrigger, definingGlobalAfterTranasition);
            definingAfterExit.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);
            definingAfterExit.AddTransition(beforeEntryTrigger, definingBeforeEntry);
            definingAfterExit.AddTransition(beforeExitTrigger, definingBeforeExit);

            definingGlobalBeforeTransition.AddTransition(stateTrigger, definingState);
            definingGlobalBeforeTransition.AddTransition(AfterTranasitionTrigger, definingGlobalAfterTranasition);
            definingGlobalBeforeTransition.AddTransition(compileTrigger, compiling);
            definingGlobalBeforeTransition.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);

            definingGlobalAfterTranasition.AddTransition(stateTrigger, definingState);
            definingGlobalAfterTranasition.AddTransition(BeforeTransitionTrigger, definingGlobalBeforeTransition);
            definingGlobalAfterTranasition.AddTransition(compileTrigger, compiling);
            definingGlobalAfterTranasition.AddTransition(globallyTransitionsToTrigger, definingGlobalTransition);

            definingGlobalTransition.AddTransition(onTrigger, definingTransitionTrigger);

            // wire transition callbacks

            definingState.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
                e.Model.FinalizeState();
            };

            definingStateInitiation.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
            };
            definingStateInitiation.Exiting += (s, e) =>
            {
                e.Model.workingInitialState = e.Model.workingState;
            };

            definingTransition.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
            };

            definingTransitionGuard.Exiting += (s, e) =>
            {
                e.Model.FinalizeTransition();
            };

            definingBeforeEntry.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
            };
            definingBeforeEntry.Exited += (s, e) =>
            {
                // grab an immediate reference to working entry.
                // will have changed by the time the lambda is called
                var entry = e.Model.workingBeforeEntry;
                e.Model.workingState.Entering += (es, ee) => entry(ee);
            };

            definingAfterEntry.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
            };

            definingAfterEntry.Exiting += (s, e) =>
            {
                // grab an immediate reference to working entry.
                // will have changed by the time the lambda is called
                var entry = e.Model.workingAfterEntry;
                e.Model.workingState.Entered += (es, ee) => entry(ee);
            };

            definingBeforeExit.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
            };
            definingBeforeExit.Exiting += (s, e) =>
            {
                // grab an immediate reference to working exit.
                // will have changed by the time the lambda is called
                var exit = e.Model.workingBeforeExit;
                e.Model.workingState.Exiting += (es, ee) => exit(ee);
            };

            definingAfterExit.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
            };
            definingAfterExit.Exiting += (s, e) =>
            {
                // grab an immediate reference to working exit.
                // will have changed by the time the lambda is called
                var exit = e.Model.workingAfterExit;
                e.Model.workingState.Exited += (es, ee) => exit(ee);
            };

            definingGlobalBeforeTransition.Entered += (s, e) =>
            {
                e.Model.FinalizeTransition();
                e.Model.FinalizeState();
            };

            definingGlobalAfterTranasition.Entered += (s, e) =>
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
