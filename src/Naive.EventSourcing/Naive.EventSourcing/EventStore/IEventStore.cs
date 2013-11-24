using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.EventStore
{
    public interface IEventStore
    {       
        void Create(Guid aggregateId, EventStream eventStream);

        void Append(Guid aggregateId, EventStream eventStream, int expectedVersion);

        ReadEventStream GetStream(Guid aggregateId);
    }
}
