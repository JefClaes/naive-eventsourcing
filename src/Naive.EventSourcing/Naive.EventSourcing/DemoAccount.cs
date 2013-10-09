using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.Demo
{
    public class Account : IEventSourcedAggregate
    {
        private readonly Guid _id;

        public Account(Guid id)
        {
            _id = id;
        }

        public Guid Id { get { return _id; } }

        public void Initialize(EventStream eventStream)
        {
            throw new NotImplementedException();
        }

        public EventStream RecordedEvents()
        {
            throw new NotImplementedException();
        }
    }
}
