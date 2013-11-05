using System;
using System.Collections.Concurrent;

namespace Naive.EventSourcing.EventStore
{
    public class Lock
    {
        private static ConcurrentDictionary<Guid, object> _locks = new ConcurrentDictionary<Guid, object>();

        public static object For(Guid aggregateId)
        {
            var aggregateLock = _locks.GetOrAdd(aggregateId, new object());

            return aggregateLock;
        }
    }
}
