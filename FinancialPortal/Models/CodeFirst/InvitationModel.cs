using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models.CodeFirst
{
    public class InvitationModel
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        [Required]
        public string Body { get; set; }
        [Required, Display(Name = "Email"), EmailAddress]
        public string EmailTo { get; set; }
    }
}