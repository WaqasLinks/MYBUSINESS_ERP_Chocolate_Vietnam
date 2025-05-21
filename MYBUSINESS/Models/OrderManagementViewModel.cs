using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MYBUSINESS.Models; // Make sure this includes your model classes

namespace MYBUSINESS.Models
{
    public class OrderManagementViewModel
    {
        public IQueryable<Customer> Customers { get; set; }
        public Order Orders { get; set; } // Changed from 'Orders' to 'Order' if that's your class name

        public Store Store { get; set; } // Removed duplicate declaration

        public List<OrderItem> OrderItemsDetail { get; set; } // Changed from 'OrderItems' to 'OrderItem'
        public IQueryable<Product> Products { get; set; }

        // Simplified product access
        public IEnumerable<Product> ProductList => Products?.AsEnumerable();

        public Product Product { get; set; }
    }
}