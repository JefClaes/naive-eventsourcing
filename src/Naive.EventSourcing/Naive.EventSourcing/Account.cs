using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class Account
    {
        private Guid _id;
        private int _amount = 0;

        private readonly EventRecorder _eventRecorder;

        public Account(Guid id)
        {
            _id = id;
            _eventRecorder = new EventRecorder();
        }

        public void Initialize(EventStream eventStream)
        {
            foreach (var @event in eventStream)
                When((dynamic)@event);
        }

        public Guid Id { get { return _id; } }

        public EventStream RecordedEvents()
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
