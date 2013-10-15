using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Naive.EventSourcing.EventStore;

namespace Naive.EventSourcing.Tests
{
    [TestClass]
    public class WhenGettingStreamAfterAppendingForTheFirstTime
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
            _eventStore.CreateOrAppend(_aggregateId, new EventStream(new List<TestEvent>() { new TestEvent(), new TestEvent() }));
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

    [TestClass]
    public class WhenGettingStreamAfterAppendingForTheSecondTime
    {
        private IEventStore _eventStore;
        private Guid _aggregateId;

        private IEnumerable<IEvent> _sut;

        [TestInitialize]
        public void Initialize()
        {
            GivenNewEventStore();
            GivenSomeEventsAppended();
            GivenSomeMoreEventsAppended();
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
            _eventStore.CreateOrAppend(_aggregateId, new EventStream(new List<TestEvent>() { new TestEvent(), new TestEvent() }));
        }

        public void GivenSomeMoreEventsAppended()
        {
            _eventStore.CreateOrAppend(_aggregateId, new EventStream(new List<TestEvent>() { new TestEvent(), new TestEvent() }));
        }

        public void WhenGettingTheEventStream()
        {
            _sut = _eventStore.GetStream(_aggregateId);
        }

        [TestMethod]
        public void FourEventsAreReturned()
        {
            Assert.AreEqual(4, _sut.Count());
        }

        private class TestEvent : IEvent { }
    }

}
