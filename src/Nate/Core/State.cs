using System;
using System.Collections.Generic;
using System.Linq;

namespace Nate.Core
{
    public class State<TStateModel> where TStateModel : IStateModel
    {
        private Dictionary<Trigger, List<Transition<TStateModel>>> transitions;
        private static readonly object transitionLock = new object();

        public string Name { get; private set; }
        public int? Code { get; set; }
        public event EventHandler<TransitionEventArgs<TStateModel>> Entering;
        public event EventHandler<TransitionEventArgs<TStateModel>> Entered;
        public event EventHandler<TransitionEventArgs<TStateModel>> Exiting;
        public event EventHandler<TransitionEventArgs<TStateModel>> Exited;

        public State(string name)
            : this(name, null)
        { }

        public State(string name, int? code)
        {
            if (String.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

            Name = name;
            Code = code;
            transitions = new Dictionary<Trigger, List<Transition<TStateModel>>>();
        }

        public virtual void AddTransition(Transition<TStateModel> transition)
        {
            if (transition == null) { throw new ArgumentNullException("transition"); }
            if (!transition.Source.Equals(this))
            {
                // only transitions can be added to this state that have the same source as this state
                throw new InvalidTransitionException(
                    String.Format("The source state ('{0}') of added transition does not match the state ('{1}') to which the transition is being added",
                        transition.Source, this));
            }
            lock (transitionLock)
            {
                if (!transitions.ContainsKey(transition.Trigger))
                {
                    transitions[transition.Trigger] = new List<Transition<TStateModel>>();
                }
                if (transitions[transition.Trigger].Contains(transition))
                {
                    // don't allow effectively duplicate transitions
                    throw new InvalidTransitionException(
                        String.Format("State '{0}' already has a defined transition from source '{1}' to target '{2}' on trigger '{3}'",
                            this, transition.Source, transition.Target, transition.Trigger));
                }
                transitions[transition.Trigger].Add(transition);
            }
        }

        public virtual void RaiseEntered(TStateModel stateModel, State<TStateModel> from, Trigger trigger)
        {
            if (stateModel == null) { throw new ArgumentNullException("stateModel"); }
            if (from == null) { throw new ArgumentNullException("from"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }

            if (Entered != null)
            {
                Entered(this, new TransitionEventArgs<TStateModel>(stateModel, from, this, trigger));
            }
        }

        public virtual void RaiseExiting(TStateModel stateModel, State<TStateModel> to, Trigger trigger)
        {
            if (stateModel == null) { throw new ArgumentNullException("stateModel"); }
            if (to == null) { throw new ArgumentNullException("to"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }

            if (Exiting != null)
            {
                Exiting(this, new TransitionEventArgs<TStateModel>(stateModel, this, to, trigger));
            }
        }

        public virtual void RaiseEntering(TStateModel stateModel, State<TStateModel> from, Trigger trigger)
        {
            if (stateModel == null) { throw new ArgumentNullException("stateModel"); }
            if (from == null) { throw new ArgumentNullException("from"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }

            if (Entering != null)
            {
                Entering(this, new TransitionEventArgs<TStateModel>(stateModel, from, this, trigger));
            }
        }

        public virtual void RaiseExited(TStateModel stateModel, State<TStateModel> to, Trigger trigger)
        {
            if (stateModel == null) { throw new ArgumentNullException("stateModel"); }
            if (to == null) { throw new ArgumentNullException("to"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }

            if (Exited != null)
            {
                Exited(this, new TransitionEventArgs<TStateModel>(stateModel, this, to, trigger));
            }
        }
        
        public virtual IEnumerable<Transition<TStateModel>> TransitionsOn(Trigger trigger)
        {
            if (trigger == null) { throw new ArgumentNullException("trigger"); }

            lock (transitionLock)
            {
                if (!transitions.ContainsKey(trigger))
                {
                    return new List<Transition<TStateModel>>();
                }
                else
                {
                    return transitions[trigger];
                }
            }
        }

        public virtual IEnumerable<Transition<TStateModel>> AvailableTransitions
        {
            get
            {
                return transitions.Values.SelectMany(t => t);
            }
        }

        #region object comparison
        public override int GetHashCode()
        {
            return Code.HasValue ? Code.Value : Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is State<TStateModel>)
            {
                var other = (State<TStateModel>)obj;
                if (other.Code.HasValue && this.Code.HasValue)
                {
                    return this.Code.Value == other.Code.Value &&
                        this.Name.Equals(other.Name);
                }
                else
                {
                    return this.Name.Equals(other.Name);
                }
            }
            else
                return this.GetHashCode() == obj.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
