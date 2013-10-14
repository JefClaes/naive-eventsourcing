using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.EventStore
{
    public interface IEventStore
    {
        void CreateOrAppend(Guid aggregateId, EventStream eventStream);

        EventStream GetStream(Guid aggregateId);
    }
}
