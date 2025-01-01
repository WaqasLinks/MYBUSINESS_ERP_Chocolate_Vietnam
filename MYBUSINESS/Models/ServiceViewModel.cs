using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ServiceViewModel
    {
        public IQueryable<Customer> Customers { get; set; }
        public Customer Customer { get; set; }
        public Service SaleOrder { get; set; }
        public List<ServiceDetail> SaleOrderDetail { get; set; }
        public IQueryable<Product> Products { get; set; }
        public Product Product { get; set; }

    }

   
}