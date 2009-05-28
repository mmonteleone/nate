using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nate.Core
{
    public class InvalidStateModelException : ArgumentException
    {
        public InvalidStateModelException()
            : base()
        { }

        public InvalidStateModelException(string message)
            : base(message)
        { }

        public InvalidStateModelException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
