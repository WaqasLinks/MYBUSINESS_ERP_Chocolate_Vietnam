﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MYBUSINESS.Models
{
    public class BOMViewModel
    {
        //public IQueryable<Customer> Customers { get; set; }
        //public Customer Customer { get; set; }
        //public SO SaleOrder { get; set; }
        public List<SubItem> SubItem { get; set; } = new List<SubItem>(); // Ensure it's initialized
        public List<ProductType> ProductType { get; set; } = new List<ProductType>(); // Ensure it's initialized
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }


        //public SelectList ProductList { get; set; }
        //public List<ProductTypeDetail> ProductTypeDetail { get; set; } = new List<ProductTypeDetail>();

        public Product Product { get; set; }
        //public ProductType ProductType { get; set; }
        public BOM BOM { get; set; }
        public Dictionary<int, SelectList> SubItemDropdowns { get; set; } = new Dictionary<int, SelectList>();
    }
}