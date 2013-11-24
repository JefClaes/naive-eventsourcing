using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naive.EventSourcing.EventStore
{
    public class EventStoreFilePaths
    {
        private EventStoreFilePaths(string databaseFile, string journalFile)
        {
            if (string.IsNullOrEmpty(databaseFile))
                throw new ArgumentNullException("databaseFile");
            if (string.IsNullOrEmpty(journalFile))
                throw new ArgumentNullException("journalFile");

            DatabaseFile = databaseFile;
            JournalFile = journalFile;
        }

        public string DatabaseFile { get; private set; }

        public string JournalFile { get; private set; }

        public static EventStoreFilePaths From(string dir, Guid aggregateId)
        {
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentNullException("dir");

            var databaseFile = Path.Combine(dir, string.Concat(aggregateId, ".txt"));
            var journalFile = Path.Combine(dir, string.Concat(aggregateId, ".journal.txt"));

            return new EventStoreFilePaths(databaseFile, journalFile);
        }
    }
}
