using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models.CodeFirst
{
    interface IBudget
    {
        void Income();
        void Expense();
    }
}