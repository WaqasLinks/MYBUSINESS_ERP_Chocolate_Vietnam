using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class QuantityToProduceViewModel
    {
        public int ProductId { get; set; }
        public string Shape { get; set; }
        public decimal ProductionQty { get; set; }
        public decimal? CalculatedWeight { get; set; }
        public decimal Weight { get; set; } // ✅ Add Weight property
    }
}