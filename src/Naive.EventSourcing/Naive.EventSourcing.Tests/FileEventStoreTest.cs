using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Naive.EventSourcing.EventStore;
using System.Threading.Tasks;

namespace Naive.EventSourcing.Tests
{
    public class FileEventStoreTests
    {
        public static IEventStore CreateFileEventStore()
        {
            return new FileEventStore(typeof(FileEventStoreTests).Assembly);
        }
    }

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
            GivenAggregateId();
            GivenSomeEventsAppended();
            WhenGettingTheEventStream();
        }

        public void GivenAggregateId()
        {
            _aggregateId = Guid.NewGuid();
        }

        public void GivenNewEventStore()
        {
            _eventStore = FileEventStoreTests.CreateFileEventStore();
        }

        public void GivenSomeEventsAppended()
        {
            _eventStore.Create(_aggregateId, new EventStream(new List<TestEvent>() { new TestEvent(), new TestEvent() }));
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
            GivenAggregateId();
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
            _eventStore = FileEventStoreTests.CreateFileEventStore();
        }

        public void GivenSomeEventsAppended()
        {
            _eventStore.Create(_aggregateId, new EventStream(new List<TestEvent>() { new TestEvent(), new TestEvent() }));
        }

        public void GivenSomeMoreEventsAppended()
        {
            var expectedVersion = _eventStore.GetStream(_aggregateId).Version;
            _eventStore.Append(_aggregateId, new EventStream(new List<TestEvent>() { new TestEvent(), new TestEvent() }), expectedVersion);
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

    [TestClass]
    public class WhenAppendingToStreamWithConcurrentChanges
    {
        private IEventStore _eventStore;
        private Guid _aggregateId;

        private ConcurrencyException _expectedConcurrencyException;

        [TestInitialize]
        public void Initialize()
        {
            try
            {
                GivenEventStore();
                GivenAggregateId();
                GivenSomeEventsAppended();
                WhenAppendingTwoEventStreamsOutOfOrder();
            }
            catch (ConcurrencyException cex) 
            {
                _expectedConcurrencyException = cex;
            }
        }

        public void GivenEventStore()
        {
            _eventStore = FileEventStoreTests.CreateFileEventStore();
        }

        public void GivenAggregateId()
        {
            _aggregateId = Guid.NewGuid();
        }

        public void GivenSomeEventsAppended()
        {
            _eventStore.Create(_aggregateId, new EventStream(new List<ConcurrencyTestEvent>() { new ConcurrencyTestEvent(), new ConcurrencyTestEvent() }));
        }

        public void WhenAppendingTwoEventStreamsOutOfOrder()
        {
            var expectedVersion = _eventStore.GetStream(_aggregateId).Version;

            _eventStore.Append(_aggregateId, new EventStream(new List<ConcurrencyTestEvent>() { new ConcurrencyTestEvent(), new ConcurrencyTestEvent() }), expectedVersion);
            _eventStore.Append(_aggregateId, new EventStream(new List<ConcurrencyTestEvent>() { new ConcurrencyTestEvent(), new ConcurrencyTestEvent() }), expectedVersion);
        }

        [TestMethod]
        public void AConcurrencyExceptionIsThrown()
        {
            Assert.IsNotNull(_expectedConcurrencyException);
        }

        [TestMethod]
        public void TheConcurrencyExceptionHasANiceMessage()
        {
            Assert.AreEqual("Version found: 3, expected: 1", _expectedConcurrencyException.Message);
        }

        private class ConcurrencyTestEvent : IEvent { }
    }  
}
