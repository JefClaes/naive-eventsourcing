using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.Projections
{
    public class Projections : 
        IProjectionOf<WithdrawalAmountExceeded>,
        IProjectionOf<AmountDeposited>,
        IProjectionOf<AmountWithdrawn>
    {
        public void Handle(WithdrawalAmountExceeded @event)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Withdrawal amount exceeded: " + @event.Amount);
        }

        public void Handle(AmountDeposited @event)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Amount deposited: " + @event.Amount);        
        }

        public void Handle(AmountWithdrawn @event)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Amount withdrawn: " + @event.Amount);
        }
    }    
}
