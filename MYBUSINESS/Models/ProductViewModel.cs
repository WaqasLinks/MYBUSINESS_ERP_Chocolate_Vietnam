using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }
        //public List<SubItem> SubItem { get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; } // Assuming you have a Supplier model



    }
}