using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models.CodeFirst
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public int BankAccountId { get; set; }
        public string AuthorId { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}