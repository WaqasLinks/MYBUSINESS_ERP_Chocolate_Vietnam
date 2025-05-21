using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MYBUSINESS.Models
{
    public class InventoryViewModel
    {
        public List<ProductInventoryItem> Products { get; set; }
        public SelectList ProductTypes { get; set; }
    }
    public class ProductInventoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal CurrentStock { get; set; } // From Stock column
        public decimal PhysicalQuantity { get; set; } // User input
        public decimal Determination { get; set; }
        public int ProductType { get; set; }
    }
}