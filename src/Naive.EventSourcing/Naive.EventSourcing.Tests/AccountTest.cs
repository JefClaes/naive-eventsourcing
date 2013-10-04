using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

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
            _account.Raised(new AmountWithdrawn(20));
        }      
    }

    [TestClass]
    public class WhenDepositingTheAmountIsDeposited
    {
        private Account _account;

        [TestInitialize]
        public void Setup()
        {
            GivenANewAccount();
            WhenIDepositTwenty();
        }

        public void GivenANewAccount()
        {
            _account = new Account(Guid.NewGuid());
        }

        public void WhenIDepositTwenty()
        {
            _account.Deposit(20);
        }

        [TestMethod]
        public void ThenTwentyShouldBeDeposited()
        {
            _account.Raised(new AmountDeposited(20));
        }
    }

    [TestClass]
    public class WhenExceedingTheAmountPolicy
    {
        private Account _account;

        [TestInitialize]
        public void Setup()
        {
            GivenANewAccount();
            WhenIWithdrawTooMuch();
        }

        public void GivenANewAccount()
        {
            _account = new Account(Guid.NewGuid());
        }

        public void WhenIWithdrawTooMuch()
        {
            _account.Withdraw(GivenExceedingAmount());
        }

        private int GivenExceedingAmount()
        {
            return AmountPolicy.Maximum + 1;
        }

        [TestMethod]
        public void ThenTheWithdrawalAmountShouldBeExceeded()
        {
            _account.Raised(new WithdrawalAmountExceeded(GivenExceedingAmount()));
        }

        [TestMethod]
        public void ThenNoMoneyShouldBeWithdrawn()
        {
            _account.DidNotRaise(new AmountWithdrawn(GivenExceedingAmount()));
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
            _account.RaisedNothing();
        }      
    }
}
