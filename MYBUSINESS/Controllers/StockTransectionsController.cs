using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MYBUSINESS.Controllers
{
    public class StockTransectionsController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();

        //[NoCache]
        //public ActionResult Summary()
        //{
        //    //var storeId = Session["StoreId"] as string; //commented due to session issue
        //    //if (storeId == null)
        //    //{
        //    //    return RedirectToAction("StoreNotFound", "UserManagement");
        //    //}
        //    //var parseId = int.Parse(storeId);

        //    DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
        //    var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
        //    var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);


        //    ViewBag.Customers = DAL.dbCustomers;


        //    ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
        //    ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");


        //    DashboardViewModel dbVM = new DashboardViewModel();

        //    List<SOD> FilteredSaleOrderDetails;// = db.Customers;
        //    List<POD> FilteredPurchaseOrderDetails;// = db.Customers;
        //    List<Product> FilteredProducts = new List<Product>();
        //    foreach (Product prod in DAL.dbProducts)
        //    {

        //        FilteredSaleOrderDetails = new List<SOD>();

        //        foreach (SOD sod in prod.SODs)
        //        {
        //            if (sod.SO.Date >= dtStartDate && sod.SO.Date <= dtEndtDate)
        //            {
        //                FilteredSaleOrderDetails.Add(sod);
        //            }
        //        }

        //        //if (FilteredSaleOrderDetails.Count > 0)
        //        //{
        //        //prod.SODs = FilteredSaleOrderDetails;


        //        //}



        //        /////////////////////////////

        //        FilteredPurchaseOrderDetails = new List<POD>();

        //        foreach (POD pod in prod.PODs)
        //        {
        //            if (pod.PO.Date >= dtStartDate && pod.PO.Date <= dtEndtDate)
        //            {
        //                FilteredPurchaseOrderDetails.Add(pod);
        //            }
        //        }

        //        //if (FilteredPurchaseOrderDetails.Count > 0)
        //        //{
        //        //prod.PODs = FilteredPurchaseOrderDetails;


        //        //}



        //        if (FilteredSaleOrderDetails.Count > 0 || FilteredPurchaseOrderDetails.Count > 0)
        //        {
        //            prod.SODs = FilteredSaleOrderDetails;
        //            prod.PODs = FilteredPurchaseOrderDetails;
        //            FilteredProducts.Add(prod);

        //        }



        //    }

        //    //dbVM.Products = FilteredProducts.Where(x=>x.StoreId== parseId).OrderBy(x => x.Name).AsQueryable(); //commented due to session issue
        //    dbVM.Products = FilteredProducts.Where(x=>x.StoreId== 1).OrderBy(x => x.Name).AsQueryable();



        //    ////////////////////////


        //    //dbVM.SOes = sOes;
        //    //dbVM.POes = db.POes;

        //    return View("Summary", dbVM);
        //    //return View("Dashboard", sOes);

        //}
        //public ActionResult FilterIndex(string custId, string suppId, string startDate, string endDate)
        //{

        //    DateTime dtStartDate;
        //    DateTime dtEndtDate;

        //    if (startDate == string.Empty)
        //    {
        //        dtStartDate = DateTime.Parse("1-1-1800");
        //    }
        //    else
        //    {
        //        dtStartDate = DateTime.Parse(startDate);
        //    }

        //    if (endDate == string.Empty)
        //    {
        //        dtEndtDate = DateTime.Today.AddDays(1);
        //    }
        //    else
        //    {
        //        dtEndtDate = DateTime.Parse(endDate);
        //        dtEndtDate = dtEndtDate.AddDays(1);
        //    }

        //    ViewBag.Customers = DAL.dbCustomers;

        //    ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
        //    ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

        //    DashboardViewModel dbVM = new DashboardViewModel();



        //    List<SOD> FilteredSaleOrderDetails;// = db.Customers;
        //    List<POD> FilteredPurchaseOrderDetails;// = db.Customers;
        //    List<Product> FilteredProducts = new List<Product>();
        //    foreach (Product prod in DAL.dbProducts)
        //    {

        //        FilteredSaleOrderDetails = new List<SOD>();

        //        foreach (SOD sod in prod.SODs)
        //        {
        //            if (sod.SO.Date >= dtStartDate && sod.SO.Date <= dtEndtDate)
        //            {
        //                FilteredSaleOrderDetails.Add(sod);
        //            }
        //        }

        //        //if (FilteredSaleOrderDetails.Count > 0)
        //        //{
        //        //prod.SODs = FilteredSaleOrderDetails;


        //        //}



        //        /////////////////////////////

        //        FilteredPurchaseOrderDetails = new List<POD>();

        //        foreach (POD pod in prod.PODs)
        //        {
        //            if (pod.PO.Date >= dtStartDate && pod.PO.Date <= dtEndtDate)
        //            {
        //                FilteredPurchaseOrderDetails.Add(pod);
        //            }
        //        }

        //        //if (FilteredPurchaseOrderDetails.Count > 0)
        //        //{
        //        //prod.PODs = FilteredPurchaseOrderDetails;


        //        //}




        //        if (FilteredSaleOrderDetails.Count > 0 || FilteredPurchaseOrderDetails.Count > 0)
        //        {
        //            prod.SODs = FilteredSaleOrderDetails;
        //            prod.PODs = FilteredPurchaseOrderDetails;
        //            FilteredProducts.Add(prod);

        //        }



        //    }

        //    dbVM.Products = FilteredProducts.OrderBy(x => x.Name).AsQueryable();





        //    return PartialView("_Summary", dbVM);
        //    //return View("Some thing went wrong");
        //}

        public ActionResult Summary(int? storeId = null)
        {
            // Get all stores for the dropdown
            ViewBag.Stores = db.Stores.ToList();

            // Set default store if none selected (0 = Factory)
            storeId = storeId ?? 0; // Default to Factory if none selected

            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            ViewBag.Customers = DAL.dbCustomers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
            ViewBag.CurrentStoreId = storeId;

            DashboardViewModel dbVM = new DashboardViewModel();
            List<Product> FilteredProducts = GetFilteredProducts(dtStartDate, dtEndtDate, storeId);

            dbVM.Products = FilteredProducts.OrderBy(x => x.Name).AsQueryable();
            return View("Summary", dbVM);
        }

        public ActionResult FilterIndex(string custId, string suppId, string startDate, string endDate, int? storeId = null)
        {
            // Set default store if none selected (0 = Factory)
            storeId = storeId ?? 0;

            DateTime dtStartDate = string.IsNullOrEmpty(startDate) ? DateTime.Parse("1-1-1800") : DateTime.Parse(startDate);
            DateTime dtEndtDate = string.IsNullOrEmpty(endDate) ? DateTime.Today.AddDays(1) : DateTime.Parse(endDate).AddDays(1);

            ViewBag.Customers = DAL.dbCustomers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
            ViewBag.CurrentStoreId = storeId;

            DashboardViewModel dbVM = new DashboardViewModel();
            List<Product> FilteredProducts = GetFilteredProducts(dtStartDate, dtEndtDate, storeId);

            dbVM.Products = FilteredProducts.OrderBy(x => x.Name).AsQueryable();
            return PartialView("_Summary", dbVM);
        }

        private List<Product> GetFilteredProducts(DateTime startDate, DateTime endDate, int? storeId)
        {
            List<Product> FilteredProducts = new List<Product>();

            // If storeId is 0 (Factory), show all products from all stores
            var productsToFilter = (storeId == 0)
                ? DAL.dbProducts
                : DAL.dbProducts.Where(x => x.StoreId == storeId);

            foreach (Product prod in productsToFilter)
            {
                var FilteredSaleOrderDetails = prod.SODs
                    .Where(sod => sod.SO.Date >= startDate && sod.SO.Date <= endDate)
                    .ToList();

                var FilteredPurchaseOrderDetails = prod.PODs
                    .Where(pod => pod.PO.Date >= startDate && pod.PO.Date <= endDate)
                    .ToList();

                if (FilteredSaleOrderDetails.Count > 0 || FilteredPurchaseOrderDetails.Count > 0)
                {
                    prod.SODs = FilteredSaleOrderDetails;
                    prod.PODs = FilteredPurchaseOrderDetails;
                    FilteredProducts.Add(prod);
                }
            }

            return FilteredProducts;
        }
    }
}