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
    [Authorize(Roles = "Admin,")]
    public class PurchaseQuotationController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        // GET: POes
        [Authorize(Roles = "Admin,Accountant")]
        public ActionResult Index()
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

            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            //IQueryable<PO> pOes = db.POes.Include(s => s.Supplier);
            IQueryable<PO> pOes = db.POes.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate && x.SupplierId > 0 && x.Quotation == true).Include(s => s.Supplier);
            //pOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var pOes = db.POes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref pOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (PO itm in pOes)
            {
                thisSerial = (int)itm.Supplier.POes.Max(x => x.POSerial);

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
            ViewBag.POSerial = thisSerial;
            var purchaseOrderQuotation = pOes.Where(x => x.StoreId == storeId).OrderByDescending(i => i.Date).ToList(); 
            //var purchaseOrderQuotation = pOes.Where(x => x.StoreId == parseId).OrderByDescending(i => i.Date).ToList(); ///Commented due to Session issue
            //var purchaseOrderQuotation = pOes.OrderByDescending(i => i.Date).ToList();
            return View(purchaseOrderQuotation);
        }
        //public ActionResult SearchData(string custName, DateTime startDate, DateTime endDate)

        //public ActionResult SearchData(string custName, string startDate, string endDate)
        public ActionResult SearchData(string suppId, string startDate, string endDate)
        {

            int intSuppId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<PO> selectedPOes = null;
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

                selectedPOes = db.POes.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            if (suppId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = db.POes;//.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers data acornding to start end date
            if (suppId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = db.POes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (suppId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intSuppId = Int32.Parse(suppId.Trim());
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = db.POes.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from defined start date to undefined end date
            if (suppId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intSuppId = Int32.Parse(suppId.Trim());
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = db.POes.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with all dates
            if (suppId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intSuppId = Int32.Parse(suppId.Trim());
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = db.POes.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customer with defined startdate and undefined end date
            if (suppId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = db.POes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers with undifined start date with defined enddate
            if (suppId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = db.POes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            GetTotalBalance(ref selectedPOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            foreach (PO itm in selectedPOes)
            {
                thisSerial = (int)itm.Supplier.POes.Max(x => x.POSerial);
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
            IQueryable<PO> pOes = db.POes.Include(s => s.Supplier);

            //sOes = db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<POD> lstPODs = db.PODs.Where(x => x.ProductId == productId && x.SaleType == false).ToList();
            List<PO> lstSlectedPO = new List<PO>();
            foreach (POD lpod in lstPODs)
            {
                if (lstSlectedPO.Where(x => x.Id == lpod.POId).FirstOrDefault() == null)
                {
                    lstSlectedPO.Add(lpod.PO);
                }
            }

            pOes = lstSlectedPO.ToList().AsQueryable();
            foreach (PO itm in pOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.ProductName = db.Products.FirstOrDefault(x => x.Id == productId).Name;
            return View("PerMonthPurchase", pOes.OrderBy(i => i.Date).ToList());
        }

        public ActionResult SearchProduct(int productId)
        {
            IQueryable<PO> pOes = db.POes.Include(s => s.Supplier);

            List<POD> lstPODs = db.PODs.Where(x => x.ProductId == productId).ToList();
            List<PO> lstSlectedPO = new List<PO>();
            foreach (POD lpod in lstPODs)
            {
                if (lstSlectedPO.Where(x => x.Id == lpod.POId).FirstOrDefault() == null)
                {
                    lstSlectedPO.Add(lpod.PO);
                }


            }
            pOes = lstSlectedPO.ToList().AsQueryable();
            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id, "BZNS"));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref pOes);
            foreach (PO itm in pOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View("Index", pOes.OrderByDescending(i => i.Date).ToList());
        }

        private void GetTotalBalance(ref IQueryable<PO> POes)
        {
            //IQueryable<SO> DistSOes = SOes.Select(x => x.CustomerId).Distinct();
            //IQueryable<PO> DistPOes = POes.GroupBy(x => x.SupplierId).Select(y => y.FirstOrDefault());

            //decimal TotalBalance = 0;
            //foreach (PO itm in DistPOes)
            //{
            //    Supplier cust = db.Suppliers.Where(x => x.Id == itm.SupplierId).FirstOrDefault();
            //    TotalBalance += (decimal)cust.Balance;
            //}
            //ViewBag.TotalBalance = TotalBalance;
            decimal TotalBalance = 0;

            try
            {
                IQueryable<PO> DistPOes = POes.GroupBy(x => x.SupplierId).Select(y => y.FirstOrDefault());

                foreach (PO itm in DistPOes)
                {
                    try
                    {
                        Supplier cust = db.Suppliers.Where(x => x.Id == itm.SupplierId).FirstOrDefault();

                        if (cust != null && cust.Balance.HasValue) // Check if cust and Balance are not null
                        {
                            TotalBalance += (decimal)cust.Balance;
                        }
                        else
                        {
                            TotalBalance = 0;
                            System.Diagnostics.Debug.WriteLine($"Supplier with ID {itm.SupplierId} not found or has no balance.");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error processing supplier with ID {itm.SupplierId}: {ex.Message}");
                    }
                }

                ViewBag.TotalBalance = TotalBalance;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating total balance: {ex.Message}");

                ViewBag.TotalBalance = 0; // Or set another default value
            }

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
            PO pO = db.POes.Find(id);
            if (pO == null)
            {
                return HttpNotFound();
            }
            return View(pO);
        }
        [Authorize(Roles = "Admin,Accountant")]
        // GET: POes/Create
        public ActionResult Create(string IsReturn)
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
            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name");
            //ViewBag.Products = db.Products;

            //int maxId = db.Suppliers.Max(p => p.Id);
            decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;


            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel.Suppliers = DAL.dbSuppliers;
            purchaseOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7 == true && x.IsService == false);
            //purchaseOrderViewModel.FundingSources = db.FundingSources.ToList() ;
            ViewBag.FundingSources = new SelectList(db.Suppliers.Where(x => x.IsCreditor == true), "Id", "Name");//db.FundingSources.ToList(); ;
            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.MalaysiaTime = DateTime.UtcNow.AddHours(8);
            ViewBag.IsReturn = IsReturn;
            Supplier defaultSupplier = db.Suppliers.FirstOrDefault(x => x.IsCreditor == false);
            ViewBag.DefaultSuppId = defaultSupplier.Id;
            ViewBag.DefaultSuppName = defaultSupplier.Name;
            ViewBag.ReportId = TempData["ReportId"] as string;
            return View(purchaseOrderViewModel);
        }
        // GET: POes/CreatePOByCategory
        public ActionResult CreatePOByCategory(string IsReturn)
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
            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name");
            //ViewBag.Products = db.Products;

            //int maxId = db.Suppliers.Max(p => p.Id);
            decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;


            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel.Suppliers = DAL.dbSuppliers;
            purchaseOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7 == true && x.IsService == false);
            //purchaseOrderViewModel.ProductListCategory = db.Products.ToList();
            //purchaseOrderViewModel.FundingSources = db.FundingSources.ToList() ;
            //var products = db.Products
            //  .Select(p => new
            //  {
            //      Product = p,
            //      Stock = db.StoreProducts
            //          .Where(sp => sp.ProductId == p.Id)
            //          .Sum(sp => sp.Stock) // Sum stock from StoreProduct
            //  })
            //  .ToList();
            //        // Group products by category, handling null categories by assigning "Uncategorized"
            //        var groupedSelectedProducts = products
            //            .GroupBy(p => string.IsNullOrEmpty(p.Product.Category) ? "Uncategorized" : p.Product.Category)
            //            .ToDictionary(
            //                g => g.Key,
            //                g => g.Select(p => new
            //                {
            //                    p.Product.Name,
            //                    p.Product.Id,
            //                    p.Product.PurchasePrice,
            //                    p.Stock
            //                }).ToList()
            //            );

            //ViewBag.SelectedProductCategory = groupedSelectedProducts;
            var products = db.Products
    .Select(p => new
    {
        Product = p,
        Stock = db.StoreProducts
            .Where(sp => sp.ProductId == p.Id)
            .Sum(sp => sp.Stock) // Sum stock from StoreProduct
    })
    .ToList();

            // Group products by category, handling null categories by assigning "Uncategorized"
            var groupedSelectedProducts = products
                .GroupBy(p => string.IsNullOrEmpty(p.Product.Category) ? "Uncategorized" : p.Product.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(p => new MYBUSINESS.Models.Product // Convert to Product
                    {
                        Id = p.Product.Id,
                        Name = p.Product.Name,
                        PurchasePrice = p.Product.PurchasePrice,
                        // Include other properties you need
                        Stock = p.Stock // Add Stock as an extra property or handle it separately
                    }).ToList()
                );

            ViewBag.SelectedProductCategory = groupedSelectedProducts;

            ViewBag.FundingSources = new SelectList(db.Suppliers.Where(x => x.IsCreditor == true), "Id", "Name");//db.FundingSources.ToList(); ;
            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.MalaysiaTime = DateTime.UtcNow.AddHours(8);
            ViewBag.IsReturn = IsReturn;
            Supplier defaultSupplier = db.Suppliers.FirstOrDefault(x => x.IsCreditor == false);
            ViewBag.DefaultSuppId = defaultSupplier.Id;
            ViewBag.DefaultSuppName = defaultSupplier.Name;
            ViewBag.ReportId = TempData["ReportId"] as string;
            return View(purchaseOrderViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "Supplier", Include = "Name,Address")] Supplier Supplier, [Bind(Prefix = "PurchaseOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,SupplierId,Remarks,Remarks2,PaymentMethod,PaymentDetail,PurchaseReturn,FundingSourceId,BankAccountId,Date")] PO pO, [Bind(Prefix = "PurchaseOrderDetail", Include = "ProductId,Quantity,SaleType,PerPack,IsPack,PurchasePrice")] List<POD> pOD)
        {
            //PO pO = new PO();
            if (ModelState.IsValid)
            {
                int? storeId = Session["StoreId"] as int?;
                //var storeId = Session["StoreId"] as string;
                if (storeId == null)
                {
                    return RedirectToAction("StoreNotFound", "UserManagement");
                }
                //var storeId = Session["StoreId"] as string;  //commented due to session issue
                //if (storeId == null)
                //{
                //    return RedirectToAction("StoreNotFound", "UserManagement");
                //}
                //var parseId = int.Parse(storeId);

                Supplier supp = db.Suppliers.FirstOrDefault(x => x.Id == pO.SupplierId);
                if (supp == null)
                {//its means new customer
                    //pO.SupplierId = 10;
                    //int maxId = db.Suppliers.Max(p => p.Id);
                    decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;
                    Supplier.Id = maxId;
                    Supplier.Balance = pO.Balance;
                    //Supplier.StoreId = parseId; //commented due to session issue
                    Supplier.StoreId = storeId;
                    db.Suppliers.Add(Supplier);
                    //db.SaveChanges();
                }
                else
                {//its means old customer. old customer balance should be updated.
                    //Supplier.Id = (int)pO.SupplierId;
                    supp.Balance = pO.Balance;
                    //supp.StoreId = parseId; //commented due to session issue
                    supp.StoreId = 1;
                    db.Entry(supp).State = EntityState.Modified;
                    //db.SaveChanges();

                    //Payment payment = new Payment();
                    //payment = db.Payments.Find(orderId);
                    //payment.Status = true;
                    //db.Entry(payment).State = EntityState.Modified;
                    //db.SaveChanges();

                }

                ////////////////////////////////////////
                BankAccount bankAccount = db.BankAccounts.FirstOrDefault(x => x.Id == pO.BankAccountId);
                bankAccount.Balance -= pO.BillPaid;
                db.BankAccounts.Attach(bankAccount);
                db.Entry(bankAccount).Property(x => x.Balance).IsModified = true;
                ////////////////////////////////////////
                //int maxId = db.POes.Max(p => p.Auto);
                decimal maxId1 = (int)db.POes.DefaultIfEmpty().Max(p => p == null ? 0 : p.POSerial);
                maxId1 += 1;
                pO.POSerial = maxId1;
                pO.Quotation = true;
                pO.StoreId = storeId;
                //pO.StoreId = parseId; //commented due to session issue
                //pO.Date = DateTime.Now;
                if (string.IsNullOrEmpty(Convert.ToString(pO.Date)))
                {
                    pO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
                }
                //pO.SaleReturn = false;
                pO.Id = System.Guid.NewGuid().ToString().ToUpper();
                pO.PurchaseOrderAmount = 0;
                pO.PurchaseOrderQty = 0;
                Employee emp = (Employee)Session["CurrentUser"];
                pO.EmployeeId = emp.Id;
                db.POes.Add(pO);
                //db.SaveChanges();
                int sno = 0;
                if (pOD != null)
                {
                    //pOD.RemoveAll(so => so.ProductId == null);
                    foreach (POD pod in pOD)
                    {
                        sno += 1;
                        pod.PODId = sno;
                        pod.PO = pO;
                        pod.POId = pO.Id;

                        Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                        //StoreProduct storeProduct = db.StoreProducts.FirstOrDefault(x => x.ProductId == pod.ProductId && x.StoreId == parseId);  //commented due to session issue
                        StoreProduct storeProduct = db.StoreProducts.FirstOrDefault(x => x.ProductId == pod.ProductId);

                        //dont do this. when user made a bill and chnage sale price. it does not reflect in bill and calculations geting wrong
                        //pod.PurchasePrice = product.PurchasePrice;
                        if (pod.Quantity == null) { pod.Quantity = 0; }
                        //pod.OpeningStock = product.Stock;
                        pod.OpeningStock = storeProduct.Stock;
                        pod.PerPack = 1;
                        if (pod.SaleType == false)//purchase
                        {
                            if (pod.IsPack == false)
                            {//piece
                                pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                //storeProduct.Stock += qty;  //--due to qutation
                                pO.PurchaseOrderQty += qty;//(int)sod.Quantity;
                            }
                            else
                            {//pack

                                pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                //storeProduct.Stock += (int)pod.Quantity * pod.PerPack;  //--due to qutation
                                pO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }

                        }
                        else//return
                        {
                            if (pod.IsPack == false)
                            {
                                pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                //storeProduct.Stock -= qty;//--due to qutation
                                pO.PurchaseOrderQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {
                                pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                //storeProduct.Stock -= (int)pod.Quantity * pod.PerPack; //--due to qutation
                                pO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;
                            }

                        }

                    }
                    db.PODs.AddRange(pOD);
                }
                db.SaveChanges();


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


                string POId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(pO.Id, "BZNS")));
                //return PrintSO(POId);
                //return PrintSO3(POId);
                //return RedirectToAction("PrintSO3", new { id = POId });
                TempData["ReportId"] = pO.Id;
                return RedirectToAction("Create", new { IsReturn = "false" });
                //return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", pO.SupplierId);
            //return View(pO);
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel.Suppliers = DAL.dbSuppliers;
            purchaseOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7 == true && x.IsService == false);
            return View(purchaseOrderViewModel);
            //return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePOByCategory([Bind(Prefix = "Supplier", Include = "Name,Address")] Supplier Supplier, [Bind(Prefix = "PurchaseOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,SupplierId,Remarks,Remarks2,PaymentMethod,PaymentDetail,PurchaseReturn,FundingSourceId,BankAccountId,Date")] PO pO, [Bind(Prefix = "PurchaseOrderDetail", Include = "ProductId,Quantity,SaleType,PerPack,IsPack,PurchasePrice")] List<POD> pOD)
        {
            //PO pO = new PO();
            if (ModelState.IsValid)
            {
                int? storeId = Session["StoreId"] as int?;
                //var storeId = Session["StoreId"] as string;
                if (storeId == null)
                {
                    return RedirectToAction("StoreNotFound", "UserManagement");
                }
                //var storeId = Session["StoreId"] as string;  //commented due to session issue
                //if (storeId == null)
                //{
                //    return RedirectToAction("StoreNotFound", "UserManagement");
                //}
                //var parseId = int.Parse(storeId);

                Supplier supp = db.Suppliers.FirstOrDefault(x => x.Id == pO.SupplierId);
                if (supp == null)
                {//its means new customer
                    //pO.SupplierId = 10;
                    //int maxId = db.Suppliers.Max(p => p.Id);
                    decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;
                    Supplier.Id = maxId;
                    Supplier.Balance = pO.Balance;
                    Supplier.StoreId = storeId;
                    //Supplier.StoreId = parseId;  //commented due to session issue
                    db.Suppliers.Add(Supplier);
                    //db.SaveChanges();
                }
                else
                {//its means old customer. old customer balance should be updated.
                    //Supplier.Id = (int)pO.SupplierId;
                    supp.Balance = pO.Balance;
                    supp.StoreId = storeId;
                    //supp.StoreId = parseId;  //commented due to session issue
                    db.Entry(supp).State = EntityState.Modified;
                    //db.SaveChanges();

                    //Payment payment = new Payment();
                    //payment = db.Payments.Find(orderId);
                    //payment.Status = true;
                    //db.Entry(payment).State = EntityState.Modified;
                    //db.SaveChanges();

                }

                ////////////////////////////////////////
                BankAccount bankAccount = db.BankAccounts.FirstOrDefault(x => x.Id == pO.BankAccountId);
                bankAccount.Balance -= pO.BillPaid;
                db.BankAccounts.Attach(bankAccount);
                db.Entry(bankAccount).Property(x => x.Balance).IsModified = true;
                ////////////////////////////////////////
                //int maxId = db.POes.Max(p => p.Auto);
                decimal maxId1 = (int)db.POes.DefaultIfEmpty().Max(p => p == null ? 0 : p.POSerial);
                maxId1 += 1;
                pO.POSerial = maxId1;
                pO.Quotation = true;
                pO.StoreId = storeId; 
                //pO.StoreId = parseId; //commented due to session issue
                //pO.Date = DateTime.Now;
                if (string.IsNullOrEmpty(Convert.ToString(pO.Date)))
                {
                    pO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
                }
                //pO.SaleReturn = false;
                pO.Id = System.Guid.NewGuid().ToString().ToUpper();
                pO.PurchaseOrderAmount = 0;

                pO.PurchaseOrderQty = 0;

                Employee emp = (Employee)Session["CurrentUser"];
                pO.EmployeeId = emp.Id;
                db.POes.Add(pO);
                //db.SaveChanges();
                int sno = 0;
                if (pOD != null)
                {
                    //pOD.RemoveAll(so => so.ProductId == null);
                    foreach (POD pod in pOD)
                    {
                        sno += 1;
                        pod.PODId = sno;
                        pod.PO = pO;
                        pod.POId = pO.Id;

                        Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                        //StoreProduct storeProduct = db.StoreProducts.FirstOrDefault(x => x.ProductId == pod.ProductId && x.StoreId == parseId); //commented due to session issue
                        StoreProduct storeProduct = db.StoreProducts.FirstOrDefault(x => x.ProductId == pod.ProductId);

                        //dont do this. when user made a bill and chnage sale price. it does not reflect in bill and calculations geting wrong
                        //pod.PurchasePrice = product.PurchasePrice;
                        if (pod.Quantity == null || pod.Quantity == 0) { pod.Quantity = 0; }
                        //pod.OpeningStock = product.Stock;
                        pod.OpeningStock = storeProduct.Stock ?? 0;
                        pod.PerPack = 1;
                        if (pod.SaleType == false)//purchase
                        {

                            if (pod.IsPack == false)
                            {//piece
                                if (pod.Quantity != null || pod.Quantity > 0)
                                {
                                    pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);

                                    //int pieceSold = (int)(sod.Quantity * product.Stock);
                                    decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                                                        //storeProduct.Stock += qty;  //--due to qutation
                                    pO.PurchaseOrderQty += qty;//(int)sod.Quantity;
                                }

                            }
                            else
                            {//pack
                                if (pod.Quantity != null || pod.Quantity > 0)
                                {
                                    pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                    //storeProduct.Stock += (int)pod.Quantity * pod.PerPack;  //--due to qutation

                                    pO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                                }
                            }

                        }
                        else//return
                        {
                            if (pod.IsPack == false)
                            {
                                if (pod.Quantity != null || pod.Quantity > 0)
                                {
                                    pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                    decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                                                        //storeProduct.Stock -= qty;//--due to qutation
                                    pO.PurchaseOrderQty += qty;//(int)sod.Quantity;
                                }

                            }
                            else
                            {
                                if (pod.Quantity != null || pod.Quantity > 0)
                                {
                                    pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                    //storeProduct.Stock -= (int)pod.Quantity * pod.PerPack; //--due to qutation
                                    pO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;
                                }
                            }

                        }

                    }
                    db.PODs.AddRange(pOD);
                }
                db.SaveChanges();

                string POId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(pO.Id, "BZNS")));
                TempData["ReportId"] = pO.Id;
                return RedirectToAction("CreatePOByCategory", new { IsReturn = "false" });
                //return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", pO.SupplierId);
            //return View(pO);
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel.Suppliers = DAL.dbSuppliers;
            purchaseOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7  && x.IsService == false);
            return View(purchaseOrderViewModel);
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
            //{ viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Purchase_Receipt.rdlc"); }
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Quotation_Receipt.rdlc"); }

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
        //public ActionResult Edit([Bind(Prefix = "PurchaseOrder", Include = "BillAmount,Balance,BillPaid,Discount")] PO pO, [Bind(Prefix = "PurchaseOrderDetail", Include = "ProductId,Quantity")] List<POD> pOD)
        public ActionResult Edit(PurchaseOrderViewModel purchaseOrderViewModel1)
        {
            PO newPO = purchaseOrderViewModel1.PurchaseOrder;
            List<POD> newPODs = purchaseOrderViewModel1.PurchaseOrderDetail;
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
                newPO.Id = Encryption.Decrypt(purchaseOrderViewModel1.PurchaseOrder.Id, "BZNS");//
                PO PO = db.POes.Where(x => x.Id == newPO.Id).FirstOrDefault();
                //PO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));//
                //PO.PurchaseReturn = false;//
                PO.BillAmount = newPO.BillAmount;//
                PO.Discount = newPO.Discount;//
                PO.BillPaid = newPO.BillPaid;//
                PO.Balance = newPO.Balance;//
                PO.Remarks = newPO.Remarks;//
                PO.Remarks2 = newPO.Remarks;//
                PO.PaymentMethod = newPO.PaymentMethod;
                PO.PaymentDetail = newPO.PaymentDetail;
                PO.FundingSourceId = newPO.FundingSourceId;
                PO.BankAccountId = newPO.BankAccountId;
                PO.Date = newPO.Date;
                PO.Quotation = true;
                PO.StoreId = storeId;
                //PO.StoreId = parseId; //commented due to session issue
                //PO.POSerial = newPO.POSerial;//should be unchanged

                ///////////////////////////////////////////

                //Supplier cust = db.Suppliers.FirstOrDefault(x => x.Id == newPO.SupplierId);
                Supplier supplier = db.Suppliers.Where(x => x.Id == newPO.SupplierId).FirstOrDefault();
                if (supplier == null)
                {//its means new supplier(not in db)
                 //PO.SupplierId = 10;
                 //int maxId = db.Suppliers.Max(p => p.Id);
                    supplier = purchaseOrderViewModel1.Supplier;
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

                if (PO.SupplierId != newPO.SupplierId)
                {//POme other db supplier
                 //first revert the previous supplier balance 
                    Supplier oldSupplier = db.Suppliers.Where(x => x.Id == PO.SupplierId).FirstOrDefault();
                    oldSupplier.Balance = db.POes.Where(x => x.Id == PO.Id).FirstOrDefault().PrevBalance;
                    db.Entry(oldSupplier).State = EntityState.Modified;
                }

                PO.PrevBalance = newPO.PrevBalance;//
                // assign balance of this supplier
                //Supplier supplier = db.Suppliers.Where(x => x.Id == newPO.SupplierId).FirstOrDefault();
                supplier.Balance = newPO.Balance;
                //assign supplier and supplierId in PO
                PO.SupplierId = newPO.SupplierId;
                PO.Supplier = supplier;

                /////////////////////////////////////////////////////////////////////////////



                List<POD> oldPODs = db.PODs.Where(x => x.POId == newPO.Id).ToList();
                //List<POD> oldPODs = db.PODs.Where(x => x.POId == newPO.Id).ToList();

                //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this is simple and stateforward.
                //due to quotation
                //foreach (POD pod in oldPODs)
                //{
                //    Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                //    if (pod.SaleType == false)//purchase
                //    {
                //        //product.Stock -= pod.Quantity;

                //        if (pod.IsPack == false)
                //        {
                //            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                //            product.Stock -= qty;
                //        }
                //        else
                //        {
                //            product.Stock -= (int)pod.Quantity * pod.PerPack;
                //        }

                //    }
                //    else//return
                //    {
                //        //product.Stock += pod.Quantity;

                //        if (pod.IsPack == false)
                //        {
                //            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                //            product.Stock += qty;
                //        }
                //        else
                //        {
                //            product.Stock += (int)pod.Quantity * pod.PerPack;
                //        }


                //    }
                //    db.Entry(product).State = EntityState.Modified;
                //}

                db.PODs.RemoveRange(oldPODs);
                //////////////////////////////////////////////////////////////////////////////

                PO.PurchaseOrderAmount = 0;

                PO.PurchaseOrderQty = 0;

                //PO.Profit = 0;
                int sno = 0;

                if (newPODs != null)
                {

                    foreach (POD pod in newPODs)
                    {
                        sno += 1;
                        pod.PODId = sno;
                        pod.PO = PO;
                        pod.POId = PO.Id;

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
                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                //product.Stock += qty;//due to quotation

                                PO.PurchaseOrderQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {//pack

                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                //product.Stock += (int)pod.Quantity * pod.PerPack;//due to quotation

                                PO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }
                        }
                        else//return
                        {
                            if (pod.IsPack == false)
                            {
                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                //product.Stock -= qty;//due to quotation
                                PO.PurchaseOrderQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {
                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                //product.Stock -= (int)pod.Quantity * pod.PerPack;//due to quotation

                                PO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }

                        }

                    }
                    //PO.Profit -= (decimal)PO.Discount;
                    db.Entry(PO).State = EntityState.Modified;
                    db.Entry(PO).Property(x => x.POSerial).IsModified = false;
                    db.PODs.AddRange(newPODs);

                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", PO.SupplierId);
            //return View(PO);
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();

            purchaseOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7 && x.IsService == false);
            return View(purchaseOrderViewModel);
            //return View();
        }
        private string Decode(string id)
        {
            byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            id = new string(Encoding.UTF8.GetString(BytesArr).ToCharArray());
            id = Encryption.Decrypt(id, "BZNS");
            return id;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpadtePurhcaseOrder(decimal? poSerialNumber)
        {
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            //var parseId = int.Parse(storeId);
            var poSerialNumbers = db.POes.FirstOrDefault(x => x.POSerial == poSerialNumber);
            if (poSerialNumbers == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<POD> pOD = db.PODs.Where(x => x.POId == poSerialNumbers.Id).ToList();
            int sno = 0;
            if (pOD != null)
            {
                //pOD.RemoveAll(so => so.ProductId == null);
                foreach (POD pod in pOD)
                {
                    sno += 1;

                    Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                    //StoreProduct storeProduct = db.StoreProducts.FirstOrDefault(x => x.ProductId == pod.ProductId && x.StoreId == parseId); //commented due to session issue
                    StoreProduct storeProduct = db.StoreProducts.FirstOrDefault(x => x.ProductId == pod.ProductId);

                    //dont do this. when user made a bill and chnage sale price. it does not reflect in bill and calculations geting wrong
                    //pod.PurchasePrice = product.PurchasePrice;

                    if (pod.SaleType == false)//purchase
                    {

                        if (pod.IsPack == false)
                        {//piece

                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            storeProduct.Stock += qty;
                            //product.Stock += qty;
                        }
                        else
                        {//pack

                            //product.Stock += (int)pod.Quantity * pod.PerPack;
                            storeProduct.Stock += (int)pod.Quantity * pod.PerPack;

                        }

                    }
                    else//return
                    {
                        if (pod.IsPack == false)
                        {

                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            storeProduct.Stock -= qty;
                            //product.Stock -= qty;
                        }
                        else
                        {
                            storeProduct.Stock -= (int)pod.Quantity * pod.PerPack;
                            //product.Stock -= (int)pod.Quantity * pod.PerPack;
                        }

                    }
                    //db.Entry(pod).State = EntityState.Modified;
                }

            }
            db.SaveChanges();
            return Json(new { Success = true, Message = "Status updated successfully" });
        }


        // GET: POes/Edit/5
        public ActionResult Edit(string id, bool update)
        {

            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
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
            id = Decode(id);

            PO pO = db.POes.Find(id);
            if (pO == null)
            {
                return HttpNotFound();
            }
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            List<POD> pod = db.PODs.Where(x => x.POId == id).ToList();
            purchaseOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7 && x.IsService == false);
            purchaseOrderViewModel.Suppliers = DAL.dbSuppliers;
            purchaseOrderViewModel.PurchaseOrderDetail = pod;
            pO.Id = Encryption.Encrypt(pO.Id, "BZNS");
            purchaseOrderViewModel.PurchaseOrder = pO;
            int orderQty = 0;
            int orderQtyPiece = 0;
            int returnQty = 0;
            int returnQtyPiece = 0;
            foreach (var item in pod)
            {
                if (pO.PurchaseReturn == false)
                {
                    if (item.IsPack == true)
                    {
                        orderQty += (int)item.Quantity;
                    }
                    else
                    {
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
            ViewBag.FundingSources = new SelectList(db.Suppliers.Where(x => x.IsCreditor == true), "Id", "Name");//db.FundingSources.ToList(); ;
            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.orderQty = orderQty;
            ViewBag.orderQtyPiece = orderQtyPiece;
            ViewBag.returnQty = returnQty;
            ViewBag.returnQtyPiece = returnQtyPiece;
            ViewBag.SupplierName = pO.Supplier.Name;
            ViewBag.SupplierAddress = pO.Supplier.Address;
            decimal subTotal = (decimal)(pO.PurchaseOrderAmount - pO.Discount);
            ViewBag.SubTotal = subTotal;
            ViewBag.Total = subTotal + (decimal)pO.PrevBalance;
            ViewBag.IsUpdate = update;
            ViewBag.IsReturn = pO.PurchaseReturn.ToString().ToLower();
            return View(purchaseOrderViewModel);
        }

        // GET: POes/Edit/5
        public ActionResult EditPOSByCategory(string id, bool update)
        {

            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
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
            id = Decode(id);

            PO pO = db.POes.Find(id);
            if (pO == null)
            {
                return HttpNotFound();
            }
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            List<POD> pod = db.PODs.Where(x => x.POId == id).ToList();
            purchaseOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7 && x.IsService == false);
            purchaseOrderViewModel.Suppliers = DAL.dbSuppliers;
            purchaseOrderViewModel.PurchaseOrderDetail = pod;
            pO.Id = Encryption.Encrypt(pO.Id, "BZNS");
            purchaseOrderViewModel.PurchaseOrder = pO;
            int orderQty = 0;
            int orderQtyPiece = 0;
            int returnQty = 0;
            int returnQtyPiece = 0;
            foreach (var item in pod)
            {
                if (pO.PurchaseReturn == false)
                {
                    if (item.IsPack == true)
                    {
                        orderQty += (int)item.Quantity;
                    }
                    else
                    {
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
            ViewBag.FundingSources = new SelectList(db.Suppliers.Where(x => x.IsCreditor == true), "Id", "Name");//db.FundingSources.ToList(); ;
            ViewBag.BankAccounts = new SelectList(db.BankAccounts, "Id", "Name");
            ViewBag.orderQty = orderQty;
            ViewBag.orderQtyPiece = orderQtyPiece;
            ViewBag.returnQty = returnQty;
            ViewBag.returnQtyPiece = returnQtyPiece;
            ViewBag.SupplierName = pO.Supplier.Name;
            ViewBag.SupplierAddress = pO.Supplier.Address;
            decimal subTotal = (pO.PurchaseOrderAmount ?? 0) - (pO.Discount ?? 0);
            ViewBag.SubTotal = subTotal;
            ViewBag.Total = subTotal + (pO.PrevBalance ?? 0);
            ViewBag.IsUpdate = update;
            var products = db.Products.Include(x => x.StoreProducts).ToList();

            // Group products by category, handling null categories by assigning "Uncategorized"
            var groupedSelectedProducts = products
                .GroupBy(p => string.IsNullOrEmpty(p.Category) ? "Uncategorized" : p.Category)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Pass the grouped products to the view using ViewBag
            ViewBag.SelectedProductCategory = groupedSelectedProducts;
            ViewBag.IsReturn = pO.PurchaseReturn.ToString().ToLower();
            return View(purchaseOrderViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Prefix = "PurchaseOrder", Include = "BillAmount,Balance,BillPaid,Discount")] PO pO, [Bind(Prefix = "PurchaseOrderDetail", Include = "ProductId,Quantity")] List<POD> pOD)
        public ActionResult UpdatePOByCategory(PurchaseOrderViewModel purchaseOrderViewModel1)
        {
            PO newPO = purchaseOrderViewModel1.PurchaseOrder;
            List<POD> newPODs = purchaseOrderViewModel1.PurchaseOrderDetail;
            if (ModelState.IsValid)
            {
                int? storeId = Session["StoreId"] as int?; //commented due to session issue
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

                newPO.Id = Encryption.Decrypt(purchaseOrderViewModel1.PurchaseOrder.Id, "BZNS");//
                PO PO = db.POes.Where(x => x.Id == newPO.Id).FirstOrDefault();
                //PO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));//
                //PO.PurchaseReturn = false;//
                PO.BillAmount = newPO.BillAmount;//
                PO.Discount = newPO.Discount;//
                PO.BillPaid = newPO.BillPaid;//
                PO.Balance = newPO.Balance;//
                PO.Remarks = newPO.Remarks;//
                PO.Remarks2 = newPO.Remarks;//
                PO.PaymentMethod = newPO.PaymentMethod;
                PO.PaymentDetail = newPO.PaymentDetail;
                PO.FundingSourceId = newPO.FundingSourceId;
                PO.BankAccountId = newPO.BankAccountId;
                PO.Date = newPO.Date;
                PO.Quotation = true;
                PO.StoreId = storeId;
                
                //PO.StoreId = parseId; //commented due to session issue
                //PO.POSerial = newPO.POSerial;//should be unchanged

                ///////////////////////////////////////////

                //Supplier cust = db.Suppliers.FirstOrDefault(x => x.Id == newPO.SupplierId);
                Supplier supplier = db.Suppliers.Where(x => x.Id == newPO.SupplierId).FirstOrDefault();
                if (supplier == null)
                {//its means new supplier(not in db)
                 //PO.SupplierId = 10;
                 //int maxId = db.Suppliers.Max(p => p.Id);
                    supplier = purchaseOrderViewModel1.Supplier;
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

                if (PO.SupplierId != newPO.SupplierId)
                {//POme other db supplier
                 //first revert the previous supplier balance 
                    Supplier oldSupplier = db.Suppliers.Where(x => x.Id == PO.SupplierId).FirstOrDefault();
                    oldSupplier.Balance = db.POes.Where(x => x.Id == PO.Id).FirstOrDefault().PrevBalance;
                    db.Entry(oldSupplier).State = EntityState.Modified;
                }

                PO.PrevBalance = newPO.PrevBalance;//
                // assign balance of this supplier
                //Supplier supplier = db.Suppliers.Where(x => x.Id == newPO.SupplierId).FirstOrDefault();
                supplier.Balance = newPO.Balance;
                //assign supplier and supplierId in PO
                PO.SupplierId = newPO.SupplierId;
                PO.Supplier = supplier;

                /////////////////////////////////////////////////////////////////////////////



                List<POD> oldPODs = db.PODs.Where(x => x.POId == newPO.Id).ToList();

                //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this is simple and stateforward.
                //due to quotation
                //foreach (POD pod in oldPODs)
                //{
                //    Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                //    if (pod.SaleType == false)//purchase
                //    {
                //        //product.Stock -= pod.Quantity;

                //        if (pod.IsPack == false)
                //        {
                //            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                //            product.Stock -= qty;
                //        }
                //        else
                //        {
                //            product.Stock -= (int)pod.Quantity * pod.PerPack;
                //        }

                //    }
                //    else//return
                //    {
                //        //product.Stock += pod.Quantity;

                //        if (pod.IsPack == false)
                //        {
                //            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                //            product.Stock += qty;
                //        }
                //        else
                //        {
                //            product.Stock += (int)pod.Quantity * pod.PerPack;
                //        }


                //    }
                //    db.Entry(product).State = EntityState.Modified;
                //}

                db.PODs.RemoveRange(oldPODs);
                //////////////////////////////////////////////////////////////////////////////

                PO.PurchaseOrderAmount = 0;

                PO.PurchaseOrderQty = 0;

                //PO.Profit = 0;
                int sno = 0;

                if (newPODs != null)
                {

                    foreach (POD pod in newPODs)
                    {
                        sno += 1;
                        pod.PODId = sno;
                        pod.PO = PO;
                        pod.POId = PO.Id;

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
                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                //product.Stock += qty;//due to quotation

                                PO.PurchaseOrderQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {//pack

                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                //product.Stock += (int)pod.Quantity * pod.PerPack;//due to quotation

                                PO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }
                        }
                        else//return
                        {
                            if (pod.IsPack == false)
                            {
                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                //product.Stock -= qty;//due to quotation
                                PO.PurchaseOrderQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {
                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                //product.Stock -= (int)pod.Quantity * pod.PerPack;//due to quotation

                                PO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }

                        }

                    }
                    //PO.Profit -= (decimal)PO.Discount;
                    db.Entry(PO).State = EntityState.Modified;
                    db.Entry(PO).Property(x => x.POSerial).IsModified = false;
                    db.PODs.AddRange(newPODs);

                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", PO.SupplierId);
            //return View(PO);
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();

            purchaseOrderViewModel.Products = DAL.dbProducts.Where(x => x.PType == 4 || x.PType == 7 && x.IsService == false);
            return View(purchaseOrderViewModel);
            //return View();
        }

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
