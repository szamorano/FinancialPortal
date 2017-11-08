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
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                ViewBag.FirstName = user.FirstName;
                ViewBag.LastName = user.LastName;
                ViewBag.FullName = user.FullName;
                ViewBag.ProfilePic = user.ProfilePic;
                ViewBag.Notifications = user.Notifications.ToList();
                if(user.HouseholdId != 0)
                {
                    ViewBag.userHID = user.HouseholdId;
                }

                base.OnActionExecuting(filterContext);
            }
        }
    }
}