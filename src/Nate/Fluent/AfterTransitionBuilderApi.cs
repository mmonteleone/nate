using System;
using Nate.Core;

namespace Nate.Fluent
{
    public class AfterTransitionBuilderApi<TStateModel> where TStateModel : IStateModel
    {
        private readonly IFluentStateMachineBuilder<TStateModel> builder;

        private AfterTransitionBuilderApi()
        {
        }

        public AfterTransitionBuilderApi(IFluentStateMachineBuilder<TStateModel> stateMachineBuilder)
        {
            builder = stateMachineBuilder;
        }

        public StateFluentBuilderApi<TStateModel> State(string stateName)
        {
            if (string.IsNullOrEmpty(stateName)) throw new ArgumentNullException("stateName");

            return State(stateName, null);
        }

        public StateFluentBuilderApi<TStateModel> State(string stateName, int? stateCode)
        {
            if (string.IsNullOrEmpty(stateName)) throw new ArgumentNullException("stateName");

            builder.State(stateName, stateCode);
            return new StateFluentBuilderApi<TStateModel>(builder);
        }

        public IFluentStateMachine<TStateModel> Compile()
        {
            return builder.Compile();
        }

        public IFluentStateMachine<TStateModel> Compile(StateMachineConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            return builder.Compile(configuration);
        }

        public BeforeTransitionBuilderApi<TStateModel> BeforeTransition(
            Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            builder.BeforeTransition(callback);
            return new BeforeTransitionBuilderApi<TStateModel>(builder);
        }

        public GloballyTransitionsToBuilderApi<TStateModel> GloballyTransitionsTo(string stateName)
        {
            if (string.IsNullOrEmpty(stateName)) throw new ArgumentNullException("stateName");

            builder.GloballyTransitionsTo(stateName);
            return new GloballyTransitionsToBuilderApi<TStateModel>(builder);
        }
    }
}