using System;
using System.Collections.Generic;
using Nate.Core;

namespace Nate
{
    public interface IFluentStateMachine<TStateModel> where TStateModel : IStateModel
    {
        void Trigger(string triggerName, TStateModel model);

        IEnumerable<State<TStateModel>> AvailableStates(TStateModel model);
        State<TStateModel> InitialState { get; }

        State<TStateModel> StateCoded(int code);
        State<TStateModel> StateNamed(string name);

        StateMachineConfiguration Configuration { get; }
    }
}
