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

            var accountId = Guid.Parse("44b9da68-8415-4adc-b99f-2631f1115f9e");

            var account = new Account(accountId);

            for (var i = 0; i < 500; i++) 
            {
                account.Withdraw(i * 1000);
                account.Deposit(i * 1000 * 2);
            }

            var eventStore = new FileEventStore();
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
