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
        public int CategoryId { get; set; }
        public string BudgetName { get; set; }
        public int HouseholdId { get; set; }
        public string AuthorId { get; set; }

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
                if (Household != null && Category != null && Frequency != null)
                {
                    decimal amount = 0;
                    if (Frequency.Name == "Weekly")
                    {
                        var previousSunday = DateTime.Now.Previous(DayOfWeek.Sunday);
                        var nextMonday = DateTime.Now.Next(DayOfWeek.Monday);
                        foreach (var trans in Category.Transactions.Where(t => t.BankAccount.HouseholdId == HouseholdId && t.DateCreated > previousSunday && t.DateCreated < nextMonday && t.Void == false).ToList())
                        {
                            amount -= trans.Amount;
                        }
                        return amount;

                    }


                    else if (Frequency.Name == "Monthly")
                    {
                        foreach (var trans in Category.Transactions.Where(t => t.BankAccount.HouseholdId == HouseholdId && t.DateCreated.Month == DateTime.Now.Month && t.DateCreated.Year == DateTime.Now.Year && t.Void == false).ToList())
                        {
                            amount -= trans.Amount;
                        }
                        return amount;
                    }
                    else if (Frequency.Name == "Yearly")
                    {
                        foreach (var trans in Category.Transactions.Where(t => t.BankAccount.HouseholdId == HouseholdId && t.DateCreated.Year == DateTime.Now.Year && t.Void == false).ToList())
                        {
                            amount -= trans.Amount;
                        }
                        return amount;
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


		public int? BudgetPercentage
        {
            get
            {
                if (StartAmount > 0)
                {
                    return Convert.ToInt32((SpentAmount / StartAmount) * 100);
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