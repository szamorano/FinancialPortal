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

namespace FinancialPortal.Controllers
{
    public class TransactionsController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            //var transactions = user.Household.BankAccount.SelectMany(t => t.BankAccountTransactions);
            var transactions = db.Transactions.Where(t => t.AuthorId == user.Id).Include(t => t.Author).Include(t => t.BankAccount).Include(t => t.Category).Include(t => t.TransactionType);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "BankAccountName");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Amount,Description,CategoryId,TransactionTypeId,BankAccountId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                transaction.AuthorId = user.Id;
                transaction.DateCreated = DateTime.Now;
                transaction.Void = false;
                db.Transactions.Add(transaction);

                BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);

                if (transaction.TransactionTypeId == 1)
                {
                    bankAccount.BankAccountBalance -= transaction.Amount;
                }
                else
                {
                    bankAccount.BankAccountBalance += transaction.Amount;
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", transaction.AuthorId);
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "BankAccountType", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type", transaction.TransactionTypeId);
            return View(transaction);


            //    var user = db.Users.Find(User.Identity.GetUserId());
            //    var household = user.Household;
            //    var updated = false;
            //    transaction.AuthorId = user.Id;
            //    transaction.DateCreated = DateTime.Now;
            //    transaction.Void = false;
            //    db.Transactions.Add(transaction);

            //    BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);


            //    if (transaction.TransactionTypeId == 1)
            //    {
            //        bankAccount.BankAccountBalance -= transaction.Amount;
            //        updated = true;
            //    }
            //    else if (transaction.TransactionTypeId == 2)
            //    {
            //        bankAccount.BankAccountBalance += transaction.Amount;
            //        updated = true;
            //    }
            //    db.SaveChanges();
            //    if (updated == true && bankAccount.Household != null)
            //    {
            //        if (bankAccount.BankAccountBalance == 0)
            //        {
            //            foreach (var u in household.HouseholdMember)
            //            {
            //                Notification n = new Notification();
            //                n.UserId = bankAccount.HouseholdId.ToString();
            //                n.Date = DateTime.Now;
            //                n.BankAccountId = bankAccount.Id;
            //                n.Type = "Zero Dollars";
            //                n.Description = "Your account: " + bankAccount.BankAccountName + " has reached an amount of zero.";
            //                db.Notifications.Add(n);
            //                db.SaveChanges();
            //            }
            //        }


            //        else if (bankAccount.BankAccountBalance < 0)
            //        {
            //            foreach (var u in household.HouseholdMember)
            //            {
            //                Notification n = new Notification();
            //                n.UserId = u.Id;
            //                n.Date = DateTime.Now;
            //                n.BankAccountId = bankAccount.Id;
            //                n.Type = "Over Draft";
            //                n.Description = "Your account: " + bankAccount.BankAccountName + " has reached a negative amount.";
            //                db.Notifications.Add(n);
            //                db.SaveChanges();
            //            }
            //        }

            //        bankAccount.Open = DateTime.Now;
            //        db.SaveChanges();

            //    }
            //    return RedirectToAction("Index");
            //}

            //ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.Id);
            //ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name", transaction.AccountTypeId);
            //ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", transaction.AuthorId);
            //ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            //ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type", transaction.TransactionTypeId);
            //return View(transaction);






        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", transaction.AuthorId);
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "BankAccountType", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type", transaction.TransactionTypeId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Amount,Description,CategoryId,TransactionTypeId,BankAccountId,AuthorId,DateCreated,Void")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", transaction.AuthorId);
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "BankAccountType", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type", transaction.TransactionTypeId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
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
