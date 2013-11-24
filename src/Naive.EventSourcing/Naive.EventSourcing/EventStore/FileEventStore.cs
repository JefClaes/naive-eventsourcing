using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Naive.EventSourcing.EventStore
{
    public class FileEventStore : IEventStore
    {     
        private Assembly _assembly;

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
            var paths = EventStoreFilePaths.From(aggregateId);

            EnsureRootDirectoryExists();

            lock (Lock.For(aggregateId))
            {
                EnsurePathsExist(paths);

                var currentVersion = GetCurrentVersion(paths.DatabaseFile);

                if (currentVersion != expectedVersion)
                    throw new OptimisticConcurrencyException(currentVersion, expectedVersion);

                using (var stream = new FileStream(paths.DatabaseFile, FileMode.Append, FileAccess.Write, FileShare.Read))
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
            lock (Lock.For(aggregateId))
            {
                var paths = EventStoreFilePaths.From(aggregateId);

                if (!File.Exists(paths.DatabaseFile))
                    return null;

                var lines = File.ReadAllLines(paths.DatabaseFile);

                if (lines.Any())
                {
                    var records = lines.Select(x => Record.Deserialize(x, _assembly));
                    var currentVersion = records.Max(x => x.Version);
                    var events = records.Select(x => x.Event).ToList();

                    return new ReadEventStream(events, currentVersion);
                }

                return null;
            }
        }

        private int GetCurrentVersion(string path)
        {
            var currentVersion = -1;

            var lines = File.ReadLines(path);
            
            if (lines.Any())
                currentVersion = lines
                    .Select(x => Record.Deserialize(x, _assembly))
                    .Max(x => x.Version);
            
            return currentVersion;
        }

        private void EnsureRootDirectoryExists()
        {
            if (!Directory.Exists(EventStoreFilePaths.Root))
                Directory.CreateDirectory(EventStoreFilePaths.Root);
        }

        private void EnsurePathsExist(EventStoreFilePaths paths)
        {
            if (!File.Exists(paths.DatabaseFile))
                using (var fs = File.Create(paths.DatabaseFile)) { };
            if (!File.Exists(paths.JournalFile))
                using (var fs = File.Create(paths.JournalFile)) { };
        }
    }
}
