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
using FinancialPortal.Models.CodeFirst.Helpers;

namespace FinancialPortal.Controllers
{
    [AuthorizeHouseholdRequired]
    public class TransactionsController : Universal
    {

        // GET: Transactions
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            //var transactions = user.Household.BankAccount.SelectMany(t => t.BankAccountTransactions);
            var transactions = user.Household.BankAccount.SelectMany(a => a.BankAccountTransactions).ToList();
            //var transactions = db.Transactions.Where(t => t.AuthorId == user.Id).Include(t => t.Author).Include(t => t.BankAccount).Include(t => t.Category).Include(t => t.TransactionType);
            return View(transactions);
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
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.BankAccountId = new SelectList(user.Household.BankAccount, "Id", "BankAccountName");
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
            var user = db.Users.Find(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {

                var household = user.Household;
                var updated = false;
                transaction.AuthorId = user.Id;
                transaction.DateCreated = DateTime.Now;
                transaction.Void = false;
                db.Transactions.Add(transaction);

                BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);


                if (transaction.TransactionTypeId == 1)
                {
                    bankAccount.BankAccountBalance -= transaction.Amount;
                    updated = true;
                }
                else if (transaction.TransactionTypeId == 2)
                {
                    bankAccount.BankAccountBalance += transaction.Amount;
                    updated = true;
                }
                db.SaveChanges();
                if (updated == true && bankAccount.Household != null)
                {
                    if (bankAccount.BankAccountBalance == 0)
                    {
                        foreach (var u in household.HouseholdMember)
                        {
                            Notification n = new Notification();
                            n.UserId = bankAccount.HouseholdId.ToString();
                            n.Date = DateTime.Now;
                            n.BankAccountId = bankAccount.Id;
                            n.Type = "Zero Dollars";
                            n.Description = "Your account: " + bankAccount.BankAccountName + " has reached an amount of zero.";
                            db.Notifications.Add(n);
                            db.SaveChanges();
                        }
                    }


                    else if (bankAccount.BankAccountBalance < 0)
                    {
                        foreach (var u in household.HouseholdMember)
                        {
                            Notification n = new Notification();
                            n.HouseholdId = household.Id;
                            n.UserId = u.Id;
                            n.Date = DateTime.Now;
                            n.BankAccountId = bankAccount.Id;
                            n.Type = "Over Draft";
                            n.Description = "Your account: " + bankAccount.BankAccountName + " has reached a negative amount.";
                            db.Notifications.Add(n);
                            db.SaveChanges();
                        }
                    }

                    bankAccount.Open = DateTime.Now;
                    db.SaveChanges();

                }
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "Name", transaction.Id);
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", transaction.AuthorId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Type", transaction.TransactionTypeId);
            return View(transaction);









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

        // GET: Transactions/Void/5
        public ActionResult Void(int? id)
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

            if (transaction.Void == true)
            {
                return RedirectToAction("Unvoid");
            }
            return View(transaction);
        }

        // POST: Transactions/Void/5
        [HttpPost, ActionName("Void")]
        [ValidateAntiForgeryToken]
        public ActionResult VoidConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);

            BankAccount account = db.BankAccounts.Find(transaction.BankAccountId);
            // REVERSE the transaction but DO NOT REMOVE from database
            // check type: 1, debit. 2, credit.

            if (transaction.TransactionTypeId == 1)
            {
                account.BankAccountBalance += transaction.Amount;
            }
            else
            {
                account.BankAccountBalance -= transaction.Amount;


            }

            // check for account overdraft on  this account's transaction.
            if (account.BankAccountBalance < 0)
            {
                ViewBag.Overdraft = "True";
            }
            else
            {
                ViewBag.Overdraft = "False";
            }

            transaction.Void = true;

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Transactions/Unvoid/5
        public ActionResult Unvoid(int? id)
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

        // POST: Transactions/UnVoid/5
        [HttpPost, ActionName("Unvoid")]
        [ValidateAntiForgeryToken]
        public ActionResult UnvoidConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);

            BankAccount account = db.BankAccounts.Find(transaction.BankAccountId);
            // ADD BACK the transaction but DO NOT REMOVE from database
            // check type: 1, debit. 2, credit.

            if (transaction.TransactionTypeId == 1)
            {
                account.BankAccountBalance -= transaction.Amount;
            }
            else
            {
                account.BankAccountBalance += transaction.Amount;


            }

            // check for account overdraft on  this account's transaction.
            if (account.BankAccountBalance < 0)
            {
                ViewBag.Overdraft = "True";
            }
            else
            {
                ViewBag.Overdraft = "False";
            }

            transaction.Void = false;

            db.SaveChanges();
            return RedirectToAction("Index");
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
