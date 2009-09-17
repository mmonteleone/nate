using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;
using Xunit;
using Moq;

namespace Nate.Tests.Unit.Core
{
    public class TransitionEventArgsTests
    {
        [Fact]
        public void TransitionEventArgs_NullModel_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TransitionEventArgs<StubStateModel>(
                    null,
                    new State<StubStateModel>("from"),
                    new State<StubStateModel>("to"),
                    new Trigger("trigger")));
        }

        [Fact]
        public void TransitionEventArgs_NullFrom_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TransitionEventArgs<StubStateModel>(
                    new StubStateModel(),
                    null,
                    new State<StubStateModel>("to"),
                    new Trigger("trigger")));
        }

        [Fact]
        public void TransitionEventArgs_NullTo_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TransitionEventArgs<StubStateModel>(
                    new StubStateModel(),
                    new State<StubStateModel>("from"),
                    null,
                    new Trigger("trigger")));
        }

        [Fact]
        public void TransitionEventArgs_NullTrigger_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TransitionEventArgs<StubStateModel>(
                    new StubStateModel(),
                    new State<StubStateModel>("from"),
                    new State<StubStateModel>("to"),
                    null));
        }

        [Fact]
        public void TransitionEventArgs_ValidParms_VerifyAssigns()
        {
            var model = new StubStateModel();
            var from = new State<StubStateModel>("from");
            var to = new State<StubStateModel>("to");
            var trigger = new Trigger("trigger");
            var args = new TransitionEventArgs<StubStateModel>(model, from, to, trigger);

            Assert.Same(from, args.From);
            Assert.Same(to, args.To);
            Assert.Same(model, args.Model);
            Assert.Same(trigger, args.Trigger);
        }
    }
}
