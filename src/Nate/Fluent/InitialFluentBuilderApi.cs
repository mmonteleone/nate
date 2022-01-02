using System;
using Nate.Core;

namespace Nate.Fluent
{
    public class InitialFluentBuilderApi<TStateModel> where TStateModel : IStateModel
    {
        private readonly IFluentStateMachineBuilder<TStateModel> _builder;

        public InitialFluentBuilderApi(IFluentStateMachineBuilder<TStateModel> stateMachineBuilder)
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

        public BeforeTransitionBuilderApi<TStateModel> BeforeTransition(
            Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _builder.BeforeTransition(callback);
            return new BeforeTransitionBuilderApi<TStateModel>(_builder);
        }

        public AfterTransitionBuilderApi<TStateModel> AfterTransition(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _builder.AfterTransition(callback);
            return new AfterTransitionBuilderApi<TStateModel>(_builder);
        }

        public GloballyTransitionsToBuilderApi<TStateModel> GloballyTransitionsTo(string stateName)
        {
            if (string.IsNullOrEmpty(stateName)) throw new ArgumentNullException(nameof(stateName));

            _builder.GloballyTransitionsTo(stateName);
            return new GloballyTransitionsToBuilderApi<TStateModel>(_builder);
        }
    }
}