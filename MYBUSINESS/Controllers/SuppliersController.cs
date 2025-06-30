using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    [Authorize(Roles = "Admin,Technical Manager")]
    public class SuppliersController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        // GET: Suppliers

        [Authorize(Roles = "Admin,Technical Manager")]
        public ActionResult Index(string id)
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            //var parseId = int.Parse(storeId);

            //var suppliers = DAL.dbSuppliers.Where(x => x.IsCreditor == false && x.Id > 0 && x.StoreId == parseId).ToList();
            var suppliers = DAL.dbSuppliers.Where(x => x.IsCreditor == false && x.Id > 0).ToList();
            return View(suppliers);
        }

        public ActionResult IndexCreditor(string id)
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            //var parseId = int.Parse(storeId);

            //var indexCreditors = DAL.dbSuppliers.Where(x => x.IsCreditor == true && x.StoreId == parseId).ToList();
            var indexCreditors = DAL.dbSuppliers.Where(x => x.IsCreditor == true && x.StoreId == 1).ToList();
            return View();
        }
        [Authorize(Roles = "Admin,Technical Manager")]
        // GET: Suppliers/Create
        public ActionResult Create()
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;
            return View();
        }
        public ActionResult CreateCreditor()
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,Balance,IsCreditor")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                if (supplier.Balance == null)
                {
                    supplier.Balance = 0;
                }
                supplier.IsCreditor = false;
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(supplier);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCreditor([Bind(Include = "Id,Name,Address,Balance,IsCreditor,Type")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                if (supplier.Balance == null)
                {
                    supplier.Balance = 0;
                }
                supplier.IsCreditor = true;
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("IndexCreditor");
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }
        public ActionResult EditCreditor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            //ViewBag.FundingSources=new  SelectList(db.Suppliers.Where(x=>x.IsCreditor==true),"Id","Name");
            List<SelectListItem> myCreditorTypeOptionList = new List<SelectListItem>()
              {
                  new SelectListItem{Text = "Self", Value = "0"},
                  new SelectListItem{Text = "Loan", Value = "1"}
              };
            ViewBag.myCreditorTypeOptionList = myCreditorTypeOptionList;




            return View(supplier);
        }
        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Balance,IsCreditor")] Supplier supplier)
        {
            supplier.IsCreditor = false;
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreditor([Bind(Include = "Id,Name,Address,Balance,IsCreditor,Type")] Supplier supplier)
        {
            supplier.IsCreditor = true;
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexCreditor");
            }
            return View(supplier);
        }
        // GET: Products/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }
        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Supplier supplier = db.Suppliers.Find(id);
            bool isPresent = false;
            if (db.POes.FirstOrDefault(x => x.SupplierId == id) != null)
            {
                isPresent = true;
            }

            if (isPresent == false)
            {
                db.Suppliers.Remove(supplier);
            }
            else
            {
                supplier.Status = "D";
                db.Entry(supplier).Property(x => x.Status).IsModified = true;

            }
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
