using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;

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

                EnsureJournalFileEmpty(paths.JournalFile);
                WriteCurrentVersionToJournalFile(paths.JournalFile, currentVersion);
                WriteEventStreamToFile(eventStream, aggregateId, paths.DatabaseFile.Value, currentVersion);
                TruncateJournalFile(paths.JournalFile);
            }
        }

        public ReadEventStream GetStream(Guid aggregateId)
        {
            lock (Lock.For(aggregateId))
            {
                var paths = EventStoreFilePaths.From(aggregateId);

                if (!File.Exists(paths.DatabaseFile.Value))
                    return null;

                var lines = File.ReadAllLines(paths.DatabaseFile.Value);

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

        private int GetCurrentVersion(DatabaseFilePath path)
        {
            var currentVersion = -1;

            var lines = File.ReadLines(path.Value);
            
            if (lines.Any())
                currentVersion = lines
                    .Select(x => Record.Deserialize(x, _assembly))
                    .Max(x => x.Version);
            
            return currentVersion;
        }

        private void WriteEventStreamToFile(EventStream eventStream, Guid aggregateId, string path, int currentVersion)
        {
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

        private void WriteCurrentVersionToJournalFile(JournalFilePath path, int currentVersion)
        {         
            using (var stream = new FileStream(path.Value, FileMode.Append, FileSystemRights.AppendData, FileShare.None, 4096, FileOptions.WriteThrough))
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.WriteLine(currentVersion.ToString("000000000"));
                }
            }
        }

        private void EnsureRootDirectoryExists()
        {
            if (!Directory.Exists(EventStoreFilePaths.Root))
                Directory.CreateDirectory(EventStoreFilePaths.Root);
        }

        private void TruncateJournalFile(JournalFilePath path)
        {
            using (var fs = File.Open(path.Value, FileMode.Truncate)) { };
        }

        private void EnsurePathsExist(EventStoreFilePaths paths)
        {
            if (!File.Exists(paths.DatabaseFile.Value))
                using (var fs = File.Create(paths.DatabaseFile.Value)) { };
            if (!File.Exists(paths.JournalFile.Value))
                using (var fs = File.Create(paths.JournalFile.Value)) { };
        }

        private void EnsureJournalFileEmpty(JournalFilePath path)
        {
            var fi = new FileInfo(path.Value);

            if (fi.Length > 0)
                throw new CorruptionException("Journal file not empty, restore the eventstore first.");
        }
    }
}
