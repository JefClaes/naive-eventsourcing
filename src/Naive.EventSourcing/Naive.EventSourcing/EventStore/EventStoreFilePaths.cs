using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naive.EventSourcing.EventStore
{
    public class EventStoreFilePaths
    {
        public const string Root = @"C:\EventStore";

        private EventStoreFilePaths(string databaseFile, string journalFile)
        {
            if (string.IsNullOrEmpty(databaseFile))
                throw new ArgumentNullException("databaseFile");
            if (string.IsNullOrEmpty(journalFile))
                throw new ArgumentNullException("journalFile");

            DatabaseFile = new DatabaseFilePath(databaseFile);
            JournalFile = new JournalFilePath(journalFile);
        }

        public DatabaseFilePath DatabaseFile { get; private set; }

        public JournalFilePath JournalFile { get; private set; }

        public static EventStoreFilePaths From(Guid aggregateId)
        {
            var databaseFile = Path.Combine(Root, string.Concat(aggregateId, ".txt"));
            var journalFile = Path.Combine(Root, string.Concat(aggregateId, ".journal.txt"));

            return new EventStoreFilePaths(databaseFile, journalFile);
        }
    }

    public class DatabaseFilePath
    {
        private readonly string _value;

        public DatabaseFilePath(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(value);

            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var typedObj = obj as DatabaseFilePath;
            if (typedObj == null)
                return false;

            return this.Value == typedObj.Value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }

    public class JournalFilePath
    {
        private readonly string _value;

        public JournalFilePath(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(value);

            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var typedObj = obj as JournalFilePath;
            if (typedObj == null)
                return false;

            return this.Value == typedObj.Value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}
