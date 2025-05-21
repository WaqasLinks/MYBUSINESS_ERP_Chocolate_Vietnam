using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class FlatBoxStockViewModel
    {
        public int ProductId { get; set; } // Changed to int
        public string ProductName { get; set; }
        public int TotalCompleteBoxes { get; set; } // Changed to int
        public List<ColorStockInfo> ColorComponents { get; set; }
        public DateTime LastUpdated { get; set; }
        public int PaperColorId { get; set; }
        public int PPSubItemId { get; set; }
        public int PackagingProductionId { get; set; }
        public int Quantity { get; set; }

    }
    public class ColorStockInfo
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public int Quantity { get; set; }
    }
}