using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class SaleOrderViewModel
    {
        public IQueryable<Customer> Customers { get; set; }
        public SO SaleOrder { get; set; }

        public Store Store { get; set; }

        public List<SOD> SaleOrderDetail { get; set; }
        public Customer Customer { get; set; }
        public IQueryable<Product> Products { get; set; }

        //public IQueryable<Product> Products { get; set; }
        //public IEnumerable<Product> Products { get; set; }
        // New property to hold IEnumerable<Product>
        public IEnumerable<Product> ProductList
        {
            get { return Products.AsEnumerable(); }
        }
        public Product Product { get; set; }

    }

   
}