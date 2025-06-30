using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Web.WebSockets;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MYBUSINESS.Controllers
{
    //[Authorize(Roles = "Admin,Shop")]
    [Authorize]
    public class StoresController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        // GET: Customers
        public StoresController()
        {

        }
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string id)
        {
            return View(DAL.dbStore);
        }
        //public ActionResult Index()
        //{
        //    using (var db = new BusinessContext()) // Ensures proper context handling
        //    {
        //        var storeBalances = (from store in db.Stores
        //                             join balance in db.DailyBalanceVnds on store.Id equals balance.StoreId into storeBalanceGroup
        //                             from balance in storeBalanceGroup.DefaultIfEmpty()
        //                             select new StoreBalanceViewModel
        //                             {
        //                                 StoreId = store.Id,
        //                                 StoreName = store.Name,
        //                                 ClosingBalance = balance != null && balance.ClosingBalance.HasValue ? balance.ClosingBalance.Value : 0m,
        //                                 ClosingDate = balance != null ? balance.ClosingDate : (DateTime?)null,
        //                                 Quantity = balance != null && balance.Quantity.HasValue ? balance.Quantity.Value : 0,
        //                                 VNDQuantity = balance != null && balance.VNDQuantity.HasValue ? balance.VNDQuantity.Value : 0,
        //                                 USDQuantity = balance != null && balance.USDQuantity.HasValue ? balance.USDQuantity.Value : 0,
        //                                 JPYQuantity = balance != null && balance.JPYQuantity.HasValue ? balance.JPYQuantity.Value : 0,
        //                                 CurrencyName = balance != null ? balance.CurrencyName : "N/A"
        //                             }).ToList(); // ✅ Ensures a List<StoreBalanceViewModel>

        //        return View(storeBalances);

        //    }
        //}



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

        [Authorize(Roles = "Admin,Shop")]
        // GET: Stores Dashboard
        public ActionResult StoreDashboard(string id)
        {
            return View(DAL.dbStore);
        }

        public ActionResult ShopDashboard(string id)
        {
            return View(DAL.dbStore);
        }
        //    public List<DailyCashSummaryViewModel> GetCashSummary(DateTime startDate, DateTime endDate)
        //    {
        //        var allStores = db.Stores.ToList();
        //        var summaries = new List<DailyCashSummaryViewModel>();

        //        foreach (var store in allStores)
        //        {
        //            var openingBalance = db.DailyBalanceVnds
        //                .Where(d => d.StoreId == store.Id &&
        //                    DbFunctions.TruncateTime(d.OpeningDate) == startDate.Date)
        //                .Select(d => (decimal?)d.OpeningBalance)
        //                .FirstOrDefault() ?? 0;

        //            //var cashSales = db.SOes
        //            //    .Where(s => s.StoreId == store.Id && s.Date >= startDate && s.Date <= endDate && s.IsCancelled != true)
        //            //    .Sum(s => (decimal?)s.BillPaidByCash) ?? 0;
        //            var cashSales = db.SOes
        //.Where(s => s.StoreId == store.Id &&
        //            s.Date >= startDate &&
        //            s.Date <= endDate &&
        //            (s.IsCancelled == null || s.IsCancelled == false))
        //.Sum(s => (decimal?)s.BillPaidByCash) ?? 0;

        //            var moneyInput = db.ShopManages
        //                .Where(t => t.ShoreId == store.Id && t.Date >= startDate && t.Date <= endDate && t.TransactionType == 2)
        //                .Sum(t => (decimal?)t.Balance) ?? 0;

        //            var bankDeposit = db.ShopManages
        //                .Where(t => t.ShoreId == store.Id && t.Date >= startDate && t.Date <= endDate && t.TransactionType == 1)
        //                .Sum(t => (decimal?)t.Balance) ?? 0;

        //            var actualClosing = openingBalance + cashSales + moneyInput - bankDeposit;

        //            var uploadedCreditAmount = db.ScanCreditCards
        //                .Where(r => r.StoreId == store.Id && r.Date >= startDate && r.Date <= endDate)
        //                .Sum(r => (decimal?)r.Amount) ?? 0;

        //            summaries.Add(new DailyCashSummaryViewModel
        //            {
        //                StoreName = store.Name,
        //                OpeningBalance = openingBalance,
        //                CashSales = cashSales,
        //                MoneyInput = moneyInput,
        //                BankDeposit = bankDeposit,
        //                ActualClosingBalance = actualClosing,
        //                UploadedCreditCardAmount = uploadedCreditAmount
        //            });
        //        }

        //        return summaries;
        //    }

        //    public ActionResult DailySummary(DateTime? startDate, DateTime? endDate)
        //    {
        //        // Set default dates: yesterday for start, today for end
        //        if (!startDate.HasValue)
        //        {
        //            startDate = DateTime.Today.AddDays(-1); // Yesterday
        //        }
        //        if (!endDate.HasValue)
        //        {
        //            endDate = DateTime.Today; // Today
        //        }

        //        var model = GetCashSummary(startDate.Value, endDate.Value);

        //        // Format dates as dd/MM/yyyy
        //        ViewBag.StartDate = startDate.Value.ToString("dd/MM/yyyy");
        //        ViewBag.EndDate = endDate.Value.ToString("dd/MM/yyyy");

        //        return View(model);
        //    }


        //public List<DailyCashSummaryViewModel> GetCashSummary(DateTime startDate, DateTime endDate)
        //{
        //    // Ensure endDate includes the entire day
        //    var endDateInclusive = endDate.Date.AddDays(1).AddSeconds(-1);

        //    var allStores = db.Stores.ToList();
        //    var summaries = new List<DailyCashSummaryViewModel>();

        //    foreach (var store in allStores)
        //    {
        //        // 1. Get Opening Balance (properly handles time component)
        //        var openingBalance = db.DailyBalanceVnds
        //            .Where(d => d.StoreId == store.Id &&
        //                   DbFunctions.TruncateTime(d.OpeningDate) == startDate.Date)
        //            .OrderByDescending(d => d.OpeningDate) // Get most recent if multiple
        //            .Select(d => (decimal?)d.OpeningBalance)
        //            .FirstOrDefault() ?? 0;

        //        // 2. Get Cash Sales (with proper NULL handling and date range)
        //        var cashSalesQuery = db.SOes
        //            .Where(s => s.StoreId == store.Id &&
        //                   s.Date >= startDate.Date &&
        //                   s.Date <= endDateInclusive &&
        //                   (s.IsCancelled == null || s.IsCancelled == false));

        //        // Debugging: Check if any records match the criteria
        //        var matchingRecords = cashSalesQuery.ToList();
        //        Debug.WriteLine($"Found {matchingRecords.Count} records for store {store.Id}");

        //        var cashSales = cashSalesQuery.Sum(s => (decimal?)s.BillPaidByCash) ?? 0;

        //        // 3. Other calculations remain the same
        //        var moneyInput = db.ShopManages
        //            .Where(t => t.ShoreId == store.Id &&
        //                   t.Date >= startDate.Date &&
        //                   t.Date <= endDateInclusive &&
        //                   t.TransactionType == 2)
        //            .Sum(t => (decimal?)t.Balance) ?? 0;

        //        var bankDeposit = db.ShopManages
        //            .Where(t => t.ShoreId == store.Id &&
        //                   t.Date >= startDate.Date &&
        //                   t.Date <= endDateInclusive &&
        //                   t.TransactionType == 1)
        //            .Sum(t => (decimal?)t.Balance) ?? 0;

        //        var actualClosing = openingBalance + cashSales + moneyInput - bankDeposit;

        //        var uploadedCreditAmount = db.ScanCreditCards
        //            .Where(r => r.StoreId == store.Id &&
        //                   r.Date >= startDate.Date &&
        //                   r.Date <= endDateInclusive)
        //            .Sum(r => (decimal?)r.Amount) ?? 0;

        //        summaries.Add(new DailyCashSummaryViewModel
        //        {
        //            StoreName = store.Name,
        //            OpeningBalance = openingBalance,
        //            CashSales = cashSales,
        //            MoneyInput = moneyInput,
        //            BankDeposit = bankDeposit,
        //            ActualClosingBalance = actualClosing,
        //            UploadedCreditCardAmount = uploadedCreditAmount
        //        });
        //    }

        //    return summaries;
        //}

        [Authorize(Roles = "Admin,Accountant")]
        public ActionResult DailySummary(DateTime? startDate, DateTime? endDate)
        {
            // Set default dates: yesterday for start, today for end
            startDate = startDate ?? DateTime.Today.AddDays(-1);
            endDate = endDate ?? DateTime.Today;

            // Convert to start of day and end of day
            var startDateAdjusted = startDate.Value.Date;
            var endDateAdjusted = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var model = GetCashSummary(startDateAdjusted, endDateAdjusted);

            // Format dates as dd/MM/yyyy
            ViewBag.StartDate = startDateAdjusted.ToString("dd/MM/yyyy");
            ViewBag.EndDate = endDate.Value.ToString("dd/MM/yyyy"); // Use original endDate without time adjustment

            // Format dates as yyyy-MM-dd for HTML date inputs (required format)
            ViewBag.HtmlStartDate = startDateAdjusted.ToString("yyyy-dd-MM");
            ViewBag.HtmlEndDate = endDate.Value.ToString("yyyy-dd-MM");


            return View(model);
        }
        public List<DailyCashSummaryViewModel> GetCashSummary(DateTime startDate, DateTime endDate)
        {
            // Ensure endDate includes the entire day
            var endDateInclusive = endDate.Date.AddDays(1).AddSeconds(-1);

            var allStores = db.Stores.ToList();
            var summaries = new List<DailyCashSummaryViewModel>();

            foreach (var store in allStores)
            {
                // 1. Get Opening Balance (from record with matching opening date)
                var openingBalanceRecord = db.DailyBalanceVnds
                    .Where(d => d.StoreId == store.Id &&
                           DbFunctions.TruncateTime(d.OpeningDate) == startDate.Date)
                    .OrderByDescending(d => d.OpeningDate)
                    .FirstOrDefault();

                var openingBalance = openingBalanceRecord?.OpeningBalance ?? 0;

                // 2. Get Closing Balance (from record with matching closing date)
                var closingBalanceRecord = db.DailyBalanceVnds
                    .Where(d => d.StoreId == store.Id &&
                           d.ClosingDate.HasValue &&
                           DbFunctions.TruncateTime(d.ClosingDate.Value) == endDate.Date)
                    .OrderByDescending(d => d.ClosingDate)
                    .FirstOrDefault();

                var closingBalance = closingBalanceRecord?.ClosingBalance ?? 0;

                // 3. Get Cash Sales
                var cashSales = db.SOes
                    .Where(s => s.StoreId == store.Id &&
                           s.Date >= startDate.Date &&
                           s.Date <= endDateInclusive &&
                           (s.IsCancelled == null || s.IsCancelled == false))
                    .Sum(s => (decimal?)s.BillPaidByCash) ?? 0;

                // 4. Get Money Input
                var moneyInput = db.ShopManages
                    .Where(t => t.ShoreId == store.Id &&
                           t.Date >= startDate.Date &&
                           t.Date <= endDateInclusive &&
                           t.TransactionType == 2)
                    .Sum(t => (decimal?)t.Balance) ?? 0;

                // 5. Get Bank Deposit
                var bankDeposit = db.ShopManages
                    .Where(t => t.ShoreId == store.Id &&
                           t.Date >= startDate.Date &&
                           t.Date <= endDateInclusive &&
                           t.TransactionType == 1)
                    .Sum(t => (decimal?)t.Balance) ?? 0;

                // 6. Calculate expected closing balance
                var calculatedClosing = openingBalance + cashSales + moneyInput - bankDeposit;

                // 7. Calculate difference (what's missing or extra)
                var difference = closingBalance - calculatedClosing;

                // 8. Get Credit Card Amount
                var uploadedCreditAmount = db.ScanCreditCards
                    .Where(r => r.StoreId == store.Id &&
                           r.Date >= startDate.Date &&
                           r.Date <= endDateInclusive)
                    .Sum(r => (decimal?)r.Amount) ?? 0;

                summaries.Add(new DailyCashSummaryViewModel
                {
                    StoreName = store.Name,
                    OpeningBalance = openingBalance,
                    CashSales = cashSales,
                    MoneyInput = moneyInput,
                    BankDeposit = bankDeposit,
                    ActualClosingBalance = calculatedClosing,
                    RecordedClosingBalance = closingBalance,
                    Difference = difference,
                    UploadedCreditCardAmount = uploadedCreditAmount
                });
            }

            return summaries;
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
        [Authorize(Roles = "Admin")]
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
        public ActionResult Create([Bind(Include = "Id,Name,Description,Address,PhoneNumber,VatNumber,CompanyName,CompanyAddress,CompanyVatNumber,StoreShortCode,StoreShortName,Email,ApiBaseUrl,ApiUsername,ApiPassword")] Store store)
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
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Description,Address,PhoneNumber,,VatNumber,CompanyName,CompanyAddress,CompanyVatNumber,StoreShortCode,StoreShortName,Email,ApiBaseUrl,ApiUsername,ApiPassword")] Store store)
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult OpenShop(StoreViewModel storeDto)
        //{
        //    //var storeId = Session["StoreId"] as string; //commented due to session issue
        //    //if (storeId == null)
        //    //{
        //    //    return RedirectToAction("StoreNotFound", "UserManagement");
        //    //}
        //    //var parseId = int.Parse(storeId);
        //    if (storeDto.Id == 0)
        //    {
        //        //if (storeDto.OpeningBalance==0)
        //        //    storeDto.OpeningBalance = 0;
        //        var store = new DailyBalanceVnd
        //        {
        //            OpeningDate = DateTime.UtcNow,
        //            OpeningBalance = storeDto.OpeningBalance,
        //            OpeningCurrencyDetail = storeDto.OpeningCurrencyDetail,
        //            StoreId = storeDto.StoreId //commented due to session issue
        //            //StoreId = 1
        //        };
        //        Session["StoreId"] = storeDto.StoreId;
        //        db.DailyBalanceVnds.Add(store);
        //        db.SaveChanges();
        //    }
        //    return Json(new { Success = true, Message = "Shop opened successfully" });
        //}
        [HttpPost]
        public ActionResult OpenShop(StoreViewModel storeDto)
        {
            try
            {
                if (storeDto == null)
                if (storeDto == null)
                {
                    return Json(new { Success = false, Message = "Invalid data received." });
                }

                // Ensure StoreId is provided
                if (storeDto.Id == 0)
                {
                    var store = new DailyBalanceVnd
                    {
                        OpeningDate = DateTime.UtcNow,
                        OpeningBalance = storeDto.OpeningBalance,
                        OpeningCurrencyDetail = storeDto.OpeningCurrencyDetail,
                        StoreId = storeDto.StoreId // Ensure this value is correctly assigned
                    };

                    // Store session data if needed
                    Session["StoreId"] = storeDto.StoreId;

                    db.DailyBalanceVnds.Add(store);
                    int recordsAffected = db.SaveChanges(); // Save changes to the database

                    if (recordsAffected > 0)
                    {
                        return Json(new { Success = true, Message = "Shop opened successfully" });
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Failed to save data. No records affected." });
                    }
                }

                return Json(new { Success = false, Message = "Invalid store data." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "An error occurred: " + ex.Message });
            }
        }

        //[HttpPost]
        //public ActionResult BankDeposit(StoreViewModel storeDto)
        //{
        //    try
        //    {
        //        if (storeDto == null)
        //        {
        //            return Json(new { Success = false, Message = "Invalid data received." });
        //        }

        //        // Ensure StoreId is provided
        //        if (storeDto.Id == 0)
        //        {
        //            var store = new DailyBalanceVnd
        //            {
        //                OpeningDate = DateTime.UtcNow,
        //                OpeningBalance = storeDto.OpeningBalance,
        //                OpeningCurrencyDetail = storeDto.OpeningCurrencyDetail,
        //                StoreId = storeDto.StoreId // Ensure this value is correctly assigned
        //            };

        //            // Store session data if needed
        //            Session["StoreId"] = storeDto.StoreId;

        //            db.DailyBalanceVnds.Add(store);
        //            int recordsAffected = db.SaveChanges(); // Save changes to the database

        //            if (recordsAffected > 0)
        //            {
        //                return RedirectToAction("PrintReport", new { storeId = storeDto.StoreId });
        //            }
        //            else
        //            {
        //                return Json(new { Success = false, Message = "Failed to save data. No records affected." });
        //            }
        //        }

        //        return Json(new { Success = false, Message = "Invalid store data." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Success = false, Message = "An error occurred: " + ex.Message });
        //    }
        //}
        //[HttpGet]
        //public ActionResult PrintReport(int storeId)
        //{
        //    try
        //    {
        //        LocalReport localReport = new LocalReport();
        //        string reportPath = Server.MapPath("~/Reports/Sale_ReceiptBankDeposit.rdlc");

        //        //if (!System.IO.File.Exists(reportPath))
        //        //{
        //        //    return HttpNotFound("Report file not found.");
        //        //}

        //        localReport.ReportPath = reportPath;

        //        // Fetch Data for Report
        //        var storeData = db.DailyBalanceVnds.Where(s => s.StoreId == storeId).ToList();

        //        // Create Report Data Source
        //        ReportDataSource reportDataSource = new ReportDataSource("YourDataSetName", storeData);
        //        localReport.DataSources.Clear();
        //        localReport.DataSources.Add(reportDataSource);

        //        // Render Report to PDF
        //        string mimeType, encoding, fileNameExtension;
        //        Warning[] warnings;
        //        string[] streams;
        //        byte[] renderedBytes = localReport.Render("PDF", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

        //        // Return File as Download
        //        return File(renderedBytes, mimeType, "Report.pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content("Error generating report: " + ex.Message);
        //    }
        //}

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult BankDeposit(ShopManagementViewModel shopmanagementDto)
        {
            if (shopmanagementDto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Generate a 10-character ID (since Id is `nchar(10)`)
            string newId = Guid.NewGuid().ToString("N").Substring(0, 10);

            var shopmanagement = new ShopManage
            {
                Id = newId,  // Ensure it matches the nchar(10) type
                Balance = shopmanagementDto.Balance,
                Quantity = shopmanagementDto.Quantity,
                VNDQuantity = shopmanagementDto.VNDQuantity,
                USDQuantity = shopmanagementDto.USDQuantity,
                JPYQuantity = shopmanagementDto.JPYQuantity,  // Fixed incorrect assignment
                CurrencyName = shopmanagementDto.CurrencyName,
                Date = DateTime.UtcNow,
                TransactionType = 1,  // Default transaction type
                ShoreId = 17,  // Default shore ID
             /*   Note = shopmanagementDto.Note ?? "No Note"*/  // Ensure Note is not null
            };

            db.ShopManages.Add(shopmanagement);

            int changes = db.SaveChanges();
            if (changes == 0)
            {
                return Json(new { Success = false, Message = "Database save failed." });
            }

            var bankDepositId = shopmanagement.Id;  // Get the ID of the saved record

            return Json(new { Success = true, bankDepositId = bankDepositId });
        }

        [HttpGet]
        public ActionResult GenerateBankDepositReport(string bankDepositId)  // Change int to string
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Sale_ReceiptBankDeposit.rdlc");

            var bankDepositData = db.Database.SqlQuery<BankDepositReportViewModel>(
                "EXEC spBankDepositReport @BankDepositId = {0}", bankDepositId).ToList();

            if (bankDepositData == null || !bankDepositData.Any())
            {
                throw new Exception("No data found for the selected Bank Deposit ID.");
            }

            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", bankDepositData);
            localReport.DataSources.Add(reportDataSource);

            string mimeType, encoding, fileNameExtension;
            Warning[] warnings;
            string[] streams;
            byte[] renderBytes;

            renderBytes = localReport.Render(
                "PDF", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            return File(renderBytes, mimeType, "BankDepositReport.pdf");
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult MoneyInput(StoreViewModel storeDto)
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
                OpeningBalance = storeDto.OpeningBalance,
                OpeningCurrencyDetail = storeDto.OpeningCurrencyDetail,
                Quantity = storeDto.Quantity,
                VNDQuantity = storeDto.VNDQuantity,
                USDQuantity = storeDto.USDQuantity,
                JPYQuantity = storeDto.USDQuantity,
                CurrencyName = storeDto.CurrencyName,
                OpeningDate = DateTime.UtcNow,
                ClosingDate = DateTime.UtcNow,
                ClosingBalance = storeDto.ClosingBalance,
                ClosingCurrencyDetail = storeDto.ClosingCurrencyDetail,
                //StoreId = parseId //commented due to session issue
                StoreId = 17,
                //StoreId = storeDto.StoreId
            };
            db.DailyBalanceVnds.Add(store);
            db.SaveChanges();
            var bankDepositId = store.Id;  // Get the ID of the bank deposit just saved

            // Call the method to generate the RDLC report
            //return GenerateBankDepositReport(bankDepositId);
            //Session["StoreId"] = null;
            return Json(new { Success = true, bankDepositId = bankDepositId });
        }
        [HttpGet]
        public ActionResult GenerateMoneyInputReport(int bankDepositId)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Sale_ReceiptMoneyInput.rdlc");  // ✅ Correct report name

            // Fetch the bank deposit data using the saved ID
            var bankDepositData = db.Database.SqlQuery<BankDepositReportViewModel>(
                "EXEC spBankDepositReport @BankDepositId = {0}", bankDepositId).ToList();

            // ✅ Check if data exists
            if (bankDepositData == null || !bankDepositData.Any())
            {
                throw new Exception("No data found for the selected Bank Deposit ID.");
            }

            // ✅ Ensure ReportDataSource Name matches RDLC dataset
            //ReportDataSource reportDataSource = new ReportDataSource("BankDepositDataSet", bankDepositData);
            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", bankDepositData); // Update if needed

            localReport.DataSources.Add(reportDataSource);

            // Render the report to PDF
            string mimeType, encoding, fileNameExtension;
            Warning[] warnings;
            string[] streams;
            byte[] renderBytes;

            renderBytes = localReport.Render(
                "PDF", null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            // ✅ Use `File` instead of `Response.AddHeader`
            return File(renderBytes, mimeType, "BankDepositReport.pdf");
        }
        //public FileContentResult GenerateBankDepositReport(int bankDepositId)
        //{
        //    LocalReport localReport = new LocalReport();
        //    localReport.ReportPath = Server.MapPath("~/Reports/BankDepositReport.rdlc");

        //    // Fetch the bank deposit data using the saved ID
        //    var bankDepositData = db.Database.SqlQuery<BankDepositReportViewModel>(
        //        "EXEC spBankDepositReport @BankDepositId = {0}", bankDepositId).ToList();

        //    // Add the data source to the report
        //    ReportDataSource reportDataSource = new ReportDataSource();
        //    reportDataSource.Name = "BankDepositDataSet";  // This should match the dataset name in your RDLC
        //    reportDataSource.Value = bankDepositData;
        //    localReport.DataSources.Add(reportDataSource);

        //    // Render the report to PDF
        //    string reportType = "PDF";
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension = "pdf";
        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderBytes;

        //    renderBytes = localReport.Render(reportType, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

        //    // Return the report as a file download
        //    Response.AddHeader("content-disposition", "attachment; filename=BankDepositReport." + fileNameExtension);
        //    return new FileContentResult(renderBytes, mimeType);
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult CloseShop(StoreViewModel storeDto)
        //{
        //    if (storeDto == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
        //    //var storeId = Session["StoreId"] as string; //commented due to session issue
        //    //if (storeId == null)
        //    //{
        //    //    return RedirectToAction("StoreNotFound", "UserManagement");
        //    //}
        //    //var parseId = int.Parse(storeId);
        //    var store = new DailyBalanceVnd
        //    {
        //        ClosingDate = DateTime.UtcNow,
        //        ClosingBalance = storeDto.ClosingBalance,
        //        ClosingCurrencyDetail = storeDto.ClosingCurrencyDetail,
        //        //StoreId = parseId //commented due to session issue
        //        //StoreId = 1
        //        StoreId = storeDto.StoreId
        //    };
        //    db.DailyBalanceVnds.Add(store);
        //    db.SaveChanges();
        //    Session["StoreId"] = null;
        //    return Json(new { Success = true, Message = "Shop closed successfully" });
        //}
        [HttpPost]
        public ActionResult CloseShop(StoreViewModel storeDto)
        {
            try
            {
                if (storeDto == null)
                {
                    return Json(new { Success = false, Message = "Invalid data received." });
                }

                // Initialize storeId with the value from DTO
                int storeId = storeDto.StoreId;

                if (storeId <= 0)
                {
                    // Fallback 1: Try from session
                    var sessionStoreId = Session["StoreId"] as int?;
                    if (sessionStoreId.HasValue && sessionStoreId > 0)
                    {
                        storeId = sessionStoreId.Value;
                    }
                    else
                    {
                        // Fallback 2: Try from most recent opening record
                        var lastOpening = db.DailyBalanceVnds
                            .Where(d => d.ClosingDate == null)
                            .OrderByDescending(d => d.OpeningDate)
                            .FirstOrDefault();

                        if (lastOpening != null)
                        {
                            storeId = lastOpening.StoreId; // No conversion needed now
                        }
                    }
                }

                if (storeId <= 0)
                {
                    return Json(new { Success = false, Message = "Could not determine store to close." });
                }

                // Rest of your method remains the same...
                var openingRecord = db.DailyBalanceVnds
                    .Where(d => d.StoreId == storeId && d.ClosingDate == null)
                    .OrderByDescending(d => d.OpeningDate)
                    .FirstOrDefault();

                if (openingRecord == null)
                {
                    return Json(new { Success = false, Message = "No open shop record found to close." });
                }

                openingRecord.ClosingDate = DateTime.UtcNow;
                openingRecord.ClosingBalance = storeDto.ClosingBalance;
                openingRecord.ClosingCurrencyDetail = storeDto.ClosingCurrencyDetail;

                db.SaveChanges();
                Session["StoreId"] = null;

                return Json(new { Success = true, Message = "Shop closed successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "An error occurred: " + ex.Message });
            }
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
