using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class OrderReceiptColorProductionViewModel
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public string StoreName { get; set; }
        public string ProductCategory { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public decimal Quantity { get; set; }
    }
}