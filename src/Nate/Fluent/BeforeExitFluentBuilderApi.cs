using System;
using Nate.Core;

namespace Nate.Fluent
{
    public class BeforeExitFluentBuilderApi<TStateModel> where TStateModel : IStateModel
    {
        private readonly IFluentStateMachineBuilder<TStateModel> _builder;

        private BeforeExitFluentBuilderApi()
        {
        }

        public BeforeExitFluentBuilderApi(IFluentStateMachineBuilder<TStateModel> stateMachineBuilder)
        {
            _builder = stateMachineBuilder;
        }

        public AfterEntryFluentBuilderApi<TStateModel> AfterEntry(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _builder.AfterEntry(callback);
            return new AfterEntryFluentBuilderApi<TStateModel>(_builder);
        }

        public TransitionsToFluentBuilderApi<TStateModel> TransitionsTo(string stateName)
        {
            if (string.IsNullOrEmpty(stateName)) throw new ArgumentNullException(nameof(stateName));

            _builder.TransitionsTo(stateName);
            return new TransitionsToFluentBuilderApi<TStateModel>(_builder);
        }

        public InitiatesFluentBuilderApi<TStateModel> Initiates()
        {
            _builder.Initiates();
            return new InitiatesFluentBuilderApi<TStateModel>(_builder);
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

        public BeforeEntryFluentBuilderApi<TStateModel> BeforeEntry(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _builder.BeforeEntry(callback);
            return new BeforeEntryFluentBuilderApi<TStateModel>(_builder);
        }

        public AfterExitFluentBuilderApi<TStateModel> AfterExit(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            _builder.AfterExit(callback);
            return new AfterExitFluentBuilderApi<TStateModel>(_builder);
        }
    }
}