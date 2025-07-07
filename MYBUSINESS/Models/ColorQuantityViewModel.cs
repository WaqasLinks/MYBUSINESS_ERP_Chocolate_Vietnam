using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ColorQuantityViewModel
    {
        public int Id { get; set; }
        public int PaperColorId { get; set; }
        public string ColorName { get; set; }
        public decimal QuantityReceived { get; set; }
        public decimal ToReceived { get; set; }
        public string Unit { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int PackagingProductionId { get; set; }
        public string ColorCode { get; set; } // For the detailed version
        public string Supplier { get; set; } // For the detailed version
        public string ReceiptStatus { get; set; } // For the detailed version
    }
}