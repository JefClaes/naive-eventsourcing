using System;
using System.IO;
using System.Linq;

namespace Naive.EventSourcing.EventStore
{
    public class FileEventStore : IEventStore
    {    
        private const string Dir = @"C:\EventStore";        

        public EventStream GetStream(Guid aggregateId)
        {           
            var path = EventStoreFilePath.From(Dir, aggregateId).Value;

            if (!File.Exists(path))
                return null;

            var lines = File.ReadAllLines(path);

            var events = lines
                .Select(x => Record.Deserialize(x))
                .Select(x => x.Event)
                .ToList();

            if (events.Any())
                return new EventStream(events);

            return null;
        }

        public void Append(Guid aggregateId, EventStream eventStream)
        {
            EnsureDirectoryExists();

            var path = EventStoreFilePath.From(Dir, aggregateId).Value;

            using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    foreach (var @event in eventStream)
                        streamWriter.WriteLine(new Record(aggregateId, @event).Serialized());
                }
            }
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);
        }
    }
}
