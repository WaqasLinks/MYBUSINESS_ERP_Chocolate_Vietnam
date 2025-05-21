using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class OrderReceiptViewModel
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public string StoreName { get; set; }
        public int StoreId { get; set; }
        public DateTime OrderDate { get; set; }
        public string UniqueCode { get; set; }
        public int OrderId { get; set; } // Add this if needed to filter orders
        public string ProductCategory { get; set; }
        public string ProductUnit { get; set; }
    }
}