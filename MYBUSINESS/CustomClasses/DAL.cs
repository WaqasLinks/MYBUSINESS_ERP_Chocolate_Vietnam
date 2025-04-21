using Azure.Core;
using MYBUSINESS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.CustomClasses
{
    public class DAL
    {
        private BusinessContext db = new BusinessContext();
        //public static List<Customer> dbCustomers()
        //{
        //    return db.Customers.Where(x => x.Status != "D").ToList<Customer>();
        //}

        //private List<Supplier> supplier;
        public IQueryable<Supplier> dbSuppliers
        {
            get { return db.Suppliers.Where(x => x.Status != "D"); }
            //set { customers = value; }
        }

        private IQueryable<Customer> customers;
        public IQueryable<Customer> dbCustomers
        {
            get { return db.Customers.Where(x => x.Status != "D"); }
            set { customers = value; }
        }

        private IQueryable<Currency> currencies;
        public IQueryable<Currency> dbCurrencies
        {
            get { return db.Currencies.Where(x => x.IsActive != false); }
            set { currencies = value; }
        }

        //private List<Product> products;
        public IQueryable<Product> dbProducts
        {
            get { return db.Products.Where(x => x.Status != "D"); }
            //set { products = value; }
        }
        public IEnumerable<Color> dbColors
        {
            get { return db.Colors.ToList(); }
            set { dbColors = value; }

        }
        public IEnumerable<FinalProduction> dbFinalProductions
        {
            get { return db.FinalProductions.ToList(); }
            set { dbFinalProductions = value; }

        }
        public IEnumerable<BOM> dbBOMs
        {
            get { return db.BOMs.ToList(); }
            set { dbBOMs = value; }
        }
        public IEnumerable<PPBOM> dbPPBOMs
        {
            get { return db.PPBOMs.ToList(); }
            set { dbPPBOMs = value; }
        }
        public IEnumerable<ScanBankDeposit> dbScanBankDeposit
        {
            get { return db.ScanBankDeposits.ToList(); }
            set { dbScanBankDeposit = value; }
        }
        public IEnumerable<ScanMoneyInput> dbScanMoneyInput
        {
            get { return db.ScanMoneyInputs.ToList(); }
            set { dbScanMoneyInput = value; }
        }
        public IEnumerable<ScanCreditCard> dbScanCreditCard
        {
            get { return db.ScanCreditCards.ToList(); }
            set { dbScanCreditCard = value; }
        }
        public IEnumerable<NewProduction> dbNewProductions
        {
            get { return db.NewProductions.ToList(); }
            set { dbNewProductions = value; }
        }
        public IEnumerable<PackagingProduction> dbPackagingProductions
        {
            get { return db.PackagingProductions.ToList(); }
            set { dbPackagingProductions = value; }
        }

        //private List<Store> stores;
        public IQueryable<Store> dbStore
        {
            get { return db.Stores.Where(x => x.Status != "D"); }
            //set { products = value; }
        }
        public IQueryable<DailyBalanceVnd> dbVndBalance
        {
            //get { return db.DailyBalanceVnds.Where(x => x.Status != "D"); }
            get { return db.DailyBalanceVnds; }
            //set { products = value; }
        }

        
    }
}