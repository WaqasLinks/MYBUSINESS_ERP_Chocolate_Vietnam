using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ChocolateProductionIngredientViewModel
    {
        public string MainIngredient { get; set; }  // Changed to match SP
        public decimal QuantityCalculate { get; set; }  // Already matches SP
        public string Unit { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? ExpiryQuantity { get; set; }
    }
}