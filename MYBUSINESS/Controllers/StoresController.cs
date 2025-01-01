using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class StoresController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        // GET: Customers
        public StoresController()
        {

        }

        public ActionResult Index(string id)
        {
            return View(DAL.dbStore);
        }
        public ActionResult GetStoreVndBalance(string id)
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
            //var getDailyBalance = DAL.dbVndBalance.Where(x => x.StoreId == parseId).ToList();
            var getDailyBalance = DAL.dbVndBalance.Where(x => x.StoreId == 1).ToList();

            return View(getDailyBalance);
        }
        // GET: Stores Dashboard
        public ActionResult StoreDashboard(string id)
        {
            return View(DAL.dbStore);
        }

        // GET: Stores/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null) 
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            //int maxId = db.Customers.Max(p => p.Id);
            decimal maxId = db.Stores.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Address,PhoneNumber")] Store store)
        {
            if (ModelState.IsValid)
            {
                //if (store.Balance==null)
                //{
                //    customer.Balance = 0;
                //}
                db.Stores.Add(store);
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

            return View(store);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }
        public ActionResult EditStoreBalance(int id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyBalanceVnd dailyBalanceVnd = db.DailyBalanceVnds.Find(id);
            if (dailyBalanceVnd == null)
            {
                return HttpNotFound();
            }
            return View(dailyBalanceVnd);
        }
        [HttpPost]
        public ActionResult UpdateDailyBalance([Bind(Include = "Id,Name,OpeningBalance,ClosingBalance,OpeningCurrencyDetail,ClosingCurrencyDetail")] DailyBalanceVnd dailyBalanceVnd)
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
            //var getDaiyBalance = db.DailyBalanceVnds.FirstOrDefault(x => x.Id == dailyBalanceVnd.Id && x.StoreId == parseId);
            var getDaiyBalance = db.DailyBalanceVnds.FirstOrDefault(x => x.Id == dailyBalanceVnd.Id && x.StoreId == 1);
            if (getDaiyBalance == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                var dailyBlanceVnd = new DailyBalanceVnd
                {
                    OpeningDate = DateTime.Now,
                    ClosingDate = DateTime.Now,
                    OpeningBalance = dailyBalanceVnd.OpeningBalance,
                    ClosingBalance = dailyBalanceVnd.ClosingBalance,
                    OpeningCurrencyDetail = dailyBalanceVnd.OpeningCurrencyDetail,
                    ClosingCurrencyDetail = dailyBalanceVnd.ClosingCurrencyDetail,
                    //StoreId = parseId
                    StoreId = 1
                };
                //db.DailyBalanceVnds.Add(dailyBlanceVnd);
                db.Entry(dailyBalanceVnd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetStoreVndBalance");
            }
            return View(dailyBalanceVnd);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Description,Address,PhoneNumber")] Store store)
        {
            if (ModelState.IsValid)
            {

                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(store);
        }
        // GET: Products/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }
        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Store store = db.Stores.Find(id);
            bool isPresent = false;
            if (db.Stores.FirstOrDefault(x => x.Id == id) != null)
            {
                isPresent = true;
            }

            if (isPresent == false)
            {
                db.Stores.Remove(store);
            }
            else
            {
                store.Status = "D";
                db.Entry(store).Property(x => x.Status).IsModified = true;

            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult SearchData(string custId, string startDate, string endDate)
        {
            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<SO> selectedSOes = null;

            if (startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);
                //selectedSOes=db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Include(s => s.Customer);
                ViewBag.SOCount = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate).ToList().Count();
                ViewBag.SOAmount = (decimal)(db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Sum(x => x.SaleOrderAmount) ?? 0);
                ViewBag.Profit = (decimal)(db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Sum(x => x.Profit) ?? 0);
                ViewBag.ProductsCount = db.Products.Where(x => x.CreateDate >= dtStartDate && x.CreateDate <= dtEndtDate).Count();
                ViewBag.CustomersCount = db.Customers.Where(x => x.CreateDate >= dtStartDate && x.CreateDate <= dtEndtDate).Count();

            }

            if (selectedSOes != null)
            {
                //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
                return PartialView("_SelectedSOSR", null);
            }
            else
            {
                //return PartialView("_SelectedSOSR", new List<SO>());
                return PartialView("_SelectedSOSR", null);
            }
            //return View("Some thing went wrong");

        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult OpenShop(StoreViewModel storeDto)
        {
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            //var parseId = int.Parse(storeId);
            if (storeDto.Id == 0)
            {
                //if (storeDto.OpeningBalance==0)
                //    storeDto.OpeningBalance = 0;
                var store = new DailyBalanceVnd
                {
                    OpeningDate = DateTime.UtcNow,
                    OpeningBalance = storeDto.OpeningBalance,
                    OpeningCurrencyDetail = storeDto.OpeningCurrencyDetail,
                    //StoreId = parseId //commented due to session issue
                    StoreId = 1
                };
                Session["StoreId"] = storeDto.StoreId;
                db.DailyBalanceVnds.Add(store);
                db.SaveChanges();
            }
            return Json(new { Success = true, Message = "Shop opened successfully" });
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CloseShop(StoreViewModel storeDto)
        {
            if (storeDto == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            //var parseId = int.Parse(storeId);
            var store = new DailyBalanceVnd
            {
                ClosingDate = DateTime.UtcNow,
                ClosingBalance = storeDto.ClosingBalance,
                ClosingCurrencyDetail = storeDto.ClosingCurrencyDetail,
                //StoreId = parseId //commented due to session issue
                StoreId = 1
            };
            db.DailyBalanceVnds.Add(store);
            db.SaveChanges();
            Session["StoreId"] = null;
            return Json(new { Success = true, Message = "Shop closed successfully" });
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
