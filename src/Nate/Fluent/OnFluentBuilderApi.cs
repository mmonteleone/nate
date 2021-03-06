﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;

namespace Nate.Fluent
{
    public class OnFluentBuilderApi<TStateModel> where TStateModel : IStateModel
    {
        private IFluentStateMachineBuilder<TStateModel> builder;

        private OnFluentBuilderApi()
        { }

        public OnFluentBuilderApi(IFluentStateMachineBuilder<TStateModel> stateMachineBuilder)
        {
            this.builder = stateMachineBuilder;
        }

        public WhenFluentBuilderApi<TStateModel> When(Func<TStateModel, bool> guard)
        {
            if (guard == null) { throw new ArgumentNullException("guard"); }

            builder.When(guard);
            return new WhenFluentBuilderApi<TStateModel>(builder);
        }

        public AfterEntryFluentBuilderApi<TStateModel> AfterEntry(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            builder.AfterEntry(callback);
            return new AfterEntryFluentBuilderApi<TStateModel>(builder);
        }

        public BeforeExitFluentBuilderApi<TStateModel> BeforeExit(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            builder.BeforeExit(callback);
            return new BeforeExitFluentBuilderApi<TStateModel>(builder);
        }

        public TransitionsToFluentBuilderApi<TStateModel> TransitionsTo(string stateName)
        {
            if (String.IsNullOrEmpty(stateName)) { throw new ArgumentNullException("stateName"); }

            builder.TransitionsTo(stateName);
            return new TransitionsToFluentBuilderApi<TStateModel>(builder);
        }

        public InitiatesFluentBuilderApi<TStateModel> Initiates()
        {
            builder.Initiates();
            return new InitiatesFluentBuilderApi<TStateModel>(builder);
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

        public IFluentStateMachine<TStateModel> Compile()
        {
            return builder.Compile();
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

        public BeforeEntryFluentBuilderApi<TStateModel> BeforeEntry(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            builder.BeforeEntry(callback);
            return new BeforeEntryFluentBuilderApi<TStateModel>(builder);
        }

        public AfterExitFluentBuilderApi<TStateModel> AfterExit(Action<TransitionEventArgs<TStateModel>> callback)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }

            builder.AfterExit(callback);
            return new AfterExitFluentBuilderApi<TStateModel>(builder);
        }
    }
}
