﻿#region license

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

namespace Nate.Core.Extensions
{
    /// <summary>
    ///     Extensions to States for simpler API usage
    /// </summary>
    public static class StateExtensions
    {
        /// <summary>
        ///     Shortcut for adding a transition to a state without first constructing a Transition object
        /// </summary>
        /// <typeparam name="TStateModel"></typeparam>
        /// <param name="state">state to hold transition</param>
        /// <param name="trigger">transition trigger</param>
        /// <param name="to">transition target</param>
        public static void AddTransition<TStateModel>(this State<TStateModel> state,
            Trigger trigger,
            State<TStateModel> to) where TStateModel : IStateModel
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));
            if (to == null) throw new ArgumentNullException(nameof(to));

            state.AddTransition(
                new Transition<TStateModel>(trigger, state, to));
        }

        /// <summary>
        ///     Shortcut for adding a transition to a state without first constructing a Transition object
        /// </summary>
        /// <typeparam name="TStateModel"></typeparam>
        /// <param name="state">state to hold transition</param>
        /// <param name="trigger">transition trigger</param>
        /// <param name="to">transition target</param>
        /// <param name="guard">lambda guard method which must evaluate to true for transition to occur</param>
        public static void AddTransition<TStateModel>(this State<TStateModel> state,
            Trigger trigger,
            State<TStateModel> to,
            Func<TStateModel, bool> guard) where TStateModel : IStateModel
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (guard == null) throw new ArgumentNullException(nameof(guard));

            state.AddTransition(
                new Transition<TStateModel>(trigger, state, to, guard));
        }
    }
}