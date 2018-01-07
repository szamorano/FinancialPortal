using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPortal.Models;
using FinancialPortal.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using FinancialPortal.Models.CodeFirst.Helpers;
using System.Net.Mail;
using System.Configuration;

namespace FinancialPortal.Controllers
{
    [Authorize]
    public class HouseholdsController : Universal
    {
        // GET: Households
        [AuthorizeHouseholdRequired]
        public ActionResult Index(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.TotalIncome = user.Household.BankAccount.SelectMany(a => a.BankAccountTransactions.Where(t => t.Void == false && t.Amount > 0)).Sum(t => t.Amount);
            ViewBag.TotalExpense = Math.Abs(user.Household.BankAccount.SelectMany(a => a.BankAccountTransactions.Where(t => t.Void == false && t.Amount < 0)).Sum(t => t.Amount));
            //var user = db.Users.Find(User.Identity.GetUserId());
            //return View(db.Households.FirstOrDefault(h => h.Id == user.HouseholdId));
            var household = db.Households.Find(id);
            //return View(user.HouseholdId);
            return View(household);

        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdName,HouseholdDescription,HouseholdAmountRange,HouseholdCreatedDate")] Household household)
        {

            var user = db.Users.Find(User.Identity.GetUserId());
            household.HouseholdCreatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {


                db.Households.Add(household);
                db.SaveChanges();
                user.HouseholdId = household.Id;
                db.SaveChanges();
                await HttpContext.RefreshAuthentication(db.Users.Find(User.Identity.GetUserId()));



                return RedirectToAction("Index", new { id = household.Id });
            }

            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdName,HouseholdDescription,HouseholdAmountRange,HouseholdCreatedDate")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //GET
        public ActionResult CreateJoinHousehold()
        {
            return View();
        }


        // POST: Household/Join
        [HttpPost]
        [ValidateAntiForgeryToken]
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




        // GET: Households/Invite
        [AuthorizeHouseholdRequired]
        public ActionResult InviteToJoin()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            InvitationModel invite = new InvitationModel();
            invite.HouseholdId = (int)user.HouseholdId;
            return View(invite);
        }

        // GET: Households/UserAlreadyAssignedToHousehold
        [AuthorizeHouseholdRequired]
        public ActionResult UserAlreadyAssignedToHousehold()
        {
            return View();
        }



        // POST: Households/Invite
        [AuthorizeHouseholdRequired]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteToJoin(InvitationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var invitee = db.Users.FirstOrDefault(u => u.Email == model.EmailTo);
                    var me = db.Users.Find(User.Identity.GetUserId());
                    model.Id = me.HouseholdId.Value;
                    if (invitee != null && invitee.HouseholdId == model.Id)
                    {
                        return RedirectToAction("UserAlreadyAssignedToHousehold");
                    }

                    var callbackUrl = "";
                    if (invitee != null)
                    {
                        callbackUrl = Url.Action("JoinHousehold", "Households", new { id = model.Id }, protocol: Request.Url.Scheme);
                    }
                    else
                    {
                        callbackUrl = Url.Action("Register", "Account", new { id = model.HouseholdId }, protocol: Request.Url.Scheme);
                    }

                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";
                    var from = "FinancialPortal<" + me.Email + ">";
                    var subject = "Invitation to Join Household!";
                    var to = model.EmailTo;


                    var email = new MailMessage(from, to)
                    {
                        Subject = subject,
                        Body = string.Format(body, me.FullName, model.Body, "Please click on the link below to confirm invitation: <br /> <a href=\"" + callbackUrl + "\">Link to invitation.</a>"),
                        IsBodyHtml = true
                    };
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                    return RedirectToAction("InviteSent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }

        // GET: Households/Create
        public ActionResult InviteSent()
        {
            return View();
        }

        // GET: Households/Create
        public ActionResult Leave()
        {
            return View();
        }

        // GET: Households/Leave
        public ActionResult LeaveHousehold()
        {
            var currentHouseholdId = User.Identity.GetHouseholdId();
            var currentHousehold = db.Households.Find(currentHouseholdId);
            return View(currentHousehold);
        }


        // POST: Households/Leave
        [HttpPost]
        public async Task<ActionResult> LeaveHousehold(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            user.HouseholdId = null;
            db.SaveChanges();

            await HttpContext.RefreshAuthentication(user);
            return RedirectToAction("Index", "Home"); // Eventually the splash page.
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
