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
using System.Collections.Generic;

namespace Nate.Core.Extensions
{
    /// <summary>
    ///     Extensions to StateModels for simpler API usage
    /// </summary>
    public static class IStateModelExtensions
    {
        /// <summary>
        ///     Returns the available states from the current one on a model, given a state machine
        /// </summary>
        /// <typeparam name="TStateModel"></typeparam>
        /// <param name="model">model containing current state</param>
        /// <param name="stateMachine">state machine to process with</param>
        /// <returns></returns>
        public static IEnumerable<State<TStateModel>> AvailableStates<TStateModel>(this IStateModel model,
            IStateMachine<TStateModel> stateMachine) where TStateModel : IStateModel
        {
            if (model == null) throw new ArgumentNullException("model");
            if (stateMachine == null) throw new ArgumentNullException("stateMachine");

            return stateMachine.AvailableStates((TStateModel)model);
        }

        /// <summary>
        ///     Returns the available triggers available given the current state of a model, and a state machine
        /// </summary>
        /// <typeparam name="TStateModel"></typeparam>
        /// <param name="model">model containing current state</param>
        /// <param name="stateMachine">state machine with which to process</param>
        /// <returns></returns>
        public static IEnumerable<Trigger> AvailableTriggers<TStateModel>(this IStateModel model,
            IStateMachine<TStateModel> stateMachine) where TStateModel : IStateModel
        {
            if (model == null) throw new ArgumentNullException("model");
            if (stateMachine == null) throw new ArgumentNullException("stateMachine");

            return stateMachine.AvailableTriggers((TStateModel)model);
        }

        /// <summary>
        ///     Triggers a transition trigger on a model given triggername and machine
        /// </summary>
        /// <typeparam name="TStateModel"></typeparam>
        /// <param name="model">model containing current state</param>
        /// <param name="triggerName">name of trigger</param>
        /// <param name="stateMachine">state machine to perform the transition</param>
        public static void Trigger<TStateModel>(this IStateModel model,
            string triggerName,
            IStateMachine<TStateModel> stateMachine) where TStateModel : IStateModel
        {
            if (model == null) throw new ArgumentNullException("model");
            if (string.IsNullOrEmpty(triggerName)) throw new ArgumentNullException("triggerName");
            if (stateMachine == null) throw new ArgumentNullException("stateMachine");

            stateMachine.Trigger(new Trigger(triggerName), (TStateModel)model);
        }

        /// <summary>
        ///     Triggers a transition trigger on a model given trigger instance and machine
        /// </summary>
        /// <typeparam name="TStateModel"></typeparam>
        /// <param name="model">model containing current state</param>
        /// <param name="trigger">trigger instance</param>
        /// <param name="stateMachine">state machine to perform the transition</param>
        public static void Trigger<TStateModel>(this IStateModel model,
            Trigger trigger,
            IStateMachine<TStateModel> stateMachine) where TStateModel : IStateModel
        {
            if (model == null) throw new ArgumentNullException("model");
            if (trigger == null) throw new ArgumentNullException("trigger");
            if (stateMachine == null) throw new ArgumentNullException("stateMachine");

            stateMachine.Trigger(trigger, (TStateModel)model);
        }
    }
}