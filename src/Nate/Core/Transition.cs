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
    public class Transition<TStateModel> where TStateModel : Nate.IStateModel
    {
        public State<TStateModel> Source { get; private set; }
        public State<TStateModel> Target { get; private set; }
        public Trigger Trigger { get; private set; }
        public Func<TStateModel, bool> Guard { get; private set; }
        private static Func<TStateModel, bool> trueGuard = model => true;

        public Transition(Trigger trigger, State<TStateModel> source, State<TStateModel> target) :
            this(trigger, source, target, trueGuard)
        { }

        public Transition(Trigger trigger, State<TStateModel> source, State<TStateModel> target, Func<TStateModel, bool> guard)
        {
            if (trigger == null) { throw new ArgumentNullException("trigger"); }
            if (target == null) { throw new ArgumentNullException("target"); }
            if (guard == null) { throw new ArgumentNullException("guard"); }

            Source = source;
            Trigger = trigger;
            Target = target;
            Guard = guard;
        }

        #region object comparison
        public override bool Equals(object obj)
        {
            if (obj is Transition<TStateModel>)
            {
                var other = obj as Transition<TStateModel>;
                // if both using no (default) guard, only compare states and trigger
                if (other.Guard == trueGuard && this.Guard == trueGuard)
                {
                    return this.Trigger.Equals(other.Trigger)
                        && ((this.Source == null && other.Source == null) || this.Source.Equals(other.Source))
                        && this.Target.Equals(other.Target);
                }
                // if one or more using custom guard, compare guard too
                else
                {
                    return this.Trigger.Equals(other.Trigger)
                        && ((this.Source == null && other.Source == null) || this.Source.Equals(other.Source))
                        && this.Target.Equals(other.Target)
                        && this.Guard.Equals(other.Guard);
                }
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
