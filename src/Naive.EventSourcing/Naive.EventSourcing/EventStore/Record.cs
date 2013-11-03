using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Naive.EventSourcing.EventStore
{
    public class Record
    {
        private const string Seperator = "+++";

        public Record(Guid aggregateId, IEvent @event, int version)
        {
            if (@event == null)
                throw new ArgumentNullException("event");

            AggregateId = aggregateId;
            Event = @event;
            Version = version;
        }

        public Guid AggregateId { get; set; }

        public IEvent Event { get; set; }

        public int Version { get; set; }

        public string Serialized()
        {
            return string.Join(Seperator, new object[] { AggregateId, Version, Event.GetType(), JsonConvert.SerializeObject(Event) });            
        }

        public static Record Deserialize(string value, Assembly assembly)
        {
            var values = value.Split(new[] { Seperator }, StringSplitOptions.RemoveEmptyEntries);           
            
            var aggregateId = Guid.Parse(values[0]);
            var version = Convert.ToInt32(values[1]);
            var eventType = assembly.GetType(values[2], true);
            var @event = (IEvent)JsonConvert.DeserializeObject(values[3], eventType);

            return new Record(aggregateId, @event, version);
        }
    }
}
