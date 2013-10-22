using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.Projections
{
    public interface IProjection
    {
        void Handle(EventStream eventStream);                     
    }  
}
