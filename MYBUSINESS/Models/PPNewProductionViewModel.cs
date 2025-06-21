using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PPNewProductionViewModel
    {
        public virtual ICollection<SubItem> SubItems { get; set; } = new List<SubItem>();
        public List<PPQuantityToProduce> PPQuantityToProduce { get; set; } = new List<PPQuantityToProduce>();
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }
        public SubItem SubItem { get; set; }
        public List<PPSubItemProduction> PPSubItemProductions { get; set; } = new List<PPSubItemProduction>();
        public Product Product { get; set; }

        public PPNewProduction PPNewProduction { get; set; }
        public decimal TotalWeight { get; set; }
        public Dictionary<int, decimal> CalculatedSubitems { get; set; } = new Dictionary<int, decimal>();
        //public decimal calculatedsubitem { get; set; }

        public decimal CalculatedValue { get; set; }
        public string ProductType { get; set; } // ✅ New Property for PType
    }
}