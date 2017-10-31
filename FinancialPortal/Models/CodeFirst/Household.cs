using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models.CodeFirst
{
    public class Household
    {
        public int Id { get; set; }
        public string HouseholdName { get; set; }
        public string HouseholdDescription { get; set; }
        public decimal HouseholdAmountRange { get; set; }
        public DateTime HouseholdCreatedDate { get; set; }
        
        public virtual ICollection<ApplicationUser> HouseholdMember { get; set; }
        public virtual ICollection<Budget> HouseholdBudget { get; set; }
        public virtual ICollection<BankAccount> BankAccount { get; set; }

    }
}