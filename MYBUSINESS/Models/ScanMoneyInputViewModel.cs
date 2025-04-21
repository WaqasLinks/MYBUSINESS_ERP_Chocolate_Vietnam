using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ScanMoneyInputViewModel
    {
        public string SelectedStoreId { get; set; } // Changed from long? to string
        public ScanMoneyInput ScanMoneyInput { get; set; }
        public IQueryable<Product> Products { get; set; }
        public Product Product { get; set; }
    }
}