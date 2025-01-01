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
    public class CustomersController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        // GET: Customers
        public CustomersController()
        {

        }

        public ActionResult Index(string id)
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            //var storeId = Session["StoreId"] as string;
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            //var parseId = int.Parse(storeId);
            var customers = DAL.dbCustomers.Where(x => x.StoreId == storeId);
            return View(customers);

            //var customers = DAL.dbCustomers;
            //return View(customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            //var storeId = Session["StoreId"] as string;
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}

            //int maxId = db.Customers.Max(p => p.Id);
            decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,Balance,CompanyName,CompanyAddress,CustomerCode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                int? storeId = Session["StoreId"] as int?;
                //var storeId = Session["StoreId"] as string;
                if (storeId == null)
                {
                    return RedirectToAction("StoreNotFound", "UserManagement");
                }
                //var storeId = Session["StoreId"] as string;
                //if (storeId == null)
                //{
                //    return RedirectToAction("StoreNotFound", "UserManagement");
                //}
                //var parseId = int.Parse(storeId);
                //customer.StoreId = parseId;
                customer.StoreId = storeId;

                if (customer.Balance == null)
                {
                    customer.Balance = 0;
                }
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //if ((TempData["Controller"]).ToString() == "SOSR" && (TempData["Action"]).ToString() == "Create")
            //{
            //    return RedirectToAction("Create", "SOSR");

            //}
            //else
            //{
            //    return View(customer);
            //}

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(decimal id)
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Balance,CompanyName,CompanyAddress,CustomerCode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                int? storeId = Session["StoreId"] as int?;
                //var storeId = Session["StoreId"] as string;
                if (storeId == null)
                {
                    return RedirectToAction("StoreNotFound", "UserManagement");
                }
                //var storeId = Session["StoreId"] as string;
                //if (storeId == null)
                //{
                //    return RedirectToAction("StoreNotFound", "UserManagement");
                //}
                //var parseId = int.Parse(storeId);
                //customer.StoreId = parseId;
                customer.StoreId = storeId;
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }
        // GET: Products/Delete/5
        public ActionResult Delete(decimal id)
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }
        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            Customer customer = db.Customers.Find(id);
            bool isPresent = false;
            if (db.SOes.FirstOrDefault(x => x.CustomerId == id) != null)
            {
                isPresent = true;
            }

            if (isPresent == false)
            {
                db.Customers.Remove(customer);
            }
            else
            {
                customer.Status = "D";
                db.Entry(customer).Property(x => x.Status).IsModified = true;

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
