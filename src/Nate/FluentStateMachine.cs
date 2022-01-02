using System;
using System.Collections.Generic;
using System.Linq;
using Nate.Core;
using Nate.Fluent;

namespace Nate
{
    public class FluentStateMachine<TStateModel> : IFluentStateMachine<TStateModel> where TStateModel : IStateModel
    {
        private readonly Dictionary<int, State<TStateModel>> _allStatesByCode;
        private readonly Dictionary<string, State<TStateModel>> _allStatesByName;
        private readonly IStateMachine<TStateModel> _stateMachine;

        public FluentStateMachine(
            IStateMachine<TStateModel> stateMachine,
            IEnumerable<State<TStateModel>> states,
            State<TStateModel> initialState,
            IEnumerable<Action<TransitionEventArgs<TStateModel>>> globalTransitionings,
            IEnumerable<Action<TransitionEventArgs<TStateModel>>> globalTransitioneds,
            IEnumerable<Transition<TStateModel>> globalTransitions)
        {
            if (states == null) throw new ArgumentNullException(nameof(states));

            this._stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

            if (globalTransitionings != null)
                foreach (var callback in globalTransitionings)
                    stateMachine.Transitioning += (s, e) => callback(e);
            if (globalTransitioneds != null)
                foreach (var callback in globalTransitioneds)
                    stateMachine.Transitioned += (s, e) => callback(e);
            if (globalTransitions != null)
                foreach (var transition in globalTransitions)
                    stateMachine.AddGlobalTransition(transition);
            var enumerableStates = states.ToList();
            _allStatesByName = enumerableStates.ToDictionary(s => s.Name);
            _allStatesByCode = enumerableStates.Where(s => s.Code.HasValue).ToDictionary(s => s.Code.Value);
            InitialState = initialState;
        }

        public StateMachineConfiguration Configuration => _stateMachine.Configuration;

        public State<TStateModel> InitialState { get; }

        public State<TStateModel> StateNamed(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            _allStatesByName.TryGetValue(name, out var state);
            return state;
        }

        public State<TStateModel> StateCoded(int code)
        {
            _allStatesByCode.TryGetValue(code, out var state);
            return state;
        }

        public void Trigger(string triggerName, TStateModel model)
        {
            if (string.IsNullOrEmpty(triggerName)) throw new ArgumentNullException(nameof(triggerName));
            if (model == null) throw new ArgumentNullException(nameof(model));

            _stateMachine.Trigger(new Trigger(triggerName), model);
        }

        public IEnumerable<State<TStateModel>> AvailableStates(TStateModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            return _stateMachine.AvailableStates(model);
        }

        public static InitialFluentBuilderApi<TStateModel> Describe()
        {
            var builder = new FluentStateMachineBuilder<TStateModel>();
            return new InitialFluentBuilderApi<TStateModel>(builder);
        }
    }
}