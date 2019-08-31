#region license

/* Nate
 * http://github.com/mmonteleone/nate
 * 
 * Copyright (C) 2009 Michael Monteleone (http://michaelmonteleone.net)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 */

#endregion

using System;

namespace Nate.Core
{
    /// <summary>
    ///     Extension methods for easy api use on StateMachines
    /// </summary>
    public static class IStateMachineExtensions
    {
        /// <summary>
        ///     Adds a global transition to a state machine by trigger and state
        /// </summary>
        /// <typeparam name="TStateModel"></typeparam>
        /// <param name="stateMachine">state machine to add global transition</param>
        /// <param name="trigger">trigger to raise transition</param>
        /// <param name="to">target state to transition to</param>
        public static void AddGlobalTransition<TStateModel>(this IStateMachine<TStateModel> stateMachine,
            Trigger trigger,
            State<TStateModel> to) where TStateModel : IStateModel
        {
            if (stateMachine == null) throw new ArgumentNullException("stateMachine");
            if (trigger == null) throw new ArgumentNullException("trigger");
            if (to == null) throw new ArgumentNullException("to");

            stateMachine.AddGlobalTransition(new Transition<TStateModel>(trigger, null, to));
        }

        /// <summary>
        ///     Adds a global transition to a state machine by trigger, state, and guard lambda
        /// </summary>
        /// <typeparam name="TStateModel"></typeparam>
        /// <param name="stateMachine">state machine to add global transition</param>
        /// <param name="trigger">trigger to raise transition</param>
        /// <param name="to">target state to transition to</param>
        /// <param name="guard">guard lambda to test evaluate before transitioning</param>
        public static void AddGlobalTransition<TStateModel>(this IStateMachine<TStateModel> stateMachine,
            Trigger trigger,
            State<TStateModel> to,
            Func<TStateModel, bool> guard) where TStateModel : IStateModel
        {
            if (stateMachine == null) throw new ArgumentNullException("stateMachine");
            if (trigger == null) throw new ArgumentNullException("trigger");
            if (to == null) throw new ArgumentNullException("to");
            if (guard == null) throw new ArgumentNullException("guard");

            stateMachine.AddGlobalTransition(new Transition<TStateModel>(trigger, null, to, guard));
        }

        /// <summary>
        ///     Triggers a trigger on a statemachine by the trigger's name, instead of trigger object instance
        /// </summary>
        /// <typeparam name="TStateModel"></typeparam>
        /// <param name="stateMachine">state machine to use for trigger</param>
        /// <param name="triggerName">name of trigger</param>
        /// <param name="model">model on which to raise trigger</param>
        public static void Trigger<TStateModel>(this IStateMachine<TStateModel> stateMachine,
            string triggerName,
            TStateModel model) where TStateModel : IStateModel
        {
            if (stateMachine == null) throw new ArgumentNullException("stateMachine");
            if (string.IsNullOrEmpty(triggerName)) throw new ArgumentNullException("triggerName");
            if (model == null) throw new ArgumentNullException("model");

            stateMachine.Trigger(new Trigger(triggerName), model);
        }
    }
}