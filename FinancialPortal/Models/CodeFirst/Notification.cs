using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models.CodeFirst
{
    public class Notification
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int HouseholdId { get; set; }
        public string UserId { get; set; }
        public DateTime AdvanceNotice { get; set; }
        public int BankAccountId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Household Household { get; set; }
    }
}