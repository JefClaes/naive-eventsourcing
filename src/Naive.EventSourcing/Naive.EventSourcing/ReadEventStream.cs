using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class ReadEventStream : EventStream
    {
        public ReadEventStream(IEnumerable<IEvent> events, int version) : base(events) 
        {
            Version = version;
        }

        public int Version { get; private set; }
    }
}
