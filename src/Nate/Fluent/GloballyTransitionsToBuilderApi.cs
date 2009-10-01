using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nate.Core;

namespace Nate.Fluent
{
    public class GloballyTransitionsToBuilderApi<TStateModel> where TStateModel : IStateModel
    {
        private IFluentStateMachineBuilder<TStateModel> builder;

        private GloballyTransitionsToBuilderApi()
        { }

        public GloballyTransitionsToBuilderApi(IFluentStateMachineBuilder<TStateModel> stateMachineBuilder)
        {
            this.builder = stateMachineBuilder;
        }

        public OnFluentBuilderApi<TStateModel> On(string triggerName)
        {
            if (String.IsNullOrEmpty(triggerName)) { throw new ArgumentNullException("triggerName"); }

            builder.On(triggerName);
            return new OnFluentBuilderApi<TStateModel>(builder);
        }
    }
}
