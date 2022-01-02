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
    ///     Represents a single definition of a trigger causing a transition from one state to
    ///     another, possibly given the optional evaluation of a logical guard lambda.
    /// </summary>
    /// <typeparam name="TStateModel"></typeparam>
    public class Transition<TStateModel> where TStateModel : IStateModel
    {
        private static readonly Func<TStateModel, bool> trueGuard = model => true;

        public Transition(Trigger trigger, State<TStateModel> source, State<TStateModel> target) :
            this(trigger, source, target, trueGuard)
        {
        }

        public Transition(Trigger trigger, State<TStateModel> source, State<TStateModel> target,
            Func<TStateModel, bool> guard)
        {
            Source = source;
            Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Guard = guard ?? throw new ArgumentNullException(nameof(guard));
        }

        public State<TStateModel> Source { get; }
        public State<TStateModel> Target { get; }
        public Trigger Trigger { get; }
        public Func<TStateModel, bool> Guard { get; }

        #region object comparison

        public override bool Equals(object obj)
        {
            if (obj is Transition<TStateModel>)
            {
                var other = obj as Transition<TStateModel>;
                // if both using no (default) guard, only compare states and trigger
                if (other.Guard == trueGuard && Guard == trueGuard)
                    return Trigger.Equals(other.Trigger)
                           && (Source == null && other.Source == null || Source.Equals(other.Source))
                           && Target.Equals(other.Target);
                // if one or more using custom guard, compare guard too
                return Trigger.Equals(other.Trigger)
                       && (Source == null && other.Source == null || Source.Equals(other.Source))
                       && Target.Equals(other.Target)
                       && Guard.Equals(other.Guard);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}