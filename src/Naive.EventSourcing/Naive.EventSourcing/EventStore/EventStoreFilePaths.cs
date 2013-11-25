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

        private EventStoreFilePaths(DatabaseFilePath databaseFile, JournalFilePath journalFile)
        {
            if (databaseFile == null)
                throw new ArgumentNullException("databaseFile");
            if (journalFile == null)
                throw new ArgumentNullException("journalFile");

            DatabaseFile = databaseFile;
            JournalFile = journalFile;
        }

        public DatabaseFilePath DatabaseFile { get; private set; }

        public JournalFilePath JournalFile { get; private set; }

        public static EventStoreFilePaths From(Guid aggregateId)
        {
            var databaseFile = new DatabaseFilePath(Root, aggregateId);
            var journalFile = new JournalFilePath(Root, aggregateId);

            return new EventStoreFilePaths(databaseFile, journalFile);
        }
    }

    public class DatabaseFilePath
    {
        private readonly string _value;        
        
        public DatabaseFilePath(string root, Guid aggregateId)
        {
            _value = Path.Combine(root, aggregateId.ToString() + ".txt");
        }

        public string Value
        {
            get { return _value; }
        }

        public string ToRestorePath()
        {
            return Value + ".restore";
        }

        public string ToCompletedRestorePath()
        {
            return Value + ".restore_complete";
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
        private readonly string _root;

        public const string Suffix = ".journal.txt";

        public JournalFilePath(string root, Guid aggregateId)
        {
            _value = Path.Combine(root, aggregateId.ToString() + Suffix);
            _root = root;
        }

        public string Value
        {
            get { return _value; }
        }

        public static string SearchPattern
        {
            get { return "*." + Suffix; }
        }

        public DatabaseFilePath ToDatabaseFilePath()
        {
            var aggregateId = _value.Substring(_value.LastIndexOf(@"/"), _value.LastIndexOf(Suffix));

            return new DatabaseFilePath(_root, Guid.Parse(aggregateId));
        }

        public static JournalFilePath Parse(string value)
        {
            if (!value.EndsWith(Suffix))
                throw new ArgumentOutOfRangeException("value");

            var aggregateId = Guid.Parse(value.Substring(0, value.IndexOf(Suffix)));

            return new JournalFilePath(EventStoreFilePaths.Root, aggregateId);
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
