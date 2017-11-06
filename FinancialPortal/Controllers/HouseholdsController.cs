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
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        public ActionResult Index()
        {
            return View(db.Households.ToList());
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

            if (ModelState.IsValid)
            {
                db.Households.Add(household);
                db.SaveChanges();

                await HttpContext.RefreshAuthentication(db.Users.Find(User.Identity.GetUserId()));



                return RedirectToAction("Index");
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

        // GET: Household/Join
        public ActionResult JoinHousehold(int? id)
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

        // GET: Households/Invite
        [AuthorizeHouseholdRequired]
        public ActionResult InviteToJoin()
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
                        callbackUrl = Url.Action("Register", "Account", new { id = model.Id }, protocol: Request.Url.Scheme);
                    }

                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";
                    var from = "FinancialPortal<" + me.Email + ">";
                    var subject = "Invitation to Join Household!";
                    var to = model.EmailTo;


                    var email = new MailMessage(from, to)
                    {
                        Subject = subject,
                        Body = string.Format(body, me.FullName, model.Body, "Please click on the link below to confirm invitation: <br /> <a href=\" " + callbackUrl + "\">Link to invitation.</a>"),
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
