using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class Account
    {
        private int _amount = 0;

        private readonly EventRecorder _eventRecorder;

        public Account()
        {
            _eventRecorder = new EventRecorder();
        }

        public void Initialize(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
                When((dynamic)@event);
        }

        public IEnumerable<IEvent> RecordedEvents()
        {
            return _eventRecorder.RecordedEvents();
        }

        public void Deposit(int amount)
        {
            Apply(new AmountDeposited(amount));
        }

        public void Withdraw(int amount)
        {
            if (amount > AmountPolicy.Maximum)
            {
                _eventRecorder.Record(new WithdrawalAmountExceeded(amount));

                return;
            }

            Apply(new AmountWithdrawn(amount));
        }

        private void Apply(IEvent @event)
        {
            When((dynamic)@event);
            _eventRecorder.Record(@event);
        }

        private void When(AmountWithdrawn @event)
        {
            _amount -= @event.Amount;
        }

        private void When(AmountDeposited @event)
        {
            _amount += @event.Amount;
        }
    }

    public class AmountPolicy
    {
        public static int Maximum { get { return 5000; } }
    }
}
