using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Naive.EventSourcing.EventStore
{
    public class FileEventStore : IEventStore
    {
        private const string Dir = @"C:\EventStore";
        private Assembly _assembly;

        private static ConcurrentDictionary<Guid, object> _locks = new ConcurrentDictionary<Guid, object>();

        public FileEventStore()
        {
            _assembly = GetType().Assembly;
        }

        public FileEventStore(Assembly assembly)
        {
            _assembly = assembly;
        }

        public void Create(Guid aggregateId, EventStream eventStream)
        {
            CreateOrAppend(aggregateId, eventStream, expectedVersion: -1);
        }

        public void Append(Guid aggregateId, EventStream eventStream, int expectedVersion)
        {
            CreateOrAppend(aggregateId, eventStream, expectedVersion);
        }

        private void CreateOrAppend(Guid aggregateId, EventStream eventStream, int expectedVersion)
        {
            var path = EventStoreFilePath.From(Dir, aggregateId).Value;

            EnsureRootDirectoryExists();
            EnsurePathExists(path);

            var aggregateLock = _locks.GetOrAdd(aggregateId, new object());            
            
            lock (aggregateLock) 
            {
                var currentVersion = -1;
                var lastLine = File.ReadLines(path).LastOrDefault();
                if (!string.IsNullOrEmpty(lastLine))
                    currentVersion = Record.Deserialize(lastLine, _assembly).Version;

                if (currentVersion != expectedVersion)
                    throw new ConcurrencyException(string.Format("Version found: {0}, expected: {1}", currentVersion, expectedVersion));

                using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        foreach (var @event in eventStream)
                        {
                            currentVersion++;

                            streamWriter.WriteLine(new Record(aggregateId, @event, currentVersion).Serialized());
                        }
                    }
                }
            }          
        }

        public ReadEventStream GetStream(Guid aggregateId)
        {
            var aggregateLock = _locks.GetOrAdd(aggregateId, new object());

            lock (aggregateLock)
            {
                var path = EventStoreFilePath.From(Dir, aggregateId).Value;

                if (!File.Exists(path))
                    return null;

                var lines = File.ReadAllLines(path);

                if (lines.Any())
                {
                    var records = lines.Select(x => Record.Deserialize(x, _assembly));
                    var maxVersion = records.Max(x => x.Version);
                    var events = records.Select(x => x.Event).ToList();

                    return new ReadEventStream(events, maxVersion);
                }

                return null;
            }
        }

        private void EnsureRootDirectoryExists()
        {
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);
        }

        private void EnsurePathExists(string path)
        {
            if (!File.Exists(path))
                using (var fs = File.Create(path)) { };
        }
    }
}
