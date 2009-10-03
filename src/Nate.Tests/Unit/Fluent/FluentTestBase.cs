using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Xunit;
using Nate.Fluent;
using Nate.Core;

namespace Nate.Tests.Unit.Fluent
{
    public class FluentTestBase
    {
        public FluentTestBase()
        {
            MockBuilder = new Mock<IFluentStateMachineBuilder<StubStateModel>>();
        }

        protected Mock<IFluentStateMachineBuilder<StubStateModel>> MockBuilder;

        protected FluentStateMachine<StubStateModel> FluentStateMachine
        {
            get
            {
                var states = new List<State<StubStateModel>>();
                states.Add(new State<StubStateModel>("s1"));
                states.Add(new State<StubStateModel>("s2"));
                states.Add(new State<StubStateModel>("s3"));
                return new FluentStateMachine<StubStateModel>(
                    new Mock<IStateMachine<StubStateModel>>().Object,
                    states,
                    new State<StubStateModel>("s1"),
                    null,
                    null,
                    null);
            }
        }

        protected IFluentStateMachineBuilder<StubStateModel> Builder
        {
            get { return MockBuilder.Object; }
        }
    }
}
