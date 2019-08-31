using System.Collections.Generic;
using Moq;
using Nate.Core;
using Nate.Fluent;

namespace Nate.Tests.Unit.Fluent
{
    public class FluentTestBase
    {
        protected Mock<IFluentStateMachineBuilder<StubStateModel>> MockBuilder;

        public FluentTestBase()
        {
            MockBuilder = new Mock<IFluentStateMachineBuilder<StubStateModel>>();
        }

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

        protected IFluentStateMachineBuilder<StubStateModel> Builder => MockBuilder.Object;
    }
}