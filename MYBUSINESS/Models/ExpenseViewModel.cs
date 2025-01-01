using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ExpenseViewModel
    {
        public IQueryable<Supplier> Suppliers { get; set; }
        public Supplier Supplier { get; set; }
        public Expense PurchaseOrder { get; set; }
        public List<ExpenseDetail> PurchaseOrderDetail { get; set; }
        public IQueryable<Product> Products { get; set; }
        public Product Product { get; set; }
        //public List<FundingSource> FundingSources { get; set; }

    }
}