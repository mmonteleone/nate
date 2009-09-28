using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nate.Fluent
{
    public class FluentSyntaxException : ArgumentException
    {
        public FluentSyntaxException()
            : base()
        { }

        public FluentSyntaxException(string message)
            : base(message)
        { }

        public FluentSyntaxException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
