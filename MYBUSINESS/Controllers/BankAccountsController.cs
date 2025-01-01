using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class BankAccountsController : Controller
    {
        private BusinessContext db = new BusinessContext();

        // GET: BankAccounts
        public async Task<ActionResult> Index()
        {
            return View(await db.BankAccounts.ToListAsync());
        }

        // GET: BankAccounts/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount businessAccount = await db.BankAccounts.FindAsync(id);
            if (businessAccount == null)
            {
                return HttpNotFound();
            }
            return View(businessAccount);
        }

        // GET: BusinessAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BusinessAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,AccountType,Description,Balance")] BankAccount bankAccount)
        {
            bankAccount.Id = Guid.NewGuid().ToString();
            
            if (ModelState.IsValid)
            {
                if (bankAccount.Balance == null) bankAccount.Balance = 0;

                db.BankAccounts.Add(bankAccount);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(bankAccount);
        }

        // GET: BusinessAccounts/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = await db.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BusinessAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,AccountType,Description,Balance")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                if (bankAccount.Balance == null) bankAccount.Balance = 0;
                db.Entry(bankAccount).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(bankAccount);
        }

        // GET: BusinessAccounts/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount businessAccount = await db.BankAccounts.FindAsync(id);
            if (businessAccount == null)
            {
                return HttpNotFound();
            }
            return View(businessAccount);
        }

        // POST: BusinessAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            BankAccount businessAccount = await db.BankAccounts.FindAsync(id);
            db.BankAccounts.Remove(businessAccount);
            await db.SaveChangesAsync();
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
