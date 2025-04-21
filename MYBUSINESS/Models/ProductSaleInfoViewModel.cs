using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ProductSaleInfoViewModel
    {
        public decimal ProductId { get; set; } // or string if your ProductId is string type
        public string ProductName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalSaleValue { get; set; }
    }
}