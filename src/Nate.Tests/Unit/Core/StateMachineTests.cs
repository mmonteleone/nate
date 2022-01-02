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
using Moq;
using Nate.Core;
using Xunit;

namespace Nate.Tests.Unit.Core
{
    public class StateMachineTests
    {
        private class TrackableStateModel : IStateModel
        {
            private object currentState;
            public EventHandler<EventArgs> Getting { get; set; }
            public EventHandler<EventArgs> Setting { get; set; }

            public object CurrentState
            {
                get
                {
                    if (Getting != null)
                        Getting(this, new EventArgs());
                    return currentState;
                }
                set
                {
                    if (Setting != null)
                        Setting(this, new EventArgs());
                    currentState = value;
                }
            }
        }

        [Fact]
        public void StateMachine_AddGlobalTransition_NullTran_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StateMachine<StubStateModel>().AddGlobalTransition(null));
        }

        [Fact]
        public void StateMachine_AddGlobalTransition_SourceNonNull_ThrowsInvalidTransEx()
        {
            var machine = new StateMachine<StubStateModel>();
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2);

            Assert.Throws<InvalidTransitionException>(() =>
                machine.AddGlobalTransition(transition1));
        }

        [Fact]
        public void
            StateMachine_AddGlobalTransition_TwoTransitionsWithSameTriggerFromToNoGuard_ThrowsDuplicateTransitionException()
        {
            var machine = new StateMachine<StubStateModel>();
            var state2 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, null, state2);
            var transition2 = new Transition<StubStateModel>(trigger1, null, state2);

            machine.AddGlobalTransition(transition1);
            Assert.Throws<InvalidTransitionException>(() =>
                machine.AddGlobalTransition(transition2));
        }

        [Fact]
        public void StateMachine_AddGlobalTransition_TwoValidTranDiffTriggers_ReturnsOneForEachTransitionsOn()
        {
            var machine = new StateMachine<StubStateModel>();
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");
            var transition1 = new Transition<StubStateModel>(trigger1, null, state2, m => 1 == 2);
            var transition2 = new Transition<StubStateModel>(trigger2, null, state3, m => 1 == 1);

            machine.AddGlobalTransition(transition1);
            machine.AddGlobalTransition(transition2);

            Assert.Same(transition1, machine.GlobalTransitionsOn(trigger1).FirstOrDefault());
            Assert.Same(transition2, machine.GlobalTransitionsOn(trigger2).FirstOrDefault());
        }

        [Fact]
        public void StateMachine_AddGlobalTransition_TwoValidTranSameTrigger_ReturnsBothWithTransitionsOn()
        {
            var machine = new StateMachine<StubStateModel>();
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, null, state2, m => 1 == 2);
            var transition2 = new Transition<StubStateModel>(trigger1, null, state3, m => 1 == 1);

            machine.AddGlobalTransition(transition1);
            machine.AddGlobalTransition(transition2);

            var trans = machine.GlobalTransitionsOn(trigger1).ToList();
            Assert.Equal(transition1, trans[0]);
            Assert.Equal(transition2, trans[1]);
        }

        [Fact]
        public void StateMachine_AddGlobalTransition_ValidTran_ReturnsWithTransitionsOnTrigger()
        {
            var machine = new StateMachine<StubStateModel>();
            var state2 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, null, state2);

            machine.AddGlobalTransition(transition1);

            Assert.Same(transition1, machine.GlobalTransitionsOn(trigger1).FirstOrDefault());
        }

        [Fact]
        public void StateMachine_AvailableStates_ConcatsModelsTransWithGlobal_ReturnsDistinctStatesByName()
        {
            var model = new StubStateModel();
            var machine = new StateMachine<StubStateModel>();

            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");
            var state4 = new State<StubStateModel>("s4");

            var statetrans3 = new Transition<StubStateModel>(trigger2, state1, state3);
            var statetrans1 = new Transition<StubStateModel>(trigger1, state1, state2);
            var statetrans2 = new Transition<StubStateModel>(trigger1, state1, state3);

            var globaltrans1 = new Transition<StubStateModel>(trigger2, null, state1);

            machine.AddGlobalTransition(globaltrans1);
            state1.AddTransition(statetrans1);
            state1.AddTransition(statetrans2);
            state1.AddTransition(statetrans3);

            model.CurrentState = state1;

            var result = machine.AvailableStates(model);

            Assert.Equal(3, result.Count());
            Assert.Equal(state1, result.ToList()[0]);
            Assert.Equal(state2, result.ToList()[1]);
            Assert.Equal(state3, result.ToList()[2]);
        }

        [Fact]
        public void StateMachine_AvailableStates_ModelStateNotState_ThrowInvalidStateModel()
        {
            var model = new StubStateModel();
            var machine = new StateMachine<StubStateModel>();
            Assert.Throws<InvalidStateModelException>(() =>
                machine.AvailableStates(model));
        }


        [Fact]
        public void StateMachine_AvailableStates_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StateMachine<StubStateModel>().AvailableStates(null));
        }

        [Fact]
        public void StateMachine_AvailableTriggers_ConcatsModelsTransWithGlobal_ReturnsDistinctTrigsByName()
        {
            var model = new StubStateModel();
            var machine = new StateMachine<StubStateModel>();

            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s3");
            var state4 = new State<StubStateModel>("s4");

            var statetrans1 = new Transition<StubStateModel>(trigger1, state1, state2);
            var statetrans2 = new Transition<StubStateModel>(trigger1, state1, state3);
            var statetrans3 = new Transition<StubStateModel>(trigger2, state1, state3);

            var globaltrans1 = new Transition<StubStateModel>(trigger2, null, state1);

            machine.AddGlobalTransition(globaltrans1);
            state1.AddTransition(statetrans1);
            state1.AddTransition(statetrans2);
            state1.AddTransition(statetrans3);

            model.CurrentState = state1;

            var result = machine.AvailableTriggers(model);

            Assert.Equal(2, result.Count());
            Assert.Equal(trigger1, result.ToList()[0]);
            Assert.Equal(trigger2, result.ToList()[1]);
        }

        [Fact]
        public void StateMachine_AvailableTriggers_ModelStateNotState_ThrowInvalidStateModel()
        {
            var model = new StubStateModel();
            var machine = new StateMachine<StubStateModel>();
            Assert.Throws<InvalidStateModelException>(() =>
                machine.AvailableTriggers(model));
        }

        [Fact]
        public void StateMachine_AvailableTriggers_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StateMachine<StubStateModel>().AvailableTriggers(null));
        }

        [Fact]
        public void StateMachine_GlobalTransitionsOn_NewTrigger_NoMatchingTrans_ReturnsEmptyNonNullEnumerable()
        {
            var machine = new StateMachine<StubStateModel>();

            var result = machine.GlobalTransitionsOn(new Trigger("sometrigger"));
            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public void StateMachine_GlobalTransitionsOn_NullTrigger_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StateMachine<StubStateModel>().GlobalTransitionsOn(null));
        }

        [Fact]
        public void StateMachine_GlobalTransitionsOn_TriggerMatchingOneTransitionNotAnother_ReturnsOnlyMatchingONe()
        {
            var machine = new StateMachine<StubStateModel>();
            var state2 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, null, state2, m => 1 == 2);

            machine.AddGlobalTransition(transition1);

            var result = machine.GlobalTransitionsOn(new Trigger("t1"));
            Assert.NotNull(result);
            Assert.Single( result);
            Assert.Same(transition1, result.FirstOrDefault());
        }

        [Fact]
        public void StateMachine_GlobalTransitionsOn_TriggerMatchingThree_ReturnsThreeInOrderAdded()
        {
            var machine = new StateMachine<StubStateModel>();
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition2 = new Transition<StubStateModel>(trigger1, null, state3, m => 1 == 1);
            var transition1 = new Transition<StubStateModel>(trigger1, null, state2, m => 1 == 2);

            machine.AddGlobalTransition(transition1);
            machine.AddGlobalTransition(transition2);

            var result = machine.GlobalTransitionsOn(new Trigger("t1"));
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Same(transition1, result.ToList()[0]);
            Assert.Same(transition2, result.ToList()[1]);
        }

        [Fact]
        public void StateMachine_GlobalTransitionsOn_TriggerMatchingTwoTransitionsNotAnother_ReturnsTwoMatching()
        {
            var machine = new StateMachine<StubStateModel>();
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, null, state2, m => 1 == 2);
            var transition2 = new Transition<StubStateModel>(trigger1, null, state3, m => 1 == 1);

            machine.AddGlobalTransition(transition1);
            machine.AddGlobalTransition(transition2);

            var result = machine.GlobalTransitionsOn(new Trigger("t1"));
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Same(transition1, result.ToList()[0]);
            Assert.Same(transition2, result.ToList()[1]);
        }

        [Fact]
        public void StateMachine_NullConfiguration_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StateMachine<StubStateModel>(null));
        }

        [Fact]
        public void StateMachine_ParamLessCtor_ConfigStillSet()
        {
            var machine = new StateMachine<StubStateModel>();
            Assert.NotNull(machine.Configuration);
        }

        [Fact]
        public void StateMachine_Trigger_ModelStateNotState_ThrowsInvalidStateModel()
        {
            var machine = new StateMachine<StubStateModel>();
            var model = new StubStateModel();
            var trigger = new Trigger("trigger");

            Assert.Throws<InvalidStateModelException>(() =>
                machine.Trigger(trigger, model));
        }

        [Fact]
        public void
            StateMachine_Trigger_NoPassingTransitionOnTrigger_ConfigRaiseOnNoPassingTranFalse_ThrowsInvalidTrigger()
        {
            var config = new StateMachineConfiguration {RaiseExceptionOnTriggerMatchingNoPassingTransition = false};
            var machine = new StateMachine<StubStateModel>(config);
            var model = new StubStateModel();
            var trigger1 = new Trigger("trigger1");
            var state1 = new State<StubStateModel>("state1");
            var state2 = new State<StubStateModel>("state2");
            state1.AddTransition(trigger1, state2, m => 1 == 2);
            model.CurrentState = state1;

            // set up so that there's a matching transition, but the guard would fail when run

            // no exceptions should happen, and that's good enough for us
            machine.Trigger(trigger1, model);
        }

        [Fact]
        public void
            StateMachine_Trigger_NoPassingTransitionOnTrigger_ConfigRaiseOnNoPassingTranTrue_ThrowsInvalidTrigger()
        {
            var config = new StateMachineConfiguration {RaiseExceptionOnTriggerMatchingNoPassingTransition = true};
            var machine = new StateMachine<StubStateModel>(config);
            var model = new StubStateModel();
            var trigger1 = new Trigger("trigger1");
            var state1 = new State<StubStateModel>("state1");
            var state2 = new State<StubStateModel>("state2");
            state1.AddTransition(trigger1, state2, m => 1 == 2);
            model.CurrentState = state1;

            // set up so that there's a matching transition, but the guard would fail when run

            Assert.Throws<InvalidTriggerException>(() =>
                machine.Trigger(trigger1, model));
        }

        [Fact]
        public void
            StateMachine_Trigger_NoStateOrGlobalTransOnTrigger_ConfigRaiseOnNoTransFalse_NotThrowsInvalidTrigger()
        {
            var config = new StateMachineConfiguration {RaiseExceptionOnTriggerMatchingNoTransition = false};
            var machine = new StateMachine<StubStateModel>(config);
            var model = new StubStateModel();
            var trigger1 = new Trigger("trigger1");
            var trigger2 = new Trigger("trigger2");
            var state1 = new State<StubStateModel>("state1");
            var state2 = new State<StubStateModel>("state2");
            state1.AddTransition(trigger1, state2);
            model.CurrentState = state1;

            // no exception shoudl happen, and that's good enough for us.
            machine.Trigger(trigger2, model);
        }

        [Fact]
        public void StateMachine_Trigger_NoStateOrGlobalTransOnTrigger_ConfigRaiseOnNoTransTrue_ThrowsInvalidTrigger()
        {
            var config = new StateMachineConfiguration {RaiseExceptionOnTriggerMatchingNoTransition = true};
            var machine = new StateMachine<StubStateModel>(config);
            var model = new StubStateModel();
            var trigger1 = new Trigger("trigger1");
            var trigger2 = new Trigger("trigger2");
            var state1 = new State<StubStateModel>("state1");
            var state2 = new State<StubStateModel>("state2");
            state1.AddTransition(trigger1, state2);
            model.CurrentState = state1;

            // set up so that current state of state 1 doesn't' define a trans for "trigger2", only "trigger1"

            Assert.Throws<InvalidTriggerException>(() =>
                machine.Trigger(trigger2, model));
        }

        [Fact]
        public void StateMachine_Trigger_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StateMachine<StubStateModel>().Trigger(new Trigger("t1"), null));
        }


        [Fact]
        public void StateMachine_Trigger_NullTrigger_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StateMachine<StubStateModel>().Trigger(null, new StubStateModel()));
        }

        [Fact]
        public void
            StateMachine_Trigger_TransitionFromCurrentToSame_ConfiggedNotToRaiseExcep_DoesNotRaiseTransitionChangeEvents()
        {
            var config = new StateMachineConfiguration {RaiseExceptionBeforeTransitionToSameState = false};
            var machine = new StateMachine<StubStateModel>(config);
            var model = new StubStateModel();
            var trigger1 = new Trigger("trigger1");

            var state1 = new State<StubStateModel>("state1");
            TransitionEventArgs<StubStateModel> transitioningArgs = null;
            state1.Entered += (s, e) => { transitioningArgs = e; };
            TransitionEventArgs<StubStateModel> transitionedArgs = null;
            state1.Exiting += (s, e) => { transitionedArgs = e; };

            state1.AddTransition(trigger1, state1);
            model.CurrentState = state1;

            // set up scenario where state would transition from current to same state.  
            // so no true transition would not occur

            // should not throw exception and that's good enough for us
            machine.Trigger(trigger1, model);

            // verify that no transition events occurred
            Assert.Null(transitioningArgs);
            Assert.Null(transitionedArgs);
        }

        [Fact]
        public void StateMachine_Trigger_TransitionsToSameState_ConfigRaiseOnSameStateTranFalse_ThrowsInvalidTrigger()
        {
            var config = new StateMachineConfiguration {RaiseExceptionBeforeTransitionToSameState = false};
            var machine = new StateMachine<StubStateModel>(config);
            var model = new StubStateModel();
            var trigger1 = new Trigger("trigger1");
            var state1 = new State<StubStateModel>("state1");
            var state2 = new State<StubStateModel>("state2");
            state1.AddTransition(trigger1, state1);
            model.CurrentState = state1;

            // set up scenario where state would transition from current to same state.  
            // so no true transition would not occur

            // should not throw exception and that's good enough for us
            machine.Trigger(trigger1, model);
        }

        [Fact]
        public void StateMachine_Trigger_TransitionsToSameState_ConfigRaiseOnSameStateTranTrue_ThrowsInvalidTrigger()
        {
            var config = new StateMachineConfiguration {RaiseExceptionBeforeTransitionToSameState = true};
            var machine = new StateMachine<StubStateModel>(config);
            var model = new StubStateModel();
            var trigger1 = new Trigger("trigger1");
            var state1 = new State<StubStateModel>("state1");
            var state2 = new State<StubStateModel>("state2");
            state1.AddTransition(trigger1, state1);
            model.CurrentState = state1;

            // set up scenario where state would transition from current to same state.  
            // so no true transition would not occur

            Assert.Throws<InvalidTriggerException>(() =>
                machine.Trigger(trigger1, model));
        }

        [Fact]
        public void StateMachine_Trigger_ValidlyTransitions_RaisesAllEventsInCorrectOrder()
        {
            var calls = new List<string>();
            var model = new TrackableStateModel();
            var state1 = new State<TrackableStateModel>("s1");
            var state2 = new State<TrackableStateModel>("s2");
            var trigger = new Trigger("t1");
            var transition = new Transition<TrackableStateModel>(trigger, state1, state2);
            state1.AddTransition(transition);
            model.CurrentState = state1;

            var stateMachine = new StateMachine<TrackableStateModel>();

            Action<object, TransitionEventArgs<TrackableStateModel>> entering = (s, e) =>
                calls.Add(string.Format("entering {0} from {1} on {2}", e.To, e.From, e.Trigger));
            Action<object, TransitionEventArgs<TrackableStateModel>> entered = (s, e) =>
                calls.Add(string.Format("entered {0} from {1} on {2}", e.To, e.From, e.Trigger));
            Action<object, TransitionEventArgs<TrackableStateModel>> exiting = (s, e) =>
                calls.Add(string.Format("exiting {0} to {1} on {2}", e.From, e.To, e.Trigger));
            Action<object, TransitionEventArgs<TrackableStateModel>> exited = (s, e) =>
                calls.Add(string.Format("exited {0} to {1} on {2}", e.From, e.To, e.Trigger));

            model.Setting += (s, e) => calls.Add("setting new current state on model");

            state1.Entering += (s, e) => entering(s, e);
            state1.Entered += (s, e) => entered(s, e);
            state1.Exiting += (s, e) => exiting(s, e);
            state1.Exited += (s, e) => exited(s, e);

            state2.Entering += (s, e) => entering(s, e);
            state2.Entered += (s, e) => entered(s, e);
            state2.Exiting += (s, e) => exiting(s, e);
            state2.Exited += (s, e) => exited(s, e);

            stateMachine.Transitioning += (s, e) =>
                calls.Add(string.Format("transitioning from {0} to {1} on {2}", e.From, e.To, e.Trigger));
            stateMachine.Transitioned += (s, e) =>
                calls.Add(string.Format("transitioned from {0} to {1} on {2}", e.From, e.To, e.Trigger));

            stateMachine.Trigger(trigger, model);

            Assert.Equal(7, calls.Count);
            Assert.Equal("transitioning from s1 to s2 on t1", calls[0]);
            Assert.Equal("exiting s1 to s2 on t1", calls[1]);
            Assert.Equal("entering s2 from s1 on t1", calls[2]);
            Assert.Equal("setting new current state on model", calls[3]);
            Assert.Equal("exited s1 to s2 on t1", calls[4]);
            Assert.Equal("entered s2 from s1 on t1", calls[5]);
            Assert.Equal("transitioned from s1 to s2 on t1", calls[6]);
        }

        [Fact]
        public void
            StateMachine_Trigger_ValidTriggerAndTrans_ChoosesFirstTranWthPassGuard_DiffAsCurrentState_Raised_Sets()
        {
            var model = new StubStateModel();
            var machine = new StateMachine<StubStateModel>();

            var transitioningEventArgs = new List<TransitionEventArgs<StubStateModel>>();
            machine.Transitioning += (s, e) => { transitioningEventArgs.Add(e); };

            var transitionedEventArgs = new List<TransitionEventArgs<StubStateModel>>();
            machine.Transitioned += (s, e) => { transitionedEventArgs.Add(e); };

            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var state1Mock = new Mock<State<StubStateModel>>("s1");
            var state2Mock = new Mock<State<StubStateModel>>("s2");
            var state3Mock = new Mock<State<StubStateModel>>("s3");
            var state4Mock = new Mock<State<StubStateModel>>("s4");

            var state1 = state1Mock.Object;
            var state2 = state2Mock.Object;
            var state3 = state3Mock.Object;
            var state4 = state4Mock.Object;

            var statetrans1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);
            var statetrans2 = new Transition<StubStateModel>(trigger1, state1, state3, m => 1 == 3);
            var statetrans3 = new Transition<StubStateModel>(trigger2, state1, state3);

            var globaltrans1 = new Transition<StubStateModel>(trigger1, null, state2, m => 1 == 1);

            machine.AddGlobalTransition(globaltrans1);
            state1.AddTransition(statetrans1);
            state1.AddTransition(statetrans2);
            state1.AddTransition(statetrans3);

            model.CurrentState = state1;

            // so when trigger 1 is triggered on state model whose current state is state1,
            // it will fall through to the first maching transition with a passing guard, the global transition to state 2
            // it will then transition to state2.

            // make sure that transition events are called for moving from state 1 to state 2
            state1Mock.Setup(s => s.RaiseExiting(model, state2, trigger1)).Verifiable();
            state2Mock.Setup(s => s.RaiseEntered(model, state1, trigger1)).Verifiable();

            machine.Trigger(trigger1, model);

            // verify that no transition events were called that shouldn't have been
            state1Mock.Verify(s =>
                s.RaiseEntered(
                    It.IsAny<StubStateModel>(),
                    It.IsAny<State<StubStateModel>>(),
                    It.IsAny<Trigger>()), Times.Never());
            state2Mock.Verify(s =>
                s.RaiseExiting(
                    It.IsAny<StubStateModel>(),
                    It.IsAny<State<StubStateModel>>(),
                    It.IsAny<Trigger>()), Times.Never());
            Assert.Single( transitioningEventArgs);
            Assert.Single( transitionedEventArgs);

            Assert.Equal(state1, transitioningEventArgs[0].From);
            Assert.Equal(state2, transitioningEventArgs[0].To);
            Assert.Equal(trigger1, transitioningEventArgs[0].Trigger);
            Assert.Equal(model, transitioningEventArgs[0].Model);
            Assert.Equal(state1, transitionedEventArgs[0].From);
            Assert.Equal(state2, transitionedEventArgs[0].To);
            Assert.Equal(trigger1, transitionedEventArgs[0].Trigger);
            Assert.Equal(model, transitionedEventArgs[0].Model);
            Assert.Equal(state2, model.CurrentState);
        }

        [Fact]
        public void StateMachine_Trigger_ValidTriggerAndTrans_ChoosesFirstTranWthPassGuard_SameAsCurrentState_NoRaised()
        {
            var model = new StubStateModel();
            var machine = new StateMachine<StubStateModel>();
            TransitionEventArgs<StubStateModel> transitioningArgs = null;
            machine.Transitioning += (s, e) => { transitioningArgs = e; };
            TransitionEventArgs<StubStateModel> transitionedArgs = null;
            machine.Transitioned += (s, e) => { transitionedArgs = e; };

            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var state1Mock = new Mock<State<StubStateModel>>("s1");
            var state2Mock = new Mock<State<StubStateModel>>("s2");
            var state3Mock = new Mock<State<StubStateModel>>("s3");
            var state4Mock = new Mock<State<StubStateModel>>("s4");

            var state1 = state1Mock.Object;
            var state2 = state2Mock.Object;
            var state3 = state3Mock.Object;
            var state4 = state4Mock.Object;

            var statetrans1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);
            var statetrans2 = new Transition<StubStateModel>(trigger1, state1, state3, m => 1 == 3);
            var statetrans3 = new Transition<StubStateModel>(trigger2, state1, state3);

            var globaltrans1 = new Transition<StubStateModel>(trigger1, null, state1, m => 1 == 1);

            machine.AddGlobalTransition(globaltrans1);
            state1.AddTransition(statetrans1);
            state1.AddTransition(statetrans2);
            state1.AddTransition(statetrans3);

            model.CurrentState = state1;

            // so when trigger1 is triggered on state model whose current state is state1,
            // it will fall through to the first matching transition with a passing guard, the global transition to state 1
            // but since this is same state as current state, it shouldn't actually transition.  no events should be called.

            machine.Trigger(trigger1, model);

            // verify that no transition events were calledb
            state1Mock.Verify(s =>
                s.RaiseEntered(
                    It.IsAny<StubStateModel>(),
                    It.IsAny<State<StubStateModel>>(),
                    It.IsAny<Trigger>()), Times.Never());
            state1Mock.Verify(s =>
                s.RaiseExiting(
                    It.IsAny<StubStateModel>(),
                    It.IsAny<State<StubStateModel>>(),
                    It.IsAny<Trigger>()), Times.Never());
            Assert.Null(transitioningArgs);
            Assert.Null(transitionedArgs);
        }

        [Fact]
        public void
            StateMachine_Trigger_ValidTriggerAndTrans_ChoosesFirstTranWthPassGuardNonGlobal_DiffAsCurrentState_Raised_Sets()
        {
            var model = new StubStateModel();
            var machine = new StateMachine<StubStateModel>();

            var transitioningEventArgs = new List<TransitionEventArgs<StubStateModel>>();
            machine.Transitioning += (s, e) => { transitioningEventArgs.Add(e); };

            var transitionedEventArgs = new List<TransitionEventArgs<StubStateModel>>();
            machine.Transitioned += (s, e) => { transitionedEventArgs.Add(e); };

            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var state1Mock = new Mock<State<StubStateModel>>("s1");
            var state2Mock = new Mock<State<StubStateModel>>("s2");
            var state3Mock = new Mock<State<StubStateModel>>("s3");
            var state4Mock = new Mock<State<StubStateModel>>("s4");

            var state1 = state1Mock.Object;
            var state2 = state2Mock.Object;
            var state3 = state3Mock.Object;
            var state4 = state4Mock.Object;

            var statetrans1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);
            var statetrans2 = new Transition<StubStateModel>(trigger1, state2, state3, m => 1 == 1);
            var statetrans3 = new Transition<StubStateModel>(trigger2, state1, state3);

            var globaltrans1 = new Transition<StubStateModel>(trigger1, null, state2, m => 1 == 1);

            machine.AddGlobalTransition(globaltrans1);
            state1.AddTransition(statetrans1);
            state1.AddTransition(statetrans2);
            state1.AddTransition(statetrans3);

            model.CurrentState = state1;

            // so when trigger 1 is triggered on state model whose current state is state1,
            // it will fall through to the first maching transition with a passing guard, the second state transition to state 2
            // it will then transition to state2.

            // make sure that transition events are called for moving from state 1 to state 2
            state1Mock.Setup(s => s.RaiseExiting(model, state2, trigger1)).Verifiable();
            state2Mock.Setup(s => s.RaiseEntered(model, state1, trigger1)).Verifiable();

            machine.Trigger(trigger1, model);

            // verify that no transition events were called that shouldn't have been
            state1Mock.Verify(s =>
                s.RaiseEntered(
                    It.IsAny<StubStateModel>(),
                    It.IsAny<State<StubStateModel>>(),
                    It.IsAny<Trigger>()), Times.Never());
            state2Mock.Verify(s =>
                s.RaiseExiting(
                    It.IsAny<StubStateModel>(),
                    It.IsAny<State<StubStateModel>>(),
                    It.IsAny<Trigger>()), Times.Never());
            Assert.Single( transitioningEventArgs);
            Assert.Single(transitionedEventArgs);

            Assert.Equal(state1, transitioningEventArgs[0].From);
            Assert.Equal(state2, transitioningEventArgs[0].To);
            Assert.Equal(trigger1, transitioningEventArgs[0].Trigger);
            Assert.Equal(model, transitioningEventArgs[0].Model);
            Assert.Equal(state1, transitionedEventArgs[0].From);
            Assert.Equal(state2, transitionedEventArgs[0].To);
            Assert.Equal(trigger1, transitionedEventArgs[0].Trigger);
            Assert.Equal(model, transitionedEventArgs[0].Model);
            Assert.Equal(state2, model.CurrentState);
        }

        [Fact]
        public void
            StateMachine_Trigger_ValidTriggerAndTrans_ChoosesFirstTranWthPassGuardNonGlobal_SameAsCurrentState_NoRaised()
        {
            var model = new StubStateModel();
            var machine = new StateMachine<StubStateModel>();
            TransitionEventArgs<StubStateModel> transitioningArgs = null;
            machine.Transitioning += (s, e) => { transitioningArgs = e; };
            TransitionEventArgs<StubStateModel> transitionedArgs = null;
            machine.Transitioned += (s, e) => { transitionedArgs = e; };

            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");

            var state1Mock = new Mock<State<StubStateModel>>("s1");
            var state2Mock = new Mock<State<StubStateModel>>("s2");
            var state3Mock = new Mock<State<StubStateModel>>("s3");
            var state4Mock = new Mock<State<StubStateModel>>("s4");

            var state1 = state1Mock.Object;
            var state2 = state2Mock.Object;
            var state3 = state3Mock.Object;
            var state4 = state4Mock.Object;

            var statetrans1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);
            var statetrans2 = new Transition<StubStateModel>(trigger1, state1, state3, m => 1 == 1);
            var statetrans3 = new Transition<StubStateModel>(trigger2, state1, state3);

            var globaltrans1 = new Transition<StubStateModel>(trigger1, null, state1, m => 1 == 1);

            machine.AddGlobalTransition(globaltrans1);
            state1.AddTransition(statetrans1);
            state1.AddTransition(statetrans2);
            state1.AddTransition(statetrans3);

            model.CurrentState = state1;

            // so when trigger1 is triggered on state model whose current state is state1,
            // it will fall through to the first matching transition with a passing guard, the second state transition to state 1
            // but since this is same state as current state, it shouldn't actually transition.  no events should be called.

            machine.Trigger(trigger1, model);

            // verify that no transition events were calledb
            state1Mock.Verify(s =>
                s.RaiseEntered(
                    It.IsAny<StubStateModel>(),
                    It.IsAny<State<StubStateModel>>(),
                    It.IsAny<Trigger>()), Times.Never());
            state1Mock.Verify(s =>
                s.RaiseExiting(
                    It.IsAny<StubStateModel>(),
                    It.IsAny<State<StubStateModel>>(),
                    It.IsAny<Trigger>()), Times.Never());
            Assert.Null(transitioningArgs);
            Assert.Null(transitionedArgs);
        }

        [Fact]
        public void StateMachine_ValidConfigPassed_SetsConfigAsCurrentConfig()
        {
            var config = new StateMachineConfiguration {RaiseExceptionOnTriggerMatchingNoTransition = true};
            var machine = new StateMachine<StubStateModel>(config);
            Assert.Same(config, machine.Configuration);
            Assert.True(machine.Configuration.RaiseExceptionOnTriggerMatchingNoTransition);
        }
    }
}