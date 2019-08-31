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
using Xunit;

namespace Nate.Tests.Unit.Core
{
    public class TransitionTests
    {
        [Fact]
        public void Transition_AllowsNullSource()
        {
            var t = new Transition<StubStateModel>(
                new Trigger("trigger"),
                null,
                new Mock<State<StubStateModel>>("to").Object);
        }

        [Fact]
        public void Transition_Equals_CustomGuardDiff_SameProps_ReturnsFalse()
        {
            var t1 = new Transition<StubStateModel>(new Trigger("t1"), new State<StubStateModel>("s1"),
                new State<StubStateModel>("s2"), m => false);
            var t2 = new Transition<StubStateModel>(new Trigger("t1"), new State<StubStateModel>("s1"),
                new State<StubStateModel>("s2"), m => true);
            Assert.False(t1.Equals(t2));
        }

        [Fact]
        public void Transition_Equals_CustomGuardSame_SameProps_ReturnsTrue()
        {
            Func<StubStateModel, bool> guard = m => true;
            var t1 = new Transition<StubStateModel>(new Trigger("t1"), new State<StubStateModel>("s1"),
                new State<StubStateModel>("s2"), guard);
            var t2 = new Transition<StubStateModel>(new Trigger("t1"), new State<StubStateModel>("s1"),
                new State<StubStateModel>("s2"), guard);
            Assert.True(t1.Equals(t2));
        }

        [Fact]
        public void Transition_Equals_NoGuardOnEither_DiffProps_ReturnsFalse()
        {
            var t1 = new Transition<StubStateModel>(new Trigger("t1"), new State<StubStateModel>("s1"),
                new State<StubStateModel>("s1"));
            var t2 = new Transition<StubStateModel>(new Trigger("t3"), new State<StubStateModel>("s2"),
                new State<StubStateModel>("s2"));
            Assert.False(t1.Equals(t2));
        }

        [Fact]
        public void Transition_Equals_NoGuardOnEither_SameProps_ReturnsTrue()
        {
            var t1 = new Transition<StubStateModel>(new Trigger("t1"), new State<StubStateModel>("s1"),
                new State<StubStateModel>("s2"));
            var t2 = new Transition<StubStateModel>(new Trigger("t1"), new State<StubStateModel>("s1"),
                new State<StubStateModel>("s2"));
            Assert.True(t1.Equals(t2));
        }

        [Fact]
        public void Transition_NullGuard_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Transition<StubStateModel>(
                    new Trigger("trigger"),
                    new Mock<State<StubStateModel>>("from").Object,
                    new Mock<State<StubStateModel>>("to").Object,
                    null));
        }

        [Fact]
        public void Transition_NullTarget_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Transition<StubStateModel>(
                    new Trigger("trigger"),
                    new Mock<State<StubStateModel>>("from").Object,
                    null));
        }

        [Fact]
        public void Transition_NullTarget_WithGuard_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Transition<StubStateModel>(
                    new Trigger("trigger"),
                    new Mock<State<StubStateModel>>("from").Object,
                    null,
                    s => true));
        }

        [Fact]
        public void Transition_NullTrigger_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Transition<StubStateModel>(
                    null,
                    new Mock<State<StubStateModel>>("from").Object,
                    new Mock<State<StubStateModel>>("to").Object));
        }


        [Fact]
        public void Transition_NullTrigger_WithGuard_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Transition<StubStateModel>(
                    null,
                    new Mock<State<StubStateModel>>("from").Object,
                    new Mock<State<StubStateModel>>("to").Object,
                    s => true));
        }

        [Fact]
        public void Transition_ValidParms_AppliesAllParamToProps()
        {
            var trigger = new Trigger("trigger");
            var from = new State<StubStateModel>("from");
            var to = new State<StubStateModel>("to");

            var transition = new Transition<StubStateModel>(trigger, from, to);

            Assert.Same(trigger, transition.Trigger);
            Assert.Same(from, transition.Source);
            Assert.Same(to, transition.Target);
        }

        [Fact]
        public void Transition_ValidParms_GuardReturnsTrue()
        {
            var trigger = new Trigger("trigger");
            var from = new State<StubStateModel>("from");
            var to = new State<StubStateModel>("to");

            var transition = new Transition<StubStateModel>(trigger, from, to);

            Assert.True(transition.Guard(new StubStateModel()));
        }

        [Fact]
        public void Transition_ValidParms_WithGuard_AppliesAllParamToProps()
        {
            var trigger = new Trigger("trigger");
            var from = new State<StubStateModel>("from");
            var to = new State<StubStateModel>("to");
            Func<StubStateModel, bool> guard = s => true;

            var transition = new Transition<StubStateModel>(trigger, from, to, guard);

            Assert.Same(trigger, transition.Trigger);
            Assert.Same(from, transition.Source);
            Assert.Same(to, transition.Target);
            Assert.Same(guard, transition.Guard);
        }

        [Fact]
        public void Transition_WithGuard_AllowsNullSource()
        {
            var t = new Transition<StubStateModel>(
                new Trigger("trigger"),
                null,
                new Mock<State<StubStateModel>>("to").Object,
                s => true);
        }
    }
}