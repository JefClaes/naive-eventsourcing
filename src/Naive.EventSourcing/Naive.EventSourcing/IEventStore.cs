using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public interface IEventStore
    {
        IEnumerable<IEvent> GetStream(Guid id);

        void AppendToStream(Guid id, IEnumerable<IEvent> events);
    }
}
