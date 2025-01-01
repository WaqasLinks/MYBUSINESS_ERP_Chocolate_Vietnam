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
        public IQueryable<Product> Products { get; set; }
        public SubItem SubItem { get; set; }
        public Product Product { get; set; }
        public NewProduction NewProduction {  get; set; } 
    }
}