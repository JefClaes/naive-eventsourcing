using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naive.EventSourcing
{
    public class Account : IEventSourcedAggregate
    {
        private readonly Guid _id;        
        private readonly EventRecorder _eventRecorder;

        public Account(Guid id)
        {
            _id = id;
            _eventRecorder = new EventRecorder();
        }      

        public Guid Id { get { return _id; } }

        public int Amount { get; private set; }

        public void Initialize(EventStream eventStream)
        {
            foreach (var @event in eventStream)
                Play((dynamic)@event);
        }

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
                Apply(new WithdrawalAmountExceeded(amount));

                return;
            }

            Apply(new AmountWithdrawn(amount));
        }

        private void Apply(IEvent @event)
        {
            Play((dynamic)@event);
            _eventRecorder.Record(@event);
        }

        private void Play(AmountWithdrawn @event)
        {
            Amount -= @event.Amount;
        }

        private void Play(AmountDeposited @event)
        {
            Amount += @event.Amount;
        }

        private void Play(WithdrawalAmountExceeded @event) { }
    }

    public class AmountPolicy
    {
        public static int Maximum { get { return 5000; } }
    }
}
