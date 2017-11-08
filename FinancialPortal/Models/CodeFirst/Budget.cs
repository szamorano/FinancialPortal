using FluentDateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models.CodeFirst
{
    public class Budget
    {
        public int Id { get; set; }
        public int FrequencyId { get; set; }
        public decimal StartAmount { get; set; }
        public string BudgetCategory { get; set; }
        public int BudgetDuration { get; set; }
        public string BudgetName { get; set; }
        public int HouseholdId { get; set; }

        //public int BudgetIncome { get; set; }
        //public int BudgetExpense { get; set; }

        public virtual Frequency Frequency { get; set; }
        public virtual Category Category { get; set; }
        public virtual Household Household { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public decimal? SpentAmount
        {
            get
            {
                if (Category != null)
                {
                    if (Frequency != null && Frequency.Name == "Weekly")
                    {
                        var previousSunday = DateTime.Now.Previous(DayOfWeek.Sunday);
                        var nextMonday = DateTime.Now.Next(DayOfWeek.Monday);
                        return Category.Transactions.Where(t => t.DateCreated.Date > previousSunday && t.DateCreated < nextMonday && t.Void == false).Sum(t => t.Amount);
                    }
                    else if (Frequency != null && Frequency.Name == "Monthly")
                    {
                        return Category.Transactions.Where(t => t.DateCreated.Month == DateTime.Now.Month && t.DateCreated.Year == DateTime.Now.Year && t.Void == false).Sum(t => t.Amount);
                    }
                    else if (Frequency != null && Frequency.Name == "Yearly")
                    {
                        return Category.Transactions.Where(t => t.DateCreated.Year == DateTime.Now.Year && t.Void == false).Sum(t => t.Amount);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }




        public decimal? LeftAmount
        {
            get
            {
                return StartAmount - SpentAmount;
            }
        }
    }
}