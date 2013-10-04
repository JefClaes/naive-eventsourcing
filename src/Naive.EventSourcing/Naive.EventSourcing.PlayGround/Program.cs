using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Naive.EventSourcing.EventStore;
using Naive.EventSourcing.Projections;

namespace Naive.EventSourcing.PlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            sw.Start();

            var accountId = Guid.Parse("a7deef0f-c5a6-49c9-b2d3-9da84c3c1694");

            var account = new Account(accountId);

            for (var i = 0; i < 5000; i++) 
            {
                account.Withdraw(i);
                account.Deposit(i * 2);
            }

            var eventStore = new FileEventStore(@"C:\EventStore.txt");
            eventStore.Append(accountId, account.RecordedEvents());

            sw.Stop();

            Console.WriteLine(string.Format("Appended {0} events to stream in {1}.", account.RecordedEvents().Count(), sw.Elapsed.TotalSeconds));

            sw.Restart();

            var stream = eventStore.GetStream(accountId);

            new ProjectionHost().Run(stream);

            sw.Stop();

            Console.WriteLine(string.Format("Read {0} events from stream in {1}.", stream.Count(), sw.Elapsed.TotalSeconds));

            sw.Reset();

            var restoredAccount = new Account(accountId);
            restoredAccount.Initialize(stream);

            sw.Stop();

            Console.WriteLine(string.Format("Replayed {0} events from stream in {1}.", stream.Count(), sw.Elapsed.TotalSeconds));
            Console.ReadLine();
        }
    }
}
