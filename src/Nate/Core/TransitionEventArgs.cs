using System;

namespace Nate.Core
{
    public class TransitionEventArgs<TStateModel> : EventArgs where TStateModel : IStateModel
    {
        public TStateModel Model { get; private set; }
        public State<TStateModel> From { get; private set; }
        public State<TStateModel> To { get; private set; }
        public Trigger Trigger { get; private set; }

        public TransitionEventArgs(TStateModel model, State<TStateModel> from, State<TStateModel> to, Trigger trigger)
        {
            if (model == null) { throw new ArgumentNullException("model"); }
            if (from == null) { throw new ArgumentNullException("from"); }
            if (to == null) { throw new ArgumentNullException("to"); }
            if (trigger == null) { throw new ArgumentNullException("trigger"); }

            Model = model;
            From = from;
            To = to;
            Trigger = trigger;
        }
    }
}
