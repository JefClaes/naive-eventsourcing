using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class AmountWithdrawn : IEvent
    {
        public AmountWithdrawn(int amount)
        {
            Amount = amount;
        }

        public int Amount { get; private set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other.GetType() != GetType())
                return false;

            var otherTyped = other as AmountWithdrawn;

            return otherTyped.Amount == Amount;
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
        public AmountDeposited(int amount)
        {
            Amount = amount;
        }

        public int Amount { get; private set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other.GetType() != GetType())
                return false;

            var otherTyped = other as AmountWithdrawn;

            return otherTyped.Amount == Amount;
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
        public WithdrawalAmountExceeded(int amount)
        {
            Amount = amount;
        }

        public int Amount { get; private set; }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other.GetType() != GetType())
                return false;

            var otherTyped = other as AmountWithdrawn;

            return otherTyped.Amount == Amount;
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
