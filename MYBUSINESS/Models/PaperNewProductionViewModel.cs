using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PaperNewProductionViewModel
    {
        public virtual ICollection<SubItem> SubItems { get; set; } = new List<SubItem>();
        public List<PaperQuantityToProduce> PaperQuantityToProduce { get; set; } = new List<PaperQuantityToProduce>();
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }
        public SubItem SubItem { get; set; }
        public virtual ICollection<PaperSubItemProduction> PaperSubItemProduction { get; set; } = new List<PaperSubItemProduction>();
        public PaperSubItemProduction PaperSubItemProductions { get; set; }
        public Product Product { get; set; }

        public PaperNewProduction PaperNewProduction { get; set; }
        public decimal TotalWeight { get; set; }
        public Dictionary<int, decimal> CalculatedSubitems { get; set; } = new Dictionary<int, decimal>();
        //public decimal calculatedsubitem { get; set; }

        public decimal CalculatedValue { get; set; }
        public string ProductType { get; set; } // ✅ New Property for PType
    }
}