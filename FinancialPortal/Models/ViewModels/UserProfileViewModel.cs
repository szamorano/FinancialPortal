using FinancialPortal.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public ApplicationUser user { get; set; }
        public virtual Household household { get; set; }
    }
}