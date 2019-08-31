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
using System.Linq;
using Nate.Core;
using Xunit;

namespace Nate.Tests.Unit.Core
{
    public class StateTests
    {
        [Fact]
        public void State_AddTransition_NullTran_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("state").AddTransition(null));
        }

        [Fact]
        public void State_Addtransition_SourceDifferentThanState_ThrowsInvalidTransEx()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, state2, state1);

            Assert.Throws<InvalidTransitionException>(() =>
                state1.AddTransition(transition1));
        }

        [Fact]
        public void State_AddTransition_TwoTransitionsWithSameTriggerFromToGuard_ThrowsDuplicateTransitionException()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            Func<StubStateModel, bool> guard = m => true;
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2, guard);
            var transition2 = new Transition<StubStateModel>(trigger1, state1, state2, guard);

            state1.AddTransition(transition1);
            Assert.Throws<InvalidTransitionException>(() =>
                state1.AddTransition(transition2));
        }

        [Fact]
        public void State_AddTransition_TwoTransitionsWithSameTriggerFromToNoGuard_ThrowsDuplicateTransitionException()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2);
            var transition2 = new Transition<StubStateModel>(trigger1, state1, state2);

            state1.AddTransition(transition1);
            Assert.Throws<InvalidTransitionException>(() =>
                state1.AddTransition(transition2));
        }

        [Fact]
        public void State_AddTransition_TwoValidTranDiffTriggers_ReturnsOneForEachTransitionsOn()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var trigger2 = new Trigger("t2");
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);
            var transition2 = new Transition<StubStateModel>(trigger2, state1, state3, m => 1 == 1);

            state1.AddTransition(transition1);
            state1.AddTransition(transition2);

            Assert.Same(transition1, state1.TransitionsOn(trigger1).FirstOrDefault());
            Assert.Same(transition2, state1.TransitionsOn(trigger2).FirstOrDefault());
        }

        [Fact]
        public void State_AddTransition_TwoValidTranSameTrigger_ReturnsBothWithTransitionsOn()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);
            var transition2 = new Transition<StubStateModel>(trigger1, state1, state3, m => 1 == 1);

            state1.AddTransition(transition1);
            state1.AddTransition(transition2);

            var trans = state1.TransitionsOn(trigger1).ToList();
            Assert.Equal(transition1, trans[0]);
            Assert.Equal(transition2, trans[1]);
        }

        [Fact]
        public void State_AddTransition_ValidTran_ReturnsWithTransitionsOnTrigger()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2);

            state1.AddTransition(transition1);

            Assert.Same(transition1, state1.TransitionsOn(trigger1).FirstOrDefault());
        }

        [Fact]
        public void State_AllowsNullCode()
        {
            var s = new State<StubStateModel>("name", null);
        }

        [Fact]
        public void State_AvailableTransitions_NoneAdded_ReturnsEmptyNonNullEnumerable()
        {
            var state1 = new State<StubStateModel>("s1");
            var result = state1.AvailableTransitions;
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void State_AvailableTransitions_ReturnsAllAdded()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition3 = new Transition<StubStateModel>(trigger1, state1, state1, m => 1 == 0);
            var transition2 = new Transition<StubStateModel>(trigger1, state1, state3, m => 1 == 1);
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);

            state1.AddTransition(transition1);
            state1.AddTransition(transition2);
            state1.AddTransition(transition3);

            var result = state1.AvailableTransitions;
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void State_Equals_BothCodedDiffCode_SameNames_False()
        {
            var s1 = new State<StubStateModel>("state", 2);
            var s2 = new State<StubStateModel>("state", 4);
            Assert.False(s1.Equals(s2));
        }

        [Fact]
        public void State_Equals_BothCodedSameCode_DiffNames_False()
        {
            var s1 = new State<StubStateModel>("state", 4);
            var s2 = new State<StubStateModel>("state1", 4);
            Assert.False(s1.Equals(s2));
        }

        [Fact]
        public void State_Equals_BothCodedSameCode_SameNames_True()
        {
            var s1 = new State<StubStateModel>("state", 4);
            var s2 = new State<StubStateModel>("state", 4);
            Assert.True(s1.Equals(s2));
        }

        [Fact]
        public void State_Equals_NeitherCoded_DiffName_False()
        {
            var s1 = new State<StubStateModel>("state");
            var s2 = new State<StubStateModel>("state2");
            Assert.False(s1.Equals(s2));
        }

        [Fact]
        public void State_Equals_OneCodedOneUncoded_DiffName_False()
        {
            var s1 = new State<StubStateModel>("state", 4);
            var s2 = new State<StubStateModel>("state1");
            Assert.False(s1.Equals(s2));
        }

        [Fact]
        public void State_Equals_OneCodedOneUncoded_SameName_True()
        {
            var s1 = new State<StubStateModel>("state", 4);
            var s2 = new State<StubStateModel>("state");
            Assert.True(s1.Equals(s2));
        }

        [Fact]
        public void State_GetHashCode_WhenCoded_ReturnsCodeHash()
        {
            var code = (int?) 5;
            var name = "state";
            var state = new State<StubStateModel>(name, code);

            Assert.Equal(code.GetHashCode(), state.GetHashCode());
        }

        [Fact]
        public void State_GetHashCode_WhenUncoded_ReturnsNameHash()
        {
            var name = "state";
            var state = new State<StubStateModel>(name);

            Assert.Equal(name.GetHashCode(), state.GetHashCode());
        }

        [Fact]
        public void State_Name_VerifyName()
        {
            var state = new State<StubStateModel>("state");
            Assert.Equal("state", state.Name);
        }

        [Fact]
        public void State_NameCode_VerifyNameCode()
        {
            var state = new State<StubStateModel>("state", 2);
            Assert.Equal("state", state.Name);
            Assert.Equal(2, state.Code.Value);
        }

        [Fact]
        public void State_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>(null));
        }

        [Fact]
        public void State_RaiseEntered_NullFrom_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("to").RaiseEntered(
                    new StubStateModel(),
                    null,
                    new Trigger("trigger")));
        }

        [Fact]
        public void State_RaiseEntered_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("to").RaiseEntered(
                    null,
                    new State<StubStateModel>("from"),
                    new Trigger("trigger")));
        }

        [Fact]
        public void State_RaiseEntered_NullTrigger_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("to").RaiseEntered(
                    new StubStateModel(),
                    new State<StubStateModel>("from"),
                    null));
        }

        [Fact]
        public void State_RaiseEntered_ValidParms_Handler_TriggersHandlerPassesParmsAndSelf()
        {
            var model = new StubStateModel();
            var fromState = new State<StubStateModel>("from");
            var toState = new State<StubStateModel>("to");
            var trigger = new Trigger("trigger");

            TransitionEventArgs<StubStateModel> args = null;
            toState.Entered += (s, e) => { args = e; };

            toState.RaiseEntered(model, fromState, trigger);

            Assert.Equal(model, args.Model);
            Assert.Equal(fromState, args.From);
            Assert.Equal(toState, args.To);
            Assert.Equal(trigger, args.Trigger);
        }

        [Fact]
        public void State_RaiseEntered_ValidParms_NoHandlers_NoException()
        {
            new State<StubStateModel>("to").RaiseEntered(
                new StubStateModel(),
                new State<StubStateModel>("from"),
                new Trigger("trigger"));
        }

        [Fact]
        public void State_RaiseEntering_NullFrom_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("to").RaiseEntering(
                    new StubStateModel(),
                    null,
                    new Trigger("trigger")));
        }


        [Fact]
        public void State_RaiseEntering_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("to").RaiseEntering(
                    null,
                    new State<StubStateModel>("from"),
                    new Trigger("trigger")));
        }

        [Fact]
        public void State_RaiseEntering_NullTrigger_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("to").RaiseEntering(
                    new StubStateModel(),
                    new State<StubStateModel>("from"),
                    null));
        }

        [Fact]
        public void State_RaiseEntering_ValidParms_Handler_TriggersHandlerPassesParmsAndSelf()
        {
            var model = new StubStateModel();
            var fromState = new State<StubStateModel>("from");
            var toState = new State<StubStateModel>("to");
            var trigger = new Trigger("trigger");

            TransitionEventArgs<StubStateModel> args = null;
            toState.Entering += (s, e) => { args = e; };

            toState.RaiseEntering(model, fromState, trigger);

            Assert.Equal(model, args.Model);
            Assert.Equal(fromState, args.From);
            Assert.Equal(toState, args.To);
            Assert.Equal(trigger, args.Trigger);
        }

        [Fact]
        public void State_RaiseEntering_ValidParms_NoHandlers_NoException()
        {
            new State<StubStateModel>("to").RaiseEntering(
                new StubStateModel(),
                new State<StubStateModel>("from"),
                new Trigger("trigger"));
        }

        [Fact]
        public void State_RaiseExited_NullFrom_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("from").RaiseExited(
                    new StubStateModel(),
                    null,
                    new Trigger("trigger")));
        }


        [Fact]
        public void State_RaiseExited_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("from").RaiseExited(
                    null,
                    new State<StubStateModel>("to"),
                    new Trigger("trigger")));
        }

        [Fact]
        public void State_RaiseExited_NullTrigger_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("from").RaiseExited(
                    new StubStateModel(),
                    new State<StubStateModel>("to"),
                    null));
        }

        [Fact]
        public void State_RaiseExited_ValidParms_Handler_TriggersHandlerPassesParmsAndSelf()
        {
            var model = new StubStateModel();
            var fromState = new State<StubStateModel>("from");
            var toState = new State<StubStateModel>("to");
            var trigger = new Trigger("trigger");

            TransitionEventArgs<StubStateModel> args = null;
            fromState.Exited += (s, e) => { args = e; };

            fromState.RaiseExited(model, toState, trigger);

            Assert.Equal(model, args.Model);
            Assert.Equal(fromState, args.From);
            Assert.Equal(toState, args.To);
            Assert.Equal(trigger, args.Trigger);
        }

        [Fact]
        public void State_RaiseExited_ValidParms_NoHandlers_NoException()
        {
            new State<StubStateModel>("from").RaiseExited(
                new StubStateModel(),
                new State<StubStateModel>("to"),
                new Trigger("trigger"));
        }

        [Fact]
        public void State_RaiseExiting_NullFrom_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("from").RaiseExiting(
                    new StubStateModel(),
                    null,
                    new Trigger("trigger")));
        }


        [Fact]
        public void State_RaiseExiting_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("from").RaiseExiting(
                    null,
                    new State<StubStateModel>("to"),
                    new Trigger("trigger")));
        }

        [Fact]
        public void State_RaiseExiting_NullTrigger_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("from").RaiseExiting(
                    new StubStateModel(),
                    new State<StubStateModel>("to"),
                    null));
        }

        [Fact]
        public void State_RaiseExiting_ValidParms_Handler_TriggersHandlerPassesParmsAndSelf()
        {
            var model = new StubStateModel();
            var fromState = new State<StubStateModel>("from");
            var toState = new State<StubStateModel>("to");
            var trigger = new Trigger("trigger");

            TransitionEventArgs<StubStateModel> args = null;
            fromState.Exiting += (s, e) => { args = e; };

            fromState.RaiseExiting(model, toState, trigger);

            Assert.Equal(model, args.Model);
            Assert.Equal(fromState, args.From);
            Assert.Equal(toState, args.To);
            Assert.Equal(trigger, args.Trigger);
        }

        [Fact]
        public void State_RaiseExiting_ValidParms_NoHandlers_NoException()
        {
            new State<StubStateModel>("from").RaiseExiting(
                new StubStateModel(),
                new State<StubStateModel>("to"),
                new Trigger("trigger"));
        }

        [Fact]
        public void State_ToString_ReturnsName()
        {
            var s = new State<StubStateModel>("state");
            Assert.Equal("state", s.ToString());
        }

        [Fact]
        public void State_TransitionsOn_NewTrigger_NoMatchingTrans_ReturnsEmptyNonNullEnumerable()
        {
            var state1 = new State<StubStateModel>("s1");

            var result = state1.TransitionsOn(new Trigger("sometrigger"));
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void State_TransitionsOn_NullTrigger_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new State<StubStateModel>("state").TransitionsOn(null));
        }

        [Fact]
        public void State_TransitionsOn_TriggerMatchingOneTransitionNotAnother_ReturnsOnlyMatchingONe()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);

            state1.AddTransition(transition1);

            var result = state1.TransitionsOn(new Trigger("t1"));
            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
            Assert.Same(transition1, result.FirstOrDefault());
        }

        [Fact]
        public void State_TransitionsOn_TriggerMatchingThree_ReturnsThreeInOrderAdded()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition3 = new Transition<StubStateModel>(trigger1, state1, state1, m => 1 == 0);
            var transition2 = new Transition<StubStateModel>(trigger1, state1, state3, m => 1 == 1);
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);

            state1.AddTransition(transition1);
            state1.AddTransition(transition2);
            state1.AddTransition(transition3);

            var result = state1.TransitionsOn(new Trigger("t1"));
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Same(transition1, result.ToList()[0]);
            Assert.Same(transition2, result.ToList()[1]);
            Assert.Same(transition3, result.ToList()[2]);
        }

        [Fact]
        public void State_TransitionsOn_TriggerMatchingTwoTransitionsNotAnother_ReturnsTwoMatching()
        {
            var state1 = new State<StubStateModel>("s1");
            var state2 = new State<StubStateModel>("s2");
            var state3 = new State<StubStateModel>("s2");
            var trigger1 = new Trigger("t1");
            var transition1 = new Transition<StubStateModel>(trigger1, state1, state2, m => 1 == 2);
            var transition2 = new Transition<StubStateModel>(trigger1, state1, state3, m => 1 == 1);

            state1.AddTransition(transition1);
            state1.AddTransition(transition2);

            var result = state1.TransitionsOn(new Trigger("t1"));
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Same(transition1, result.ToList()[0]);
            Assert.Same(transition2, result.ToList()[1]);
        }
    }
}