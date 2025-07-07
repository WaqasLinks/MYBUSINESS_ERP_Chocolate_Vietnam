using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MYBUSINESS.Models
{
    public class ProductCategoryViewModel
    {
        public int SelectedStoreId { get; set; }

        public List<SelectListItem> StoreList { get; set; }
        public string CategoryName { get; set; }
        //public List<Product> Products { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<ProductCategoryViewModel> ProductCategories { get; set; }
        public List<ProductOrderViewModel> Products { get; set; } // Changed from List<Product>
    }

    public class ProductOrderViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
    }

}