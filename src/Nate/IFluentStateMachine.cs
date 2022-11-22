using System.Collections.Generic;
using Nate.Core;

namespace Nate
{
    public interface IFluentStateMachine<TStateModel> where TStateModel : IStateModel
    {
        State<TStateModel> InitialState { get; }

        StateMachineConfiguration Configuration { get; }
        void Trigger(string triggerName, TStateModel model);

        IEnumerable<State<TStateModel>> AvailableStates(TStateModel model);

        State<TStateModel> StateCoded(int code);
        State<TStateModel> StateNamed(string name);
    }
}