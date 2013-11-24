using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.EventStore
{
    public class CorruptionException : Exception
    {
        public CorruptionException(string msg) : base(msg) { }
    }
}
