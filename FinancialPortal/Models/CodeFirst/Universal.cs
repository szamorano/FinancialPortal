using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinancialPortal.Models.CodeFirst
{
    public class Universal : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if nobody's logged in none of the viewbags will be sent and if you use count they will have errors because count method provides int which is value type so you need to have an error handling for the if statement with elseOK
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                ViewBag.FirstName = user.FirstName;
                ViewBag.LastName = user.LastName;
                ViewBag.FullName = user.FullName;
                ViewBag.ProfilePic = user.ProfilePic;
                ViewBag.Notifications = user.Notifications.ToList();
                ViewBag.EmailAddress = user.Email;

                if (user.HouseholdId == null)
                {
                    ViewBag.OverDraft = "False";
                }
                else
                {
                    // Find current user accounts with a balance below $0.
                    List<BankAccount> currentUserAccountsOverDraft = new List<BankAccount>();
                    currentUserAccountsOverDraft = user.Household.BankAccount.Where(b => b.BankAccountBalance < 0).ToList();
                    if (currentUserAccountsOverDraft.Count() == 0)
                    {
                        ViewBag.OverDraft = "False"; // will be checked in the _Layout view
                    }
                    else
                    {
                        ViewBag.OverDraft = "True"; // will be checked in the _Layout view
                    }
                }




                if (user.HouseholdId != 0)
                {
                    ViewBag.userHID = user.HouseholdId;
                }

                base.OnActionExecuting(filterContext);
            }
        }
    }
}