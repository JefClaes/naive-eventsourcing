using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class EventStream : IEnumerable<IEvent>
    {
        private readonly IEnumerable<IEvent> _events;

        public EventStream(IEnumerable<IEvent> events)
        {
            if (events == null)
                throw new ArgumentException("events");

            _events = events;
        }

        public IEnumerator<IEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _events.GetEnumerator();
        }
    }
}
