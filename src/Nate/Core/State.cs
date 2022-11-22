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
using System.Collections.Generic;
using System.Linq;

namespace Nate.Core
{
    /// <summary>
    ///     Represents a single state for a given StateModel type
    /// </summary>
    /// <typeparam name="TStateModel"></typeparam>
    public class State<TStateModel> where TStateModel : IStateModel
    {
        private readonly object _transitionLock = new object();
        private readonly Dictionary<Trigger, List<Transition<TStateModel>>> _transitions;

        public State(string name)
            : this(name, null)
        {
        }

        public State(string name, int? code)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            Code = code;
            _transitions = new Dictionary<Trigger, List<Transition<TStateModel>>>();
        }

        public string Name { get; }
        public int? Code { get; set; }

        public virtual IEnumerable<Transition<TStateModel>> AvailableTransitions
        {
            get
            {
                lock (_transitionLock)
                {
                    return _transitions.Values.SelectMany(t => t);
                }
            }
        }

        public event EventHandler<TransitionEventArgs<TStateModel>> Entering;
        public event EventHandler<TransitionEventArgs<TStateModel>> Entered;
        public event EventHandler<TransitionEventArgs<TStateModel>> Exiting;
        public event EventHandler<TransitionEventArgs<TStateModel>> Exited;

        public virtual void AddTransition(Transition<TStateModel> transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));
            if (!transition.Source.Equals(this))
                // only transitions can be added to this state that have the same source as this state
                throw new InvalidTransitionException(
                    $"The source state ('{transition.Source}') of added transition does not match the state ('{this}') to which the transition is being added");
            lock (_transitionLock)
            {
                if (!_transitions.ContainsKey(transition.Trigger))
                    _transitions[transition.Trigger] = new List<Transition<TStateModel>>();
                if (_transitions[transition.Trigger].Contains(transition))
                    // don't allow effectively duplicate transitions
                    throw new InvalidTransitionException(
                        $"State '{this}' already has a defined transition from source '{transition.Source}' to target '{transition.Target}' on trigger '{transition.Trigger}'");
                _transitions[transition.Trigger].Add(transition);
            }
        }

        public virtual void RaiseEntered(TStateModel stateModel, State<TStateModel> from, Trigger trigger)
        {
            if (stateModel == null) throw new ArgumentNullException(nameof(stateModel));
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));

            Entered?.Invoke(this, new TransitionEventArgs<TStateModel>(stateModel, from, this, trigger));
        }

        public virtual void RaiseExiting(TStateModel stateModel, State<TStateModel> to, Trigger trigger)
        {
            if (stateModel == null) throw new ArgumentNullException(nameof(stateModel));
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));

            Exiting?.Invoke(this, new TransitionEventArgs<TStateModel>(stateModel, this, to, trigger));
        }

        public virtual void RaiseEntering(TStateModel stateModel, State<TStateModel> from, Trigger trigger)
        {
            if (stateModel == null) throw new ArgumentNullException(nameof(stateModel));
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));

            Entering?.Invoke(this, new TransitionEventArgs<TStateModel>(stateModel, from, this, trigger));
        }

        public virtual void RaiseExited(TStateModel stateModel, State<TStateModel> to, Trigger trigger)
        {
            if (stateModel == null) throw new ArgumentNullException(nameof(stateModel));
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));

            Exited?.Invoke(this, new TransitionEventArgs<TStateModel>(stateModel, this, to, trigger));
        }

        public virtual IEnumerable<Transition<TStateModel>> TransitionsOn(Trigger trigger)
        {
            if (trigger == null) throw new ArgumentNullException(nameof(trigger));

            lock (_transitionLock)
            {
                if (!_transitions.ContainsKey(trigger))
                    return new List<Transition<TStateModel>>();
                return _transitions[trigger];
            }
        }

        #region object comparison

        public override int GetHashCode()
        {
            return Code.HasValue ? Code.Value : Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is State<TStateModel> other)
            {
                if (other.Code.HasValue && Code.HasValue)
                    return Code.Value == other.Code.Value &&
                           Name.Equals(other.Name);
                return Name.Equals(other.Name);
            }

            return GetHashCode() == obj?.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}