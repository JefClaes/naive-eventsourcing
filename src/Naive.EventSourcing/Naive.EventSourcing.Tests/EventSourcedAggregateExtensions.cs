using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Naive.EventSourcing.Tests
{
    public static class EventSourcedAggregateExtensions 
    {
        public static void Raised(this IEventSourcedAggregate ar, IEvent @event) 
        {
            Assert.IsTrue(ar.RecordedEvents().Contains(@event), string.Format("Aggregate didn't raise {0}.", @event));
        }

        public static void RaisedNothing(this IEventSourcedAggregate ar)
        {
            Assert.IsTrue(!ar.RecordedEvents().Any(), string.Format("Aggregate raised {0} events, expected zero.", ar.RecordedEvents().Count());
        }
    }
}
