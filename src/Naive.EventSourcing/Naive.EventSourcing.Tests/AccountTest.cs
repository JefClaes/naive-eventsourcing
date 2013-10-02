using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Naive.EventSourcing.Tests
{   
    [TestClass]  
    public class AccountTest 
    {
        private Account _account;
        
        public void GivenANewAccount()
        {
            _account = new Account();
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
}
