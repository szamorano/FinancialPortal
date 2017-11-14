﻿using System;
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
    public class BudgetsController : Universal
    {

        // GET: Budgets
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var budgets = user.Household.HouseholdBudget.ToList();
            return View(budgets);
        }

        // GET: Budgets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // GET: Budgets/Create
        public ActionResult Create()
        {
            ViewBag.FrequencyId = new SelectList(db.Frequencies, "Id", "Name");
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FrequencyId,StartAmount,CategoryId,BudgetDuration,BudgetName,HouseholdId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());

                budget.AuthorId = user.Id;
                budget.HouseholdId = user.HouseholdId.Value;
                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budget.CategoryId);
            ViewBag.FrequencyId = new SelectList(db.Frequencies, "Id", "Name", budget.FrequencyId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budget.CategoryId);
            ViewBag.FrequencyId = new SelectList(db.Frequencies, "Id", "Name", budget.FrequencyId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", budget.HouseholdId);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FrequencyId,StartAmount,CategoryId,BudgetDuration,BudgetName,HouseholdId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", budget.CategoryId);
            ViewBag.FrequencyId = new SelectList(db.Frequencies, "Id", "Name", budget.FrequencyId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "HouseholdName", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
            db.Budgets.Remove(budget);
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
