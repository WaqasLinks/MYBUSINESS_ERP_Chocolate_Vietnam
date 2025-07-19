using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ChocolatePotProductionShapeViewModel
    {
        public string Shape { get; set; }
        public decimal ShapeQuantity { get; set; }  // Maps to ProductionQty from SP
        public decimal ShapeWeight { get; set; }
    }
}