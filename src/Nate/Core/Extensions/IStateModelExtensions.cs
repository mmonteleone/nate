using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nate.Core
{
    public static class IStateModelExtensions
    {
        public static IEnumerable<State<TStateModel>> AvailableStates<TStateModel>(this IStateModel model,
            IStateMachine<TStateModel> stateMachine) where TStateModel : IStateModel
        {
            if (model == null) { throw new ArgumentNullException("model"); }
            if (stateMachine == null) { throw new ArgumentNullException("stateMachine"); }

            return stateMachine.AvailableStates((TStateModel)model);
        }

        public static IEnumerable<Trigger> AvailableTriggers<TStateModel>(this IStateModel model,
            IStateMachine<TStateModel> stateMachine) where TStateModel : IStateModel
        {
            if (model == null) { throw new ArgumentNullException("model"); }
            if (stateMachine == null) { throw new ArgumentNullException("stateMachine"); }

            return stateMachine.AvailableTriggers((TStateModel)model);
        }

        public static void Trigger<TStateModel>(this IStateModel model,
            string triggerName,
            IStateMachine<TStateModel> stateMachine) where TStateModel : IStateModel
        {
            if (model == null) { throw new ArgumentNullException("model"); }
            if (String.IsNullOrEmpty(triggerName)) { throw new ArgumentNullException("triggerName"); }
            if (stateMachine == null) { throw new ArgumentNullException("stateMachine"); }
         
            stateMachine.Trigger(new Trigger(triggerName), (TStateModel)model);
        }

        public static void Trigger<TStateModel>(this IStateModel model,
            Trigger trigger,
            IStateMachine<TStateModel> stateMachine) where TStateModel : IStateModel
        {
            if (model == null) { throw new ArgumentNullException("model"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }
            if (stateMachine == null) { throw new ArgumentNullException("stateMachine"); }

            stateMachine.Trigger(trigger, (TStateModel)model);
        }
    }
}
