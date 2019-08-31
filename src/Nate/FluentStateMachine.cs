using System;
using System.Collections.Generic;
using System.Linq;
using Nate.Core;
using Nate.Fluent;

namespace Nate
{
    public class FluentStateMachine<TStateModel> : IFluentStateMachine<TStateModel> where TStateModel : IStateModel
    {
        private readonly Dictionary<int, State<TStateModel>> allStatesByCode;
        private readonly Dictionary<string, State<TStateModel>> allStatesByName;
        private readonly IStateMachine<TStateModel> stateMachine;

        public FluentStateMachine(
            IStateMachine<TStateModel> stateMachine,
            IEnumerable<State<TStateModel>> states,
            State<TStateModel> initialState,
            IEnumerable<Action<TransitionEventArgs<TStateModel>>> globalTransitionings,
            IEnumerable<Action<TransitionEventArgs<TStateModel>>> globalTransitioneds,
            IEnumerable<Transition<TStateModel>> globalTransitions)
        {
            if (stateMachine == null) throw new ArgumentNullException("stateMachine");
            if (states == null) throw new ArgumentNullException("states");

            this.stateMachine = stateMachine;

            if (globalTransitionings != null)
                foreach (var callback in globalTransitionings)
                    stateMachine.Transitioning += (s, e) => callback(e);
            if (globalTransitioneds != null)
                foreach (var callback in globalTransitioneds)
                    stateMachine.Transitioned += (s, e) => callback(e);
            if (globalTransitions != null)
                foreach (var transition in globalTransitions)
                    stateMachine.AddGlobalTransition(transition);
            allStatesByName = states.ToDictionary(s => s.Name);
            allStatesByCode = states.Where(s => s.Code.HasValue).ToDictionary(s => s.Code.Value);
            InitialState = initialState;
        }

        public StateMachineConfiguration Configuration => stateMachine.Configuration;

        public State<TStateModel> InitialState { get; }

        public State<TStateModel> StateNamed(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            State<TStateModel> state = null;
            allStatesByName.TryGetValue(name, out state);
            return state;
        }

        public State<TStateModel> StateCoded(int code)
        {
            State<TStateModel> state = null;
            allStatesByCode.TryGetValue(code, out state);
            return state;
        }

        public void Trigger(string triggerName, TStateModel model)
        {
            if (string.IsNullOrEmpty(triggerName)) throw new ArgumentNullException("triggerName");
            if (model == null) throw new ArgumentNullException("model");

            stateMachine.Trigger(new Trigger(triggerName), model);
        }

        public IEnumerable<State<TStateModel>> AvailableStates(TStateModel model)
        {
            if (model == null) throw new ArgumentNullException("model");

            return stateMachine.AvailableStates(model);
        }

        public static InitialFluentBuilderApi<TStateModel> Describe()
        {
            var builder = new FluentStateMachineBuilder<TStateModel>();
            return new InitialFluentBuilderApi<TStateModel>(builder);
        }
    }
}