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
    public class BankAccountLedgerController : Controller
    {
        private BusinessContext db = new BusinessContext();

        // GET: SOes
        public ActionResult Index(string accountId)
        {
            //EnterProfit();
            //accountId="020678dd-9c6b-4973-8acf-18d5dcdd55f9";
            //IQueryable<BankAccount> AccountTransections = db.BankAccounts.Include(s => s.SOes).Include(p=>p.POes).Where(x => x.Id==accountId);
            IQueryable<SO> sOes = db.SOes.Include(s => s.Customer).Where(x => x.BankAccountId == accountId);
            decimal OpenignBal = 0;
            //GetTotalBalance(ref sOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            int thisSerial = 0;
            AccountLedgerViewModel accountLedgerViewModel;
            List<AccountLedgerViewModel> LstaccountLedgerViewModel= new List<AccountLedgerViewModel>();
            foreach (SO itm in sOes)
            {
                OpenignBal += itm.BillPaid;
                thisSerial = (int)itm.Customer.SOes.Max(x => x.SOSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.CustomerId))
                {
                    LstMaxSerialNo.Add(itm.Customer.Id, thisSerial);
                }

                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));

                ////////////////
                accountLedgerViewModel = new AccountLedgerViewModel { Id=itm.Id, Serial=itm.SOSerial,  Date=itm.Date, OrderAmountDiscounted=itm.SaleOrderAmount-itm.Discount, BillPaid=itm.BillPaid,
                Balance=itm.Balance, IsSale=true,IsReturn=itm.SaleReturn, PrevBalance=itm.PrevBalance, CashIn=itm.SaleReturn=false?true:false,PersonName=""};
               
                LstaccountLedgerViewModel.Add(accountLedgerViewModel);
                ////////////////
            }
            IQueryable<PO> pOes = db.POes.Include(s => s.Supplier).Where(x => x.BankAccountId == accountId);
            foreach (PO itm in pOes)
            {
                OpenignBal -= itm.BillPaid;
                thisSerial = (int)itm.Supplier.POes.Max(x => x.POSerial);

                if (!LstMaxSerialNo.ContainsKey((int)itm.SupplierId))
                {
                    LstMaxSerialNo.Add(itm.Supplier.Id, thisSerial);
                }

                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
                accountLedgerViewModel = new AccountLedgerViewModel {Id = itm.Id, Serial=itm.POSerial, Date = itm.Date,OrderAmountDiscounted = itm.PurchaseOrderAmount - itm.Discount,BillPaid = itm.BillPaid,Balance = itm.Balance,IsSale = false,
                    IsReturn = itm.PurchaseReturn,PrevBalance = itm.PrevBalance, CashIn=itm.PurchaseReturn=false?false:true,PersonName=""};
                LstaccountLedgerViewModel.Add(accountLedgerViewModel);
            }
            ViewBag.OpeningBal = OpenignBal;
            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            ViewBag.ThisCustomer = db.Customers.Where(x => x.Id == 1).FirstOrDefault();
            ViewBag.BankAccount = db.BankAccounts.FirstOrDefault(x => x.Id == accountId);
            ViewBag.Customers = db.Customers;
            return View(LstaccountLedgerViewModel.OrderBy(i => i.Date).ToList());
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

        //public ActionResult SearchData(string custName, DateTime startDate, DateTime endDate)

        //public ActionResult SearchData(string custName, string startDate, string endDate)
        public ActionResult SearchData(string BankAccountId, string startDate, string endDate)
        {
            //BankAccountId = "020678dd-9c6b-4973-8acf-18d5dcdd55f9";
            if (startDate == null)
            {
                startDate = string.Empty;
            }
            if (endDate == null)
            {
                endDate = string.Empty;
            }

            //int intCustId=0;
            DateTime dtStartDate = DateTime.Parse("1-1-1800");
            DateTime dtEndtDate = DateTime.Parse("1-1-2099");

            //if (accountId != string.Empty)
            //{
            //    intCustId = Int32.Parse(accountId);
            //}

            if (startDate != string.Empty)
            {
                dtStartDate = DateTime.Parse(startDate);
            }

            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
            }


            IQueryable<SO> selectedSOes = null;
            IQueryable<PO> selectedPOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (BankAccountId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                selectedSOes = db.SOes.Where(so => so.BankAccountId == BankAccountId && so.Date >= dtStartDate && so.Date <= dtEndtDate);
                selectedPOes = db.POes.Where(po => po.BankAccountId == BankAccountId && po.Date >= dtStartDate && po.Date <= dtEndtDate);
            }

            if (BankAccountId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);
                selectedSOes = db.SOes;//.Where(so => so.CustomerId == intCustId && so.Date >= dtStartDate && so.Date <= dtEndtDate);
                selectedPOes = db.POes;
            }

            //get all customers data acornding to start end date
            if (BankAccountId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);
                selectedPOes = db.POes.Where(po => po.Date >= dtStartDate && po.Date <= dtEndtDate);
            }

            //get this customer with from undefined startdate to this defined enddate
            if (BankAccountId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                selectedSOes = db.SOes.Where(so => so.BankAccountId == BankAccountId && so.Date >= dtStartDate && so.Date <= dtEndtDate);
                selectedPOes = db.POes.Where(po => po.BankAccountId == BankAccountId && po.Date >= dtStartDate && po.Date <= dtEndtDate);
            }

            //get this customer with from defined start date to undefined end date
            if (BankAccountId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                dtEndtDate = DateTime.Today.AddDays(1);
                selectedSOes = db.SOes.Where(so => so.BankAccountId == BankAccountId && so.Date >= dtStartDate && so.Date <= dtEndtDate);
                selectedPOes = db.POes.Where(po => po.BankAccountId == BankAccountId && po.Date >= dtStartDate && po.Date <= dtEndtDate);
            }

            //get this customer with all dates
            if (BankAccountId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);
                selectedSOes = db.SOes.Where(so => so.BankAccountId == BankAccountId && so.Date >= dtStartDate && so.Date <= dtEndtDate);
                selectedPOes = db.POes.Where(po => po.BankAccountId == BankAccountId && po.Date >= dtStartDate && po.Date <= dtEndtDate);
            }

            //get all customer with defined startdate and undefined end date
            if (BankAccountId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtEndtDate = DateTime.Today.AddDays(1);
                selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);
                selectedPOes = db.POes.Where(po => po.Date >= dtStartDate && po.Date <= dtEndtDate);
            }

            //get all customers with undifined start date with defined enddate
            if (BankAccountId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                selectedSOes = db.SOes.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);
                selectedPOes = db.POes.Where(po => po.Date >= dtStartDate && po.Date <= dtEndtDate);
            }


            foreach (SO itm in selectedSOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }
            foreach (PO itm in selectedPOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
            }

            /////////////////////////////
            decimal OpenignBal = 0;
            int thisSerial = 0;
            AccountLedgerViewModel accountLedgerViewModel;
            List<AccountLedgerViewModel> LstaccountLedgerViewModel = new List<AccountLedgerViewModel>();
            foreach (SO itm in selectedSOes)
            {
                OpenignBal += itm.BillPaid;
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));

                ////////////////
                accountLedgerViewModel = new AccountLedgerViewModel
                {
                    Id = itm.Id,
                    Serial = itm.SOSerial,
                    Date = itm.Date,
                    OrderAmountDiscounted = itm.SaleOrderAmount - itm.Discount,
                    BillPaid = itm.BillPaid,
                    Balance = itm.Balance,
                    IsSale = true,
                    IsReturn = itm.SaleReturn,
                    PrevBalance = itm.PrevBalance,
                    CashIn = itm.SaleReturn = false ? true : false,
                    PersonName = ""
                };
                LstaccountLedgerViewModel.Add(accountLedgerViewModel);
                ////////////////
            }
           
            foreach (PO itm in selectedPOes)
            {
                OpenignBal -= itm.BillPaid;
                //itm.Id = Encryption.Encrypt(itm.Id, "BZNS");
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id, "BZNS")));
                accountLedgerViewModel = new AccountLedgerViewModel
                {
                    Id = itm.Id,
                    Serial = itm.POSerial,
                    Date = itm.Date,
                    OrderAmountDiscounted = itm.PurchaseOrderAmount - itm.Discount,
                    BillPaid = itm.BillPaid,
                    Balance = itm.Balance,
                    IsSale = false,
                    IsReturn = itm.PurchaseReturn,
                    PrevBalance = itm.PrevBalance,
                    CashIn = itm.PurchaseReturn = false ? false : true,
                    PersonName = ""
                };
                LstaccountLedgerViewModel.Add(accountLedgerViewModel);
            }
            /////////////////////////////
            ViewBag.OpeningBal = OpenignBal;
            //GetTotalBalance(ref selectedSOes);
            //ViewBag.Customers = db.Customers;
            //ViewBag.ThisCustomer = db.Customers.Where(x => x.Id == intCustId).FirstOrDefault();
            return PartialView("_Ledger", LstaccountLedgerViewModel.OrderBy(i => i.Date).ToList());

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
