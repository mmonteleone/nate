using System;

namespace Nate.Fluent
{
    public class GloballyTransitionsToBuilderApi<TStateModel> where TStateModel : IStateModel
    {
        private readonly IFluentStateMachineBuilder<TStateModel> builder;

        private GloballyTransitionsToBuilderApi()
        {
        }

        public GloballyTransitionsToBuilderApi(IFluentStateMachineBuilder<TStateModel> stateMachineBuilder)
        {
            builder = stateMachineBuilder;
        }

        public OnFluentBuilderApi<TStateModel> On(string triggerName)
        {
            if (string.IsNullOrEmpty(triggerName)) throw new ArgumentNullException("triggerName");

            builder.On(triggerName);
            return new OnFluentBuilderApi<TStateModel>(builder);
        }
    }
}