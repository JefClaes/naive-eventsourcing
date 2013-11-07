using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.EventStore
{
    public class OptimisticConcurrencyException : ConcurrencyException
    {
        public OptimisticConcurrencyException(int expectedVersion, int currentVersion) :
            base(string.Format("Version found: {0}, expected: {1}", currentVersion, expectedVersion)) { }
    }
}
