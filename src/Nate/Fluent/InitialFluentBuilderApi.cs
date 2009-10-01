using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;

namespace Nate.Fluent
{
    public class InitialFluentBuilderApi<TStateModel> where TStateModel : IStateModel
    {
        private IFluentStateMachineBuilder<TStateModel> builder;

        private InitialFluentBuilderApi()
        { }

        public InitialFluentBuilderApi(IFluentStateMachineBuilder<TStateModel> stateMachineBuilder)
        {
            this.builder = stateMachineBuilder;
        }

        public StateFluentBuilderApi<TStateModel> State(string stateName)
        {
            if (String.IsNullOrEmpty(stateName)) { throw new ArgumentNullException("stateName"); }

            return this.State(stateName, null);
        }

        public StateFluentBuilderApi<TStateModel> State(string stateName, int? stateCode)
        {
            if (String.IsNullOrEmpty(stateName)) { throw new ArgumentNullException("stateName"); }

            builder.State(stateName, stateCode);
            return new StateFluentBuilderApi<TStateModel>(builder);
        }

        public BeforeTransitionBuilderApi<TStateModel> BeforeTransition(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            builder.BeforeTransition(callback);
            return new BeforeTransitionBuilderApi<TStateModel>(builder);
        }

        public AfterTransitionBuilderApi<TStateModel> AfterTransition(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }         

            builder.AfterTransition(callback);
            return new AfterTransitionBuilderApi<TStateModel>(builder);
        }

        public GloballyTransitionsToBuilderApi<TStateModel> GloballyTransitionsTo(string stateName)
        {
            if (String.IsNullOrEmpty(stateName)) { throw new ArgumentNullException("stateName"); }

            builder.GloballyTransitionsTo(stateName);
            return new GloballyTransitionsToBuilderApi<TStateModel>(builder);
        }
    }
}
