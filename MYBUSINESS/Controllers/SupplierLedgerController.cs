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
    public class SupplierLedgerController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        // GET: POes
        public ActionResult Index(int SuppId)
        {
            //EnterProfit();
            //var storeId = Session["StoreId"] as string; //commented due to session issue
            //if (storeId == null)
            //{
            //    return RedirectToAction("StoreNotFound", "UserManagement");
            //}
            //var parseId = int.Parse(storeId);

            IQueryable<PO> pOes = db.POes.Include(s => s.Supplier).Where(x => x.SupplierId == SuppId);

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
            ViewBag.ThisSupplier = db.Suppliers.Where(x => x.Id == SuppId).FirstOrDefault();

            ViewBag.Suppliers = DAL.dbSuppliers;
            //var purchaseOrders = pOes.Where(x => x.StoreId == parseId).OrderBy(i => i.Date).ToList(); //commented due to session issue
            var purchaseOrders = pOes.Where(x => x.StoreId == 1).OrderBy(i => i.Date).ToList();
            return View(purchaseOrders);
        }

        private void GetTotalBalance(ref IQueryable<PO> POes)
        {
            //IQueryable<PO> DistPOes = POes.Select(x => x.SupplierId).Distinct();
            IQueryable<PO> DistPOes = POes.GroupBy(x => x.SupplierId).Select(y => y.FirstOrDefault());

            decimal TotalBalance = 0;
            foreach (PO itm in DistPOes)
            {
                Supplier Supp = db.Suppliers.Where(x => x.Id == itm.SupplierId).FirstOrDefault();
                TotalBalance += (decimal)Supp.Balance;
            }
            ViewBag.TotalBalance = TotalBalance;

        }
        //[ChildActionOnly]
        //public PartialViewResult _SelectedPOSR()
        //{

        //    return PartialView(db.POes);
        //}

        //public ActionResult SearchData(string SuppName, DateTime startDate, DateTime endDate)

        //public ActionResult SearchData(string SuppName, string startDate, string endDate)
        public ActionResult SearchData(string SuppId, string startDate, string endDate)
        {
            if (startDate == null)
            {
                startDate = string.Empty;
            }
            if (endDate == null)
            {
                endDate = string.Empty;
            }

            int intSuppId = 0;
            DateTime dtStartDate = DateTime.Parse("1-1-1800");
            DateTime dtEndtDate = DateTime.Parse("1-1-2099");

            if (SuppId != string.Empty)
            {
                intSuppId = Int32.Parse(SuppId);
            }

            if (startDate != string.Empty)
            {
                dtStartDate = DateTime.Parse(startDate);
            }

            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
            }


            IQueryable<PO> selectedPOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (SuppId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                selectedPOes = db.POes.Where(po => po.SupplierId == intSuppId && po.Date >= dtStartDate && po.Date <= dtEndtDate);
            }

            if (SuppId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);
                selectedPOes = db.POes;//.Where(po => po.SupplierId == intSuppId && po.Date >= dtStartDate && po.Date <= dtEndtDate);

            }

            //get all Suppliers data acornding to start end date
            if (SuppId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                selectedPOes = db.POes.Where(po => po.Date >= dtStartDate && po.Date <= dtEndtDate);
            }

            //get this Supplier with from undefined startdate to this defined enddate
            if (SuppId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                selectedPOes = db.POes.Where(po => po.SupplierId == intSuppId && po.Date >= dtStartDate && po.Date <= dtEndtDate);

            }

            //get this Supplier with from defined start date to undefined end date
            if (SuppId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                dtEndtDate = DateTime.Today.AddDays(1);
                selectedPOes = db.POes.Where(po => po.SupplierId == intSuppId && po.Date >= dtStartDate && po.Date <= dtEndtDate);

            }

            //get this Supplier with all dates
            if (SuppId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);
                selectedPOes = db.POes.Where(po => po.SupplierId == intSuppId && po.Date >= dtStartDate && po.Date <= dtEndtDate);

            }

            //get all Supplier with defined startdate and undefined end date
            if (SuppId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtEndtDate = DateTime.Today.AddDays(1);
                selectedPOes = db.POes.Where(po => po.Date >= dtStartDate && po.Date <= dtEndtDate);

            }

            //get all Suppliers with undifined start date with defined enddate
            if (SuppId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                selectedPOes = db.POes.Where(po => po.Date >= dtStartDate && po.Date <= dtEndtDate);

            }


            foreach (PO itm in selectedPOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            GetTotalBalance(ref selectedPOes);
            //ViewBag.Suppliers = db.Suppliers;
            //ViewBag.ThisSupplier = db.Suppliers.Where(x => x.Id == intSuppId).FirstOrDefault();
            return PartialView("_Ledger", selectedPOes.OrderBy(i => i.Date).ToList());

            //return View("Some thing went wrong");


        }






        private string Decode(string id)
        {
            byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            id = new string(Encoding.UTF8.GetString(BytesArr).ToCharArray());
            id = Encryption.Decrypt(id, "BZNS");
            return id;
        }





    }

}
