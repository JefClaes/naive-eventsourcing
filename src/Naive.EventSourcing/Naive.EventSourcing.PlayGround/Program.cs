using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.PlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountId = Guid.Parse("a7deef0f-c5a6-49c9-b2d3-9da84c3c1694");

            var account = new Account(accountId);

            account.Withdraw(50);
            account.Deposit(60);
            account.Withdraw(100);

            foreach (var @event in account.RecordedEvents())
                Console.WriteLine(@event);

            var eventStore = new FileEventStore(@"C:\EventStore.txt");
            eventStore.Append(accountId, account.RecordedEvents());

            Console.ReadLine();

            var stream = eventStore.GetStream(accountId);

            foreach (var @event in stream)
                Console.WriteLine(@event);

            Console.ReadLine();
        }
    }
}
