using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models.CodeFirst
{
    public class BankAccount
    {
        public int Id { get; set; }
        public DateTime Open { get; set; }
        public string BankAccountType { get; set; }
        public decimal BankAccountBalance { get; set; }
        public decimal BankAccountDeposit { get; set; }
        public decimal BankAccountWithdrawal { get; set; }
        public string BankAccountName { get; set; }
        public bool BankAccountIsJoint { get; set; }
        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }
        public virtual ICollection<Transaction> BankAccountTransactions { get; set; }



    }
}