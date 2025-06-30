using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.EnterpriseServices.CompensatingResourceManager;
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
    [Authorize(Roles = "Admin,Purchasing manager,Technical Manager,Stock staff,Stock manager")]
    public class SPackagingController : Controller
    {
        // GET: SPackaging
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        // GET: POes
        //public ActionResult Index()
        //{
        //    int? storeId = Session["StoreId"] as int?;
        //    //var storeId = Session["StoreId"] as string;
        //    if (storeId == null)
        //    {
        //        return RedirectToAction("StoreNotFound", "UserManagement");
        //    }
        //    //var storeId = Session["StoreId"] as string; //commented due to session issue
        //    //if (storeId == null)
        //    //{
        //    //    return RedirectToAction("StoreNotFound", "UserManagement");
        //    //}
        //    //var parseId = int.Parse(storeId);

        //    DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
        //    var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
        //    var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);
        //    //DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
        //    //var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
        //    //var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

        //    //IQueryable<PO> pOes = db.POes.Include(s => s.Supplier);
        //    IQueryable<PO> pOes = db.POes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate && x.SupplierId > 0).Include(s => s.Supplier);
        //    //pOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
        //    //var pOes = db.POes.Where(s => s.SaleReturn == false);
        //    GetTotalBalance(ref pOes);
        //    Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
        //    int thisSerial = 0;
        //    foreach (PO itm in pOes)
        //    {
        //        thisSerial = (int)itm.Supplier.POes.Max(x => x.POSerial);

        //        if (!LstMaxSerialNo.ContainsKey((int)itm.SupplierId))
        //        {
        //            LstMaxSerialNo.Add(itm.Supplier.Id, thisSerial);
        //        }

        //        //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
        //        itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
        //    }
        //    ViewBag.LstMaxSerialno = LstMaxSerialNo;
        //    ViewBag.Suppliers = DAL.dbSuppliers;
        //    ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
        //    ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
        //    var poess = pOes.Where(x => x.StoreId == storeId).OrderByDescending(i => i.Date).ToList();
        //    //var poess = pOes.Where(x => x.StoreId == parseId).OrderByDescending(i => i.Date).ToList();//commented due to session issue
        //    //var poess = pOes.OrderByDescending(i => i.Date).ToList();
        //    return View(poess);
        //}
        [Authorize(Roles = "Admin,Purchasing manager,Technical Manager,Stock staff,Stock manager")]
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

            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            //IQueryable<PO> pOes = db.POes.Include(s => s.Supplier);
            //IQueryable<PO> pOes = db.POes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate && x.SupplierId > 0).Include(s => s.Supplier);
            IQueryable<SPackaging> sPackagings = db.SPackagings
     .Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate && x.SupplierId > 0)
     .Include(s => s.Supplier)
     .Include(p => p.SPackagingDetails.Select(pod => pod.Product));
            //pOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var pOes = db.POes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref sPackagings);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (SPackaging itm in sPackagings)
            {
                thisSerial = (int)itm.Supplier.SPackagings.Max(x => x.POSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.SupplierId))
                {
                    LstMaxSerialNo.Add(itm.Supplier.Id, thisSerial);
                }

                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            ViewBag.Suppliers = DAL.dbSuppliers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
            var poess = sPackagings.Where(x => x.StoreId == storeId).OrderByDescending(i => i.Date).ToList();
            //var poess = pOes.Where(x => x.StoreId == parseId).OrderByDescending(i => i.Date).ToList();//commented due to session issue
            //var poess = pOes.OrderByDescending(i => i.Date).ToList();
            return View(poess);
        }



        //public ActionResult SearchData(string custName, DateTime startDate, DateTime endDate)

        //public ActionResult SearchData(string custName, string startDate, string endDate)
        public ActionResult SearchData(string suppId, string startDate, string endDate)
        {

            int intSuppId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<SPackaging> selectedPOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (suppId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                intSuppId = Int32.Parse(suppId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = db.SPackagings.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            if (suppId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = db.SPackagings;//.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers data acornding to start end date
            if (suppId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = db.SPackagings.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (suppId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intSuppId = Int32.Parse(suppId.Trim());
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = db.SPackagings.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from defined start date to undefined end date
            if (suppId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intSuppId = Int32.Parse(suppId.Trim());
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = db.SPackagings.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with all dates
            if (suppId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intSuppId = Int32.Parse(suppId.Trim());
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = db.SPackagings.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customer with defined startdate and undefined end date
            if (suppId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = db.SPackagings.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers with undifined start date with defined enddate
            if (suppId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = db.SPackagings.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            GetTotalBalance(ref selectedPOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (SPackaging itm in selectedPOes)
            {
                thisSerial = (int)itm.Supplier.SPackagings.Max(x => x.POSerial);
                if (!LstMaxSerialNo.ContainsKey((int)itm.SupplierId))
                {
                    LstMaxSerialNo.Add(itm.Supplier.Id, thisSerial);
                }
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            return PartialView("_SelectedPOPR", selectedPOes.OrderByDescending(i => i.Date).ToList());

            //return View("Some thing went wrong");


        }
        public ActionResult PerMonthPurchase(int productId)
        {
            IQueryable<SPackaging> sPackagings = db.SPackagings.Include(s => s.Supplier);

            //sOes = db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<SPackagingDetail> lstPODs = db.SPackagingDetails.Where(x => x.ProductId == productId && x.SaleType == false).ToList();
            List<SPackaging> lstSlectedPO = new List<SPackaging>();
            foreach (SPackagingDetail lpod in lstPODs)
            {
                if (lstSlectedPO.Where(x => x.Id == lpod.SPackagingId).FirstOrDefault() == null)
                {
                    lstSlectedPO.Add(lpod.SPackaging);
                }
            }

            sPackagings = lstSlectedPO.ToList().AsQueryable();
            foreach (SPackaging itm in sPackagings)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.ProductName = db.Products.FirstOrDefault(x => x.Id == productId).Name;
            return View("PerMonthPurchase", sPackagings.OrderBy(i => i.Date).ToList());
        }

        public ActionResult SearchProduct(int productId)
        {
            IQueryable<SPackaging> sPackagings = db.SPackagings.Include(s => s.Supplier);

            List<SPackagingDetail> lstPODs = db.SPackagingDetails.Where(x => x.ProductId == productId).ToList();
            List<SPackaging> lstSlectedPO = new List<SPackaging>();
            foreach (SPackagingDetail lpod in lstPODs)
            {
                if (lstSlectedPO.Where(x => x.Id == lpod.SPackagingId).FirstOrDefault() == null)
                {
                    lstSlectedPO.Add(lpod.SPackaging);
                }


            }

            sPackagings = lstSlectedPO.ToList().AsQueryable();

            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref sPackagings);
            foreach (SPackaging itm in sPackagings)
            {

                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View("Index", sPackagings.OrderByDescending(i => i.Date).ToList());
        }

        private void GetTotalBalance(ref IQueryable<SPackaging> sPackagings)
        {
            //IQueryable<SO> DistSOes = SOes.Select(x => x.CustomerId).Distinct();
            IQueryable<SPackaging> DistsPackagings = sPackagings.GroupBy(x => x.SupplierId).Select(y => y.FirstOrDefault());

            decimal TotalBalance = 0;
            foreach (SPackaging itm in DistsPackagings)
            {
                Supplier cust = db.Suppliers.Where(x => x.Id == itm.SupplierId).FirstOrDefault();
                TotalBalance += (decimal)(cust.Balance ?? 0);
            }
            ViewBag.TotalBalance = TotalBalance;

        }
        //[ChildActionOnly]
        //public PartialViewResult _SelectedPOPR()
        //{

        //    return PartialView(db.POes);
        //}

        // GET: POes/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SPackaging sPackaging = db.SPackagings.Find(id);
            if (sPackaging == null)
            {
                return HttpNotFound();
            }
            return View(sPackaging);
        }



        [Authorize(Roles = "Admin,Purchasing manager,Technical Manager,Stock staff,Stock manager")]
        // GET: POes/Create
        public ActionResult Create(string IsReturn)
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
            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name");
            //ViewBag.Products = db.Products;

            //int maxId = db.Suppliers.Max(p => p.Id);
            decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;


            SPackagingViewModel spackagingViewModel = new SPackagingViewModel();
            spackagingViewModel.Suppliers = DAL.dbSuppliers;
            spackagingViewModel.Products = DAL.dbProducts.Include(x => x.StoreProducts).Where(x => x.PType == 8  && x.IsService == false);
            //purchaseOrderViewModel.FundingSources = db.FundingSources.ToList() ;
            ViewBag.FundingSources = new SelectList(db.Suppliers.Where(x => x.IsCreditor == true), "Id", "Name");//db.FundingSources.ToList(); ;
            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.MalaysiaTime = DateTime.UtcNow.AddHours(8);
            ViewBag.IsReturn = IsReturn;
            Supplier defaultSupplier = db.Suppliers.FirstOrDefault(x => x.IsCreditor == false);
            ViewBag.DefaultSuppId = defaultSupplier.Id;
            ViewBag.DefaultSuppName = defaultSupplier.Name;
            ViewBag.ReportId = TempData["ReportId"] as string;
            return View(spackagingViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "Supplier", Include = "Name,Address")] Supplier Supplier, [Bind(Prefix = "SPackaging", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,SupplierId,Remarks,Remarks2,PaymentMethod,PaymentDetail,PurchaseReturn,FundingSourceId,BankAccountId,Date")] SPackaging sPackaging, [Bind(Prefix = "SPackagingDetail", Include = "ProductId,Quantity,SaleType,PerPack,IsPack,PurchasePrice,ExpiryDate,PurchasingDate,Unit")] List<SPackagingDetail> sPackagingDetails)
        {
            //PO pO = new PO();

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
            Supplier supp = db.Suppliers.FirstOrDefault(x => x.Id == sPackaging.SupplierId);
            if (supp == null)
            {//its means new customer
             //pO.SupplierId = 10;
             //int maxId = db.Suppliers.Max(p => p.Id);
                decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                maxId += 1;
                Supplier.Id = maxId;
                Supplier.Balance = sPackaging.Balance;
                Supplier.StoreId = storeId;
                //Supplier.StoreId = parseId; //commented due to session issue
                db.Suppliers.Add(Supplier);
                //db.SaveChanges();
            }
            else
            {//its means old customer. old customer balance should be updated.
             //Supplier.Id = (int)pO.SupplierId;
             //supp.StoreId = parseId; //commented due to session issue
                supp.StoreId = storeId;
                supp.Balance = sPackaging.Balance;
                db.Entry(supp).State = EntityState.Modified;
                //db.SaveChanges();

                //Payment payment = new Payment();
                //payment = db.Payments.Find(orderId);
                //payment.Status = true;
                //db.Entry(payment).State = EntityState.Modified;
                //db.SaveChanges();

            }

            ////////////////////////////////////////
            BankAccount bankAccount = db.BankAccounts.FirstOrDefault(x => x.Id == sPackaging.BankAccountId);
            bankAccount.Balance -= sPackaging.BillPaid;
            db.BankAccounts.Attach(bankAccount);
            db.Entry(bankAccount).Property(x => x.Balance).IsModified = true;
            ////////////////////////////////////////
            //int maxId = db.POes.Max(p => p.Auto);
            decimal maxId1 = (int)db.SPackagings.DefaultIfEmpty().Max(p => p == null ? 0 : p.POSerial);
            maxId1 += 1;
            sPackaging.POSerial = maxId1;
            //pO.Date = DateTime.Now;
            if (string.IsNullOrEmpty(Convert.ToString(sPackaging.Date)))
            {
                sPackaging.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            }
            //pO.SaleReturn = false;
            sPackaging.Id = System.Guid.NewGuid().ToString().ToUpper();
            sPackaging.PurchaseOrderAmount = 0;

            sPackaging.PurchaseOrderQty = 0;
            sPackaging.StoreId = storeId;
            //pO.StoreId = parseId; //commented due to session issue
            //pO.StoreId = 1;
            //Employee emp = (Employee)Session["CurrentUser"];
            Employee emp = Session["CurrentUser"] as Employee ?? new Employee { Id = 0 }; // or some default ID
            sPackaging.EmployeeId = emp.Id;
            db.SPackagings.Add(sPackaging);
            //db.SaveChanges();
            int sno = 0;
            if (sPackagingDetails != null)
            {
                //pOD.RemoveAll(so => so.ProductId == null);
                foreach (SPackagingDetail pod in sPackagingDetails)
                {
                    sno += 1;
                    pod.SPDId = sno;
                    pod.SPackaging = sPackaging;
                    pod.SPackagingId = sPackaging.Id;

                    Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);

                    //dont do this. when user made a bill and chnage sale price. it does not reflect in bill and calculations geting wrong
                    //pod.PurchasePrice = product.PurchasePrice;
                    if (pod.Quantity == null) { pod.Quantity = 0; }
                    pod.OpeningStock = product.Stock;
                    pod.PerPack = 1;
                    if (pod.SaleType == false)//purchase
                    {

                        if (pod.IsPack == false)
                        {//piece
                            sPackaging.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                            //int pieceSold = (int)(sod.Quantity * product.Stock);
                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            product.Stock += qty;

                            sPackaging.PurchaseOrderQty += qty;//(int)sod.Quantity;

                        }
                        else
                        {//pack

                            sPackaging.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                            product.Stock += (int)pod.Quantity * pod.PerPack;

                            sPackaging.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                        }

                    }
                    else//return
                    {
                        if (pod.IsPack == false)
                        {
                            sPackaging.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            product.Stock -= qty;
                            sPackaging.PurchaseOrderQty += qty;//(int)sod.Quantity;

                        }
                        else
                        {
                            sPackaging.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                            product.Stock -= (int)pod.Quantity * pod.PerPack;

                            sPackaging.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                        }

                    }


                    db.SPackagingDetails.AddRange(sPackagingDetails);
                }
                db.SaveChanges();
                return RedirectToAction("Index");

                //SqlParameter param1 = new SqlParameter("@PurchaseOrderID", pO.Id);
                ////var result = db.Database.ExecuteSqlCommand("spSOReceipt @PurchaseOrderID", param1);
                //var result = db.Database.SqlQuery<Object>("spSOReceipt @PurchaseOrderID", param1);


                //var cr = new ReportDocument();
                //cr.Load(@"E:\PROJECTS\MYBUSINESS - v.4.6\MYBUSINESS\Reports\SOReceipt.rpt");
                //cr.DataDefinition.RecordSelectionFormula = "{PurchaseOrderID} = '" + pO.Id + "'";
                //cr.PrintToPrinter(1, true, 0, 0);


                ////////////////////////finalized
                //string pathh = HttpRuntime.AppDomainAppPath;
                //ReportDocument reportDocument = new ReportDocument();
                //reportDocument.Load(pathh + @"Reports\SOReceipt.rpt");
                //reportDocument.SetParameterValue("@PurchaseOrderID", pO.Id);
                //System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                ////printerSettings.PrinterName = PrinterName;
                //reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);
                /////////////////////////////////////


                string POId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(sPackaging.Id, "BZNS")));
                //return PrintSO(POId);
                //return PrintSO3(POId);
                //return RedirectToAction("PrintSO3", new { id = POId });
                TempData["ReportId"] = sPackaging.Id;
                return RedirectToAction("Create", new { IsReturn = "false" });
                //return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", pO.SupplierId);
            //return View(pO);
            SPackagingViewModel spackagingViewModel = new SPackagingViewModel();
            spackagingViewModel.Suppliers = DAL.dbSuppliers;
            spackagingViewModel.Products = DAL.dbProducts.Where(x => (x.PType == 4 || x.PType == 7) && x.IsService == false);
            return View(spackagingViewModel);
            //return View();

        }
        //public void PrintSO(string POId)
        //{
        //    POId = Encryption.Decrypt(POId, "BZNS");
        //    string pathh = HttpRuntime.AppDomainAppPath;
        //    ReportDocument reportDocument = new ReportDocument();
        //    reportDocument.Load(pathh + @"Reports\POPRReceipt2.rpt");
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

        //    reportDocument.SetParameterValue("@PurchaseOrderID", POId);
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
            if (id.Length > 36)
            {
                id = Decode(id);
            }

            int POSerial = (int)db.POes.FirstOrDefault(x => x.Id == id).POSerial;
            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType = "application/pdf";
            string encoding = string.Empty;
            string extension = "pdf";


            // Setup the report viewer object and get the array of bytes
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;

            //PO pO = db.POes.FirstOrDefault(x => x.Id == id);
            Employee emp = db.Employees.FirstOrDefault(x => x.Id == db.POes.FirstOrDefault(y => y.Id == id).EmployeeId);
            if (emp.Login == "LahoreKarachi")
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Purchase_LahoreKarachi.rdlc"); }
            else
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Purchase_Receipt.rdlc"); }

            ReportDataSource reportDataSource = new ReportDataSource();

            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = db.spPOReport(id).AsEnumerable();//db.spSOReceipt;// BusinessDataSetTableAdapters
            viewer.LocalReport.DataSources.Add(reportDataSource);
            viewer.LocalReport.SetParameters(new ReportParameter("PurchaseOrderID", id));
            viewer.LocalReport.Refresh();
            //byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            byte[] bytes = viewer.LocalReport.Render("PDF");//, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline; filename=" + "Purchase Receipt " + POSerial.ToString("D4") + "." + extension);
            Response.BinaryWrite(bytes); // create the file
            //return Response.Flush(); // send it to the client to download
            return new FileContentResult(bytes, mimeType);




            //System.IO.Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            ////stream.Seek(0, System.IO.SeekOrigin.Begin);
            //System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
            //byte[] getBytes = null;
            //getBytes = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
            //HttpContext.Response.AddHeader("content-disposition", "inline; filename=" + "POPR.pdf");

            //return File(getBytes, "application/pdf");


        }

        public decimal GetPreviousBalance(int id)
        {
            IQueryable lstSO = db.POes.Where(x => x.SupplierId == id);

            //lstSO.ForEachAsync(c => { c. = 0; c.GroupID = 0; c.CompanyID = 0; });
            decimal POAmount = 0;
            decimal PRAmount = 0;
            foreach (PO itm in lstSO)
            {
                POAmount += (decimal)itm.PurchaseOrderAmount;


            }

            return (POAmount - PRAmount);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Prefix = "PurchaseOrder", Include = "BillAmount,Balance,BillPaid,Discount")] PO pO, [Bind(Prefix = "PurchaseOrderDetail", Include = "ProductId,Quantity,ExpiryDate,PurchasingDate,Validate")] List<POD> pOD)
        public ActionResult Edit(SPackagingViewModel spackagingViewModel)
        {
            SPackaging newSPackagings = spackagingViewModel.SPackaging;
            List<SPackagingDetail> newSPackagingDetails = spackagingViewModel.SPackagingDetail;
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
                newSPackagings.Id = Encryption.Decrypt(spackagingViewModel.SPackaging.Id, "BZNS");//
                SPackaging SPackaging = db.SPackagings.Where(x => x.Id == newSPackagings.Id).FirstOrDefault();
                //PO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));//
                //PO.PurchaseReturn = false;//
                SPackaging.BillAmount = newSPackagings.BillAmount;//
                SPackaging.Discount = newSPackagings.Discount;//
                SPackaging.BillPaid = newSPackagings.BillPaid;//
                SPackaging.Balance = newSPackagings.Balance;//
                SPackaging.Remarks = newSPackagings.Remarks;//
                SPackaging.Remarks2 = newSPackagings.Remarks;//
                SPackaging.PaymentMethod = newSPackagings.PaymentMethod;
                SPackaging.PaymentDetail = newSPackagings.PaymentDetail;
                SPackaging.FundingSourceId = newSPackagings.FundingSourceId;
                SPackaging.BankAccountId = newSPackagings.BankAccountId;
                SPackaging.Date = newSPackagings.Date;
                SPackaging.StoreId = storeId;
                //PO.StoreId = parseId; //commented due to session issue
                //PO.StoreId = 1;
                //PO.POSerial = newPO.POSerial;//should be unchanged

                ///////////////////////////////////////////

                //Supplier cust = db.Suppliers.FirstOrDefault(x => x.Id == newPO.SupplierId);
                Supplier supplier = db.Suppliers.Where(x => x.Id == newSPackagings.SupplierId).FirstOrDefault();
                if (supplier == null)
                {//its means new supplier(not in db)
                 //PO.SupplierId = 10;
                 //int maxId = db.Suppliers.Max(p => p.Id);
                    supplier = spackagingViewModel.Supplier;
                    decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;

                    supplier.Id = maxId;
                    //supplier.Balance = newPO.Balance;
                    db.Suppliers.Add(supplier);
                }
                else
                {
                    db.Entry(supplier).State = EntityState.Modified;
                }

                if (SPackaging.SupplierId != newSPackagings.SupplierId)
                {//POme other db supplier
                 //first revert the previous supplier balance 
                    Supplier oldSupplier = db.Suppliers.Where(x => x.Id == SPackaging.SupplierId).FirstOrDefault();
                    oldSupplier.Balance = db.POes.Where(x => x.Id == SPackaging.Id).FirstOrDefault().PrevBalance;
                    db.Entry(oldSupplier).State = EntityState.Modified;
                }

                SPackaging.PrevBalance = newSPackagings.PrevBalance;//
                // assign balance of this supplier
                //Supplier supplier = db.Suppliers.Where(x => x.Id == newPO.SupplierId).FirstOrDefault();
                supplier.Balance = newSPackagings.Balance;
                //assign supplier and supplierId in PO
                SPackaging.SupplierId = newSPackagings.SupplierId;
                SPackaging.Supplier = supplier;

                /////////////////////////////////////////////////////////////////////////////



                List<SPackagingDetail> oldPODs = db.SPackagingDetails.Where(x => x.SPackagingId == newSPackagings.Id).ToList();

                //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this is simple and stateforward.
                foreach (SPackagingDetail pod in oldPODs)
                {
                    Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                    if (pod.SaleType == false)//purchase
                    {
                        //product.Stock -= pod.Quantity;

                        if (pod.IsPack == false)
                        {
                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            product.Stock -= qty;
                        }
                        else
                        {
                            product.Stock -= (int)pod.Quantity * pod.PerPack;
                        }

                    }
                    else//return
                    {
                        //product.Stock += pod.Quantity;

                        if (pod.IsPack == false)
                        {
                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            product.Stock += qty;
                        }
                        else
                        {
                            product.Stock += (int)pod.Quantity * pod.PerPack;
                        }


                    }
                    db.Entry(product).State = EntityState.Modified;
                }

                db.SPackagingDetails.RemoveRange(oldPODs);
                //////////////////////////////////////////////////////////////////////////////

                SPackaging.PurchaseOrderAmount = 0;

                SPackaging.PurchaseOrderQty = 0;

                //PO.Profit = 0;
                int sno = 0;

                if (newSPackagingDetails != null)
                {

                    foreach (SPackagingDetail pod in newSPackagingDetails)
                    {
                        sno += 1;
                        pod.SPDId = sno;
                        pod.SPackaging = SPackaging;
                        pod.SPackagingId = SPackaging.Id;

                        Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                        //POd.purchasePrice is now from view
                        //POd.PurchasePrice = product.PurchasePrice;
                        //dont do this. calculation are geting wrong. when user open an old bill and just press save. all calculations distrubs
                        //pod.PurchasePrice = product.PurchasePrice;
                        if (pod.Quantity == null) { pod.Quantity = 0; }
                        pod.OpeningStock = product.Stock;
                        if (pod.SaleType == false)//purchase
                        {

                            if (pod.IsPack == false)
                            {//piece
                                SPackaging.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;

                                SPackaging.PurchaseOrderQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {//pack

                                SPackaging.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock += (int)pod.Quantity * pod.PerPack;

                                SPackaging.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }
                        }
                        else//return
                        {
                            if (pod.IsPack == false)
                            {
                                SPackaging.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock -= qty;
                                SPackaging.PurchaseOrderQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {
                                SPackaging.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock -= (int)pod.Quantity * pod.PerPack;

                                SPackaging.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }

                        }

                    }
                    //PO.Profit -= (decimal)PO.Discount;
                    db.Entry(SPackaging).State = EntityState.Modified;
                    db.Entry(SPackaging).Property(x => x.POSerial).IsModified = false;
                    db.SPackagingDetails.AddRange(newSPackagingDetails);

                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", PO.SupplierId);
            //return View(PO);
            SPackagingViewModel sPackagingViewModel = new SPackagingViewModel();

            spackagingViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7 && x.IsService == false);
            return View(spackagingViewModel);
            //return View();
        }

        private string Decode(string id)
        {
            byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            id = new string(Encoding.UTF8.GetString(BytesArr).ToCharArray());
            id = Encryption.Decrypt(id, "BZNS");
            return id;
        }









        // GET: POes/Edit/5
        public ActionResult Edit(string id, bool? update, bool readonlyMode = false)
        {

            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            //id = new string( Encoding.UTF8.GetString(BytesArr).ToCharArray());
            //id = Encryption.Decrypt(id,"BZNS");

            decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;

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

            SPackaging pO = db.SPackagings.Find(id);
            if (pO == null)
            {
                return HttpNotFound();
            }
            SPackagingViewModel spackagingViewModel = new SPackagingViewModel();
            List<SPackagingDetail> pod = db.SPackagingDetails.Where(x => x.SPackagingId == id).ToList();
            spackagingViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7 && x.IsService == false);
            spackagingViewModel.Suppliers = DAL.dbSuppliers;
            spackagingViewModel.SPackagingDetail = pod;
            pO.Id = Encryption.Encrypt(pO.Id, "BZNS");
            spackagingViewModel.SPackaging = pO;
            int orderQty = 0;
            int orderQtyPiece = 0;//orderQtyPiece 'P for piece' 
            int returnQty = 0;
            int returnQtyPiece = 0;//orderQtyPiece 'P for piece' 
            foreach (var item in pod)
            {
                if (pO.PurchaseReturn == false)
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

            //List<SelectListItem> FundingSources = new List<SelectListItem>();
            //foreach (FundingSource fundingSource in db.FundingSources)
            //{
            //    FundingSources.Add(new SelectListItem() { Text = fundingSource.Source, Value = fundingSource.Id.ToString() });
            //}
            //ViewBag.FundingSources = db.Suppliers.Where(x => x.IsCreditor == true).ToList();//FundingSources;
            ViewBag.FundingSources = new SelectList(db.Suppliers.Where(x => x.IsCreditor == true), "Id", "Name");//db.FundingSources.ToList(); ;
            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.orderQty = orderQty;
            ViewBag.orderQtyPiece = orderQtyPiece;
            ViewBag.returnQty = returnQty;
            ViewBag.returnQtyPiece = returnQtyPiece;
            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", sO.SupplierId);
            ViewBag.SupplierName = pO.Supplier.Name;
            ViewBag.SupplierAddress = pO.Supplier.Address;
            decimal subTotal = (decimal)(pO.PurchaseOrderAmount - pO.Discount);
            ViewBag.SubTotal = subTotal;
            ViewBag.Total = subTotal + (decimal)pO.PrevBalance;
            ViewBag.IsUpdate = update ?? false;
            ViewBag.IsReturn = pO.PurchaseReturn.ToString().ToLower();
            return View(spackagingViewModel);
        }




        [HttpPost]
        public JsonResult Validation(List<MYBUSINESS.Models.POPViewModel> LstProductionVM)
        {
            try
            {
                if (LstProductionVM == null || !LstProductionVM.Any())
                {
                    return Json(new { success = false, message = "No data received." });
                }

                foreach (var item in LstProductionVM)
                {
                    int prodId = item.productId;
                    string poId = item.poId;

                    // Find the matching POD record by POId and ProductId
                    var podRecord = db.SPackagingDetails.FirstOrDefault(p =>
                        p.SPackagingId.ToString() == poId && p.ProductId == prodId);

                    if (podRecord == null)
                    {
                        return Json(new { success = false, message = $"No POD found for ProductId {prodId} and POId {poId}." });
                    }

                    // Update the Validate field
                    podRecord.Validate = true;

                    // Mark the field as modified
                    db.Entry(podRecord).Property(x => x.Validate).IsModified = true;
                }

                // Save all changes to the database
                db.SaveChanges();

                string redirectUrl = Url.Action("Index", "SPackaging");
                return Json(new { success = true, message = "Validation completed and records updated.", redirectUrl = redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error: " + ex.Message });
            }
        }

        // REMOVE or comment out [ValidateAntiForgeryToken]
        //public JsonResult Validation(List<MYBUSINESS.Models.POPViewModel> LstProductionVM)
        //{
        //    try
        //    {
        //        if (LstProductionVM == null || !LstProductionVM.Any())
        //        {
        //            return Json(new { success = false, message = "No data received." });
        //        }

        //        foreach (var item in LstProductionVM)
        //        {
        //            int prodId = item.productId;
        //            string poId = item.poId;

        //            var product = db.Products.FirstOrDefault(p => p.Id == prodId);
        //            var po = db.PODs.FirstOrDefault(p => p.POId.ToString() == poId);

        //            if (product == null || po == null)
        //            {
        //                return Json(new { success = false, message = "Invalid product or PO." });
        //            }
        //            podRecord.Validate = true;

        //            // Mark the field as modified
        //            db.Entry(podRecord).Property(x => x.Validate).IsModified = true;
        //            // Your logic here
        //        }

        //        return Json(new { success = true, message = "Validation complete." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Server error: " + ex.Message });
        //    }
        //}





        // GET: POes/Delete/5
        public ActionResult Delete(string id)
        {
            return null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = Decode(id);
            PO pO = db.POes.Find(id);
            if (pO == null)
            {
                return HttpNotFound();
            }
            return View(pO);
        }

        // POST: POes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            return null;
            id = Decode(id);

            List<POD> oldSODs = db.PODs.Where(x => x.POId == id).ToList();
            //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this si simple and stateforward.
            foreach (POD pod in oldSODs)
            {
                Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                product.Stock += pod.Quantity;
            }
            db.PODs.RemoveRange(oldSODs);

            PO pO = db.POes.Find(id);
            db.POes.Remove(pO);
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