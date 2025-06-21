using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PPPostProductionViewModel
    {
        public virtual ICollection<SubItem> SubItems { get; set; } = new List<SubItem>();
        public List<PPQuantityToProduce> PPQuantityToProduce { get; set; } = new List<PPQuantityToProduce>();
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }
        public SubItem SubItem { get; set; }
        public virtual ICollection<PPSubItemProduction> PPSubItemProduction { get; set; } = new List<PPSubItemProduction>();
        public PPSubItemProduction PPSubItemProductions { get; set; }
        public Product Product { get; set; }

        public PPNewProduction PPNewProduction { get; set; }
        public PPPostProduction PPPostProduction { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal CalculatedValue { get; set; }
    }
}