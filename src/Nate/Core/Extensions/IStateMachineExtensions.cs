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
