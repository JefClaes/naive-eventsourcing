using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Naive.EventSourcing.Projections
{
    public class ProjectionHost
    {
        public void Run(EventStream eventStream)
        {
            foreach (var @event in eventStream)
            {
                var projectionType = typeof(IProjectionFor<>).MakeGenericType(@event.GetType());
                var projectionInstances = typeof(IProjectionFor<>).Assembly
                    .GetTypes().Where(x => projectionType.IsAssignableFrom(x))
                    .Select(x => Activator.CreateInstance(x))
                    .ToList();

                foreach (var projectionInstance in projectionInstances)
                    ((dynamic)projectionInstance).Handle((dynamic)@event);
            }
        }
    }
}
