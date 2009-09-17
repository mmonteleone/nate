using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nate.Tests
{
    public class StubStateModel : Nate.IStateModel
    {
        public object CurrentState { get; set; }
    }
}
