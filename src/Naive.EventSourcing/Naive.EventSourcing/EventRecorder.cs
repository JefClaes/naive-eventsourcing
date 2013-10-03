using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class EventRecorder
    {
        private readonly List<IEvent> _events = new List<IEvent>();

        public void Record(IEvent @event)
        {
            _events.Add(@event);
        }

        public EventStream RecordedEvents()
        {
            return new EventStream(_events);
        }
    }
}
