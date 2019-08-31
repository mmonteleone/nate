using System;
using Nate.Core;

namespace Nate.Fluent
{
    public interface IFluentStateMachineBuilder<TStateModel> where TStateModel : IStateModel
    {
        IFluentStateMachine<TStateModel> Compile();
        IFluentStateMachine<TStateModel> Compile(StateMachineConfiguration configuration);
        void GloballyTransitionsTo(string state);
        void Initiates();
        void On(string name);
        void BeforeEntry(Action<TransitionEventArgs<TStateModel>> callback);
        void AfterEntry(Action<TransitionEventArgs<TStateModel>> callback);
        void BeforeExit(Action<TransitionEventArgs<TStateModel>> callback);
        void AfterExit(Action<TransitionEventArgs<TStateModel>> callback);
        void AfterTransition(Action<TransitionEventArgs<TStateModel>> callback);
        void BeforeTransition(Action<TransitionEventArgs<TStateModel>> callback);
        void State(string name, int? code);
        void TransitionsTo(string name);
        void When(Func<TStateModel, bool> guard);
    }
}