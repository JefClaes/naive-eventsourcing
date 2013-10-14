using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Naive.EventSourcing.EventStore
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly ConcurrentDictionary<Guid, EventStream> _state = new ConcurrentDictionary<Guid, EventStream>();

        public EventStream GetStream(Guid aggregateId)
        {
            return _state[aggregateId];
        }

        public void CreateOrAppend(Guid aggregateId, EventStream eventStream)
        {
            if (_state.ContainsKey(aggregateId))
            {               
                var exisingEventStream = _state[aggregateId];
                var updatedEvents = exisingEventStream.ToList();
                updatedEvents.AddRange(eventStream);

                if (!_state.TryUpdate(aggregateId, new EventStream(updatedEvents), exisingEventStream))
                    throw new EventStoreException("Couldn't append to existing stream");                
            }
            else
            {
                if (!_state.TryAdd(aggregateId, eventStream))
                    throw new EventStoreException("Couldn't create new stream");
            }
        }       
    }
}
