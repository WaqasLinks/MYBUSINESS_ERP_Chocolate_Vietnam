using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Microsoft.Reporting.WebForms;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{

    //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    //[NoCache]
    public class DashboardController : Controller
    {
        private BusinessContext db = new BusinessContext();

        // GET: SOes
        public ActionResult Index()
        {


            //EnterProfit();
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            IQueryable<SO> sOes = db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Include(s => s.Customer);
            //sOes = sOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate);
            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            
            

            
            ViewBag.Customers = db.Customers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
            ///////Tiles/////////
            decimal SaleOrderCount = db.SOes.Count();
            ViewBag.SOCount = SaleOrderCount;


            decimal SaleOrderAmount = (decimal)(db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Sum(x => x.SaleOrderAmount) ?? 0);
            ViewBag.SOAmount = SaleOrderAmount;

            //decimal RentalAmount = (decimal)(db.Rents.Sum(x => x.SaleOrderAmount) ?? 0);
            //ViewBag.RentalAmount = RentalAmount;

            //decimal IncomeAmount = (decimal)(db.Services.Sum(x => x.SaleOrderAmount) ?? 0);
            //ViewBag.IncomeAmount = IncomeAmount;

            //decimal PurchaseOrderCount = db.POes.Count();
            //ViewBag.POCount = PurchaseOrderCount;

            //decimal PurchaseOrderAmount = (decimal)(db.POes.Sum(x => x.PurchaseOrderAmount) ?? 0);
            //ViewBag.POAmount = PurchaseOrderAmount;

            //decimal LoanAmount = (decimal)(db.Loans.Sum(x => x.PurchaseOrderAmount) ?? 0);
            //ViewBag.LoanAmount = LoanAmount;

            //decimal ExpenseAmount = (decimal)(db.Expenses.Sum(x => x.PurchaseOrderAmount) ?? 0);
            //ViewBag.ExpenseAmount = ExpenseAmount;

            decimal Profit = (decimal)(db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Sum(x => x.Profit) ?? 0);
            //ViewBag.Profit = (decimal)(SaleOrderCount - PurchaseOrderCount);

            ViewBag.Profit = Profit;
            ViewBag.ProductsCount = db.Products.Where(x => x.CreateDate >= dtStartDate && x.CreateDate <= dtEndtDate).Count();

            //ViewBag.SuppliersCount = db.Suppliers.Count();

            ViewBag.CustomersCount = db.Customers.Where(x => x.CreateDate >= dtStartDate && x.CreateDate <= dtEndtDate).Count();

            //ViewBag.EmployeesCount = db.Employees.Count();

            ////////////////

            return View(sOes.OrderByDescending(i => i.Date).ToList());
        }

        //public ActionResult SearchData(string custName, DateTime startDate, DateTime endDate)

        //public ActionResult SearchData(string custName, string startDate, string endDate)
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

        //public ActionResult DashboardStats(DateTime? startDate, DateTime? endDate)
        //{
        //    // Use user-selected dates if provided, otherwise default to last 7 days
        //    var sDate = startDate ?? DateTime.Today.AddDays(-7);
        //    var eDate = endDate ?? DateTime.Today;

        //    var model = new DashboardStatsViewModel
        //    {
        //        StartDate = sDate,
        //        EndDate = eDate,
        //        TotalSalesWithoutVAT = db.SOes
        //            .Where(so => so.Date >= sDate && so.Date <= eDate)
        //            .Sum(so => (decimal?)so.SaleOrderAmount) ?? 0,
        //        TotalSalesWithVAT = db.SOes
        //            .Where(so => so.Date >= sDate && so.Date <= eDate)
        //            .Sum(so => (decimal?)so.SaleOrderAmountWithVaT) ?? 0,
        //        ProductSales = (from sod in db.SODs
        //                        join so in db.SOes on sod.SOId equals so.Id
        //                        join p in db.Products on sod.ProductId equals p.Id
        //                        where so.Date >= sDate && so.Date <= eDate
        //                        group new { sod, p } by new { sod.ProductId, p.Name } into g
        //                        select new ProductSaleInfoViewModel
        //                        {
        //                            ProductName = g.Key.Name,
        //                            TotalQuantity = g.Sum(x => (decimal?)x.sod.Quantity) ?? 0,
        //                            TotalSaleValue = g.Sum(x => (decimal?)(x.sod.Quantity * x.sod.SalePrice)) ?? 0
        //                        }).ToList()
        //    };


        //    return View(model);
        //}

        //public ActionResult DashboardStats(DateTime? startDate, DateTime? endDate, int? storeId)
        //{
        //    // Use user-selected dates if provided, otherwise default to last 7 days
        //    var sDate = startDate ?? DateTime.Today.AddDays(-7);
        //    var eDate = endDate ?? DateTime.Today;

        //    // Fetch stores for the dropdown
        //    ViewBag.Stores = new SelectList(db.Stores, "Id", "Name"); // Assuming your stores have 'Id' and 'Name'

        //    var model = new DashboardStatsViewModel
        //    {
        //        StartDate = sDate,
        //        EndDate = eDate,
        //        StoreId = storeId, // Add this property to your ViewModel to hold the selected store ID
        //        TotalSalesWithoutVAT = db.SOes
        //            .Where(so => so.Date >= sDate && so.Date <= eDate &&
        //                        (storeId == null || so.StoreId == storeId)) // Filter by store if selected
        //            .Sum(so => (decimal?)so.SaleOrderAmount) ?? 0,
        //        TotalSalesWithVAT = db.SOes
        //            .Where(so => so.Date >= sDate && so.Date <= eDate &&
        //                        (storeId == null || so.StoreId == storeId)) // Filter by store if selected
        //            .Sum(so => (decimal?)so.SaleOrderAmountWithVaT) ?? 0,
        //        ProductSales = (from sod in db.SODs
        //                        join so in db.SOes on sod.SOId equals so.Id
        //                        join p in db.Products on sod.ProductId equals p.Id
        //                        where so.Date >= sDate && so.Date <= eDate &&
        //                              (storeId == null || so.StoreId == storeId) // Filter by store if selected
        //                        group new { sod, p } by new { sod.ProductId, p.Name } into g
        //                        select new ProductSaleInfoViewModel
        //                        {
        //                            ProductName = g.Key.Name,
        //                            TotalQuantity = g.Sum(x => (decimal?)x.sod.Quantity) ?? 0,
        //                            TotalSaleValue = g.Sum(x => (decimal?)(x.sod.Quantity * x.sod.SalePrice)) ?? 0
        //                        }).ToList()
        //    };

        //    return View(model);
        //}
        public ActionResult DashboardStats(DateTime? startDate, DateTime? endDate, int? storeId)
        {
            // Set default dates to today if not provided
            var sDate = startDate ?? DateTime.Today;
            var eDate = endDate ?? DateTime.Today;

            // Fetch stores for the dropdown
            ViewBag.Stores = new SelectList(db.Stores, "Id", "Name");

            var model = new DashboardStatsViewModel
            {
                StartDate = sDate,
                EndDate = eDate,
                StoreId = storeId,
                TotalSalesWithoutVAT = db.SOes
                    .Where(so => so.Date >= sDate && so.Date <= eDate &&
                                (storeId == null || so.StoreId == storeId))
                    .Sum(so => (decimal?)so.SaleOrderAmount) ?? 0,
                TotalSalesWithVAT = db.SOes
                    .Where(so => so.Date >= sDate && so.Date <= eDate &&
                                (storeId == null || so.StoreId == storeId))
                    .Sum(so => (decimal?)so.SaleOrderAmountWithVaT) ?? 0,
                ProductSales = (from sod in db.SODs
                                join so in db.SOes on sod.SOId equals so.Id
                                join p in db.Products on sod.ProductId equals p.Id
                                where so.Date >= sDate && so.Date <= eDate &&
                                      (storeId == null || so.StoreId == storeId)
                                group new { sod, p } by new { sod.ProductId, p.Name } into g
                                select new ProductSaleInfoViewModel
                                {
                                    ProductName = g.Key.Name,
                                    TotalQuantity = g.Sum(x => (decimal?)x.sod.Quantity) ?? 0,
                                    TotalSaleValue = g.Sum(x => (decimal?)(x.sod.Quantity * x.sod.SalePrice)) ?? 0
                                }).ToList()
            };

            return View(model);
        }


    }

}
