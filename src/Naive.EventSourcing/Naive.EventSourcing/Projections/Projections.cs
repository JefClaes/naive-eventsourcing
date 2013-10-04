using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.Projections
{
    public class ProjectionForWithdrawalAmountExceeded : IProjectionFor<WithdrawalAmountExceeded>
    {
        public void Handle(WithdrawalAmountExceeded @event)
        {
            Console.WriteLine("Withdrawal amount exceeded: " + @event.Amount);
        }
    }

    public class ProjectionForAmountDeposited : IProjectionFor<AmountDeposited>
    {
        public void Handle(AmountDeposited @event)
        {
            Console.WriteLine("Amount deposited: " + @event.Amount);
        }
    }

    public class ProjectionForAmountWithdrawn : IProjectionFor<AmountWithdrawn>
    {
        public void Handle(AmountWithdrawn @event)
        {
            Console.WriteLine("Amount withdrawn: " + @event.Amount);
        }
    }
}
