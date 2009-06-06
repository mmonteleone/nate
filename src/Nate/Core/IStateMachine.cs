using System;
using System.Collections.Generic;

namespace Nate.Core
{
    public interface IStateMachine<TStateModel> where TStateModel : IStateModel
    {
        void Trigger(Trigger trigger, TStateModel model);
        StateMachineConfiguration Configuration { get; }

        IEnumerable<State<TStateModel>> AvailableStates(TStateModel model);
        IEnumerable<Trigger> AvailableTriggers(TStateModel model);

        event EventHandler<TransitionEventArgs<TStateModel>> Transitioned;
        event EventHandler<TransitionEventArgs<TStateModel>> Transitioning;

        void AddGlobalTransition(Transition<TStateModel> transition);
    }
}
