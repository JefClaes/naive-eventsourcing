using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public interface IEventSourcedAggregate : IAggregate
    {
        void Initialize(EventStream eventStream);

        EventStream RecordedEvents();
    }

    public interface IAggregate
    {
        Guid Id { get; }
    }
}
