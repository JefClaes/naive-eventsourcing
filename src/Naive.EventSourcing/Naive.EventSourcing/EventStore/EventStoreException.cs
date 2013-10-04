using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.EventStore
{
    public class EventStoreException : Exception
    {
        public EventStoreException(string message) : base(message) { }
    }
}
