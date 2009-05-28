using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nate.Core
{
    public class InvalidTriggerException : ArgumentException
    {
        public InvalidTriggerException()
            : base()
        { }

        public InvalidTriggerException(string message)
            : base(message)
        { }

        public InvalidTriggerException(string message, Exception inner)
            : base(message, inner)
        { }

        internal IList<Trigger> AvailableTriggers { get; set; }
    }
}
