using FinancialPortal.Models.CodeFirst;
using FinancialPortal.Models.CodeFirst.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace FinancialPortal.Controllers
{
    [Authorize]
    public class HomeController : Universal
    {


        //[AuthorizeHouseholdRequired]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }





        public ActionResult CreateJoinHousehold([Bind(Include = "Id,Name,AuthorId")] Household household)
        {
            //Implementation for creating and joining household
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user != null)
                {
                    household.HouseholdCreatedDate = DateTime.Now;
                    db.Households.Add(household);
                    db.SaveChanges();
                    user.HouseholdId = household.Id;
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Households", new { id = household.Id });
            }

            return View();
        }






        public async Task<ActionResult> LeaveHousehold()
        {
            //Implementation of leaving household
            var user = db.Users.Find(User.Identity.GetUserId());
            await ControllerContext.RequestContext.HttpContext.RefreshAuthentication(user);
            return View();
        }









    }

}