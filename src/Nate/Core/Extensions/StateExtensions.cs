using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nate.Core
{
    public static class StateExtensions
    {
        public static void AddTransition<TStateModel>(this State<TStateModel> state, 
            Trigger trigger,
            State<TStateModel> to) where TStateModel : IStateModel
        {
            if (state == null) { throw new ArgumentNullException("state"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }
            if (to == null) { throw new ArgumentNullException("to"); }

            state.AddTransition(
                new Transition<TStateModel>(trigger, state, to));
        }

        public static void AddTransition<TStateModel>(this State<TStateModel> state, 
            Trigger trigger,
            State<TStateModel> to,
            Func<TStateModel, bool> guard) where TStateModel : IStateModel
        {
            if (state == null) { throw new ArgumentNullException("state"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }
            if (to == null) { throw new ArgumentNullException("to"); }
            if (guard == null) { throw new ArgumentNullException("guard"); }

            state.AddTransition(
                new Transition<TStateModel>(trigger, state, to, guard));
        }
    }
}
