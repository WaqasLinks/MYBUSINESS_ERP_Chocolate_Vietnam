using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PaperOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public string SupplierName { get; set; }
        public string Color { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public string ProductName { get; set; }
        public string Box { get; set; }
        public string ColorCode { get; set; }
        public bool? Validate { get; set; }
    }
}