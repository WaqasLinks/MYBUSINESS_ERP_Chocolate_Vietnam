using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class NewProductionViewModel
    {
        //public IQueryable<Customer> Customers { get; set; }
        //public Customer Customer { get; set; }
        //public SO SaleOrder { get; set; }
        public virtual ICollection<SubItem> SubItems { get; set; } = new List<SubItem>();
        public List<QuantityToProduce> QuantityToProduce { get; set; } = new List<QuantityToProduce>();
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }
        public SubItem SubItem { get; set; }
        public virtual ICollection<SubItemProduction> SubItemProduction { get; set; } = new List<SubItemProduction>();
        public SubItemProduction SubItemProductions { get; set; }
        public Product Product { get; set; }
       
        public NewProduction NewProduction { get; set; }
        public decimal TotalWeight { get; set; }
        public Dictionary<int, decimal> CalculatedSubitems { get; set; } = new Dictionary<int, decimal>();
        //public decimal calculatedsubitem { get; set; }

        public decimal CalculatedValue { get; set; }
        public string ProductType { get; set; } // ✅ New Property for PType

    }
}