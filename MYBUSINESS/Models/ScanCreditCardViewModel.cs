using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ScanCreditCardViewModel
    {
        public string SelectedStoreId { get; set; } // Changed from long? to string
        public ScanCreditCard ScanCreditCard { get; set; }
        public IQueryable<Product> Products { get; set; }
        public Product Product { get; set; }
    }
}