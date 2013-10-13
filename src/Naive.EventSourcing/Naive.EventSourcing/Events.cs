using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class AmountWithdrawn : IEvent
    {
        public AmountWithdrawn(Amount amount)
        {
            Amount = amount;
        }

        public Amount Amount { get; private set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other.GetType() != GetType())
                return false;

            var otherTyped = other as AmountWithdrawn;

            return otherTyped.Amount.Equals(Amount);
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode() ^ 97;
        }

        public override string ToString()
        {
            return string.Format("Amount withdrawn: {0}", Amount);
        }
    }

    public class AmountDeposited : IEvent
    {
        public AmountDeposited(Amount amount)
        {
            Amount = amount;
        }

        public Amount Amount { get; private set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other.GetType() != GetType())
                return false;

            var otherTyped = other as AmountDeposited;

            return otherTyped.Amount.Equals(Amount);
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode() ^ 97;
        }

        public override string ToString()
        {
            return string.Format("Amount deposited: {0}", Amount);
        }        
    }

    public class WithdrawalAmountExceeded : IEvent
    {
        public WithdrawalAmountExceeded(Amount amount)
        {
            Amount = amount;
        }

        public Amount Amount { get; private set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other.GetType() != GetType())
                return false;

            var otherTyped = other as WithdrawalAmountExceeded;

            return otherTyped.Amount.Equals(Amount);
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode() ^ 97;
        }

        public override string ToString()
        {
            return string.Format("Amount exceeded: {0}", Amount);
        }
    }
}
