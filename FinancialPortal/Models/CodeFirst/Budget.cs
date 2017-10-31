using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models.CodeFirst
{
    public class Budget
    {
        public int Id { get; set; }
        public string BudgetCategory { get; set; }
        public decimal BudgetAmount { get; set; }
        public int BudgetDuration { get; set; }
        public string BudgetName { get; set; }
        public int HouseholdId { get; set; }

        //public int BudgetIncome { get; set; }
        //public int BudgetExpense { get; set; }


        public virtual Household Household { get; set; }
    }
}