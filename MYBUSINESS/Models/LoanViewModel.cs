using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class LoanViewModel
    {
        public IQueryable<Supplier> Suppliers { get; set; }
        public Supplier Supplier { get; set; }
        public Loan PurchaseOrder { get; set; }
        public List<LoanDetail> PurchaseOrderDetail { get; set; }
        public IQueryable<Product> Products { get; set; }
        public Product Product { get; set; }
        //public List<FundingSource> FundingSources { get; set; }

    }
}