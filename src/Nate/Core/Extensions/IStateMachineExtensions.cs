using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nate.Core
{
    public static class IStateMachineExtensions
    {
        public static void AddGlobalTransition<TStateModel>(this IStateMachine<TStateModel> stateMachine, 
            Trigger trigger,
            State<TStateModel> to) where TStateModel : IStateModel
        {
            if (stateMachine == null) { throw new ArgumentNullException("stateMachine"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }
            if (to == null) { throw new ArgumentNullException("to"); }

            stateMachine.AddGlobalTransition(new Transition<TStateModel>(trigger, null, to));
        }

        public static void AddGlobalTransition<TStateModel>(this IStateMachine<TStateModel> stateMachine, 
            Trigger trigger,
            State<TStateModel> to, 
            Func<TStateModel, bool> guard) where TStateModel : IStateModel
        {
            if (stateMachine == null) { throw new ArgumentNullException("stateMachine"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }
            if (to == null) { throw new ArgumentNullException("to"); }
            if (guard == null) { throw new ArgumentNullException("guard"); }

            stateMachine.AddGlobalTransition(new Transition<TStateModel>(trigger, null, to, guard));
        }

        public static void Trigger<TStateModel>(this IStateMachine<TStateModel> stateMachine, 
            string triggerName,
            TStateModel model) where TStateModel : IStateModel
        {
            if (stateMachine == null) { throw new ArgumentNullException("stateMachine"); }
            if (String.IsNullOrEmpty(triggerName)) { throw new ArgumentNullException("triggerName"); }
            if (model == null) { throw new ArgumentNullException("model"); }

            stateMachine.Trigger(new Trigger(triggerName), model);
        }
    }
}
