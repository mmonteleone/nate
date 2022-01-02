using System;
using Nate.Core;

namespace Nate.Fluent
{
    public class AfterTransitionBuilderApi<TStateModel> where TStateModel : IStateModel
    {
        private readonly IFluentStateMachineBuilder<TStateModel> _builder;

        private AfterTransitionBuilderApi()
        {
        }

        public AfterTransitionBuilderApi(IFluentStateMachineBuilder<TStateModel> stateMachineBuilder)
        {
            _builder = stateMachineBuilder;
        }

        public StateFluentBuilderApi<TStateModel> State(string stateName)
        {
            if (string.IsNullOrEmpty(stateName)) throw new ArgumentNullException(nameof(stateName));

            return State(stateName, null);
        }

        public StateFluentBuilderApi<TStateModel> State(string stateName, int? stateCode)
        {
            if (string.IsNullOrEmpty(stateName)) throw new ArgumentNullException(nameof(stateName));

            _builder.State(stateName, stateCode);
            return new StateFluentBuilderApi<TStateModel>(_builder);
        }

        public IFluentStateMachine<TStateModel> Compile()
        {
            return _builder.Compile();
        }

        public IFluentStateMachine<TStateModel> Compile(StateMachineConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return _builder.Compile(configuration);
        }

        public BeforeTransitionBuilderApi<TStateModel> BeforeTransition(
            Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _builder.BeforeTransition(callback);
            return new BeforeTransitionBuilderApi<TStateModel>(_builder);
        }

        public GloballyTransitionsToBuilderApi<TStateModel> GloballyTransitionsTo(string stateName)
        {
            if (string.IsNullOrEmpty(stateName)) throw new ArgumentNullException(nameof(stateName));

            _builder.GloballyTransitionsTo(stateName);
            return new GloballyTransitionsToBuilderApi<TStateModel>(_builder);
        }
    }
}