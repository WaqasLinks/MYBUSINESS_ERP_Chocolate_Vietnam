using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PaperOrderViewModel
    {
        public int PackagingProductionId { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal ProductId { get; set; }  // Changed from int to decimal
        public decimal? TotalQuantity { get; set; }  // Matches decimal(38, 30)
        public bool? Validate { get; set; }
        public DateTime PProdDate { get; set; }
        public string Box { get; set; }
        public string ProductColorCode { get; set; }
        public string Supplier { get; set; }
        public int ColorId { get; set; }
        public string Color { get; set; }
        public decimal ColorQuantity { get; set; }  // Matches numeric(18, 0)
        public string ColorSpecificCode { get; set; }
    }
}