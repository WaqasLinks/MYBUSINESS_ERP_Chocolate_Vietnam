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
using System.Web.Routing;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class DashboardController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        //private IQueryable<Product> _dbFilteredProducts;
        //private IQueryable<Supplier> _dbFilteredSuppliers;
        //private IQueryable<Account> _dbFilteredAccounts;
        //private IQueryable<PO> _dbFilteredPO;
        //private IQueryable<SO> _dbFilteredSO;

        //private IQueryable<Employee> _dbFilteredEmployees;
        //private IQueryable<Department> _dbFilteredDepartments;
        //private IQueryable<Customer> _dbFilteredCustomers;


        //protected override void Initialize(RequestContext requestContext)
        //{
        //    //Employee employee1 = TempData["mydata"] as Employee;
        //    //Employee employee = ViewBag.data;
        //    base.Initialize(requestContext);
        //    //decimal BusinessId = decimal.Parse(this.ControllerContext.RouteData.Values["CurrentBusiness"].ToString());
        //    Business CurrentBusiness = (Business)Session["CurrentBusiness"];
        //    _dbFilteredSuppliers = db.Suppliers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        //    _dbFilteredProducts = db.Products.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
        //    _dbFilteredPO = db.POes.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
        //    _dbFilteredAccounts = db.Accounts.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        //    _dbFilteredSO = db.SOes.AsQueryable().Where(x => x.Employee.Department.bizId == CurrentBusiness.Id);


        //    _dbFilteredEmployees = db.Employees.AsQueryable().Where(x => x.Department.bizId == CurrentBusiness.Id);
        //    _dbFilteredDepartments = db.Departments.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        //    _dbFilteredCustomers = db.Customers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        //    _dbFilteredProducts = db.Products.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);

        //}

        // GET: Dashboard
        public ActionResult Index()
        {
            decimal SaleOrderCount  = db.SOes.Count();
            ViewBag.SOCount = SaleOrderCount;


            decimal SaleOrderAmount= (decimal)(db.SOes.Where(x=> x.SaleReturn == false).Sum(x => x.SaleOrderAmount) ?? 0);
            ViewBag.SOAmount = SaleOrderAmount;

            decimal RentalAmount = (decimal)(db.Rents.Sum(x => x.SaleOrderAmount) ?? 0);
            ViewBag.RentalAmount = RentalAmount;

            decimal IncomeAmount = (decimal)(db.Services.Sum(x => x.SaleOrderAmount) ?? 0);
            ViewBag.IncomeAmount = IncomeAmount;

            decimal PurchaseOrderCount = db.POes.Where(x=>x.SupplierId>0).Count();
            ViewBag.POCount = PurchaseOrderCount;

            decimal PurchaseOrderAmount = (decimal)(db.POes.Sum(x => x.PurchaseOrderAmount) ?? 0);
             ViewBag.POAmount = PurchaseOrderAmount;

            decimal LoanAmount = (decimal)(db.Loans.Sum(x => x.PurchaseOrderAmount) ?? 0);
            ViewBag.LoanAmount = LoanAmount;

            decimal ExpenseAmount = (decimal)(db.Expenses.Sum(x => x.PurchaseOrderAmount) ?? 0);
            ViewBag.ExpenseAmount = ExpenseAmount;
            
            decimal Profit = (decimal)(db.SOes.Where(x => x.SaleReturn == false).Sum(x => x.Profit) ?? 0);
            //ViewBag.Profit = (decimal)(SaleOrderCount - PurchaseOrderCount);

            ViewBag.Profit = Profit;


            ViewBag.Products = DAL.dbProducts.Count();
            
            ViewBag.Suppliers = DAL.dbSuppliers.Where(x => x.Id > 0).Count();
            
            ViewBag.Customers = DAL.dbCustomers.Count();
            
            ViewBag.Employees = db.Employees.Count();

            return View();
        }
    }
}