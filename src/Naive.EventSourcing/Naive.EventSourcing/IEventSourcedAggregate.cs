﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public interface IEventSourcedAggregate
    {
        Guid Id { get; }

        void Initialize(EventStream eventStream);

        EventStream RecordedEvents();
    }
}
