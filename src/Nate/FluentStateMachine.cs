using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;
using Nate.Fluent;

namespace Nate
{
    public class FluentStateMachine<TStateModel> : IFluentStateMachine<TStateModel> where TStateModel : IStateModel
    {
        private IStateMachine<TStateModel> stateMachine;
        private Dictionary<string, State<TStateModel>> allStatesByName;
        private Dictionary<int, State<TStateModel>> allStatesByCode;

        public StateMachineConfiguration Configuration
        {
            get { return stateMachine.Configuration; }
        }

        public State<TStateModel> InitialState { get; private set; }

        public State<TStateModel> StateNamed(string name)
        {
            if (String.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

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

        public FluentStateMachine(
            IStateMachine<TStateModel> stateMachine,
            IEnumerable<State<TStateModel>> states, 
            State<TStateModel> initialState,
            IEnumerable<Action<TransitionEventArgs<TStateModel>>> globalTransitionings,
            IEnumerable<Action<TransitionEventArgs<TStateModel>>> globalTransitioneds,
            IEnumerable<Transition<TStateModel>> globalTransitions)
        {
            if (stateMachine == null) { throw new ArgumentNullException("stateMachine"); }
            if (states == null) { throw new ArgumentNullException("states"); }

            this.stateMachine = stateMachine;

            if (globalTransitionings != null)
            {
                foreach (var callback in globalTransitionings)
                {
                    stateMachine.Transitioning += (s, e) => callback(e);
                }
            }
            if (globalTransitioneds != null)
            {
                foreach (var callback in globalTransitioneds)
                {
                    stateMachine.Transitioned += (s, e) => callback(e);
                }
            }
            if (globalTransitions != null)
            {
                foreach (var transition in globalTransitions)
                {
                    stateMachine.AddGlobalTransition(transition);
                }
            }
            this.allStatesByName = states.ToDictionary(s => s.Name);
            this.allStatesByCode = states.Where(s => s.Code.HasValue).ToDictionary(s => s.Code.Value);
            this.InitialState = initialState;
        }

        public static InitialFluentBuilderApi<TStateModel> Describe()
        {
            var builder = new FluentStateMachineBuilder<TStateModel>();
            return new InitialFluentBuilderApi<TStateModel>(builder);
        }

        public void Trigger(string triggerName, TStateModel model)
        {
            if (String.IsNullOrEmpty(triggerName)) { throw new ArgumentNullException("triggerName"); }
            if (model == null) { throw new ArgumentNullException("model"); }

            stateMachine.Trigger(new Trigger(triggerName), model);
        }

        public IEnumerable<State<TStateModel>> AvailableStates(TStateModel model)
        {
            if (model == null) { throw new ArgumentNullException("model"); }

            return stateMachine.AvailableStates(model);
        }
    }
}
