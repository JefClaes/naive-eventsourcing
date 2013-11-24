﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Naive.EventSourcing.EventStore;
using System.Threading.Tasks;
using System.IO;

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
    public class WhenGettingStreamAfterAppendingForTheFirstTimeSpecs
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
    public class WhenGettingStreamAfterAppendingForTheSecondTimeSpecs
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
    public class WhenAppendingToStreamWithConcurrentChangesSpecs
    {
        private IEventStore _eventStore;
        private Guid _aggregateId;

        private OptimisticConcurrencyException _expectedConcurrencyException;

        [TestInitialize]
        public void Initialize()
        {
            try
            {
                GivenEventStore();
                GivenAggregateId();
                GivenEventStreamCreated();
                WhenAppendingTwoEventStreamsWithTheSameExpectedVersion();
            }
            catch (OptimisticConcurrencyException ocex) 
            {
                _expectedConcurrencyException = ocex;
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

        public void GivenEventStreamCreated()
        {
            _eventStore.Create(_aggregateId, new EventStream(new List<ConcurrencyTestEvent>() { new ConcurrencyTestEvent(), new ConcurrencyTestEvent() }));
        }

        public void WhenAppendingTwoEventStreamsWithTheSameExpectedVersion()
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

    [TestClass]
    public class WhenCreatingStreamSpecs
    {
        private IEventStore _eventStore;
        private Guid _aggregateId;

        [TestInitialize]
        public void Initialize()
        {
            GivenEventStore();
            GivenAggregateId();
            WhenCreatingStream();
        }

        public void GivenEventStore()
        {
            _eventStore = FileEventStoreTests.CreateFileEventStore();
        }

        public void GivenAggregateId()
        {
            _aggregateId = Guid.NewGuid();
        }

        public void WhenCreatingStream()
        {
            _eventStore.Create(_aggregateId, new EventStream(new List<ConcurrencyTestEvent>() { new ConcurrencyTestEvent(), new ConcurrencyTestEvent() }));
        }

        [TestMethod]
        public void TheDatabaseFileIsCreated()
        {
            File.Exists(EventStoreFilePaths.From(_aggregateId).DatabaseFile.Value);
        }

        [TestMethod]
        public void TheJournalFileIsCreated()
        {
            File.Exists(EventStoreFilePaths.From(_aggregateId).JournalFile.Value);
        }

        [TestMethod]
        public void TheJournalFileIsTruncatedAfterwards()
        {
            var fi = new FileInfo(EventStoreFilePaths.From(_aggregateId).JournalFile.Value);
            Assert.AreEqual(0, fi.Length);
        }

        private class ConcurrencyTestEvent : IEvent { }
    }

    [TestClass]
    public class WhenAppendingWithNonEmptyJournalFileSpecs 
    {
        private IEventStore _eventStore;
        private Guid _aggregateId;

        private CorruptionException _expectedException;

        [TestInitialize]
        public void Initialize()
        {
            try
            {
                GivenEventStore();
                GivenAggregateId();
                GivenEventStreamCreated();
                GivenNonEmptyJournalFile();
                WhenAppendingEventStream();
            }
            catch (CorruptionException corrEx)
            {
                _expectedException = corrEx;
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

        public void GivenEventStreamCreated()
        {
            _eventStore.Create(_aggregateId, new EventStream(new List<TestEvent>() { new TestEvent(), new TestEvent() }));
        }

        public void GivenNonEmptyJournalFile()
        {
            using (var writer = File.AppendText(EventStoreFilePaths.From(_aggregateId).JournalFile.Value))
                writer.WriteLine("NOTEMPTY");            
        }

        public void WhenAppendingEventStream() 
        {
            var stream = _eventStore.GetStream(_aggregateId);
            _eventStore.Append(
                _aggregateId, 
                new EventStream(new List<TestEvent>() { new TestEvent(), new TestEvent() }),
                stream.Version);
        }

        [TestMethod]
        public void ACorruptionExceptionIsThrown()
        {
            Assert.IsNotNull(_expectedException);
        }

        private class TestEvent : IEvent { }
    }

    [TestClass]
    public class EventStoreFilePathsSpecs
    {
        [TestMethod]
        public void DatabaseFileNameIsCorrect()
        {
            var paths = EventStoreFilePaths.From(Guid.Parse("B6C4C31B-BD48-4545-83E7-CE5DD4A6C801"));

            Assert.AreEqual(@"C:\EventStore\B6C4C31B-BD48-4545-83E7-CE5DD4A6C801.txt", paths.DatabaseFile.Value, ignoreCase : true);
        } 

        [TestMethod]
        public void JournalFileNameIsCorrect()
        {
             var paths = EventStoreFilePaths.From(Guid.Parse("B6C4C31B-BD48-4545-83E7-CE5DD4A6C801"));

            Assert.AreEqual(@"C:\EventStore\B6C4C31B-BD48-4545-83E7-CE5DD4A6C801.journal.txt", paths.JournalFile.Value, ignoreCase : true);
        }
    }
}
