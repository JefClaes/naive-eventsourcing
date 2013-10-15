using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class Amount
    {      

        public Amount(int value)
        {
            Value = value;
        }

        public int Value { get; protected set; }

        public bool IsOver(Amount amount)
        {
            return Value > amount.Value;
        }

        public Amount Substract(Amount amount)
        {
            return new Amount(Value - amount.Value);
        }

        public Amount Add(Amount amount)
        {
            return new Amount(Value + amount.Value);
        }

        public override bool Equals(object obj)
        {
            var typedOther = obj as Amount;

            if (typedOther == null)
                return false;

            return typedOther.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
