using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.EventStore
{
    public interface IEventStore
    {
        EventStream GetStream(Guid id);

        void Append(Guid id, EventStream eventStream);
    }
}
