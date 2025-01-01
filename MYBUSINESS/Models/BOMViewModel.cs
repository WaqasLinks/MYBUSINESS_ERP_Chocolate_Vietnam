using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class BOMViewModel
    {
        //public IQueryable<Customer> Customers { get; set; }
        //public Customer Customer { get; set; }
        //public SO SaleOrder { get; set; }
        public List<SubItem> SubItem { get; set; } = new List<SubItem>(); // Ensure it's initialized
        public IQueryable<Product> Products { get; set; }


        public Product Product { get; set; }
        public BOM BOM { get; set; }
    }
}