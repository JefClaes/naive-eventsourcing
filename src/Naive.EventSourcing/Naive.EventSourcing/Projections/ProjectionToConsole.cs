using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.Projections
{
    public class ProjectionToConsole : IProjection        
    {
        public void Handle(EventStream eventStream)
        {
            foreach (var @event in eventStream)            
                Handle((dynamic)@event);            
        }

        public void When(WithdrawalAmountExceeded @event)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Withdrawal amount exceeded: " + @event.Amount);
        }

        public void When(AmountDeposited @event)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Amount deposited: " + @event.Amount);        
        }

        public void When(AmountWithdrawn @event)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Amount withdrawn: " + @event.Amount);
        }      
    }    
}
