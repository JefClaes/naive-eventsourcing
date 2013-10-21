using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Naive.EventSourcing.Projections
{
    public class ProjectionHost
    {
        private readonly Assembly _assembly;

        public ProjectionHost() 
        {
            _assembly = typeof(IProjectionFor<>).Assembly;
        }

        public ProjectionHost(Assembly assembly)
        {
            _assembly = assembly;
        }

        public void Run(EventStream eventStream)
        {
            foreach (var @event in eventStream)
            {
                var projectionType = typeof(IProjectionFor<>).MakeGenericType(@event.GetType());
                var projectionInstances = _assembly
                    .GetTypes().Where(x => projectionType.IsAssignableFrom(x))
                    .Select(x => Activator.CreateInstance(x))
                    .ToList();

                foreach (var projectionInstance in projectionInstances)
                    ((dynamic)projectionInstance).Handle((dynamic)@event);
            }
        }
    }
}
