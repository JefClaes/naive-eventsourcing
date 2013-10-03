using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Naive.EventSourcing
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly ConcurrentDictionary<Guid, EventStream> _state = new ConcurrentDictionary<Guid, EventStream>();

        public EventStream GetStream(Guid aggregateId)
        {
            return _state[aggregateId];
        }

        public void Append(Guid aggregateId, EventStream eventStream)
        {
            if (_state.ContainsKey(aggregateId))
            {               
                var exisingEventStream = _state[aggregateId];
                var updatedEvents = exisingEventStream.ToList();
                updatedEvents.AddRange(eventStream);
                var updated = _state.TryUpdate(aggregateId, new EventStream(updatedEvents), exisingEventStream);
            }
            else
            {
                var added = _state.TryAdd(aggregateId, eventStream);
            }
        }       
    }
}
