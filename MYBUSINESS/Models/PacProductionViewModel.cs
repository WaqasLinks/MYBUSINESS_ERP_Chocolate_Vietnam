using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PacProductionViewModel
    {
        public virtual ICollection<PacSubitem> PacSubitems { get; set; } = new List<PacSubitem>();
        public List<PacColor> PacColor { get; set; } = new List<PacColor>();
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }
        public PacSubitem PacSubitem { get; set; }
        public PackagingBOM PackagingBOM { get; set; }
        public virtual ICollection<PacSubItemProduction> PacSubItemProduction { get; set; } = new List<PacSubItemProduction>();
        public PacSubItemProduction PacSubItemProductions { get; set; }
        
        public Product Product { get; set; }

        public PacProduction PacProduction { get; set; }
        public decimal TotalWeight { get; set; }
        public Dictionary<int, decimal> CalculatedSubitems { get; set; } = new Dictionary<int, decimal>();
        //public decimal calculatedsubitem { get; set; }

        public decimal CalculatedValue { get; set; }
        public string ProductType { get; set; } // ✅ New Property for PType

        public string ProductionProcessCategory { get; set; }
    }
}