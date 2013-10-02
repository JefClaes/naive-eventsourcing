using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Naive.EventSourcing.Tests
{   
    [TestClass]  
    public class WhenWithdrawingTheAmountIsWithdrawn 
    {
        private Account _account;
        
        public void GivenANewAccount()
        {
            _account = new Account(Guid.NewGuid());
        }      

        public void WhenIWithdrawTwenty()
        {
            _account.Withdraw(50);
        }

        public void ThenTwentyShouldBeWithdrawn()
        {
            Assert.IsTrue(_account.RecordedEvents().Contains(new AmountWithdrawn(50)));
        }

        [TestMethod]
        public void Execute()
        {
            GivenANewAccount();
            WhenIWithdrawTwenty();
            ThenTwentyShouldBeWithdrawn();
        }
    }

    [TestClass]
    public class WhenInitializingEverythingIsInitialized
    {
        private Account _account;

        public void GivenANewAccount(Guid accountId)
        {
            _account = new Account(accountId);
        }

        public void ThenTheIdIsSet(Guid accountId)
        {
            Assert.AreEqual(accountId, _account.Id);
        }

        public void ThenTheRecordedEventsAreEmpty()
        {
            Assert.AreEqual(0, _account.RecordedEvents().Count());
        }

        [TestMethod]
        public void Execute()
        {
            var accountId = Guid.NewGuid();

            GivenANewAccount(accountId);
            ThenTheIdIsSet(accountId);
            ThenTheRecordedEventsAreEmpty();
        }
    }
}
