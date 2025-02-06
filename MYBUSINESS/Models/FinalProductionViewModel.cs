using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class FinalProductionViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal SubtractValue { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal CalculatedValue { get; set; }
    }
}