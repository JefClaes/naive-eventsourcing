using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class Account : IEventSourcedAggregate
    {
        private readonly Guid _id;
        private Amount _amount;
        private readonly EventRecorder _eventRecorder;

        public Account(Guid id)
        {
            _id = id;
            _eventRecorder = new EventRecorder();
            _amount = new Amount();
        }      

        public Guid Id { get { return _id; } }

        public Amount Amount { get { return _amount; } }

        public void Initialize(EventStream eventStream)
        {
            foreach (var @event in eventStream)
                When((dynamic)@event);
        }

        public EventStream RecordedEvents()
        {
            return _eventRecorder.RecordedEvents();
        }

        public void Deposit(Amount amount)
        {
            Apply(new AmountDeposited(amount));
        }

        public void Withdraw(Amount amount)
        {
            if (amount.IsOver(AmountPolicy.Maximum))
            {
                Apply(new WithdrawalAmountExceeded(amount));

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
            _amount = _amount.Substract(@event.Amount);
        }

        private void When(AmountDeposited @event)
        {
            _amount = _amount.Add(@event.Amount);
        }

        private void When(WithdrawalAmountExceeded @event) { }
    }

    public class AmountPolicy
    {
        public static Amount Maximum { get { return new Amount(5000); } }
    }
}
