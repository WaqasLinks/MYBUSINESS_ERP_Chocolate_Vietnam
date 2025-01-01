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
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using Microsoft.Reporting.WebForms;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{

    //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    //[NoCache]
    public class SaleQuotationController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
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

            IQueryable<SO> sOes = db.SOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate && x.SaleReturn == false).Include(s => s.Customer);
            //sOes = sOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate);
            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref sOes);
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
            //var salesOrderQuotation = sOes.Where(x => x.StoreId == parseId).OrderByDescending(i => i.Date).ToList(); //commented due to session issue
            var salesOrderQuotation = sOes.Where(x => x.StoreId == 1).OrderByDescending(i => i.Date).ToList();
            return View(salesOrderQuotation);
        }
        public ActionResult IndexReturn()
        {
            //EnterProfit();
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
            return View(sOes.OrderByDescending(i => i.Date).ToList());
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

                TotalBalance += (decimal)cust.Balance;

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

        public ActionResult Create(string IsReturn)
        {

            //ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name");
            //ViewBag.Products = db.Products;

            //int maxId = db.Customers.Max(p => p.Id);
            decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;
            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.MalaysiaTime = DateTime.UtcNow.AddHours(8);
            SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();
            saleOrderViewModel.Customers = DAL.dbCustomers;
            saleOrderViewModel.Products = DAL.dbProducts.Where(x => x.Saleable == true);
            //bool IsReturn1 = true;
            ViewBag.IsReturn = IsReturn;
            //string isReturn1 = "true";
            //ViewBag.isReturn = isReturn1;
            ViewBag.ReportId = TempData["ReportId"] as string;
            return View(saleOrderViewModel);
        }




        //[OutputCache(NoStore = true, Duration = 0)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "Customer", Include = "Name,Address")] Customer Customer, [Bind(Prefix = "SaleOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,CustomerId,Remarks,Remarks2,PaymentMethod,PaymentDetail,SaleReturn,BankAccountId,Date")] SO sO, [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,SalePrice,Quantity,SaleType,PerPack,IsPack,Product")] List<SOD> sOD, FormCollection collection)

        {

            string SOId = string.Empty;
            //SO sO = new SO();
            if (ModelState.IsValid)
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

                Customer cust = db.Customers.FirstOrDefault(x => x.Id == sO.CustomerId);

                if (cust == null)
                {//its means new customer
                    //sO.CustomerId = 10;
                    //int maxId = db.Customers.Max(p => p.Id);
                    decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;
                    Customer.Id = maxId;
                    Customer.Balance += sO.Balance;
                    //Customer.StoreId = parseId;
                    Customer.StoreId = 1;
                    db.Customers.Add(Customer);
                    //db.SaveChanges();
                }
                else
                {//its means old customer. old customer balance should be updated.
                 //Customer.Id = (int)sO.CustomerId;
                    if (cust.Balance == null) cust.Balance = 0;
                    if (sO.SaleReturn == false)
                    {
                        cust.Balance += sO.Balance;
                    }
                    else
                    {
                        cust.Balance -= sO.Balance;
                    }
                    //cust.StoreId = parseId; //commented due to session issue
                    cust.StoreId = 1;
                    db.Entry(cust).State = EntityState.Modified;
                    //db.SaveChanges();




                    //Payment payment = new Payment();
                    //payment = db.Payments.Find(orderId);
                    //payment.Status = true;
                    //db.Entry(payment).State = EntityState.Modified;
                    //db.SaveChanges();

                }
                ////////////////////////////////////////
                BankAccount bankAccount = db.BankAccounts.FirstOrDefault(x => x.Id == sO.BankAccountId);
                bankAccount.Balance += sO.BillPaid;
                db.BankAccounts.Attach(bankAccount);
                db.Entry(bankAccount).Property(x => x.Balance).IsModified = true;
                ////////////////////////////////////////
                ////////////////////////////////////////
                //int maxId = db.SOes.Max(p => p.Auto);
                decimal maxId1 = (int)db.SOes.DefaultIfEmpty().Max(p => p == null ? 0 : p.SOSerial);
                maxId1 += 1;
                sO.SOSerial = maxId1;

                //sO.Date = DateTime.Now;
                if (string.IsNullOrEmpty(Convert.ToString(sO.Date)))
                {
                    sO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
                }

                //sO.SaleReturn = false;
                sO.Id = System.Guid.NewGuid().ToString().ToUpper();
                sO.SaleOrderAmount = 0;

                sO.SaleOrderQty = 0;

                sO.Profit = 0;
                //sO.StoreId = parseId; //commented due to session issue
                sO.StoreId = 1;
                Employee emp = (Employee)Session["CurrentUser"];
                sO.EmployeeId = emp.Id;

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
                                Saleable = true,
                                PerPack = 1,
                                ShowIn = "S"
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
                        sod.OpeningStock = product.Stock;
                        sod.PerPack = 1;
                        if (sod.SaleType == false)//sale
                        {

                            if (sod.IsPack == false)
                            {//piece
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                product.Stock -= qty;

                                sO.SaleOrderQty += qty;//(int)sod.Quantity;
                                sO.Profit += (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else//return
                            {

                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock -= (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
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
                                sO.Profit += (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock += (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }

                        }
                        sod.Product = null;
                        //db.SODs.Attach(sod);
                        //db.Entry(sod).Property(x => x.Product).IsModified = false;
                    }

                    sO.Profit -= (decimal)sO.Discount;


                    db.SODs.AddRange(sOD);
                }
                db.SaveChanges();
                TempData["ReportId"] = sO.Id;
                SOId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(sO.Id, "BZNS")));
            }
            return RedirectToAction("Create", new { IsReturn = "false" });
        }


        public FileContentResult PrintSO2(string id)
        {
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
        public FileContentResult PrintSO3(string id)
        {
            if (id.Length > 36)
            {
                id = Decode(id);
            }
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

            //SO sO = db.SOes.FirstOrDefault(x => x.Id == id);
            Employee emp = db.Employees.FirstOrDefault(x => x.Id == db.SOes.FirstOrDefault(y => y.Id == id).EmployeeId);
            if (emp.Login == "LahoreKarachi")
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Sale_LahoreKarachi.rdlc"); }
            else
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Sale_Receipt.rdlc"); }


            ReportDataSource reportDataSource = new ReportDataSource();

            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = db.spSOReport(id).AsEnumerable();//db.spSOReceipt;// BusinessDataSetTableAdapters
            viewer.LocalReport.DataSources.Add(reportDataSource);
            viewer.LocalReport.SetParameters(new ReportParameter("SaleOrderID", id));
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
            saleOrderViewModel.Products = DAL.dbProducts.Where(x => x.Saleable == true);
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
                //sO.StoreId = parseId; //commented due to session issue
                sO.StoreId = 1;
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
                    //customer.StoreId = parseId; //commented due to session issue
                    customer.StoreId = 1;
                    //customer.Balance = newSO.Balance;
                    db.Customers.Add(customer);
                }
                else
                {
                    //customer.StoreId = parseId; //commented due to session issue
                    customer.StoreId = 1;
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

            saleOrderViewModel.Products = DAL.dbProducts.Where(x => x.Saleable == true);
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

    }

}
