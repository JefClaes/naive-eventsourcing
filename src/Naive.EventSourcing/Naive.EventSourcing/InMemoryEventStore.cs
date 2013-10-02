using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Naive.EventSourcing
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly ConcurrentDictionary<Guid, List<IEvent>> _state = new ConcurrentDictionary<Guid, List<IEvent>>();

        public IEnumerable<IEvent> GetStream(Guid aggregateId)
        {
            return _state[aggregateId];
        }

        public void AppendToStream(Guid aggregateId, IEnumerable<IEvent> events)
        {
            if (_state.ContainsKey(aggregateId))
                _state[aggregateId].AddRange(events);
            else
                _state.TryAdd(aggregateId, events.ToList());
        }       
    }
}
