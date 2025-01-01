using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class RentViewModel
    {
        public IQueryable<Customer> Customers { get; set; }
        public Customer Customer { get; set; }
        public Rent SaleOrder { get; set; }
        public List<RentDetail> SaleOrderDetail { get; set; }
        public IQueryable<Product> Products { get; set; }
        public Product Product { get; set; }
        public List<ProductDetail> ProductDetail { get; set; }

    }

   
}