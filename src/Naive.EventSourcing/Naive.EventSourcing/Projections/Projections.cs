﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing.Projections
{
    public class Projections : 
        IProjectionFor<WithdrawalAmountExceeded>,
        IProjectionFor<AmountDeposited>,
        IProjectionFor<AmountWithdrawn>
    {
        public void Handle(WithdrawalAmountExceeded @event)
        {
            Console.WriteLine("Withdrawal amount exceeded: " + @event.Amount);
        }

        public void Handle(AmountDeposited @event)
        {
            Console.WriteLine("Amount deposited: " + @event.Amount);        
        }

        public void Handle(AmountWithdrawn @event)
        {
            Console.WriteLine("Amount withdrawn: " + @event.Amount);
        }
    }    
}