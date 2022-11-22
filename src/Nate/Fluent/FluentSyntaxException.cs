using System;

namespace Nate.Fluent
{
    public class FluentSyntaxException : ArgumentException
    {
        public FluentSyntaxException()
        {
        }

        public FluentSyntaxException(string message)
            : base(message)
        {
        }

        public FluentSyntaxException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}