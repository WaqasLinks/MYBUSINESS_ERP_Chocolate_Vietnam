using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class StoreOrderReceiptViewModel
    {
        public string SelectedStoreId { get; set; }
        public StoreOrderReceipt StoreOrderReceipt { get; set; }
        public IQueryable<Product> Products { get; set; }
        public Product Product { get; set; }
    }
}