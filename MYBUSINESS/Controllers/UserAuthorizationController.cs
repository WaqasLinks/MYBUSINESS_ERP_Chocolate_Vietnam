using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class UserAuthorizationController : Controller
    {
        private BusinessContext db = new BusinessContext();

        public ActionResult UnAuthorized()
        {

            return View();
        }
        // GET: Customers
        public ActionResult Index(string id)
        {

            return View(db.UserAuthorizations.Where(x=>x.Show==true).ToList());
        }
        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id,Description,Authorize")] List<UserAuthorization> NewListUserAuthorization)
        {
            
            if (ModelState.IsValid)
            {
                foreach (UserAuthorization newUserAuthorization in NewListUserAuthorization)
                {
                    //usrAuth.IsActive = IsActive(de);
                    var usrAuth = db.UserAuthorizations.FirstOrDefault(x => x.Id == newUserAuthorization.Id);
                    if (usrAuth !=null)
                    {
                        usrAuth.Authorize = newUserAuthorization.Authorize;
                        db.Entry(usrAuth).Property(x => x.Authorize).IsModified = true;

                        //   db.Entry(usrAuth).State = EntityState.Modified;
                           
                    }
                    
                }
                db.SaveChanges();
                //TempData["msg"] = "<script>alert('Change succesfully');</script>";
                return RedirectToAction("Index");
            }
            return View();
            
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
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
        public ActionResult Create([Bind(Include = "Id,Name,Address,Balance")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer.Balance==null)
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
        public ActionResult Edit([Bind(Include = "Id,UrlPath,Authorized")] UserAuthorization userAuthorization)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(userAuthorization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userAuthorization);
        }

        //// GET: Customers/Delete/5
        //public ActionResult Delete(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Customer customer = db.Customers.Find(id);
        //    if (customer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(customer);
        //}

        //// POST: Customers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(decimal id)
        //{
        //    Customer customer = db.Customers.Find(id);
        //    db.Customers.Remove(customer);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
