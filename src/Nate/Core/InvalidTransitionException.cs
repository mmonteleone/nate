using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nate.Core
{
    public class InvalidTransitionException : ArgumentException
    {
        public InvalidTransitionException()
            : base()
        { }

        public InvalidTransitionException(string message)
            : base(message)
        { }

        public InvalidTransitionException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
