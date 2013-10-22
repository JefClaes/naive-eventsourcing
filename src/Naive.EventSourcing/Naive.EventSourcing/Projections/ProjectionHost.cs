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
            _assembly = typeof(IProjection).Assembly;
        }

        public ProjectionHost(Assembly assembly)
        {
            _assembly = assembly;
        }

        public void RunOver(EventStream eventStream)
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
