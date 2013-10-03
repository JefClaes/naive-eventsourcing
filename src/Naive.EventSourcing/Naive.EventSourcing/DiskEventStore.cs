using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Naive.EventSourcing
{
    public class DiskEventStore : IEventStore
    {
        private string _path;

        public DiskEventStore(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            _path = path;
        }

        public EventStream GetStream(Guid aggregateId)
        {           
            var lines = File.ReadAllLines(_path);

            var events = lines
                .Select(x => RecordOnDisk.Deserialize(x))
                .Where(x => x.AggregateId == aggregateId)
                .Select(x => x.Event)
                .ToList();

            if (events.Any())
                return new EventStream(events);

            return null;
        }

        public void Append(Guid aggregateId, EventStream eventStream)
        {
            using (var stream = new FileStream(_path, FileMode.Append, FileAccess.Write))
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    foreach (var @event in eventStream)
                        streamWriter.WriteLine(new RecordOnDisk(aggregateId, @event).Serialized());
                }
            }
        }               
    }
}
