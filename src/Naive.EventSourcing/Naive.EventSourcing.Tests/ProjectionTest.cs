using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Naive.EventSourcing.Projections;

namespace Naive.EventSourcing.Tests
{
    [TestClass]
    public class ProjectionTest
    {
        [TestMethod]
        public void ReadModelIsMaintedWhileProjectingTheEventStream()
        {
            var events = new List<IEvent>() 
            {
                new WithdrawalAmountExceeded(new Amount(3000)),
                new AmountDeposited(new Amount(300)),
                new AmountDeposited(new Amount(500)),
                new AmountWithdrawn(new Amount(100))
            };
            var stream = new EventStream(events);

            new ProjectionHost(GetType().Assembly).RunOver(stream);

            Assert.AreEqual(1, EvilStatisticsReadModel.WithdrawalAmountExceededCount);
            Assert.AreEqual(2, EvilStatisticsReadModel.AmountDepositedCount);
            Assert.AreEqual(1, EvilStatisticsReadModel.AmountWithdrawnCount);
        }

        public class ProjectionsToEvilStaticsReadModel : IProjection            
        {
            public void Handle(EventStream eventStream)
            {
                foreach (var @event in eventStream)
                    When((dynamic)@event);
            }

            public void When(WithdrawalAmountExceeded @event)
            {
                EvilStatisticsReadModel.WithdrawalAmountExceededCount++;
            }

            public void When(AmountDeposited @event)
            {
                EvilStatisticsReadModel.AmountDepositedCount++;
            }

            public void When(AmountWithdrawn @event)
            {
                EvilStatisticsReadModel.AmountWithdrawnCount++;
            }           
        }

        public class EvilStatisticsReadModel
        {
            public static int WithdrawalAmountExceededCount { get; set; }

            public static int AmountDepositedCount { get; set; }

            public static int AmountWithdrawnCount { get; set; }
        }
    }
}
