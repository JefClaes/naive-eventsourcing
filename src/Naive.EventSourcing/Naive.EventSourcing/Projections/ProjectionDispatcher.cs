using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Naive.EventSourcing.Projections
{
    public class ProjectionDispatcher
    {
        private readonly Assembly _assembly;

        public ProjectionDispatcher()
        {
            _assembly = typeof(IProjection).Assembly;
        }

        public ProjectionDispatcher(Assembly assembly)
        {
            _assembly = assembly;
        }

        public void Dispatch(EventStream eventStream)
        {
            var projectionType = typeof(IProjection);
            var projectionInstances = _assembly
                .GetTypes().Where(x => projectionType.IsAssignableFrom(x))
                .Select(x => (IProjection)Activator.CreateInstance(x))
                .ToList();

            foreach (var projectionInstance in projectionInstances)
                projectionInstance.Handle(eventStream);
        }
    }
}
