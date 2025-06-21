using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using Microsoft.Reporting.WebForms;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using log4net;
using MYBUSINESS.CustomClasses.MetaDataClasses;
using Microsoft.AspNetCore.Mvc;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Core.Metadata.Edm;
using Microsoft.AspNet.SignalR;
using MYBUSINESS.HubConnection;
using AuthAttribute = System.Web.Mvc.AuthorizeAttribute;
namespace MYBUSINESS.Controllers
{

    //[AuthAttribute(Roles = "Admin,Manager,User")]
    public class SOSRController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        //public dynamic _jsonResponseWebservice = null;
        //private static readonly ILog log = LogManager.GetLogger(typeof(SOSRController));
        // GET: SOes
        public ActionResult Index()
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
            //EnterProfit();

            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            var stores = db.Stores.ToList();
            ViewBag.Stores = new SelectList(stores, "Id", "StoreShortName", storeId);

            IQueryable<SO> sOes = db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate && x.SaleReturn == false).Include(s => s.Customer);
            //sOes = sOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate);
            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref sOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (SO itm in sOes)
            {
                //thisSerial = (int)itm?.Customer.SOes.Max(x => x?.SOSerial);
                thisSerial = (itm?.Customer?.SOes != null && itm.Customer.SOes.Any())
                  ? (int)(itm.Customer.SOes.Max(x => (decimal?)x?.SOSerial ?? 0))
                  : 0;

                //if (!LstMaxSerialNo.ContainsKey((int)itm.CustomerId))
                //{
                //    LstMaxSerialNo.Add(itm.Customer.Id, thisSerial);
                //}
                if (!LstMaxSerialNo.ContainsKey(itm.CustomerId ?? 0)) // Default to 0 if CustomerId is null
                {
                    LstMaxSerialNo.Add(itm.Customer?.Id ?? 0, thisSerial); // Default to 0 if Customer.Id is null
                }


                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                //itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            ViewBag.Customers = DAL.dbCustomers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
            var getSoes = sOes.Where(x => x.StoreId == storeId).OrderByDescending(i => i.Date).ToList();//commented due to session issue
            //var getSoes = sOes.Where(x => x.StoreId == parseId).OrderByDescending(i => i.Date).ToList();//commented due to session issue
            //var getSoes = sOes.OrderByDescending(i => i.Date).ToList();
            return View(getSoes);
        }

        public ActionResult FilterSales(int? storeId, string startDate, string endDate, string customerName)
        {
            DateTime dtStartDate, dtEndtDate;

            // Parse dates or use defaults
            if (!DateTime.TryParse(startDate, out dtStartDate))
            {
                dtStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            if (!DateTime.TryParse(endDate, out dtEndtDate))
            {
                dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);
            }

            IQueryable<SO> sOes = db.SOes
                .Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate && x.SaleReturn == false)
                .Include(s => s.Customer);

            // Apply store filter if selected
            if (storeId.HasValue && storeId > 0)
            {
                sOes = sOes.Where(x => x.StoreId == storeId);
            }

            // Apply customer name filter if provided
            if (!string.IsNullOrEmpty(customerName))
            {
                sOes = sOes.Where(x => x.Customer.Name.Contains(customerName));
            }

            GetTotalBalance(ref sOes);
            var getSoes = sOes.OrderByDescending(i => i.Date).ToList();

            return PartialView("_SelectedSOSR", getSoes);
        }

        public ActionResult ClosePosPopup()
        {
            int? storeId = Session["StoreId"] as int?;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            return PartialView("_StoreClosePopup");
        }
        public ActionResult OpenPosPopup()
        {
            int? storeId = Session["StoreId"] as int?;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            return PartialView("_StoreClosePopup");
        }
        public ActionResult IndexReturn()
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            //EnterProfit();
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            //var parseId = int.Parse(storeId);
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            IQueryable<SO> sOes = db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate && x.SaleReturn == true).Include(s => s.Customer);
            //sOes = sOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate);
            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            //GetTotalBalance(ref sOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (SO itm in sOes)
            {
                thisSerial = (int)itm.Customer.SOes.Max(x => x.SOSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.CustomerId))
                {
                    LstMaxSerialNo.Add(itm.Customer.Id, thisSerial);
                }


                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            ViewBag.Customers = DAL.dbCustomers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
            var indexReturn = sOes.Where(x => x.StoreId == storeId).OrderByDescending(i => i.Date).ToList(); //commented due to session issue
            //var indexReturn = sOes.Where(x => x.StoreId == parseId).OrderByDescending(i => i.Date).ToList(); //commented due to session issue
            //var indexReturn = sOes.OrderByDescending(i => i.Date).ToList(); //commented due to session issue
            return View(indexReturn);
        }

        //[HttpPost]
        //public ActionResult CancelBill(string soId)
        //{
        //    // 1. Get SO (Sale Order)
        //    var so = db.SOes.FirstOrDefault(x => x.Id == soId);

        //    // Ensure SO exists and is not cancelled
        //    if (so == null || (so.IsCancelled != null && so.IsCancelled == true))
        //    {
        //        return Content("Bill not found or already cancelled.");
        //    }

        //    // 2. Get related Sale Order Details
        //    var sodItems = db.SODs.Where(x => x.SOId == soId).ToList();

        //    // 3. Update product stock
        //    foreach (var item in sodItems)
        //    {
        //        var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
        //        if (product != null)
        //        {
        //            product.Stock += item.Quantity;
        //        }
        //    }

        //    // 4. Update DailyBalanceVnd
        //    var billDate = so.Date ?? DateTime.Now;

        //    int storeIdInt = Convert.ToInt32(so.StoreId);

        //    var balanceRecord = db.DailyBalanceVnds
        //        .FirstOrDefault(b => b.StoreId == storeIdInt
        //            && b.OpeningDate.HasValue
        //            && b.OpeningDate.Value.Year == billDate.Year
        //            && b.OpeningDate.Value.Month == billDate.Month
        //            && b.OpeningDate.Value.Day == billDate.Day);


        //    if (balanceRecord != null)
        //    {
        //        decimal totalBillAmount = (decimal)sodItems.Sum(i => i.Quantity * i.SalePrice);
        //        balanceRecord.ClosingBalance -= totalBillAmount;
        //    }

        //    // 5. Mark SO as cancelled
        //    so.IsCancelled = true;

        //    db.SaveChanges();

        //    return Content("Bill Cancelled Successfully");
        //}
        [HttpPost]
        public ActionResult CancelBill(string soId)
        {
            try
            {
                // 1. Get SO (Sale Order)
                var so = db.SOes.FirstOrDefault(x => x.Id == soId);

                // Ensure SO exists and is not cancelled
                if (so == null || (so.IsCancelled != null && so.IsCancelled == true))
                {
                    TempData["ErrorMessage"] = "Bill not found or already cancelled.";
                    return RedirectToAction("Index");
                }

                // 2. Get related Sale Order Details
                var sodItems = db.SODs.Where(x => x.SOId == soId).ToList();

                // 3. Update product stock
                foreach (var item in sodItems)
                {
                    var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                    }
                }

                // 4. Update DailyBalanceVnd
                var billDate = so.Date ?? DateTime.Now;
                int storeIdInt = Convert.ToInt32(so.StoreId);

                var balanceRecord = db.DailyBalanceVnds
                    .FirstOrDefault(b => b.StoreId == storeIdInt
                        && b.OpeningDate.HasValue
                        && b.OpeningDate.Value.Year == billDate.Year
                        && b.OpeningDate.Value.Month == billDate.Month
                        && b.OpeningDate.Value.Day == billDate.Day);

                if (balanceRecord != null)
                {
                    decimal totalBillAmount = (decimal)sodItems.Sum(i => i.Quantity * i.SalePrice);
                    balanceRecord.ClosingBalance -= totalBillAmount;
                }

                // 5. Mark SO as cancelled
                so.IsCancelled = true;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Bill cancelled successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error cancelling bill: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
        //public ActionResult CancelBill(string soId)
        //{
        //    string decodedString = DecryptAsciiEncodedId(soId);

        //    var so = db.SOes.FirstOrDefault(x => x.Id == decodedString);

        //    if (so == null || (so.IsCancelled ?? false))
        //    {
        //        return Content("Bill not found or already cancelled.");
        //    }

        //    var sodItems = db.SODs.Where(x => x.SOId == decodedString).ToList();

        //    foreach (var item in sodItems)
        //    {
        //        var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
        //        if (product != null)
        //        {
        //            product.Stock += item.Quantity;
        //        }
        //    }

        //    var billDate = so.Date ?? DateTime.Now;
        //    int storeIdInt = Convert.ToInt32(so.StoreId);

        //    var balanceRecord = db.DailyBalanceVnds
        //        .FirstOrDefault(b => b.StoreId == storeIdInt
        //            && b.OpeningDate.HasValue
        //            && b.OpeningDate.Value.Date == billDate.Date);

        //    if (balanceRecord != null)
        //    {
        //        decimal totalBillAmount = (decimal)sodItems.Sum(i => i.Quantity * (i.SalePrice ?? 0));
        //        balanceRecord.ClosingBalance -= totalBillAmount;
        //    }

        //    so.IsCancelled = true;
        //    db.SaveChanges();

        //    return Content("Bill Cancelled Successfully");
        //}

        private string DecryptAsciiEncodedId(string encoded)
        {
            var byteStrings = encoded.Split('-');
            byte[] bytes = byteStrings.Select(b => Convert.ToByte(b)).ToArray();
            string encryptedText = Encoding.ASCII.GetString(bytes);
            return Encryption.Decrypt(encryptedText, "BZNS");
        }


        public string EncodeToBase64String(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);  // Base64 encode the plain text ID
        }




        public ActionResult ProductRentStatus(string prodId, string sellDate)
        {
            int intProdId;
            DateTime dtSellDate;
            DateTime dtEndDate;
            //IQueryable<ProductDetail> selectedSOes = null;
            List<ProductDetail> selectedSOes = null;
            //if (prodId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            //{
            intProdId = Int32.Parse(prodId);
            //dtSellDate = DateTime.Parse(sellDate);
            //dtSellDate = DateTime.ParseExact(sellDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            dtSellDate = DateTime.ParseExact(sellDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            //dtEndDate = DateTime.Parse(endDate);

            selectedSOes = db.ProductDetails.Where(so => so.ProductId == intProdId && (so.RentStartDate >= dtSellDate || so.RentEndDate >= dtSellDate)).ToList();

            //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
            return PartialView("_ProductDetail", selectedSOes);//.ToList());
            //return PartialView("1");
        }
        //public ActionResult SearchData(string custName, DateTime startDate, DateTime endDate)

        //public ActionResult SearchData(string custName, string startDate, string endDate)
        public ActionResult SearchData(string custId, string startDate, string endDate)
        {

            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<SO> selectedSOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (custId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == false);

            }

            if (custId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.SOes;//.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn==false);

            }

            //get all customers data acornding to start end date
            if (custId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == false);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (custId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == false);

            }

            //get this customer with from defined start date to undefined end date
            if (custId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == false);

            }

            //get this customer with all dates
            if (custId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == false);

            }

            //get all customer with defined startdate and undefined end date
            if (custId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == false);

            }

            //get all customers with undifined start date with defined enddate
            if (custId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == false);

            }
            //GetTotalBalance(ref selectedSOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (SO itm in selectedSOes)
            {
                thisSerial = (int)itm.Customer.SOes.Max(x => x.SOSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.CustomerId))
                {
                    LstMaxSerialNo.Add(itm.Customer.Id, thisSerial);
                }
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            return PartialView("_SelectedSOSRReturn", selectedSOes.OrderByDescending(i => i.Date).ToList());

            //return View("Some thing went wrong");


        }
        public ActionResult SearchDataReturn(string custId, string startDate, string endDate)
        {

            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<SO> selectedSOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (custId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == true);

            }

            if (custId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.SOes;//.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn==true);

            }

            //get all customers data acornding to start end date
            if (custId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == true);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (custId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == true);

            }

            //get this customer with from defined start date to undefined end date
            if (custId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == true);

            }

            //get this customer with all dates
            if (custId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.SOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == true);

            }

            //get all customer with defined startdate and undefined end date
            if (custId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == true);

            }

            //get all customers with undifined start date with defined enddate
            if (custId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate && so.SaleReturn == true);

            }
            //GetTotalBalance(ref selectedSOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (SO itm in selectedSOes)
            {
                thisSerial = (int)itm.Customer.SOes.Max(x => x.SOSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.CustomerId))
                {
                    LstMaxSerialNo.Add(itm.Customer.Id, thisSerial);
                }
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            return PartialView("_SelectedSOSRReturn", selectedSOes.OrderByDescending(i => i.Date).ToList());

            //return View("Some thing went wrong");


        }
        public ActionResult CustomerWiseSale(int custId, string custName)
        {

            //DateTime dtEndtDate = DateTime.Today.AddDays(1);
            //DateTime dtStartDate = dtEndtDate.AddDays(-7);
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            ViewBag.CustomerId = custId;
            ViewBag.CustName = custName;
            //ViewBag.SupplierName = supplierName;//db.Products.FirstOrDefault(x => x.Id == productId).Name;
            ViewBag.Customers = DAL.dbCustomers;
            //01-Jan-2019

            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

            IQueryable<SO> sOes = db.SOes;//.Include(s => s.Customer);
            sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.SOSerial).AsQueryable();
            //foreach (SO itm in sOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            //}

            return View("CustomerWiseSale", sOes);

            //return View("CustomerWiseSale", sOes.OrderBy(i => i.Date).ToList());
        }
        public ActionResult FilterCustomerWiseSale(string custId, string suppId, string startDate, string endDate)
        {

            /////////////////////////////////////////////////////////////////////////////
            IQueryable<SO> sOes = db.SOes;//.Include(s => s.Customer);
            //sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.Date).OrderBy(i => i.SOSerial).AsQueryable();






            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<SO> selectedSOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (custId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            if (custId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes;//.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers data acornding to start end date
            if (custId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (custId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from defined start date to undefined end date
            if (custId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with all dates
            if (custId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customer with defined startdate and undefined end date
            if (custId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers with undifined start date with defined enddate
            if (custId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = sOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }


            //foreach (SO itm in selectedSOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            //}
            //GetTotalBalance(ref selectedSOes);
            //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
            //_ProfitGainFromSupplier
            return PartialView("_CustomerWiseSale", selectedSOes.OrderBy(i => i.SOSerial).ToList());
            //return View("Some thing went wrong");
        }
        public ActionResult ProductWiseSale(int productId)
        {
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);
            ViewBag.ProductId = productId;
            ViewBag.ProductName = db.Products.FirstOrDefault(x => x.Id == productId).Name;
            ViewBag.Customers = DAL.dbCustomers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

            List<SO> sOes = db.SOes.ToList();//.Include(s => s.Customer);

            //sOes = db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SOD> lstSODs = db.SODs.Where(x => x.ProductId == productId).ToList();
            List<SO> lstSlectedSO = new List<SO>();
            foreach (SOD lsod in lstSODs)
            {
                //do not add if already added
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.SO);
                }
            }

            sOes = lstSlectedSO.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).ToList();
            foreach (SO itm in sOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            return View("ProductWiseSale", sOes.OrderBy(i => i.SOSerial).ToList());
        }
        public ActionResult FilterProductWiseSale(string prodId, string custId, string suppId, string startDate, string endDate)
        {


            DateTime dtStartDate = DateTime.Today;//just to defer error
            DateTime dtEndtDate = DateTime.Today;//just to defer error


            if (startDate != string.Empty)
            {
                dtStartDate = DateTime.Parse(startDate);
            }

            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                //dtEndtDate = dtEndtDate.AddDays(1);
            }

            if (startDate == string.Empty)
            {
                dtStartDate = DateTime.Parse("1-1-1800");
            }

            if (endDate == string.Empty)
            {
                dtEndtDate = DateTime.Today.AddDays(1);
            }


            //List<SO> sOes = db.SOes.ToList();//.Include(s => s.Customer);
            List<SO> selectedSOes = null;

            selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate).ToList();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////

            int intProdId;
            intProdId = Int32.Parse(prodId);

            //List<SOD> lstSODs = db.SODs.Where(x => x.ProductId == intProdId).ToList();

            List<SO> newSOes = new List<SO>();
            List<SOD> newSODs;
            foreach (SO thisSO in selectedSOes)
            {

                newSODs = new List<SOD>();
                foreach (SOD thisSOD in thisSO.SODs)
                {
                    if (thisSOD.ProductId == intProdId)
                    {
                        newSODs.Add(thisSOD);
                    }

                }
                if (newSODs.Count > 0)
                {
                    thisSO.SODs = newSODs;
                    newSOes.Add(thisSO);
                }


            }

            //foreach (SO itm in selectedSOes)
            //{
            //    //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
            //    itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            //}
            //GetTotalBalance(ref selectedSOes);
            //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
            //_ProfitGainFromSupplier
            return PartialView("_ProductWiseSale", newSOes.OrderBy(i => i.SOSerial).ToList());
            //return View("Some thing went wrong");
        }
        public ActionResult PerMonthSale(int productId)
        {
            IQueryable<SO> sOes = db.SOes.Include(s => s.Customer);

            //sOes = db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SOD> lstSODs = db.SODs.Where(x => x.ProductId == productId && x.SaleType == false).ToList();
            List<SO> lstSlectedSO = new List<SO>();
            foreach (SOD lsod in lstSODs)
            {
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.SO);
                }
            }

            sOes = lstSlectedSO.ToList().AsQueryable();
            foreach (SO itm in sOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.ProductName = db.Products.FirstOrDefault(x => x.Id == productId).Name;
            return View("PerMonthSale", sOes.OrderBy(i => i.Date).ToList());
        }

        public ActionResult SearchProduct(int productId)
        {
            IQueryable<SO> sOes = db.SOes.Include(s => s.Customer);

            //sOes = db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SOD> lstSODs = db.SODs.Where(x => x.ProductId == productId).ToList();
            List<SO> lstSlectedSO = new List<SO>();
            foreach (SOD lsod in lstSODs)
            {
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.SO);
                }


            }

            sOes = lstSlectedSO.ToList().AsQueryable();

            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref sOes);
            foreach (SO itm in sOes)
            {

                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.Customers = DAL.dbCustomers;
            return View("Index", sOes.OrderByDescending(i => i.Date).ToList());
        }

        private void GetTotalBalance(ref IQueryable<SO> SOes)
        {
            //IQueryable<SO> DistSOes = SOes.Select(x => x.CustomerId).Distinct();
            IQueryable<SO> DistSOes = SOes.GroupBy(x => x.CustomerId).Select(y => y.FirstOrDefault());

            decimal TotalBalance = 0;
            foreach (SO itm in DistSOes)
            {
                Customer cust = db.Customers.Where(x => x.Id == itm.CustomerId).FirstOrDefault();

                //TotalBalance += (decimal)cust.Balance;
                TotalBalance += cust?.Balance ?? 0;

            }
            ViewBag.TotalBalance = TotalBalance;

        }
        //[ChildActionOnly]
        //public PartialViewResult _SelectedSOSR()
        //{

        //    return PartialView(db.SOes);
        //}


        // GET: SOes/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SO sO = db.SOes.Find(id);
            if (sO == null)
            {
                return HttpNotFound();
            }
            return View(sO);
        }

        // GET: SOes/Create

        //public ActionResult Create(string IsReturn)
        public async Task<ActionResult> Create(string IsReturn, int? pType = null)
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }

            var productTypes = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "VariableProduct" },
        new SelectListItem { Value = "2", Text = "ExcessProduct" },
        new SelectListItem { Value = "3", Text = "ByProduct" },
        new SelectListItem { Value = "4", Text = "FinishedProduct" },
        new SelectListItem { Value = "5", Text = "IngredientProduct" },
        new SelectListItem { Value = "6", Text = "IntermedataryProduct" },
        new SelectListItem { Value = "7", Text = "Merchendise" }
    };
            ViewBag.ProductTypes = productTypes;
            ViewBag.SelectedPType = pType;

            //ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name");
            //ViewBag.Products = db.Products;

            //int maxId = db.Customers.Max(p => p.Id);
            decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;
            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");

            maxId = db.SOes.DefaultIfEmpty().Max(p => p == null ? 0 : p.SOSerial).Value;
            maxId += 1;
            // Convert maxId to integer for formatting with leading zeros
            //int serialNumber = (int)maxId; //For int
            string prefix = "HN";
            string datePart = DateTime.Now.ToString("yyyyMMdd"); // Format current date as YYYYMMDD
            //string formattedSerial = $"{prefix}-{datePart}-{serialNumber:D3}"; // Format serial with leading zeros for int
            string formattedSerial = $"{prefix}-{datePart}-{maxId:000}"; /* $"{maxId:000}";*/ // Format serial with leading zeros for decimal

            // Set ViewBag.SuggestedNewProductId to the formatted serial number
            ViewBag.SuggestedNewProductIds = formattedSerial;
            ViewBag.SuggestedNewProductId = maxId;
            var productsQuery = db.Products.AsQueryable();
            if (pType != null)
            {
                productsQuery = productsQuery.Where(x => x.PType == pType);
            }
            else
            {
                // Default filter (if you want to keep your original filter)
                productsQuery = productsQuery;
            }
            var productsByCategory = productsQuery
       .Select(p => new
       {
           Product = p,
           Stock = db.StoreProducts
               .Where(sp => sp.ProductId == p.Id && sp.StoreId == storeId)
               .Sum(sp => sp.Stock)
       })
       .ToList();
            var groupedSelectedProducts = productsByCategory
       .GroupBy(p => string.IsNullOrEmpty(p.Product.Category) ? "Uncategorized" : p.Product.Category)
       .ToDictionary(
           g => g.Key,
           g => g.Select(p => new MYBUSINESS.Models.Product
           {
               Id = p.Product.Id,
               Name = p.Product.Name,
               PurchasePrice = p.Product.PurchasePrice,
               SalePrice = p.Product.SalePrice,
               Category = p.Product.Category,
               Stock = p.Stock
           }).ToList()
       );

            // Get distinct categories for dropdown
            var categories = productsQuery
                .Select(p => string.IsNullOrEmpty(p.Category) ? "Uncategorized" : p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.MalaysiaTime = DateTime.UtcNow.AddHours(8);
            SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();
            saleOrderViewModel.Customers = DAL.dbCustomers;
            saleOrderViewModel.Products = DAL.dbProducts; 







            //New Code to get Products by Category
            var productsByyCategory = db.Products
              .Select(p => new
              {
                  Product = p,
                  Stock = db.StoreProducts
                      .Where(sp => sp.ProductId == p.Id)
                      .Sum(sp => sp.Stock) // Sum stock from StoreProduct
              })
                .ToList();

            // Group products by category, handling null categories by assigning "Uncategorized"
            var groupedSelecteddProducts = productsByCategory
                .GroupBy(p => string.IsNullOrEmpty(p.Product.Category) ? "Uncategorized" : p.Product.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(p => new MYBUSINESS.Models.Product // Convert to Product
                    {
                        Id = p.Product.Id,
                        Name = p.Product.Name,
                        PurchasePrice = p.Product.PurchasePrice,
                        SalePrice = p.Product.SalePrice,
                        Category = p.Product.Category,
                        // Include other properties you need
                        Stock = p.Stock // Add Stock as an extra property or handle it separately
                    }).ToList()
                );
            //bool IsReturn1 = true;
            var categoriees = db.Products
        .Select(p => string.IsNullOrEmpty(p.Category) ? "Uncategorized" : p.Category)
        .Distinct()
        .OrderBy(c => c)
        .ToList();

            ViewBag.Categories = new SelectList(categories);
            var stores = db.Stores
       .Select(s => new {
           Id = s.Id,
           ShortName = s.StoreShortName ?? s.Name // Fallback to Name if ShortName is null
       })
       .ToList();

            ViewBag.Stores = new SelectList(stores, "Id", "ShortName", storeId);
            ViewBag.IsReturn = IsReturn;
            //string isReturn1 = "true";
            //ViewBag.isReturn = isReturn1;
            ViewBag.SelectedProductListByCategory = groupedSelectedProducts;
            ViewBag.ReportId = TempData["ReportId"] as string;

            ViewBag.WebserviceDownError = TempData["WebserviceDownError"] as string;
            TempData["_CustomerName"] = TempData["CustomerName"] as string;
            TempData["_CustomerEmail"] = TempData["CustomerEmail"] as string;
            TempData["_CustomerAddress"] = TempData["CustomerAddress"] as string;
            TempData["_CompanyName"] = TempData["CompanyName"] as string;
            TempData["_POSName"] = TempData["POSName"] as string;
            TempData["_POSAddress"] = TempData["POSAddress"] as string;
            TempData["_POSPhoneNumber"] = TempData["POSPhoneNumber"] as string;
            TempData["_CustomerVatNumber"] = TempData["CustomerVatNumber"] as string;
            TempData["_CustomerCompanyAddress"] = TempData["CustomerCompanyAddress"] as string;
            TempData["_InvoiceSeries"] = TempData["InvoiceSeries"] as string;
            //TempData["_InvoiceNumber"] = TempData["InvoiceNumber"] as string;
            TempData["_InvoiceNumber"] = TempData["InvoiceNumber"] as string;

            TempData["_Macqt"] = TempData["Macqt"] as string;
            TempData["_Sobaomat"] = TempData["_Sobaomat"] as string;

            //TempData["_Sobaomat"] = TempData["sbmat"] as string ;

            var store = db.Stores.FirstOrDefault(s => s.Id == storeId);
            ViewBag.StoreName = store?.Name ?? "Unknown Store"; // Fallback if store not found
            //var jsonResponseWebservicess1 = TempData["JsonResponseWebservice"] as string;
            //TempData["_JsonResponseWebservice"] = "TestTempData"; //TempData["JsonResponseWebservice"] as string;
            //string jsonResponseWebservicess2 = TempData["_JsonResponseWebservice"] as string;
            //TempData["_WebserviceDownError"] = TempData["WebserviceDownError"] as string;
            return View(saleOrderViewModel);
        }


        //[OutputCache(NoStore = true, Duration = 0)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Prefix = "Customer", Include = "Name,Address,Email,Vat,CompanyName")] Customer Customer, [Bind(Prefix = "SaleOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,CustomerId,Remarks,Remarks2,PaymentMethod,PaymentDetail,SaleReturn,BankAccountId,Date")] SO sO, [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,SalePrice,PurchasePrice,Quantity,SaleType,PerPack,IsPack,Product")] List<SOD> sOD, FormCollection collection)
        //{
        //public ActionResult Create(
        public async Task<ActionResult> Create(
    [Bind(Prefix = "SaleOrder", Include = "Id,BillAmount,Balance,PrevBalance,BillPaid,BillPaidByCash,Discount,CustomerId,Remarks,Remarks2,PaymentMethod,PaymentDetail,SaleReturn,BankAccountId,Date,SaleOrderAmountWithVaT,SaleOrderAmountWithC")] SO sO,
    [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,SalePrice,PurchasePrice,Quantity,SaleType,PerPack,IsPack,Product.Name,Product")] List<SOD> sOD,
    [Bind(Prefix = "Customer", Include = "Id,Name,Address,Email,Vat,CompanyName")] Customer Customer,
    FormCollection collection
    )
        {
            int? storeId = Session["StoreId"] as int?;
            //var storeId = Session["StoreId"] as string;
            if (storeId == null)
            {
                return RedirectToAction("StoreNotFound", "UserManagement");
            }
            //var parseId = int.Parse(storeId);

            var getStoreName = db.Stores.FirstOrDefault(x => x.Id == storeId);
            var getVatTaxInPercent = db.MyBusinessInfoes.FirstOrDefault().TaxInPercent;
           
            string SOId = string.Empty;
            //SO sO = new SO();
            //if (ModelState.IsValid)
            //{
            //if (string.IsNullOrEmpty(sO.Id)) //Commented By Sohail to add plan text Id in table SO Format(XX-XXXXXXXX-XXX)
            if (!string.IsNullOrEmpty(sO.Id))
            {
                //due to customer from webservice
                //Customer cust = db.Customers.FirstOrDefault(x => x.Id == sO.CustomerId);

                //if (cust == null)
                //{//its means new customer
                //    //sO.CustomerId = 10;
                //    //int maxId = db.Customers.Max(p => p.Id);
                //    //decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id); //due to customer from webservice
                //    //maxId += 1;//due to customer from webservice

                //    //Customer.Id = maxId;//due to customer from webservice
                //    //Customer.Balance += sO.Balance; // Assuming sO.Balance is the initial balance to be added //due to customer from webservice
                //    //Customer.StoreId = parseId;      // Assuming parseId is the StoreId you want to assign //due to customer from webservice

                //    //db.Customers.Add(Customer);
                //    //db.SaveChanges();
                //}
                //else //due to customer from webservice
                //{//its means old customer. old customer balance should be updated.
                // //Customer.Id = (int)sO.CustomerId;
                //    if (cust.Balance == null) cust.Balance = 0;
                //    if (sO.SaleReturn == false)
                //    {
                //        cust.Balance += sO.Balance;
                //    }
                //    else
                //    {
                //        cust.Balance -= sO.Balance;
                //    }
                //    cust.StoreId = parseId;
                //    db.Entry(cust).State = EntityState.Modified;
                //    //db.SaveChanges();




                //    //Payment payment = new Payment();
                //    //payment = db.Payments.Find(orderId);
                //    //payment.Status = true;
                //    //db.Entry(payment).State = EntityState.Modified;
                //    //db.SaveChanges();

                //}


                //due to customer from webservice

                ////////////////////////////////////////
                BankAccount bankAccount = db.BankAccounts.FirstOrDefault(x => x.Id == sO.BankAccountId);
                bankAccount.Balance += sO.BillPaid;
                db.BankAccounts.Attach(bankAccount);
                db.Entry(bankAccount).Property(x => x.Balance).IsModified = true;
                ////////////////////////////////////////
                ////////////////////////////////////////
                //int maxId = db.SOes.Max(p => p.Auto);
                decimal maxId1 = (int)db.SOes.DefaultIfEmpty().Max(p => p == null ? 0 : p.SOSerial);
                // Get the VAT tax percentage safely
                decimal vatTaxPercent = db.MyBusinessInfoes.FirstOrDefault()?.TaxInPercent ?? 0m;

                maxId1 += 1;
                sO.SOSerial = maxId1;

                //sO.Date = DateTime.Now;
                if (string.IsNullOrEmpty(Convert.ToString(sO.Date)))
                {
                    sO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
                }

                //sO.SaleReturn = false;
                //sO.Id = System.Guid.NewGuid().ToString().ToUpper(); //Commented due to Bill Number
                sO.SaleOrderAmount = 0;
                sO.SaleOrderAmountWithVaT = 0;
                sO.SaleOrderQty = 0;
                sO.SaleOrderAmountWithC = "";
                sO.Profit = 0;
                //Employee emp = (Employee)Session["CurrentUser"];
                //sO.EmployeeId = emp.Id;
                Employee emp = Session["CurrentUser"] as Employee ?? new Employee { Id = 0 }; // or some default ID
                sO.EmployeeId = emp.Id;
                //StoreId 
                //sO.StoreId = parseId; commented due to session issue
                sO.StoreId = storeId;

                db.SOes.Add(sO);
                //db.SaveChanges();
                int sno = 0;
                decimal totalPurchaseAmount = 0;
                Product newProd = null;
                //sOD.RemoveAll(so => so.ProductId == null);
                if (sOD != null)
                {

                    foreach (SOD sod in sOD)
                    {
                        Product dbProd = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                        //StoreProduct storeProduct = db.StoreProducts.FirstOrDefault(x => x.ProductId == sod.ProductId && x.StoreId == parseId);
                        //if (storeProduct == null)
                        //    storeProduct.Stock = 0;
                        //StoreProduct storeProduct = db.StoreProducts.FirstOrDefault(x => x.ProductId == sod.ProductId && x.StoreId == parseId); commented due to session issue
                        StoreProduct storeProduct = db.StoreProducts.FirstOrDefault(x => x.ProductId == sod.ProductId && x.StoreId == storeId); //commented due to session issue
                        decimal salePriceWithVat = sod.SalePrice.Value; // The price with VAT included

                        // Reverse VAT removal (divide by 1 + VAT rate in decimal)
                        decimal salePriceWithoutVat = salePriceWithVat / (1 + vatTaxPercent / 100);
                        sO.SaleOrderAmountWithC = (sO.BillPaid + sO.BillPaidByCash).ToString();


                        // Round to the nearest whole number
                        sod.SalePrice = Math.Round(salePriceWithoutVat); // This will give you the original price, e.g., 100

                        if (storeProduct == null)
                        {
                            storeProduct = new StoreProduct
                            {
                                ProductId = dbProd.Id,
                                //StoreId = parseId, commented due to session issue
                                StoreId = 1,
                                Stock = 0,
                            };
                        }

                        if (dbProd == null || dbProd.Name != sod.Product.Name)
                        {
                            decimal maxId = db.Products.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                            maxId += 1;
                            sod.ProductId = maxId;
                            sod.PerPack = 1;
                            newProd = new Product
                            {
                                Id = maxId,
                                CreateDate = DateTime.Today,
                                IsService = true,
                                Name = sod.Product.Name,
                                PurchasePrice = 0,
                                SalePrice = sod.SalePrice.Value,
                                Stock = 0,
                                totalPiece = 0,
                                PType = 4,
                                PerPack = 1,
                                ShowIn = "S",
                                StoreId = storeId,//commented due to session issue
                                //StoreId = parseId,//commented due to session issue
                            };
                            // Create and initialize StoreProduct instances
                            var storeProducts = new StoreProduct
                            {
                                //StoreId = parseId, // commented due to session issue
                                StoreId = storeId,
                                ProductId = newProd.Id, // Associate with the product
                                Stock = 0 // Set initial stock value
                            };
                            db.Products.Add(newProd);
                            db.SaveChanges();
                        }

                        sno += 1;
                        sod.SODId = sno;
                        sod.SO = sO;
                        sod.SOId = sO.Id;

                        Product product = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                        if (product == null)
                        {
                            //its mean new product
                            product = newProd;
                        }
                        //sod.Sale Price in now from view
                        //sod.SalePrice = product.SalePrice;
                        //dont do this uneessary and sacary(no we have to do it here but not in update. if we not do it here then purchase price will remain empty. and cause error in productwisesale etc)
                        sod.PurchasePrice = product.PurchasePrice;
                        if (sod.Quantity == null) { sod.Quantity = 0; }
                        //sod.OpeningStock = product.Stock;

                        sod.OpeningStock = (storeProduct.Stock ?? 0);
                        sod.PerPack = 1;
                        sod.SaleType = true;
                        if (sod.SaleType == true)//sale
                        {

                            if (sod.IsPack == false)
                            {//piece
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                sO.SaleOrderAmountWithVaT = sO.SaleOrderAmount + (sO.SaleOrderAmount * vatTaxPercent / 100);
                                sO.SaleOrderAmountWithC = (sO.BillPaid + sO.BillPaidByCash).ToString();
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                storeProduct.Stock -= qty;
                                //product.Stock -= qty;

                                sO.SaleOrderQty += qty;//(int)sod.Quantity;
                                sO.Profit += (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else//return
                            {

                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                storeProduct.Stock -= (int)sod.Quantity * sod.PerPack;
                                //product.Stock -= (int)sod.Quantity * sod.PerPack;
                                sO.SaleOrderAmountWithVaT = sO.SaleOrderAmount + (sO.SaleOrderAmount * vatTaxPercent / 100);
                                sO.SaleOrderAmountWithC = (sO.BillPaid + sO.BillPaidByCash).ToString();
                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }
                        }
                        else//return
                        {
                            if (sod.IsPack == false)
                            {
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice);
                                sO.SaleOrderAmountWithVaT = sO.SaleOrderAmount + (sO.SaleOrderAmount * vatTaxPercent / 100);
                                sO.SaleOrderAmountWithC = (sO.BillPaid + sO.BillPaidByCash).ToString();
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                storeProduct.Stock += qty;
                                //product.Stock += qty;
                                sO.SaleOrderQty += qty;//(int)sod.Quantity;
                                sO.Profit += (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                storeProduct.Stock += (int)sod.Quantity * sod.PerPack;
                                //product.Stock += (int)sod.Quantity * sod.PerPack;
                                sO.SaleOrderAmountWithVaT = sO.SaleOrderAmount + (sO.SaleOrderAmount * vatTaxPercent / 100);
                                sO.SaleOrderAmountWithC = (sO.BillPaid + sO.BillPaidByCash).ToString();
                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }
                            


                        }
                        sod.Product = null;
                        //db.SODs.Attach(sod);
                        //db.Entry(sod).Property(x => x.Product).IsModified = false;
                    }

                    // Call the web service login function  
                    // Call the web service login function synchronously

                    //var loginToWebService = LoginToWebService();
                    var loginToWebService = await LoginToWebServiceAsync();
                    if (loginToWebService == null || loginToWebService.Data == null)
                    {
                        return Json(new { Success = false, Messsag = "Invalid Login attempt to web service,please use correct credentials" });
                    }
                    // Convert Data property to JSON string
                    string dataString = JsonConvert.SerializeObject(loginToWebService.Data);
                    // Deserialize the JSON string to get the token
                    dynamic jsonResponse = JsonConvert.DeserializeObject(dataString);
                    string authToken = jsonResponse?.Token;  // Make sure the key name matches exactly with the response

                    //dynamic jsonResponse = JsonConvert.DeserializeObject(loginToWebService.ContentType);
                    //string authToken = jsonResponse.token;

                    //if (loginToWebService == null)
                    //    return Json(new { Success = false, Messsag = "Invalid Login attempt to web service,please use correct credentials" });

                    var webServiceResponse = await AddWebServiceCustomerDetails(authToken, Customer, sO, sOD);
                    if (webServiceResponse == null || !webServiceResponse.Success)
                    {
                        return Json(new { Success = false, Messsag = "Web service not responding" });
                    }
                    var dataType = webServiceResponse.Data.GetType();
                    if (dataType == typeof(JObject))
                    {
                        JObject dataObject = (JObject)webServiceResponse.Data;
                        string code = dataObject["code"]?.ToString();
                        string nestedDataObjects = dataObject["data"].ToString();
                        Console.WriteLine($"Code: {code}");
                        JObject nestedDataObject = dataObject["data"] as JObject;
                        if (nestedDataObject != null)
                        {
                            // Example of accessing a value within the nested 'data'
                            string invoiceSeries = nestedDataObject["khieu"]?.ToString();
                            //string invoiceNumber = nestedDataObject["inv_invoiceNumber"]?.ToString();
                            string invoiceNumber = nestedDataObject["shdon"]?.ToString();
                            // Correct key name
                            string macqt = nestedDataObject["macqt"]?.ToString();
                            string sbmat = nestedDataObject["sbmat"]?.ToString();
                            TempData["InvoiceSeries"] = invoiceSeries;
                            TempData["InvoiceNumber"] = invoiceNumber;
                            TempData["Macqt"] = macqt;
                            TempData["_Sobaomat"] = sbmat;
                            TempData.Keep("_Sobaomat");

                        }
                    }
                    if (webServiceResponse.Success)
                    {
                        // Use the response data as needed
                        TempData["WebServiceMessage"] = webServiceResponse.Message;
                        // Proceed with further logic
                    }
                    else
                    {
                        // Handle the error case
                        ViewBag.ErrorMessage = webServiceResponse.Message;
                        return View("Error"); // or any error handling view
                    }
                    //var addWebServiceCustomerDetails = await AddWebServiceCustomerDetails(authToken, Customer, sO, sOD);
                    ///////////////////////

                    TempData["CustomerName"] = Customer.Name;
                    TempData["CompanyName"] = Customer.CompanyName;
                    TempData["SaleOrderAmountWithVaT"] = sO.SaleOrderAmountWithVaT;
                    TempData["CustomerEmail"] = Customer.Email;
                    TempData["CustomerAddress"] = Customer.Address;
                    TempData["POSName"] = getStoreName.Name;
                    TempData["POSAddress"] = getStoreName.Address;
                    TempData["POSPhoneNumber"] = getStoreName.PhoneNumber;
                    TempData["CustomerVatNumber"] = Customer.Vat;
                    TempData["CustomerCompanyAddress"] = Customer.CompanyAddress;
                    string aa = TempData["JsonResponseWebservicess"] as string;

                    //var addWebServiceCuromerDetails =  AddWebServiceCustomerDetails(authToken, cust,sO,sOD);
                    //try
                    //{
                    //    // Call the web service login function synchronously
                    //    var loginResult = LoginToWebService();

                    //    if (!loginResult.IsSuccess)
                    //    {
                    //        // Handle login failure (e.g., show error message, redirect, etc.)
                    //        ViewBag.ErrorMessage = "Failed to log in to the external service.";
                    //        return View();
                    //    }
                    //}
                    //catch { 

                    //}
                    sO.Profit -= (decimal)sO.Discount;


                    db.SODs.AddRange(sOD);
                }
            }
            db.SaveChanges();
            TempData["ReportId"] = sO.Id;

            //SOId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(sO.Id, "BZNS"))); //Commented due to Id decrypted Id save in Db table SO
            //}
            return RedirectToAction("Create", new { IsReturn = "false" });
            //return RedirectToAction("PrintSO3", new { id = sO.Id });
            //return View();
        }
        // Function to login to the web service
        // Function to login to the web service synchronously
        //private (bool IsSuccess, string ResponseContent)  LoginToWebService()

        public ActionResult CloseStorePopup()
        {
            return PartialView("_StoreClosePopup");
        }

        public ActionResult BankDepositPopup()
        {
            return PartialView("_BankDepositPopup");
        }
        public ActionResult MoneyInputPopup()
        {
            return PartialView("_MoneyInputPopup");
        }
        public async Task<JsonResult> LoginToWebServiceAsync()
        {
            try
            {
                // Enforce TLS 1.2 or TLS 1.3
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                using (var client = new HttpClient())
                {
                    // Define the login endpoint URL (replace with the actual API endpoint)
                    string url = "https://0401485182.minvoice.com.vn/api/Account/Login"; // Replace with the actual endpoint

                    // Create the request body with the necessary parameters
                    var requestBody = new
                    {
                        //username = "PHEVA",
                        username = "POSDN2",
                        password = "4£j5Zm60HNl?",
                        //password = "2BM@g0J%5sguJ@",
                        //ma_dvcs = "VP"
                    };

                    // Convert the request body to JSON format
                    string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

                    // Set up the HttpClient and request headers
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                    // Send a POST request with the JSON request body to get the token
                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response body as a string
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Check if the response is in JSON format
                        if (response.Content.Headers.ContentType?.MediaType == "application/json")
                        {
                            // Parse the response JSON to extract the token
                            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                            string token = jsonResponse.token;

                            if (!string.IsNullOrEmpty(token))
                            {
                                // Store the token (e.g., in session or other storage)
                                Session["AuthToken"] = token;

                                // Success message or subsequent requests
                                return Json(new { Success = true, Token = token });
                            }
                            else
                            {
                                // Handle case where token is null or empty
                                return Json(new { Success = false, Message = "Token is null or empty" });
                            }
                        }
                        else
                        {
                            // Log or handle non-JSON response
                            return Json(new { Success = false, Message = "Unexpected response format: " + responseBody });
                        }
                    }
                    else
                    {
                        // Handle error responses
                        string errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new { Success = false, Message = $"Failed to log in. Status code: {response.StatusCode}. Response: {errorContent}" });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                System.Diagnostics.Debug.WriteLine("Exception occurred in LoginToWebService: " + ex.Message);
                return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }
        }

        //Dynamic public async Task<JsonResult> LoginToWebServiceAsync(int storeId)
        // {
        //     try
        //     {
        //         // Fetch the store info from the database
        //         var store = await db.Stores.FindAsync(storeId);
        //         if (store == null)
        //             return Json(new { Success = false, Message = "Store not found." });

        //         // Ensure TLS
        //         ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        //         using (var client = new HttpClient())
        //         {
        //             string url = store.ApiBaseUrl;
        //             var requestBody = new
        //             {
        //                 username = store.ApiUsername,
        //                 password = store.ApiPassword
        //             };

        //             string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

        //             client.DefaultRequestHeaders.Accept.Clear();
        //             client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //             client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        //             HttpResponseMessage response = await client.PostAsync(
        //                 url,
        //                 new StringContent(jsonRequestBody, Encoding.UTF8, "application/json")
        //             );

        //             string responseBody = await response.Content.ReadAsStringAsync();

        //             if (response.IsSuccessStatusCode && response.Content.Headers.ContentType?.MediaType == "application/json")
        //             {
        //                 dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
        //                 string token = jsonResponse.token;

        //                 if (!string.IsNullOrEmpty(token))
        //                 {
        //                     Session["AuthToken"] = token;

        //                     // Generate invoice series
        //                     string invoiceSeries = $"1C{DateTime.Now.Year % 100}{store.StoreShortCode}";

        //                     return Json(new { Success = true, Token = token, InvoiceSeries = invoiceSeries });
        //                 }

        //                 return Json(new { Success = false, Message = "Token is empty." });
        //             }

        //             return Json(new { Success = false, Message = $"Login failed. {response.StatusCode} - {responseBody}" });
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json(new { Success = false, Message = ex.Message });
        //     }
        // }

        //public async Task<JsonResult> LoginToWebServiceAsync()
        //{
        //    try
        //    {
        //        // Enforce TLS 1.2 or TLS 1.3
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        //        using (var client = new HttpClient())
        //        {
        //            // Define the login endpoint URL (replace with the actual API endpoint)
        //            string url = "https://0401485182.minvoice.com.vn/api/Account/Login"; // Replace with the actual endpoint

        //            // Create the request body with the necessary parameters
        //            var requestBody = new
        //            {
        //                //username = "PHEVA",
        //                username = "POSDN2",
        //                password = "4£j5Zm60HNl?",
        //                //password = "2BM@g0J%5sguJ@",
        //                //ma_dvcs = "VP"
        //            };

        //            // Convert the request body to JSON format
        //            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

        //            // Set up the HttpClient and request headers
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

        //            // Send a POST request with the JSON request body to get the token
        //            HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json"));

        //            if (response.IsSuccessStatusCode)
        //            {
        //                // Read the response body as a string
        //                string responseBody = await response.Content.ReadAsStringAsync();

        //                // Check if the response is in JSON format
        //                if (response.Content.Headers.ContentType?.MediaType == "application/json")
        //                {
        //                    // Parse the response JSON to extract the token
        //                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
        //                    string token = jsonResponse.token;

        //                    if (!string.IsNullOrEmpty(token))
        //                    {
        //                        // Store the token (e.g., in session or other storage)
        //                        Session["AuthToken"] = token;

        //                        // Success message or subsequent requests
        //                        return Json(new { Success = true, Token = token });
        //                    }
        //                    else
        //                    {
        //                        // Handle case where token is null or empty
        //                        return Json(new { Success = false, Message = "Token is null or empty" });
        //                    }
        //                }
        //                else
        //                {
        //                    // Log or handle non-JSON response
        //                    return Json(new { Success = false, Message = "Unexpected response format: " + responseBody });
        //                }
        //            }
        //            else
        //            {
        //                // Handle error responses
        //                string errorContent = await response.Content.ReadAsStringAsync();
        //                return Json(new { Success = false, Message = $"Failed to log in. Status code: {response.StatusCode}. Response: {errorContent}" });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception or handle it as needed
        //        System.Diagnostics.Debug.WriteLine("Exception occurred in LoginToWebService: " + ex.Message);
        //        return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
        //    }
        //}
        //public JsonResult LoginToWebService()
        //{
        //    //try
        //    //{
        //    //    // Enforce TLS 1.2 or TLS 1.3
        //    //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        //    //    using (var client = new HttpClient())
        //    //    {
        //    //        // Define the login endpoint URL (replace with the actual API endpoint)
        //    //        //string url = "https://hddt.minvoice.com.vn/api/login"; // Replace with the actual endpoint
        //    //        string url = "https://0106026495-998.minvoice.pro/api/Account/Login"; // Replace with the actual endpoint

        //    //        //        // Create the request body with the necessary parameters
        //    //        //        //var requestBody = new
        //    //        //        //{
        //    //        //        //    taxCode = "0106026495-998",
        //    //        //        //    username = "PHEVA",
        //    //        //        //    password = "2BM@g0J%5sguJ@"
        //    //        //        //};
        //    //        var requestBody = new
        //    //        {
        //    //            username = "PHEVA",
        //    //            password = "2BM@g0J%5sguJ@",
        //    //            ma_dvcs = "VP"
        //    //        };

        //    //        // Convert the request body to JSON format
        //    //        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

        //    //        // Set up the HttpClient and request headers
        //    //        client.DefaultRequestHeaders.Accept.Clear();
        //    //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    //        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        //    //        // Send a POST request with the JSON request body
        //    //        HttpResponseMessage response = client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json")).Result;

        //    //        // Check the response status code
        //    //        if (response.IsSuccessStatusCode)
        //    //        {
        //    //            string responseBody = response.Content.ReadAsStringAsync().Result;

        //    //            // Check if the response is in JSON format
        //    //            if (response.Content.Headers.ContentType.MediaType == "application/json")
        //    //            {
        //    //                // Parse the response JSON to extract the token
        //    //                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
        //    //                string token = jsonResponse.token;

        //    //                // Store the token (e.g., in session or other storage)
        //    //                Session["AuthToken"] = token;

        //    //                // Success message or subsequent requests
        //    //                ViewBag.SuccessMessage = "Login successful!";
        //    //                return View("Success");
        //    //            }
        //    //            else
        //    //            {
        //    //                // Handle HTML or unexpected response
        //    //                ViewBag.ErrorMessage = $"Unexpected response format. Content-Type: {response.Content.Headers.ContentType.MediaType}. Response: {responseBody}";
        //    //                return View("Error");
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            // Handle error responses
        //    //            string errorContent = response.Content.ReadAsStringAsync().Result;
        //    //            ViewBag.ErrorMessage = $"Failed to log in. Status code: {response.StatusCode}. Response: {errorContent}";
        //    //            return View("Error");
        //    //        }
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    // Log the exception or handle it as needed
        //    //    System.Diagnostics.Debug.WriteLine("Exception occurred in LoginToWebService: " + ex.Message);
        //    //    ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
        //    //    return View("Error");
        //    //}

        //    try
        //    {
        //        Session["AuthToken"] = "";
        //        // Enforce TLS 1.2 or TLS 1.3
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        //        using (var client = new HttpClient())
        //        {
        //            // Define the login endpoint URL (replace with the actual API endpoint)
        //            //string url = "https://hddt.minvoice.com.vn/api/login"; // Replace with the actual endpoint
        //            string url = "https://0106026495-998.minvoice.pro/api/Account/Login"; // Replace with the actual endpoint

        //            // Create the request body with the necessary parameters
        //            //var requestBody = new
        //            //{
        //            //    taxCode = "0106026495-998",
        //            //    username = "PHEVA",
        //            //    password = "2BM@g0J%5sguJ@"
        //            //};
        //            var requestBody = new
        //            {
        //                username = "PHEVA",
        //                password = "2BM@g0J%5sguJ@",
        //                ma_dvcs = "VP"
        //            };

        //            // Convert the request body to JSON format
        //            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

        //            // Set up the HttpClient and request headers
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

        //            // Send a POST request with the JSON request body to get the token
        //            HttpResponseMessage response = client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json")).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                // Read the response body as a string
        //                string responseBody = response.Content.ReadAsStringAsync().Result;

        //                // Check if the response is in JSON format
        //                if (response.Content.Headers.ContentType.MediaType == "application/json")
        //                {
        //                    // Parse the response JSON to extract the token
        //                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
        //                    string token = jsonResponse.token;
        //                    if (token != "" || token != null)
        //                    {
        //                        ViewBag.SuccessMessage = "Request successful!";
        //                        return Json("Success", responseBody);
        //                    }
        //                    // Store the token (e.g., in session or other storage)
        //                    Session["AuthToken"] = token;

        //                    // Use this token in subsequent requests
        //                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //                    // Example of making a subsequent API request
        //                    //string anotherApiUrl = "https://hddt.minvoice.com.vn/api/anotherEndpoint";
        //                    //HttpResponseMessage anotherResponse = client.GetAsync(anotherApiUrl).Result;

        //                    //if (anotherResponse.IsSuccessStatusCode)
        //                    //{
        //                    //    // Process the response as needed
        //                    //    string anotherResponseBody = anotherResponse.Content.ReadAsStringAsync().Result;
        //                    //    ViewBag.SuccessMessage = "Request successful!";
        //                    //    return View("Success", anotherResponseBody);
        //                    //}
        //                    //else
        //                    //{
        //                    //    ViewBag.ErrorMessage = "Failed to make the authenticated request.";
        //                    //    return View("Error");
        //                    //}
        //                }
        //                else
        //                {
        //                    // Log or handle non-JSON response
        //                    ViewBag.ErrorMessage = "Unexpected response format: " + responseBody;
        //                    return Json("Error");
        //                }
        //            }
        //            else
        //            {
        //                ViewBag.ErrorMessage = "Failed to log in to the external service.";
        //                return Json("Error");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception or handle it as needed
        //        System.Diagnostics.Debug.WriteLine("Exception occurred in LoginToWebService: " + ex.Message);
        //        ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
        //        return Json("Error");
        //    }
        //    return Json(new { Success = true, Token = Session["AuthToken"] });
        //}
        //public async Task<JsonResult> AddWebServiceCustomerDetails(string authToken, Customer cust, SO saleOrder, List<SOD> saleOrderDetails)
        public async Task<WebServiceResponse> AddWebServiceCustomerDetails(string authToken, Customer cust, SO saleOrder, List<SOD> saleOrderDetails)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                //return Json(new { Success = false, Message = "Invalid token" });
            }

            // Check for null references in the input parameters
            if (cust == null)
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                //return Json(new { Success = false, Message = "Customer information is missing." });
            }

            if (saleOrder == null)
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                //return Json(new { Success = false, Message = "Sale order information is missing." });
            }

            if (saleOrderDetails == null || saleOrderDetails.Count == 0)
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                //return Json(new { Success = false, Message = "Sale order details are missing." });
            }

            try
            {
                var tax = 8;/*db.MyBusinessInfoes.TaxInPercent*/
                //var soId = db.SODs.Select(d => d.SOId== saleOrder.Id).FirstOrDefault();
               // var dbsaleOrderDetails = db.SODs
               // .Include(d => d.Product)
               // .Where(d => d.SOId == saleOrder.Id)
               //.ToList();

                foreach(SOD sod in saleOrderDetails)
                {
                    sod.Product = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                }

                string url = "https://0401485182.minvoice.com.vn/api/InvoiceApi78/SaveV2"; //Real url working test env 
                //string url = "https://0106026495-998.minvoice.pro/api/InvoiceApi780/Save"; //To check if webservice down / do not respond

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                    var getCurrentYear = DateTime.Now.Year; // Gets the current year (e.g., 2024)
                    var lastTwoDigits = getCurrentYear % 100; // Extracts the last two digits (e.g., 24)
                    var lastTwoDigitsString = lastTwoDigits.ToString("D2");

                    // Safely create the request model
                    //var invoiceDetails = saleOrderDetails
                    //    .Where(detail => detail != null && detail.Product != null)
                    //    .Select(detail => new InvoiceDetail
                    //    {
                    //        tchat = 1,
                    //        stt_rec0 = 4,// detail.SODId ?? 0,
                    //        inv_itemCode = "CB005",//detail.Product != null ? "CB001" : "", // Default value if Product is null
                    //        inv_itemName = "Chocolatebox001", //detail.Product?.Name ?? "Unknown Product", // Default value if Product.Name is null
                    //        inv_unitCode = "Box",//detail.Product != null ? "Box/Pack" : "Unknown",
                    //        inv_quantity = 1,//detail.Quantity ?? 0,
                    //        inv_unitPrice = 12000, //detail.SalePrice ?? 0m,
                    //        inv_discountPercentage = 0,
                    //        inv_discountAmount = 0,
                    //        inv_TotalAmountWithoutVat = 120000, //saleOrder.BillAmount,//saleOrder.BillAmount ?? 0 // Assuming BillAmount is required
                    //        ma_thue = 8,
                    //        inv_vatAmount = 9600,
                    //        inv_TotalAmount = 129600
                    //    })
                    //    .ToList();

                    var invoiceDetails = saleOrderDetails//dbsaleOrderDetails
                        .Where(detail => detail != null && detail.ProductId != null)
                        .Select(detail => new InvoiceDetail

                        {

                            tchat = 1,
                            stt = detail.SODId, // Ensure dynamic property exists
                            //stt = (int?)detail.ProductId,
                            /*ma = detail.Product?.EInvoicePCode?? "",*/ // Handle null Product gracefully
                            ma = detail.Product.EInvoicePCode ?? "",
                            ten = detail.Product?.Name ?? "",
                            dvtinh = detail.Product?.Unit ?? "", // Fix: Use null-conditional operator
                            sluong = detail.Quantity, // Ensure Quantity is not null
                            dgia = detail.SalePrice ?? 0, // Ensure SalePrice is not null
                            tlckhau = 0,
                            ttcktmai = 0,
                            stckhau = 0,
                            thtien = (decimal)((detail.Quantity ) * (detail.SalePrice )), // Ensure null handling
                            tsuat = tax, // Tax code
                            tthue = (decimal)((detail.Quantity ) * (detail.SalePrice ) * tax / 100m), // Ensure null handling
                            tgtien = (decimal)(((detail.Quantity ) * (detail.SalePrice )) + ((detail.Quantity ?? 1) * (detail.SalePrice ) * tax) / 100),

                        })
                       .ToList();
                
                    // Ensure saleOrder properties are not null before using them
                    var invoice = new Invoice
                    {
                        //inv_invoiceSeries = saleOrder.InvoiceSeries ?? "1C24MPE", // Default value if InvoiceSeries is null
                        khieu = $"1C{lastTwoDigitsString}MDX",//"1C24MPE", // Default value if InvoiceSeries is null Invoice symobol fo receipt
                        tdlap = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd HH:mm:ss"),//saleOrder.InvoiceIssuedDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddDays(13).ToString("yyyy-MM-dd"),
                        dvtte = "VND",//saleOrder.CurrencyCode ?? "VND",
                        tgia = 1,//saleOrder.ExchangeRate ?? 1,
                        //sdhang = saleOrder.Id,//"HN-20242309-002", //saleOrder.SOSerial ?? "HN-20241509-001",
                        //sdhang = $"{saleOrder.Id}", /*-{DateTime.UtcNow.Ticks}*/
                        sdhang = $"{saleOrder.Id}-{DateTime.UtcNow.Ticks}",
                        //sdhang = $"{saleOrder.Id}",
                        tnmua = cust.Name,
                        ten = cust.CompanyName,
                        mst = cust.Vat,/*saleOrder.SOSerial.ToString()*///"0401485182",//cust.TaxCode ?? "0401485182",
                        dchi = cust.Address,
                        //inv_buyerEmail = cust.Email ?? "unknown@example.com",
                        email = cust.Email,
                        htttoan = saleOrder.PaymentMethod ?? "TM/CK",
                        ttcktmai = (decimal)saleOrder.Discount,
                        tgtcthue = (decimal)saleOrder.SaleOrderAmount / (1 + tax / 100),//saleOrder.TotalAmountWithoutVat ?? 0,
                        tgtthue = (decimal)(saleOrder.SaleOrderAmount * tax / 100m),
                        
                        tgtttbso = (decimal)saleOrder.SaleOrderAmountWithVaT,//saleOrder.BillPaid ?? 0, = saleOrder.BillAmount,//saleOrder.BillPaid ?? 0, = saleOrder.BillAmount,//saleOrder.BillPaid ?? 0,
                        /*key_API = saleOrder.Id,*///"HN-20242309-002",//saleOrder.ApiKey ?? "HN-20241509-001",
                       
                        details = new List<InvoiceDetailsWrapper> { new InvoiceDetailsWrapper { data = invoiceDetails } }
                    };

                    var requestBody = new InvoiceRequest
                    {
                        editmode = 1,
                        data = new List<Invoice> { invoice }
                    };

                    string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

                    // Sending the request to the web service
                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        if (response.Content.Headers.ContentType.MediaType == "application/json")
                        {
                            try
                            {
                                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

                                // Check if deserialization failed
                                if (jsonResponse == null)
                                {
                                    return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                                    //return Json(new { Success = false, Message = "Failed to parse the response." });
                                }
                                //StaticDto.Data = jsonResponse;
                                // Check if 'code' property exists and is not null
                                if (jsonResponse?.code != null && jsonResponse.code == "00")
                                {
                                    return new WebServiceResponse
                                    {
                                        Success = true,
                                        Message = jsonResponse.message,
                                        Data = jsonResponse
                                    };
                                }
                                else
                                {
                                    return new WebServiceResponse
                                    {
                                        Success = false,
                                        Message = "Unexpected response code.",
                                        Data = jsonResponse
                                    };
                                }
                                //if (jsonResponse?.code != null && jsonResponse.code == "00")
                                //{
                                //    // Store the serialized JSON string in TempData
                                //    string jsonResponseString = JsonConvert.SerializeObject(jsonResponse);
                                //    //Session["JsonResponseWebservice"] = jsonResponseString;
                                //    TempData["JsonResponseWebservicess"] = jsonResponseString;
                                //    //ViewBag.LLLL = TempData["JsonResponseWebservicess"];

                                //    // Retrieve the TempData value as a dynamic object
                                //    dynamic tempDataResponse = JsonConvert.DeserializeObject(TempData["JsonResponseWebservicess"].ToString());
                                //    //StaticDto.Data = tempDataResponse;
                                //    //_jsonResponseWebservice = JsonConvert.DeserializeObject(responseBody);

                                //    // Now you can use tempDataResponse
                                //    return Json(new { Success = true, Message = jsonResponse.message, Data = tempDataResponse });
                                //}
                                //else
                                //{
                                //    return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
                                //}
                                //dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                                //// Check if deserialization failed
                                //if (jsonResponse == null)
                                //{
                                //    return Json(new { Success = false, Message = "Failed to parse the response." });
                                //}

                                //// Check if 'code' property exists and is not null
                                //if (jsonResponse?.code != null && jsonResponse.code == "00")
                                //{
                                //    string jsonResponseString = JsonConvert.SerializeObject(jsonResponse);
                                //    // Store the serialized JSON string in TempData
                                //    TempData["JsonResponseWebservice"] = jsonResponseString;
                                //    // Optional: If you want to retrieve it as a dynamic object later
                                //    dynamic tempDataResponse = JsonConvert.DeserializeObject(TempData["JsonResponseWebservice"].ToString());
                                //    //string jsonResponseString = JsonConvert.SerializeObject(jsonResponse);
                                //    //TempData["JsonResponseWebservice"] = jsonResponse;
                                //    //string abc =(string) TempData["JsonResponseWebservice"];
                                //    //TempData["_JsonResponseWebservice"] = TempData["JsonResponseWebservice"];
                                //    return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse });
                                //    //return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
                                //}
                                //else
                                //{
                                //    return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
                                //}

                                //if (jsonResponse.code == "00")
                                //{
                                //    // Store jsonResponse in TempData
                                //    Session["JsonResponse"] = jsonResponse;
                                //    return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
                                //}
                                //else
                                //{
                                //    return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
                                //}
                            }
                            catch (JsonException jsonEx)
                            {
                                return new WebServiceResponse { Success = false, Message = "Request failed with status code: " + response.StatusCode };
                                // return Json(new { Success = false, Message = "Error parsing JSON response.", Error = jsonEx.Message });
                            }
                        }
                        else
                        {
                            return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                            //return Json(new { Success = false, Message = "Received non-JSON response.", Response = responseBody });
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        TempData["WebserviceDownError"] = "Sever is down VAT Invoice cannot print at this time";
                        return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                        //return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " + ex.Message };
                //return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }


            //if (string.IsNullOrEmpty(authToken))
            //{
            //    return Json(new { Success = false, Message = "Invalid token" });
            //}

            //try
            //{
            //    string url = "https://0106026495-998.minvoice.pro/api/InvoiceApi78/Save";

            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            //        // Creating the request model
            //        var invoiceDetails = saleOrderDetails.Select(detail => new InvoiceDetail
            //        {
            //            tchat = 1,
            //            stt_rec0 = detail.SODId ?? 0, // Use null-coalescing operator to handle null values
            //            inv_itemCode = "CB001",//detail.Product != null ? "CB001" : "", // Provide a default value if Product is null
            //            inv_itemName = detail.Product?.Name ?? "Unknown Product", // Use null-conditional operator and provide a default value
            //            inv_unitCode = detail.Product != null ? "Box/Pack" : "Unknown", // Check if Product is not null
            //            inv_quantity = detail.Quantity ?? 0, // Provide a default value if Quantity is null
            //            inv_unitPrice = detail.PurchasePrice ?? 0m, // Provide a default value if PurchasePrice is null
            //            inv_discountPercentage = 0, // Detail-specific value or default
            //            inv_discountAmount = 20, // Provide a fixed value or calculate if needed
            //            inv_TotalAmountWithoutVat = 220000,//saleOrder.BillAmount ?? 0, // Provide a default value if BillAmount is null
            //            ma_thue = 8, // Detail-specific value
            //            inv_vatAmount = 0, // Detail-specific value
            //            inv_TotalAmount = 0 // Detail-specific value
            //        }).ToList();

            //        var invoice = new Invoice
            //        {
            //            inv_invoiceSeries = "1C24MPE",
            //            inv_invoiceIssuedDate = DateTime.Now.AddDays(13).ToString("yyyy-MM-dd"),//DateTime.Now.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture), //saleOrder.InvoiceIssuedDate.ToString("yyyy-MM-dd"),
            //            inv_currencyCode = "VND",//saleOrder.CurrencyCode,
            //            inv_exchangeRate = 1,//saleOrder.ExchangeRate,
            //            so_benh_an = "HN-20241509-001", //saleOrder.SOSerial.ToString(),
            //            inv_buyerDisplayName = cust.Name,
            //            inv_buyerLegalName = cust.Name,
            //            inv_buyerTaxCode = "0401485182", //cust.TaxCode,
            //            inv_buyerAddressLine = cust.Address,
            //            inv_buyerEmail = cust.Email,
            //            inv_paymentMethodName = "TM/CK", //saleOrder.PaymentMethod,
            //            inv_discountAmount = saleOrder.Discount,
            //            inv_TotalAmountWithoutVat = 0,//saleOrder.TotalAmountWithoutVat,
            //            inv_vatAmount = 0,//saleOrder.VatAmount,
            //            inv_TotalAmount = saleOrder.BillPaid,
            //            key_api = "HN-20241509-001", //saleOrder.ApiKey,
            //            details = new List<InvoiceDetailsWrapper> { new InvoiceDetailsWrapper { data = invoiceDetails } }
            //        };

            //        var requestBody = new InvoiceRequest
            //        {
            //            editmode = 1,
            //            data = new List<Invoice> { invoice }
            //        };

            //        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

            //        // Sending the request to the web service
            //        HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json"));

            //        if (response.IsSuccessStatusCode)
            //        {
            //            string responseBody = await response.Content.ReadAsStringAsync();

            //            if (response.Content.Headers.ContentType.MediaType == "application/json")
            //            {
            //                try
            //                {
            //                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

            //                    // Check for the "code" field in the response to determine success
            //                    if (jsonResponse.code == "00")
            //                    {
            //                        return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
            //                    }
            //                    else
            //                    {
            //                        return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
            //                    }
            //                }
            //                catch (JsonException jsonEx)
            //                {
            //                    return Json(new { Success = false, Message = "Error parsing JSON response.", Error = jsonEx.Message });
            //                }
            //            }
            //            else
            //            {
            //                return Json(new { Success = false, Message = "Received non-JSON response.", Response = responseBody });
            //            }
            //        }
            //        else
            //        {
            //            string errorContent = await response.Content.ReadAsStringAsync();
            //            return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception or handle it as needed
            //    return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            //}



            //if (string.IsNullOrEmpty(authToken))
            //{
            //    return Json(new { Success = false, Message = "Invalid token" });
            //}

            //try
            //{
            //    string url = "https://0106026495-998.minvoice.pro/api/InvoiceApi78/Save"; // Endpoint to add customer details

            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            //        var requestBody = new
            //        {
            //            inv_invoiceSeries = "1C24MPE ",
            //            inv_invoiceIssuedDate = "2024-29-08",
            //            inv_currencyCode = "VND",
            //            inv_exchangeRate = 1,
            //            so_benh_an = "HN-20242908-001",
            //            inv_buyerDisplayName = "Mister XYZ",
            //            inv_buyerLegalName = "company A",
            //            inv_buyerTaxCode = "0401485182",
            //            inv_buyerAddressLine = "Company A address",
            //            inv_buyerEmail = "companya@gmail.com",
            //            inv_buyerBankAccount = "",
            //            inv_buyerBankName = "",
            //            inv_paymentMethodName = "TM/CK",
            //            inv_discountAmount = 0,
            //            inv_TotalAmountWithoutVat = 220000,
            //            inv_vatAmount = 17600,
            //            inv_TotalAmount = 237600,
            //            key_api = "HN-20242908-001",
            //        };

            //        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

            //        // Make sure to use await and async
            //        HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json"));

            //        if (response.IsSuccessStatusCode)
            //        {
            //            string responseBody = await response.Content.ReadAsStringAsync();

            //            if (response.Content.Headers.ContentType.MediaType == "application/json")
            //            {
            //                try
            //                {
            //                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

            //                    // Check for the "code" field in the response to determine success
            //                    if (jsonResponse.code == "00")
            //                    {
            //                        return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
            //                    }
            //                    else
            //                    {
            //                        return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
            //                    }
            //                }
            //                //catch (Exception ex)
            //                //{
            //                //    //log.Error("Error in ", ex);
            //                //}
            //                catch (JsonException jsonEx)
            //                {
            //                    return Json(new { Success = false, Message = "Error parsing JSON response.", Error = jsonEx.Message });
            //                }
            //            }
            //            else
            //            {
            //                return Json(new { Success = false, Message = "Received non-JSON response.", Response = responseBody });
            //            }
            //        }
            //        else
            //        {
            //            string errorContent = await response.Content.ReadAsStringAsync();
            //            return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception or handle it as needed
            //    return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            //}



            //if (string.IsNullOrEmpty(authToken))
            //{
            //    return Json(new { Success = false, Message = "Invalid token" });
            //}

            //try
            //{
            //    string url = "https://0106026495-998.minvoice.pro/api/InvoiceApi78/Save"; // Endpoint to add customer details
            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        // Correctly set the Authorization header with a space between "Bearer" and the token
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            //        var requestBody = new
            //        {
            //            // Map your customer properties here
            //            inv_invoiceIssuedDate = DateTime.UtcNow,
            //            inv_invoiceSeries = "1C24MPE",
            //            inv_paymentMethodName = "TM/CK",
            //            inv_buyerDisplayName = cust.Name,
            //            ma_dt = "Cust_00123_dt",
            //            inv_buyerLegalName = cust.Name,
            //            inv_buyerTaxCode = "inv_buyr_Tax_Code",
            //            inv_buyerAddressLine = cust.Address,
            //            inv_buyerEmail = cust.Email,
            //            amount_to_word = "Amount in word",
            //            inv_TotalAmount = 0.00,
            //            inv_discountAmount = 0.00,
            //            inv_vatAmount = 0.00,
            //            TotalAmountWithoutVat = 0.00,
            //            key_api = "Do not have",
            //            inv_itemCode = "#134 Item Code",
            //            inv_itemName = "item Name",
            //            inv_quantity = 11.22,
            //            inv_unitPrice = 22.00,
            //            inv_discountPercentage = 10.13,
            //            inv_TotalAmountWithoutVat = 2.13,
            //            ma_thue = 2.13,
            //        };

            //        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            //        HttpResponseMessage response = client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json")).Result;

            //        if (response.IsSuccessStatusCode)
            //        {
            //            // Read response content as string
            //            string responseBody = response.Content.ReadAsStringAsync().Result;

            //            // Deserialize the response to a dynamic object to check its structure
            //            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

            //            // Check for the "code" field in the response to determine success
            //            if (jsonResponse.code == "00")
            //            {
            //                return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
            //            }
            //            else
            //            {
            //                return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
            //            }
            //        }
            //        else
            //        {
            //            string errorContent = response.Content.ReadAsStringAsync().Result;
            //            return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception or handle it as needed
            //    return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            //}
            //if (string.IsNullOrEmpty(authToken))
            //{
            //    return Json(new { Success = false, Message = "Invalid token" });
            //}

            //try
            //{
            //    string url = "https://hddt.minvoice.com.vn/api/InvoiceApi78/Save"; // Endpoint to add customer details
            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            //        var requestBody = new
            //        {
            //            // Map your customer properties here
            //            inv_invoiceIssuedDate = DateTime.UtcNow,
            //            inv_invoiceSeries= "1C24MPE",
            //            inv_paymentMethodName = "TM/CK",
            //            inv_buyerDisplayName = cust.Name,
            //            ma_dt = "Cust_00123_dt",
            //            inv_buyerLegalName = cust.Name,
            //            inv_buyerTaxCode = "inv_buyr_Tax_Code",
            //            inv_buyerAddressLine = cust.Address,
            //            inv_buyerEmail = cust.Email,
            //            amount_to_word="Amount in word",
            //            inv_TotalAmount=0.00,
            //            inv_discountAmount = 0.00,
            //            inv_vatAmount = 0.00,
            //            TotalAmountWithoutVat = 0.00,
            //            key_api = "Do not have",
            //            inv_itemCode = "#134 Item Code",
            //            inv_itemName = "item Name",
            //            inv_quantity = 11.22,
            //            inv_unitPrice = 22.00,
            //            inv_discountPercentage = 10.13,
            //            inv_TotalAmountWithoutVat = 2.13,
            //            ma_thue = 2.13,
            //        };

            //        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            //        HttpResponseMessage response = client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json")).Result;

            //        if (response.IsSuccessStatusCode)
            //        {
            //            return Json(new { Success = true, Message = "Customer details added successfully" });
            //        }
            //        else
            //        {
            //            string errorContent = response.Content.ReadAsStringAsync().Result;
            //            return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log or handle the exception as needed
            //    System.Diagnostics.Debug.WriteLine("Exception occurred in AddWebServiceCustomerDetails: " + ex.Message);
            //    return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            //}

            //return Json(new { Success = true, Message = "" });
        }

        //public InvoiceRequest CreateInvoiceRequest(Customer customer, SO saleOrder, List<SOD> saleOrderDetails)
        //{
        //    // Assuming you have methods or logic to get these values from your database or application
        //    var invoiceDetails = saleOrderDetails.Select(detail => new InvoiceDetail
        //    {
        //        tchat = detail.SomeValue,
        //        stt_rec0 = detail.Sequence,
        //        inv_itemCode = detail.ItemCode,
        //        inv_itemName = detail.ItemName,
        //        inv_unitCode = detail.UnitCode,
        //        inv_quantity = detail.Quantity,
        //        inv_unitPrice = detail.UnitPrice,
        //        inv_discountPercentage = detail.DiscountPercentage,
        //        inv_discountAmount = detail.DiscountAmount,
        //        inv_TotalAmountWithoutVat = detail.TotalAmountWithoutVat,
        //        ma_thue = detail.TaxCode,
        //        inv_vatAmount = detail.VatAmount,
        //        inv_TotalAmount = detail.TotalAmount
        //    }).ToList();

        //    var invoice = new Invoice
        //    {
        //        inv_invoiceSeries = saleOrder.InvoiceSeries,
        //        inv_invoiceIssuedDate = saleOrder.InvoiceIssuedDate.ToString("yyyy-MM-dd"),
        //        inv_currencyCode = saleOrder.CurrencyCode,
        //        inv_exchangeRate = saleOrder.ExchangeRate,
        //        so_benh_an = saleOrder.SaleOrderNumber,
        //        inv_buyerDisplayName = customer.DisplayName,
        //        inv_buyerLegalName = customer.LegalName,
        //        inv_buyerTaxCode = customer.TaxCode,
        //        inv_buyerAddressLine = customer.AddressLine,
        //        inv_buyerEmail = customer.Email,
        //        inv_paymentMethodName = saleOrder.PaymentMethodName,
        //        inv_discountAmount = saleOrder.DiscountAmount,
        //        inv_TotalAmountWithoutVat = saleOrder.TotalAmountWithoutVat,
        //        inv_vatAmount = saleOrder.VatAmount,
        //        inv_TotalAmount = saleOrder.TotalAmount,
        //        key_api = saleOrder.ApiKey,
        //        details = new List<InvoiceDetailsWrapper> { new InvoiceDetailsWrapper { data = invoiceDetails } }
        //    };

        //    return new InvoiceRequest
        //    {
        //        editmode = 1, // or whatever is appropriate
        //        data = new List<Invoice> { invoice }
        //    };
        //}
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetLastBillId()
        {
            try
            {
                int? storeId = Session["StoreId"] as int?;
                if (storeId == null)
                {
                    return Json(new { Success = false, Message = "Store not selected" });
                }

                // Get the most recent bill for the current store
                var lastBill = db.SOes
                    .Where(x => x.StoreId == storeId && (x.IsCancelled == false || x.IsCancelled == null))
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();

                if (lastBill == null)
                {
                    return Json(new { Success = false, Message = "No bills found" });
                }

                return Json(new { Success = true, Id = lastBill.Id });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        public JsonResult GetSalesSummary(DateTime today, DateTime yesterday, int? storeId)
        {
            // Get today's sales
            var todaySales = GetSalesForDate(today, storeId);

            // Get yesterday's sales
            var yesterdaySales = GetSalesForDate(yesterday, storeId);

            return Json(new
            {
                today = todaySales,
                yesterday = yesterdaySales
            }, JsonRequestBehavior.AllowGet);
        }

        private object GetSalesForDate(DateTime date, int? storeId)
        {
            // Use your existing BusinessContext
            using (var db = new BusinessContext())
            {
                // Base query for the date and store
                var query = db.SOes.Where(x => DbFunctions.TruncateTime(x.Date) == date.Date);

                if (storeId.HasValue)
                {
                    query = query.Where(x => x.StoreId == storeId.Value);
                }

                // Cash sales (TM)
                var cashSales = query.Where(x => x.PaymentMethod == "TM")
                                    .Select(x => new { x.BillPaid })
                                    .ToList();

                // Card sales
                var cardSales = query.Where(x => x.PaymentMethod == "Card")
                                    .Select(x => new { x.BillPaid })
                                    .ToList();

                // Mixed sales (TM/CK)
                var mixedSales = query.Where(x => x.PaymentMethod == "TM/CK")
                                     .Select(x => new { x.BillPaid })
                                     .ToList();

                // Calculate sums properly
                decimal cashAmount = cashSales.Sum(x => x.BillPaid) ;
                decimal cardAmount = cardSales.Sum(x => x.BillPaid);
                decimal mixedAmount = mixedSales.Sum(x => x.BillPaid);
                int cashCount = cashSales.Count;
                int cardCount = cardSales.Count;
                int mixedCount = mixedSales.Count;

                return new
                {
                    cashAmount = cashAmount,
                    cashCount = cashCount,
                    cardAmount = cardAmount,
                    cardCount = cardCount,
                    mixedAmount = mixedAmount,
                    mixedCount = mixedCount,
                    totalAmount = cashAmount + cardAmount + mixedAmount,
                    totalCount = cashCount + cardCount + mixedCount
                };
            }
        }
        public FileContentResult PrintSO2(string id)
        {
            //var storeId = Session["StoreId"] as string;
            //if (storeId == null)
            //{
            //    // Set an appropriate error message
            //    string errorMessage = "Store ID is not found in the session.";

            //    // Return a simple text file with the error message
            //    byte[] fileContents = System.Text.Encoding.UTF8.GetBytes(errorMessage);
            //    return new FileContentResult(fileContents, "text/plain")
            //    {
            //        FileDownloadName = "Error.txt"
            //    };
            //}
            id = Decode(id);
            LocalReport localreport = new LocalReport();
            localreport.ReportPath = Server.MapPath("~/Reports/Report3.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "ReportDataSet";
            reportDataSource.Value = null;//db.vSaleOrders.Where(x=> x.Id==id);
            localreport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";
            Warning[] warnings;
            string[] streams;
            byte[] renderBytes;

            renderBytes = localreport.Render(reportType, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            Response.AddHeader("content-disposition", "attachment; filename=Urls." + fileNameExtension);

            return File(renderBytes, mimeType);

        }
        //public FileContentResult PrintSO3(string id, string customerName, string customerEmail, string customerAddress, string posName)
        public FileContentResult PrintSO3(string id)
        {
            //string codes = StaticDto.Data.code;
            dynamic StaticDtoss = StaticDto.Data;
            //string jsonResponseStrings = ViewBag.LLLL as string;
            //dynamic aaa = _jsonResponseWebservice;
            //var jsonResponseWebservicess = TempData["JsonResponseWebservice"] as string;
            //if (!string.IsNullOrEmpty(jsonResponseWebservicess))
            //{
            //    Console.WriteLine("Data = " + jsonResponseWebservicess);
            //}
            //if (TempData["JsonResponseWebservice"] != null)
            //{
            //    dynamic jsonResponseWebservice = JsonConvert.DeserializeObject(TempData["JsonResponseWebservice"].ToString());
            //}
            // Retrieve the JSON response stored in TempData
            //string jsonResponseString = TempData["JsonResponseWebservice"] as string;

            //// Initialize a dynamic variable to hold the deserialized response
            //dynamic jsonResponseWebservice = null;

            //// Check if TempData has the response and deserialize it
            //if (!string.IsNullOrEmpty(jsonResponseString))
            //{
            //    jsonResponseWebservice = JsonConvert.DeserializeObject(jsonResponseString);
            //}

            //// Example usage of jsonResponseWebservice
            //if (jsonResponseWebservice != null)
            //{
            //    Console.WriteLine("Data = " + jsonResponseWebservice);
            //    // You can access properties here, e.g.:
            //    string message = jsonResponseWebservice.message;
            //    string buyerDisplayName = jsonResponseWebservice.data.inv_buyerDisplayName;
            //    // Continue processing as needed...
            //}
            //else
            //{
            //    Console.WriteLine("No valid JSON response found in TempData.");
            //}





            string _customerName = TempData["_CustomerName"] as string;
            //string _saleOrderAmountWithVat = TempData["_SaleOrderAmountWithVaT"] as string;
            string _customerEmail = TempData["_CustomerEmail"] as string;
            string _customerAddress = TempData["_CustomerAddress"] as string;
            string _companyName = TempData["_CompanyName"] as string;
            string _customerPosName = TempData["_POSName"] as string;
            string _customerPosAddress = TempData["_POSAddress"] as string;
            string _customerPosPhoneNumber = TempData["_POSPhoneNumber"] as string;
            string _customerVatNumber = TempData["_CustomerVatNumber"] as string;
            string _customerCompanyAddress = TempData["_CustomerCompanyAddress"] as string;
            //string _tgtttbso = TempData["_Tgtttbso"] as string;

            string _invoiceSeries = TempData["_InvoiceSeries"] as string;
            string _invoiceNumber = TempData["_InvoiceNumber"] as string;
            string _macqt = TempData["_Macqt"] as string;
            //string _sbmat = TempData["_Sobaomat"] as string;
            string _sbmat = TempData["_Sobaomat"] as string;  // Ensure the key matches exactly

            //dynamic tempDataResponse1 = JsonConvert.DeserializeObject(TempData["JsonResponseWebservice"].ToString());
            TempData.Keep("_InvoiceSeries");
            TempData.Keep("_InvoiceNumber");
            TempData.Keep("_Macqt");
            TempData.Keep("_Sobaomat");
            //string _jsonResponseWebservice = TempData["_JsonResponseWebservice"] as string;

            //var jsonResponseWebservice = Session["JsonResponse"];
            //string _webserviceDownError = TempData["_WebserviceDownError"] as string;

            //if (id.Length > 36)
            //{
            //    id = Decode(id);
            //}
            int SOSerial = (int)db.SOes.FirstOrDefault(x => x.Id == id).SOSerial;

            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType = "application/pdf";//"application/octet-stream"
            string encoding = string.Empty;
            string extension = "pdf";
            byte[] bytes;

            // Setup the report viewer object and get the array of bytes
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            bool hasCustomerInfo = !string.IsNullOrEmpty(_customerName) ||
                       !string.IsNullOrEmpty(_customerEmail) ||
                       !string.IsNullOrEmpty(_customerAddress);
            //SO sO = db.SOes.FirstOrDefault(x => x.Id == id);
            Employee emp = db.Employees.FirstOrDefault(x => x.Id == db.SOes.FirstOrDefault(y => y.Id == id).EmployeeId);
            if (emp.Login == "LahoreKarachi")
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Sale_LahoreKarachi.rdlc"); }
            else
                //{ viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Sale_Receipt.rdlc"); }
                //{ viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Sale_Receipt.rdlc"); }
                viewer.LocalReport.ReportPath = hasCustomerInfo
                ? Server.MapPath("~/Reports/Sale_Receipt.rdlc")
                : Server.MapPath("~/Reports/Sale_ReceiptWoutCustomer.rdlc");

            ReportDataSource reportDataSource = new ReportDataSource();
            viewer.LocalReport.EnableHyperlinks = true;
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = db.spSOReport(id).AsEnumerable();//db.spSOReceipt;// BusinessDataSetTableAdapters
            viewer.LocalReport.DataSources.Add(reportDataSource);
            //viewer.LocalReport.SetParameters(new ReportParameter("SaleOrderID", id));
            // Set the report parameters for Customer Information
            viewer.LocalReport.SetParameters(new ReportParameter[]
            {
        //new ReportParameter("SaleOrderID", id),                 // Assuming you already have this parameter
        //new ReportParameter("CustomerName", "N/A"),  // Pass Customer Name
        //new ReportParameter("CustomerEmail","N/A"), // Pass Customer Email
        //new ReportParameter("CustomerAddress", "N/A"), // Pass Customer Address
        //new ReportParameter("POSName",  "N/A"),   // Pass POS Name
        //new ReportParameter("CustomerVatNumber", "N/A")   // Pass POS Name 
        new ReportParameter("SaleOrderID", id),

        new ReportParameter("CustomerName", _customerName ?? "-"),  //// Pass Customer Name
        //new ReportParameter("SaleOrderAmountWithVaT", _saleOrderAmountWithVat ?? "-"),
        //new ReportParameter("CustomerEmail", _customerEmail ?? "N/A"), //// Pass Customer Email
        new ReportParameter("CustomerEmail", _customerEmail ?? "-"), //// Pass Customer Email
        new ReportParameter("CustomerAddress", _customerAddress ?? "-"), //// Pass Customer Address
        new ReportParameter("CompanyName", _companyName ?? "-"),
        new ReportParameter("POSName", _customerPosName ?? "-"),   // Pass POS Name
        new ReportParameter("POSAddress", _customerPosAddress ?? "-"),   // Pass POS Name
        new ReportParameter("POSPhoneNumber", _customerPosPhoneNumber ?? "-"),   // Pass POS Name
        new ReportParameter("CustomerVatNumber", _customerVatNumber ?? "-"),  // Pass POS Name
        new ReportParameter("CustomerCompanyAddress", _customerCompanyAddress ?? "-"),
        new ReportParameter("InvoiceSeries", _invoiceSeries ?? "-"),  // Pass POS Name
        new ReportParameter("InvoiceNumber", _invoiceNumber ?? "-"),  // Pass POS Name
        new ReportParameter("Macqt", _macqt ?? "-"),  // Pass POS Name
        new ReportParameter("Sobaomat", _sbmat ?? "-"),  // Pass POS Name
       
        //new ReportParameter("SaleOrderID", id),
        //new ReportParameter("CustomerName", _customerName ?? "N/A"),  //// Pass Customer Name
        ////new ReportParameter("CustomerEmail", _customerEmail ?? "N/A"), //// Pass Customer Email
        //new ReportParameter("CustomerEmail", _customerEmail ?? "-"), //// Pass Customer Email
        //new ReportParameter("CustomerAddress", _customerAddress ?? "N/A"), //// Pass Customer Address
        //new ReportParameter("POSName", _customerPosName ?? "N/A"),   // Pass POS Name
        //new ReportParameter("CustomerVatNumber", _customerVatNumber ?? "N/A"),  // Pass POS Name

        //new ReportParameter("InvoiceSeries", _invoiceSeries ?? "N/A"),  // Pass POS Name
        //new ReportParameter("InvoiceNumber", _invoiceNumber ?? "N/A"),  // Pass POS Name
        //new ReportParameter("Macqt", macqt ?? "N/A"),  // Pass POS Name
        //new ReportParameter("Sobaomat", _sbmat ?? "N/A"),  // Pass POS Name
            });
            viewer.LocalReport.Refresh();
            //byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            if (FileTempered(viewer.LocalReport.ReportPath) == true)
            {
                bytes = Encoding.ASCII.GetBytes("Report not found. Contact +92 300 88 55 011");
            }
            else
            { bytes = viewer.LocalReport.Render("PDF"); }//, null, out mimeType, out encoding, out extension, out streamIds, out warnings);}

            //// Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; filename=" + "Sale Receipt " + SOSerial.ToString("D4") + "." + extension);
            //Response.Write(@"<script type='text/javascript' language='javascript'>window.open('page.html','_blank').focus();</script>");
            Response.BinaryWrite(bytes); // create the file
            //Response.End();
            ////return Response.Flush(); // send it to the client to download

            //return File(bytes, "application/pdf");

            return new FileContentResult(bytes, mimeType);

            //System.IO.Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            ////stream.Seek(0, System.IO.SeekOrigin.Begin);
            //System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
            //byte[] getBytes = null;
            //getBytes = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
            //HttpContext.Response.AddHeader("content-disposition", "inline; filename=" + "SOSR.pdf");

            //return File(getBytes, "application/pdf");

        }

        public decimal GetPreviousBalance(int id)
        {
            IQueryable lstSO = db.SOes.Where(x => x.CustomerId == id);

            //lstSO.ForEachAsync(c => { c. = 0; c.GroupID = 0; c.CompanyID = 0; });
            decimal SOAmount = 0;
            decimal SRAmount = 0;
            foreach (SO itm in lstSO)
            {
                if (itm.SaleReturn == false)
                {
                    SOAmount += (decimal)itm.SaleOrderAmount;
                }
                else
                {
                    SRAmount += (decimal)itm.SaleOrderAmount;
                }

            }

            return (SOAmount - SRAmount);
        }


        // GET: SOes/Edit/5
        public ActionResult Edit(string id, bool update)
        {

            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            //id = new string( Encoding.UTF8.GetString(BytesArr).ToCharArray());
            //id = Encryption.Decrypt(id,"BZNS");

            decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;

            List<MySaleType> myOptionLst = new List<MySaleType> {
                            new MySaleType {
                                Text = "Order",
                                Value = "false"
                            },
                            new MySaleType {
                                Text = "Return",
                                Value = "true"
                            }
                        };
            ViewBag.OptionLst = myOptionLst;

            ////////////////
            List<MyPaymentMethod> myPaymentOptionLst = new List<MyPaymentMethod> {
                            new MyPaymentMethod {
                                Text = "Cash",
                                Value = "Cash"
                            },
                            new MyPaymentMethod {
                                Text = "Online",
                                Value = "Online"
                            },
                            new MyPaymentMethod {
                                Text = "Cheque",
                                Value = "Cheque"
                            },
                            new MyPaymentMethod {
                                Text = "Other",
                                Value = "Other"
                            }
                        };

            ViewBag.PaymentMethodOptionLst = myPaymentOptionLst;

            List<MyUnitType> myUnitTypeOptionList = new List<MyUnitType> {
                            new MyUnitType {
                                Text = "Piece",
                                Value = "false"
                            },
                            new MyUnitType {
                                Text = "Pack",
                                Value = "true"
                            }
                        };

            ViewBag.UnitTypeOptionList = myUnitTypeOptionList;
            string iid = Decode(id);
            //Payment pmnt = db.Payments.Where(x => x.SOId == iid).FirstOrDefault();
            //if (pmnt != null)
            //{
            //    ViewBag.paymentMethod = pmnt.PaymentMethod;
            //    ViewBag.paymentRemarks = pmnt.Remarks;
            //}
            ///////////////////

            id = Decode(id);

            SO sO = db.SOes.Find(id);
            if (sO == null)
            {
                return HttpNotFound();
            }
            SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();
            List<SOD> sod = db.SODs.Where(x => x.SOId == id).ToList();
            saleOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7);
            saleOrderViewModel.Customers = DAL.dbCustomers;
            saleOrderViewModel.SaleOrderDetail = sod;
            sO.Id = Encryption.Encrypt(sO.Id, "BZNS");
            saleOrderViewModel.SaleOrder = sO;
            int orderQty = 0;
            int orderQtyPiece = 0;//orderQtyPiece 'P for piece' 
            int returnQty = 0;
            int returnQtyPiece = 0;//orderQtyPiece 'P for piece' 
            foreach (var item in sod)
            {
                if (sO.SaleReturn == false)
                {
                    if (item.IsPack == true)
                    {//Pack
                        orderQty += (int)item.Quantity;
                    }
                    else
                    {//Item
                        orderQtyPiece += (int)item.Quantity;
                    }
                }
                else
                {
                    if (item.IsPack == true)
                    {//Pack
                        returnQty += (int)item.Quantity;
                    }
                    else
                    {//Item
                        returnQtyPiece += (int)item.Quantity;
                    }

                }

            }
            ViewBag.orderQty = orderQty;
            ViewBag.orderQtyPiece = orderQtyPiece;
            ViewBag.returnQty = returnQty;
            ViewBag.returnQtyPiece = returnQtyPiece;
            //ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", sO.CustomerId);
            ViewBag.CustomerName = sO.Customer.Name;
            ViewBag.CustomerAddress = sO.Customer.Address;
            ViewBag.CompanyName = sO.Customer.CompanyName;
            decimal subTotal = (decimal)(sO.SaleOrderAmount - sO.Discount);
            ViewBag.SubTotal = subTotal;
            ViewBag.Total = subTotal + (decimal)sO.PrevBalance;
            ViewBag.IsUpdate = update;
            ViewBag.IsReturn = sO.SaleReturn.ToString().ToLower();
            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");
            return View(saleOrderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Prefix = "SaleOrder", Include = "BillAmount,Balance,BillPaid,Discount")] SO sO, [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,Quantity")] List<SOD> sOD)
        public ActionResult Edit(SaleOrderViewModel saleOrderViewModel1)
        {
            SO newSO = saleOrderViewModel1.SaleOrder;
            List<SOD> newSODs = saleOrderViewModel1.SaleOrderDetail;
            if (ModelState.IsValid)
            {
                newSO.Id = Encryption.Decrypt(saleOrderViewModel1.SaleOrder.Id, "BZNS");//
                SO sO = db.SOes.Where(x => x.Id == newSO.Id).FirstOrDefault();
                //sO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));//
                //sO.SaleReturn = false;//
                decimal beforeEditSOBalance = (decimal)sO.Balance;
                sO.Date = newSO.Date;
                sO.BillAmount = newSO.BillAmount;//
                sO.Discount = newSO.Discount;//
                sO.BillPaid = newSO.BillPaid;//
                sO.Balance = newSO.Balance;//
                sO.Remarks = newSO.Remarks;//
                sO.Remarks2 = newSO.Remarks;//
                sO.PaymentMethod = newSO.PaymentMethod;
                sO.PaymentDetail = newSO.PaymentDetail;
                sO.BankAccountId = newSO.BankAccountId;
                //sO.SOSerial = newSO.SOSerial;//should be unchanged

                ///////////////////////////////////////////

                //Customer cust = db.Customers.FirstOrDefault(x => x.Id == newSO.CustomerId);
                Customer customer = db.Customers.Where(x => x.Id == newSO.CustomerId).FirstOrDefault();
                if (customer == null)
                {//its means new customer(not in db)
                 //sO.CustomerId = 10;
                 //int maxId = db.Customers.Max(p => p.Id);
                    customer = saleOrderViewModel1.Customer;
                    decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;

                    customer.Id = maxId;
                    //customer.Balance = newSO.Balance;
                    db.Customers.Add(customer);
                }
                else
                {
                    db.Entry(customer).State = EntityState.Modified;
                }

                if (sO.CustomerId != newSO.CustomerId)
                {//some other db customer
                 //first revert the previous customer balance 
                    Customer oldCustomer = db.Customers.Where(x => x.Id == sO.CustomerId).FirstOrDefault();
                    // in new model where every so has its own balance. this line will cause prbolem as prev. balance concept ended
                    oldCustomer.Balance = db.SOes.Where(x => x.Id == sO.Id).FirstOrDefault().PrevBalance;
                    db.Entry(oldCustomer).State = EntityState.Modified;
                }

                sO.PrevBalance = newSO.PrevBalance;//
                // assign balance of this customer
                //Customer customer = db.Customers.Where(x => x.Id == newSO.CustomerId).FirstOrDefault();
                if (customer.Balance == null) customer.Balance = 0;

                if (sO.SaleReturn == false)
                {
                    customer.Balance -= beforeEditSOBalance;//revert the old balnace first
                    customer.Balance += newSO.Balance;
                }
                else
                {
                    customer.Balance += beforeEditSOBalance;//revert the old balnace first
                    customer.Balance -= newSO.Balance;
                }

                //assign customer and customerId in SO
                sO.CustomerId = newSO.CustomerId;
                sO.Customer = customer;

                /////////////////////////////////////////////////////////////////////////////



                List<SOD> oldSODs = db.SODs.Where(x => x.SOId == newSO.Id).ToList();

                //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this is simple and stateforward.
                foreach (SOD sod in oldSODs)
                {
                    Product product = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);

                    if (sod.SaleType == false)//sale
                    {
                        //product.Stock += sod.Quantity;

                        if (sod.IsPack == false)
                        {
                            decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                            product.Stock += qty;
                        }
                        else
                        {
                            product.Stock += (int)sod.Quantity * sod.PerPack;
                        }


                    }
                    else//return
                    {
                        //product.Stock -= sod.Quantity;

                        if (sod.IsPack == false)
                        {
                            decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                            product.Stock -= qty;
                        }
                        else
                        {
                            product.Stock -= (int)sod.Quantity * sod.PerPack;
                        }



                    }
                    db.Entry(product).State = EntityState.Modified;
                }

                db.SODs.RemoveRange(oldSODs);
                //////////////////////////////////////////////////////////////////////////////

                sO.SaleOrderAmount = 0;

                sO.SaleOrderQty = 0;

                sO.Profit = 0;
                int sno = 0;

                if (newSODs != null)
                {

                    foreach (SOD sod in newSODs)
                    {
                        sno += 1;
                        sod.SODId = sno;
                        sod.SO = sO;
                        sod.SOId = sO.Id;

                        Product product = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                        //sod.salePrice is now from view
                        //sod.SalePrice = product.SalePrice;
                        //dont do this. when user even just open a old bill and just press save. and price was updated after that old bill. all calculations gets wrong
                        //if we dont do this then error in product wise sale
                        //sod.PurchasePrice = product.PurchasePrice;
                        if (sod.Quantity == null) { sod.Quantity = 0; }
                        sod.OpeningStock = product.Stock;
                        if (sod.SaleType == false)//sale
                        {

                            if (sod.IsPack == false)
                            {//piece
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                product.Stock -= qty;

                                sO.SaleOrderQty += qty;//(int)sod.Quantity;
                                sO.Profit += (qty * sod.SalePrice) - (decimal)(qty * sod.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {//pack

                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock -= (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * sod.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }

                        }
                        else//return
                        {
                            if (sod.IsPack == false)
                            {
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice);
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;
                                sO.SaleOrderQty += qty;//(int)sod.Quantity;
                                sO.Profit += (qty * sod.SalePrice) - (decimal)(qty * sod.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock += (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * sod.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }

                        }
                        sod.Product = null;
                    }
                    sO.Profit -= (decimal)sO.Discount;
                    db.Entry(sO).State = EntityState.Modified;
                    db.Entry(sO).Property(x => x.SOSerial).IsModified = false;
                    db.SODs.AddRange(newSODs);

                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", sO.CustomerId);
            //return View(sO);
            SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();

            saleOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7);
            return View(saleOrderViewModel);
            //return View();
        }

        private string Decode(string id)
        {
            byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            id = new string(Encoding.UTF8.GetString(BytesArr).ToCharArray());
            id = Encryption.Decrypt(id, "BZNS");
            return id;
        }
        //Commentted due to logout issue
        public async Task<ActionResult> USRLWB(string taxCode, int storeId)
        {
            try
            {
                // Log entry to the method
                System.Diagnostics.Debug.WriteLine("USRLWB action method hit.");

                //var loginToWebService = LoginToWebService();  // Make this asynchronous
                var loginToWebService = await LoginToWebServiceAsync();  // Make this asynchronous

                //    if (loginToWebService == null)
                //        return new ContentResult { Content = JsonConvert.SerializeObject(new { Success = false, Message = "Invalid login attempt to web service, please use correct credentials" }), ContentType = "application/json" };

                //    dynamic jsonResponse = JsonConvert.DeserializeObject(loginToWebService.ContentType);
                //    string authToken = jsonResponse?.token;

                //    if (string.IsNullOrEmpty(authToken))
                //        return new ContentResult { Content = JsonConvert.SerializeObject(new { Success = false, Message = "Authentication token is missing" }), ContentType = "application/json" };

                //    string tax = "0401485182";
                //    var getServiceCustomerDetails = await GetCompanyByTextCode(authToken, tax); // Await the asynchronous call

                //    // Log or inspect serialized response if needed
                //    string serializedDetails = JsonConvert.SerializeObject(getServiceCustomerDetails);
                //    System.Diagnostics.Debug.WriteLine("Serialized Response: " + serializedDetails);

                //    return new ContentResult { Content = JsonConvert.SerializeObject(new { Success = true, Response = getServiceCustomerDetails }), ContentType = "application/json" };
                //}
                //if (loginToWebService == null || string.IsNullOrEmpty(loginToWebService.ContentType))
                //{
                //    return new ContentResult
                //    {
                //        Content = JsonConvert.SerializeObject(new { Success = false, Message = "Invalid login attempt to web service, please use correct credentials" }),
                //        ContentType = "application/json"
                //    };
                //}

                if (loginToWebService == null || loginToWebService.Data == null)
                {
                    return new ContentResult
                    {
                        Content = JsonConvert.SerializeObject(new
                        {
                            Success = false,
                            Message = "Invalid login attempt to web service, please use correct credentials"
                        }),
                        ContentType = "application/json"
                    };
                }

                // Convert Data property to JSON string
                string dataString = JsonConvert.SerializeObject(loginToWebService.Data);

                // Deserialize the JSON string to get the token
                dynamic jsonResponse = JsonConvert.DeserializeObject(dataString);
                string authToken = jsonResponse?.Token;  // Make sure the key name matches exactly with the response

                //dynamic jsonResponse = JsonConvert.DeserializeObject(loginToWebService.ContentType);
                //string authToken = jsonResponse?.token;

                ////if (string.IsNullOrEmpty(authToken))
                ////{
                ////    return new ContentResult
                ////    {
                ////        Content = JsonConvert.SerializeObject(new { Success = false, Message = "Authentication token is missing" }),
                ////        ContentType = "application/json"
                ////    };
                ////}

                if (string.IsNullOrEmpty(authToken))
                {
                    return new ContentResult
                    {
                        Content = JsonConvert.SerializeObject(new { Success = false, Message = "Authentication token is missing" }),
                        ContentType = "application/json"
                    };
                }

                //string tax = "0401485182";
                var getServiceCustomerDetails = await GetCompanyByTextCode(authToken, taxCode); // Await the asynchronous call

                // Log or inspect serialized response if needed
                string serializedDetails = JsonConvert.SerializeObject(getServiceCustomerDetails);
                System.Diagnostics.Debug.WriteLine("Serialized Response: " + serializedDetails);

                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(new { Success = true, Response = getServiceCustomerDetails }),
                    ContentType = "application/json"
                };
            }
            catch (Exception ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.ToString());
                return new ContentResult { Content = JsonConvert.SerializeObject(new { Success = false, Message = "An internal error occurred.", Details = ex.Message }), ContentType = "application/json" };
            }
        }
        public async Task<object> GetCompanyByTextCode(string authToken, string taxCode)
        {
            try
            {
                string url = $"http://mst.minvoice.com.vn/api/System/SearchTaxCode?tax={taxCode}";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    client.Timeout = TimeSpan.FromSeconds(100); // Set timeout

                    Console.WriteLine("Sending request to URL: " + url);
                    HttpResponseMessage response = await client.GetAsync(url);
                    Console.WriteLine("Response Status Code: " + response.StatusCode);

                    if (!response.IsSuccessStatusCode)
                        return new { Success = false, Message = $"Error: {response.ReasonPhrase}" };

                    Console.WriteLine("Reading Response Content...");
                    string responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response Content Read Successfully");

                    return new { Success = true, Response = responseData }; // Return the response data directly
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                return new { Success = false, Message = "An error occurred.", Details = ex.Message };
            }
        }










        //public async Task<JsonResult> USRLWB()
        //{
        //    var loginToWebService = LoginToWebService();

        //    if (loginToWebService == null)
        //        return Json(new { Success = false, Message = "Invalid login attempt to web service, please use correct credentials" });

        //    dynamic jsonResponse = JsonConvert.DeserializeObject(loginToWebService.ContentType);
        //    string authToken = jsonResponse.token;

        //    if (string.IsNullOrEmpty(authToken))
        //        return Json(new { Success = false, Message = "Authentication token is missing" });

        //    string tax = "0401485182";
        //    var addWebServiceCustomerDetails = await GetCompanyByTextCode(authToken, tax);

        //    return Json(new { Success = true, Response = addWebServiceCustomerDetails });
        //}

        //public async Task<JsonResult> GetCompanyByTextCode(string authToken, string taxCode)
        //{
        //    string url = $"http://mst.minvoice.com.vn/api/System/SearchTaxCode?tax={taxCode}";

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        //        HttpResponseMessage response = await client.GetAsync(url);

        //        if (!response.IsSuccessStatusCode)
        //            return Json(new { Success = false, Message = $"Error: {response.ReasonPhrase}", Response = response });

        //        string responseData = await response.Content.ReadAsStringAsync();

        //        try
        //        {
        //            var result = JsonConvert.DeserializeObject(responseData);
        //            return Json(new { Success = true, Response = result });
        //        }
        //        catch (JsonException)
        //        {
        //            return Json(new { Success = false, Message = "Received non-JSON response.", Response = responseData });
        //        }
        //    }
        //}

        // GET: SOes/Delete/5
        public ActionResult Delete(string id)
        {
            return null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = Decode(id);
            SO sO = db.SOes.Find(id);
            if (sO == null)
            {
                return HttpNotFound();
            }
            return View(sO);
        }

        // POST: SOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            return null;
            id = Decode(id);

            List<SOD> oldSODs = db.SODs.Where(x => x.SOId == id).ToList();
            //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this si simple and stateforward.
            foreach (SOD sod in oldSODs)
            {
                Product product = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                product.Stock += sod.Quantity;
            }
            db.SODs.RemoveRange(oldSODs);

            SO sO = db.SOes.Find(id);
            db.SOes.Remove(sO);
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
        bool FileTempered(string filePath)
        {
            return false;
            //DateTime modificationDate = System.IO.File.GetLastWriteTime(filePath);
            //long fileSize = new System.IO.FileInfo(filePath).Length;
            //return (fileSize != 60576) ? true : false;


        }

        void EnterProfit()
        {

            foreach (SO so in db.SOes)
            {
                //decimal totalPurchasePrice = 0;
                List<SOD> lstSODItems = db.SODs.Where(x => x.SOId == so.Id).ToList();

                decimal soProfit = 0;
                foreach (SOD sod in lstSODItems)

                {
                    Product prod = DAL.dbProducts.Where(x => x.Id == sod.ProductId).FirstOrDefault();

                    if (sod.SaleType == true)//return
                    {
                        //totalPurchasePrice += (decimal)(prod.PurchasePrice * sod.Quantity);
                        soProfit -= (decimal)(sod.Quantity * prod.SalePrice) - (decimal)(sod.Quantity * prod.PurchasePrice); //- (decimal)(so.Discount);
                    }
                    else
                    {
                        //totalPurchasePrice += (decimal)(prod.PurchasePrice * sod.Quantity);
                        soProfit += (decimal)(sod.Quantity * prod.SalePrice) - (decimal)(sod.Quantity * prod.PurchasePrice); //- (decimal)(so.Discount);
                    }

                    sod.PurchasePrice = prod.PurchasePrice;
                    db.Entry(sod).State = EntityState.Modified;
                }
                so.Profit = soProfit - (decimal)so.Discount;

                //so.Profit = (decimal)(so.SaleOrderAmount - so.Discount) - totalPurchasePrice;
                db.Entry(so).State = EntityState.Modified;
            }
            db.SaveChanges();
        }
        public ActionResult ProductDetailsCustomer()
        {
            // Retrieve the cookie
            //var cookie = Request.Cookies["selectedProducts"];
            //var products = new List<CustomerDetailsViewModel>();

            //if (cookie != null)
            //{
            //    // Parse the cookie value (assuming it is a JSON string)
            //    var cookieValue = cookie.Value;

            //    // Deserialize the JSON string into a list of CustomerDetailsViewModel objects
            //    products = JsonConvert.DeserializeObject<List<CustomerDetailsViewModel>>(cookieValue);
            //}

            // Pass the product list to the view
            //return View(products);
            return View();
        }
        //public ActionResult SendProductUpdate(string productDetails)
        //{
        //    var context = GlobalHost.ConnectionManager.GetHubContext<PhevaHub>();
        //    context.Clients.All.ReceiveProductUpdate(productDetails);  // Broadcast to all clients
        //    return Json(new { success = true });
        //}
    }
    // Product model for deserialization


}
