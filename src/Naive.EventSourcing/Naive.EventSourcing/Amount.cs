using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class Amount
    {
        private readonly int _value;

        public Amount()
        {
            _value = 0;
        }

        public Amount(int value)
        {
            _value = value;
        }

        public int Value { get { return _value; } }

        public bool IsOver(Amount amount)
        {
            return _value > amount.Value;
        }

        public Amount Substract(Amount amount)
        {
            return new Amount(_value - amount.Value);
        }

        public Amount Add(Amount amount)
        {           
            return new Amount(_value + amount.Value);
        }

        public override bool Equals(object obj)
        {
            var typedOther = obj as Amount;

            if (typedOther == null)
                return false;

            return typedOther.Value == _value;
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
