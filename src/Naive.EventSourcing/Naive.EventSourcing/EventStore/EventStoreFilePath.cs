using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naive.EventSourcing.EventStore
{
    public class EventStoreFilePath
    {
        private EventStoreFilePath(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            Value = value;
        }

        public string Value { get; private set; }

        public static EventStoreFilePath From(string dir, Guid aggregateId)
        {
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentNullException("dir");

            return new EventStoreFilePath(Path.Combine(dir, string.Concat(aggregateId, ".txt")));
        }
    }
}
