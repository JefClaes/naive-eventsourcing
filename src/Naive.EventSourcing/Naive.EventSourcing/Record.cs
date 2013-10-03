using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Naive.EventSourcing
{
    public class Record
    {
        private const string Seperator = "+++";

        public Record(Guid aggregateId, IEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException("event");

            AggregateId = aggregateId;
            Event = @event;        
        }

        public Guid AggregateId { get; set; }

        public IEvent Event { get; set; }

        public string Serialized()
        {
            return string.Join(Seperator, new object[] { AggregateId, Event.GetType(), JsonConvert.SerializeObject(Event) });            
        }

        public static Record Deserialize(string value)
        {
            var values = value.Split(new[] { Seperator }, StringSplitOptions.RemoveEmptyEntries);

            var aggregateId = Guid.Parse(values[0]);
            var eventType = Type.GetType(values[1]);
            var @event = (IEvent)JsonConvert.DeserializeObject(values[2], eventType);

            return new Record(aggregateId, @event);
        }
    }
}
