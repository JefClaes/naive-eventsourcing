using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Naive.EventSourcing.Tests
{
    [TestClass]
    public class WhenGettingStreamAfterAppending
    {
        private IEventStore _eventStore;
        private Guid _aggregateId;

        private IEnumerable<IEvent> _sut;

        [TestInitialize]
        public void Initialize()
        {
            GivenNewEventStore();
            GivenSomeEventsAppended();
            WhenGettingTheEventStream();
        }

        public void GivenAggregateId()
        {
            _aggregateId = Guid.NewGuid();
        }

        public void GivenNewEventStore()
        {
            _eventStore = new InMemoryEventStore();
        }

        public void GivenSomeEventsAppended()
        {
            _eventStore.AppendToStream(_aggregateId, new List<TestEvent>() { new TestEvent(), new TestEvent() });
        }

        public void WhenGettingTheEventStream()
        {
            _sut = _eventStore.GetStream(_aggregateId);
        }

        [TestMethod]
        public void TwoEventsAreReturned()
        {
            Assert.AreEqual(2, _sut.Count());
        }

        private class TestEvent : IEvent { }
    }
}
