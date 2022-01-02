using System;

namespace Nate.Fluent
{
    public class GloballyTransitionsToBuilderApi<TStateModel> where TStateModel : IStateModel
    {
        private readonly IFluentStateMachineBuilder<TStateModel> _builder;

        public GloballyTransitionsToBuilderApi(IFluentStateMachineBuilder<TStateModel> stateMachineBuilder)
        {
            _builder = stateMachineBuilder;
        }

        public OnFluentBuilderApi<TStateModel> On(string triggerName)
        {
            if (string.IsNullOrEmpty(triggerName)) throw new ArgumentNullException(nameof(triggerName));

            _builder.On(triggerName);
            return new OnFluentBuilderApi<TStateModel>(_builder);
        }
    }
}