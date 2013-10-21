using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.Projections
{
    public interface IProjectionOf<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}
