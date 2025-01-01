using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Microsoft.Reporting.WebForms;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{

    //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    //[NoCache]
    public class RentController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        // GET: SOes
        public ActionResult Index()
        {
            //EnterProfit();
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            IQueryable<Rent> sOes = db.Rents.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Include(s => s.Customer);
            //sOes = sOes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate);
            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref sOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (Rent itm in sOes)
            {
                thisSerial = (int)itm.Customer.Rents.Max(x => x.SOSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.CustomerId))
                {
                    LstMaxSerialNo.Add(itm.Customer.Id, thisSerial);
                }


                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            ViewBag.Customers = db.Customers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
            return View(sOes.OrderByDescending(i => i.Date).ToList());
        }
        //public ActionResult SearchData(string custName, DateTime startDate, DateTime endDate)

        //public ActionResult SearchData(string custName, string startDate, string endDate)
        public ActionResult ProductRentStatus(string prodId, string startDate, string endDate)
        {
            int intProdId;
            DateTime dtStartDate;
            DateTime dtEndDate;
            //IQueryable<ProductDetail> selectedSOes = null;
            List<ProductDetail> selectedSOes = null;
            //if (prodId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            //{
            intProdId = Int32.Parse(prodId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndDate = DateTime.Parse(endDate);
            
            selectedSOes = db.ProductDetails.Where(so => so.ProductId == intProdId && ((dtStartDate >= so.RentStartDate && dtStartDate <= so.RentEndDate) || (dtEndDate >= so.RentStartDate && dtEndDate <= so.RentEndDate))).ToList(); 
            
            //return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());
            return PartialView("_ProductDetail", selectedSOes);//.ToList());
            //return PartialView("1");
        }
        public ActionResult SearchData(string custId, string startDate, string endDate)
        {

            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<Rent> selectedSOes = null;
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

                selectedSOes = db.Rents.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            if (custId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.Rents;//.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers data acornding to start end date
            if (custId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.Rents.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (custId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.Rents.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from defined start date to undefined end date
            if (custId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.Rents.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with all dates
            if (custId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intCustId = Int32.Parse(custId);
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.Rents.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customer with defined startdate and undefined end date
            if (custId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedSOes = db.Rents.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers with undifined start date with defined enddate
            if (custId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedSOes = db.Rents.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }
            GetTotalBalance(ref selectedSOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (Rent itm in selectedSOes)
            {
                thisSerial = (int)itm.Customer.Rents.Max(x => x.SOSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.CustomerId))
                {
                    LstMaxSerialNo.Add(itm.Customer.Id, thisSerial);
                }
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            return PartialView("_SelectedSOSR", selectedSOes.OrderByDescending(i => i.Date).ToList());

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
            ViewBag.Customers = db.Customers;
            //01-Jan-2019

            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

            IQueryable<Rent> sOes = db.Rents;//.Include(s => s.Customer);
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
            IQueryable<Rent> sOes = db.Rents;//.Include(s => s.Customer);
            //sOes = sOes.Where(x => x.CustomerId == custId && x.Date >= dtStartDate && x.Date <= dtEndtDate).OrderBy(i => i.Date).OrderBy(i => i.SOSerial).AsQueryable();






            int intCustId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<Rent> selectedSOes = null;
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
            ViewBag.Customers = db.Customers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");

            List<Rent> sOes = db.Rents.ToList();//.Include(s => s.Customer);

            //sOes = db.SOes.Where(x => x.RentDetails.Where(y => y.ProductId == productId));

            List<RentDetail> lstSODs = db.RentDetails.Where(x => x.ProductId == productId).ToList();
            List<Rent> lstSlectedSO = new List<Rent>();
            foreach (RentDetail lsod in lstSODs)
            {
                //do not add if already added
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.Rent);
                }
            }

            sOes = lstSlectedSO.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).ToList();
            foreach (Rent itm in sOes)
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


            //List<SO> sOes = db.Rents.ToList();//.Include(s => s.Customer);
            List<Rent> selectedSOes = null;

            selectedSOes = db.Rents.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate).ToList();
            /////////////////////////////////////////////////////////////////////////////////////////////////////////

            int intProdId;
            intProdId = Int32.Parse(prodId);

            //List<RentDetail> lstSODs = db.RentDetails.Where(x => x.ProductId == intProdId).ToList();

            List<Rent> newSOes = new List<Rent>();
            List<RentDetail> newSODs;
            foreach (Rent thisSO in selectedSOes)
            {

                newSODs = new List<RentDetail>();
                foreach (RentDetail thisSOD in thisSO.RentDetails)
                {
                    if (thisSOD.ProductId == intProdId)
                    {
                        newSODs.Add(thisSOD);
                    }

                }
                if (newSODs.Count > 0)
                {
                    thisSO.RentDetails = newSODs;
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
            IQueryable<Rent> sOes = db.Rents.Include(s => s.Customer);

            //sOes = db.Rents.Where(x => x.RentDetails.Where(y => y.ProductId == productId));

            List<RentDetail> lstSODs = db.RentDetails.Where(x => x.ProductId == productId && x.SaleType == false).ToList();
            List<Rent> lstSlectedSO = new List<Rent>();
            foreach (RentDetail lsod in lstSODs)
            {
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.Rent);
                }
            }

            sOes = lstSlectedSO.ToList().AsQueryable();
            foreach (Rent itm in sOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.ProductName = db.Products.FirstOrDefault(x => x.Id == productId).Name;
            return View("PerMonthSale", sOes.OrderBy(i => i.Date).ToList());
        }

        public ActionResult SearchProduct(int productId)
        {
            IQueryable<Rent> sOes = db.Rents.Include(s => s.Customer);

            //sOes = db.Rents.Where(x => x.RentDetails.Where(y => y.ProductId == productId));

            List<RentDetail> lstSODs = db.RentDetails.Where(x => x.ProductId == productId).ToList();
            List<Rent> lstSlectedSO = new List<Rent>();
            foreach (RentDetail lsod in lstSODs)
            {
                if (lstSlectedSO.Where(x => x.Id == lsod.SOId).FirstOrDefault() == null)
                {
                    lstSlectedSO.Add(lsod.Rent);
                }


            }

            sOes = lstSlectedSO.ToList().AsQueryable();

            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref sOes);
            foreach (Rent itm in sOes)
            {

                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.Customers = db.Customers;
            return View("Index", sOes.OrderByDescending(i => i.Date).ToList());
        }

        private void GetTotalBalance(ref IQueryable<Rent> Rents)
        {
            //IQueryable<SO> DistSOes = Rents.Select(x => x.CustomerId).Distinct();
            IQueryable<Rent> DistSOes = Rents.GroupBy(x => x.CustomerId).Select(y => y.FirstOrDefault());

            decimal TotalBalance = 0;
            foreach (Rent itm in DistSOes)
            {
                Customer cust = DAL.dbCustomers.Where(x => x.Id == itm.CustomerId).FirstOrDefault();

                TotalBalance += (decimal)cust.Balance;

            }
            ViewBag.TotalBalance = TotalBalance;

        }
        //[ChildActionOnly]
        //public PartialViewResult _SelectedSOSR()
        //{

        //    return PartialView(db.Rents);
        //}


        // GET: Rents/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rent sO = db.Rents.Find(id);
            if (sO == null)
            {
                return HttpNotFound();
            }
            return View(sO);
        }

        // GET: Rents/Create

        public ActionResult Create(string IsReturn)
        {

            //ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name");
            //ViewBag.Products = db.Products;

            //int maxId = db.Customers.Max(p => p.Id);
            decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;


            RentViewModel saleOrderViewModel = new RentViewModel();
            saleOrderViewModel.Customers = db.Customers;
            saleOrderViewModel.Products = db.Products.Where(x => x.Saleable == true);
            //bool IsReturn1 = true;
            ViewBag.IsReturn = IsReturn;
            //string isReturn1 = "true";
            //ViewBag.isReturn = isReturn1;
            return View(saleOrderViewModel);
        }


        //[OutputCache(NoStore = true, Duration = 0)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "Customer", Include = "Name,Address")] Customer Customer, [Bind(Prefix = "SaleOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,CustomerId,Remarks,Remarks2,PaymentMethod,PaymentDetail,SaleReturn,RentStartDate,RentEndDate")] Rent sO, [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,SalePrice,Quantity,SaleType,PerPack,IsPack")] List<RentDetail> sOD, FormCollection collection)

        {

            string SOId = string.Empty;
            //SO sO = new SO();
            if (ModelState.IsValid)
            {


                Customer cust = db.Customers.FirstOrDefault(x => x.Id == sO.CustomerId);
                if (cust == null)
                {//its means new customer
                    //sO.CustomerId = 10;
                    //int maxId = db.Customers.Max(p => p.Id);
                    decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;
                    Customer.Id = maxId;
                    Customer.Balance = sO.Balance;
                    db.Customers.Add(Customer);
                    //db.SaveChanges();
                }
                else
                {//its means old customer. old customer balance should be updated.
                    //Customer.Id = (int)sO.CustomerId;
                    cust.Balance = sO.Balance;
                    db.Entry(cust).State = EntityState.Modified;
                    //db.SaveChanges();




                    //Payment payment = new Payment();
                    //payment = db.Payments.Find(orderId);
                    //payment.Status = true;
                    //db.Entry(payment).State = EntityState.Modified;
                    //db.SaveChanges();

                }

                ////////////////////////////////////////
                //int maxId = db.Rents.Max(p => p.Auto);
                decimal maxId1 = (int)db.Rents.DefaultIfEmpty().Max(p => p == null ? 0 : p.SOSerial);
                maxId1 += 1;
                sO.SOSerial = maxId1;

                //sO.Date = DateTime.Now;
                sO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
                //sO.SaleReturn = false;
                sO.Id = System.Guid.NewGuid().ToString().ToUpper();
                sO.SaleOrderAmount = 0;

                sO.SaleOrderQty = 0;

                sO.Profit = 0;
                Employee emp = (Employee)Session["CurrentUser"];
                sO.EmployeeId = emp.Id;

                db.Rents.Add(sO);
                //db.SaveChanges();
                int sno = 0;
                decimal totalPurchaseAmount = 0;
                //sOD.RemoveAll(so => so.ProductId == null);
                if (sOD != null)
                {

                    foreach (RentDetail sod in sOD)
                    {
                        sno += 1;
                        sod.SODId = sno;
                        sod.Rent = sO;
                        sod.SOId = sO.Id;

                        Product product = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                        //sod.Sale Price in now from view
                        //sod.SalePrice = product.SalePrice;
                        //dont do this uneessary and sacary(no we have to do it here but not in update. if we not do it here then purchase price will remain empty. and cause error in productwisesale etc)
                        sod.PurchasePrice = product.PurchasePrice;
                        if (sod.Quantity == null) { sod.Quantity = 0; }
                        sod.OpeningStock = product.Stock;
                        if (sod.SaleType == true)//return
                        {
                            if (sod.IsPack == false)
                            {
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice);
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;
                                sO.SaleOrderQty += qty;//(int)sod.Quantity;
                                sO.Profit -= (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock += (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit -= (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }
                        }
                        else//sale
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
                            else
                            {//pack

                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock -= (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }

                        }
                        db.ProductDetails.Add(new ProductDetail() { RentId=sO.Id, ProductId = product.Id, RentStartDate = sO.RentStartDate, RentEndDate = sO.RentEndDate });

                    }

                    sO.Profit -= (decimal)sO.Discount;
                    db.RentDetails.AddRange(sOD);
                }

                ///////////////////////add values to payment table

                //int maxPaymentId = db.Payments.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                //maxPaymentId += 1;
                //Payment payment = new Payment();
                //payment.PaymentAmount = (decimal)sO.BillPaid;
                //payment.Id = maxPaymentId;
                //payment.SOId = sO.Id;
                //payment.PaymentMethod = collection.Get("SaleOrder.PaymentMethod");//"Cash"; //cash//other
                //payment.Remarks = collection.Get("SaleOrder.PaymentRemarks");
                //payment.ReceivedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));

                //db.Payments.Add(payment);

                ////////////////////////
                
                

                db.SaveChanges();




                SOId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(sO.Id, "BZNS")));

                //RedirectToAction("Create", PrintSO3(SOId));

                //return RedirectToAction("Create");
            }

            //return PrintSO3(SOId);
            //return View("Index");
            //return RedirectToAction("Index", "Report", new { id = SOId });
            return RedirectToAction("PrintSO3", new { id = SOId });


            //RentViewModel saleOrderViewModel = new RentViewModel();
            //saleOrderViewModel.Customers = db.Customers;
            //saleOrderViewModel.Products = db.Products;

            //return View(saleOrderViewModel);


        }
        //public void PrintSO(string SOId)
        //{
        //    SOId = Encryption.Decrypt(SOId, "BZNS");
        //    string pathh = HttpRuntime.AppDomainAppPath;
        //    ReportDocument reportDocument = new ReportDocument();
        //    reportDocument.Load(pathh + @"Reports\SOSRReceipt2.rpt");
        //    //reportDocument.SetDatabaseLogon("sa", "abc", "LAPTOP-MGR35B58", "Business");


        //    ////
        //    CrystalDecisions.CrystalReports.Engine.Database oCRDb = reportDocument.Database;
        //    CrystalDecisions.CrystalReports.Engine.Tables oCRTables = oCRDb.Tables;
        //    //CrystalDecisions.CrystalReports.Engine.Table oCRTable;
        //    CrystalDecisions.Shared.TableLogOnInfo oCRTableLogonInfo;
        //    CrystalDecisions.Shared.ConnectionInfo oCRConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
        //    oCRConnectionInfo.DatabaseName = "Business";
        //    oCRConnectionInfo.ServerName = "(local)";
        //    oCRConnectionInfo.UserID = "sa";
        //    oCRConnectionInfo.Password = "abc";
        //    foreach (CrystalDecisions.CrystalReports.Engine.Table oCRTable in oCRTables)
        //    {
        //        oCRTableLogonInfo = oCRTable.LogOnInfo;
        //        oCRTableLogonInfo.ConnectionInfo = oCRConnectionInfo;
        //        oCRTable.ApplyLogOnInfo(oCRTableLogonInfo);
        //    }
        //    ////

        //    reportDocument.SetParameterValue("@SaleOrderID", SOId);
        //    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
        //    //printerSettings.PrinterName = "abc";
        //    reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);

        //}


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
            id = Decode(id);
            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType = "application/pdf";
            string encoding = string.Empty;
            string extension = "pdf";
            byte[] bytes;

            // Setup the report viewer object and get the array of bytes
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;

            //SO sO = db.Rents.FirstOrDefault(x => x.Id == id);
            Employee emp = db.Employees.FirstOrDefault(x => x.Id == db.Rents.FirstOrDefault(y => y.Id == id).EmployeeId);
            if (emp.Login == "LahoreKarachi")
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Sale_LahoreKarachi.rdlc"); }
            else
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Sale_GoldPanel.rdlc"); }

            ReportDataSource reportDataSource = new ReportDataSource();

            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = db.spRentReport(id).AsEnumerable();//db.spSOReceipt;// BusinessDataSetTableAdapters
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
            Response.AddHeader("content-disposition", "inline; filename=" + "Sale Receipt" + "." + extension);
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
            IQueryable lstSO = db.Rents.Where(x => x.CustomerId == id);

            //lstSO.ForEachAsync(c => { c. = 0; c.GroupID = 0; c.CompanyID = 0; });
            decimal SOAmount = 0;
            decimal SRAmount = 0;
            foreach (Rent itm in lstSO)
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


        // GET: Rents/Edit/5
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

            Rent sO = db.Rents.Find(id);
            if (sO == null)
            {
                return HttpNotFound();
            }
            RentViewModel saleOrderViewModel = new RentViewModel();
            List<RentDetail> sod = db.RentDetails.Where(x => x.SOId == id).ToList();
            saleOrderViewModel.Products = db.Products.Where(x => x.Saleable == true);
            saleOrderViewModel.Customers = db.Customers;
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
            return View(saleOrderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Prefix = "SaleOrder", Include = "BillAmount,Balance,BillPaid,Discount")] SO sO, [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,Quantity")] List<RentDetail> sOD)
        public ActionResult Edit(RentViewModel saleOrderViewModel1)
        {
            Rent newSO = saleOrderViewModel1.SaleOrder;
            List<RentDetail> newSODs = saleOrderViewModel1.SaleOrderDetail;
            if (ModelState.IsValid)
            {
                newSO.Id = Encryption.Decrypt(saleOrderViewModel1.SaleOrder.Id, "BZNS");//
                Rent sO = db.Rents.Where(x => x.Id == newSO.Id).FirstOrDefault();
                sO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));//
                //sO.SaleReturn = false;//
                sO.BillAmount = newSO.BillAmount;//
                sO.Discount = newSO.Discount;//
                sO.BillPaid = newSO.BillPaid;//
                sO.Balance = newSO.Balance;//
                sO.Remarks = newSO.Remarks;//
                sO.Remarks2 = newSO.Remarks;//
                sO.PaymentMethod = newSO.PaymentMethod;
                sO.PaymentDetail = newSO.PaymentDetail;
                sO.RentStartDate = newSO.RentStartDate;
                sO.RentEndDate = newSO.RentEndDate;
                //sO.SOSerial = newSO.SOSerial;//should be unchanged

                ///////////////////////////////////////////

                //Customer cust = db.Customers.FirstOrDefault(x => x.Id == newSO.CustomerId);
                Customer customer = DAL.dbCustomers.Where(x => x.Id == newSO.CustomerId).FirstOrDefault();
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
                    Customer oldCustomer = DAL.dbCustomers.Where(x => x.Id == sO.CustomerId).FirstOrDefault();
                    oldCustomer.Balance = db.Rents.Where(x => x.Id == sO.Id).FirstOrDefault().PrevBalance;
                    db.Entry(oldCustomer).State = EntityState.Modified;
                }

                sO.PrevBalance = newSO.PrevBalance;//
                // assign balance of this customer
                //Customer customer = db.Customers.Where(x => x.Id == newSO.CustomerId).FirstOrDefault();
                customer.Balance = newSO.Balance;
                //assign customer and customerId in SO
                sO.CustomerId = newSO.CustomerId;
                sO.Customer = customer;

                /////////////////////////////////////////////////////////////////////////////



                List<RentDetail> oldSODs = db.RentDetails.Where(x => x.SOId == newSO.Id).ToList();
                List<ProductDetail> prodDetailsRmv = new List<ProductDetail>();
                //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this is simple and stateforward.
                foreach (RentDetail sod in oldSODs)
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

                db.RentDetails.RemoveRange(oldSODs);
                db.ProductDetails.RemoveRange(db.ProductDetails.Where(x => x.RentId == sO.Id));
                //////////////////////////////////////////////////////////////////////////////

                sO.SaleOrderAmount = 0;

                sO.SaleOrderQty = 0;

                sO.Profit = 0;
                int sno = 0;

                if (newSODs != null)
                {

                    foreach (RentDetail sod in newSODs)
                    {
                        sno += 1;
                        sod.SODId = sno;
                        sod.Rent = sO;
                        sod.SOId = sO.Id;

                        Product product = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                        //sod.salePrice is now from view
                        //sod.SalePrice = product.SalePrice;
                        //dont do this. when user even just open a old bill and just press save. and price was updated after that old bill. all calculations gets wrong
                        //if we dont do this then error in product wise sale
                        sod.PurchasePrice = product.PurchasePrice;
                        if (sod.Quantity == null) { sod.Quantity = 0; }
                        sod.OpeningStock = product.Stock;
                        if (sod.SaleType == true)//return
                        {
                            if (sod.IsPack == false)
                            {
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice);
                                decimal qty = (decimal)sod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;
                                sO.SaleOrderQty += qty;//(int)sod.Quantity;
                                sO.Profit -= (qty * sod.SalePrice) - (decimal)(qty * product.PurchasePrice); //- (decimal)(sO.Discount);
                            }
                            else
                            {
                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock += (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit -= (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }
                        }
                        else//sale
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
                            else
                            {//pack

                                sO.SaleOrderAmount += (sod.Quantity * sod.SalePrice * sod.PerPack);
                                product.Stock -= (int)sod.Quantity * sod.PerPack;

                                sO.SaleOrderQty += (int)sod.Quantity * sod.PerPack;
                                sO.Profit += (sod.Quantity * sod.SalePrice * sod.PerPack) - (decimal)(sod.Quantity * product.PurchasePrice * sod.PerPack); //- (decimal)(sO.Discount);
                            }

                        }
                        db.ProductDetails.Add(new ProductDetail() {RentId=sO.Id, ProductId = product.Id, RentStartDate = sO.RentStartDate, RentEndDate = sO.RentEndDate });
                    }
                    sO.Profit -= (decimal)sO.Discount;
                    db.Entry(sO).State = EntityState.Modified;
                    db.Entry(sO).Property(x => x.SOSerial).IsModified = false;
                    db.RentDetails.AddRange(newSODs);

                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", sO.CustomerId);
            //return View(sO);
            RentViewModel saleOrderViewModel = new RentViewModel();

            saleOrderViewModel.Products = db.Products.Where(x => x.Saleable == true);
            return View(saleOrderViewModel);
            //return View();
        }
        public void GetRentStatus(string prodId, string startDate, string endDate)
        {
            DateTime dtStartDate;
            DateTime dtEndtDate;
            int intProdId;
            //db.Rents.Where(x=>x.RentStartDate)
            //get this customer with from defined start date to undefined end date
            if (prodId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intProdId = Int32.Parse(prodId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                //selectedSOes = db.Rents.Where(so => so == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

        }
        private string Decode(string id)
        {
            byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            id = new string(Encoding.UTF8.GetString(BytesArr).ToCharArray());
            id = Encryption.Decrypt(id, "BZNS");
            return id;
        }


        // GET: Rents/Delete/5
        public ActionResult Delete(string id)
        {
            return null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = Decode(id);
            Rent sO = db.Rents.Find(id);
            if (sO == null)
            {
                return HttpNotFound();
            }
            return View(sO);
        }

        // POST: Rents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            return null;
            id = Decode(id);

            List<RentDetail> oldSODs = db.RentDetails.Where(x => x.SOId == id).ToList();
            //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this si simple and stateforward.
            foreach (RentDetail sod in oldSODs)
            {
                Product product = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                product.Stock += sod.Quantity;
            }
            db.RentDetails.RemoveRange(oldSODs);

            Rent sO = db.Rents.Find(id);
            db.Rents.Remove(sO);
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
                List<RentDetail> lstSODItems = db.RentDetails.Where(x => x.SOId == so.Id).ToList();

                decimal soProfit = 0;
                foreach (RentDetail sod in lstSODItems)

                {
                    Product prod = db.Products.Where(x => x.Id == sod.ProductId).FirstOrDefault();

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
