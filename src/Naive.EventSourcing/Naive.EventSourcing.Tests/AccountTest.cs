using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Naive.EventSourcing.Tests
{   
    [TestClass]  
    public class WhenWithdrawingTheAmountIsWithdrawn 
    {
        private Account _account;
        
        [TestInitialize]
        public void Setup()
        {
            GivenANewAccount();
            WhenIWithdrawTwenty();
        }

        public void GivenANewAccount()
        {
            _account = new Account(Guid.NewGuid());
        }      

        public void WhenIWithdrawTwenty()
        {
            _account.Withdraw(20);
        }

        [TestMethod]
        public void ThenTwentyShouldBeWithdrawn()
        {
            Assert.IsTrue(_account.RecordedEvents().Contains(new AmountWithdrawn(20)));
        }      
    }

    [TestClass]
    public class WhenInitializingEverythingIsInitialized
    {
        private Account _account;
        private Guid _accountId;

        [TestInitialize]
        public void Setup() 
        {
            _accountId = Guid.NewGuid();

            GivenANewAccount();
        }

        public void GivenANewAccount()
        {
            _account = new Account(_accountId);
        }

        [TestMethod]
        public void ThenTheIdIsSet()
        {
            Assert.AreEqual(_accountId, _account.Id);
        }

        [TestMethod]
        public void ThenNothingHappened()
        {
            Assert.AreEqual(0, _account.RecordedEvents().Count());
        }      
    }
}
