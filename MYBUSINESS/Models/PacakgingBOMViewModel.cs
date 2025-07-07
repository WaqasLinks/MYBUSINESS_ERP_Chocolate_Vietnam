using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MYBUSINESS.Models
{
    public class PacakgingBOMViewModel
    {
        public List<PacSubitem> PacSubitem { get; set; } = new List<PacSubitem>(); // Ensure it's initialized
        public List<PackagingColor> PackagingColor { get; set; } = new List<PackagingColor>(); // Ensure it's initialized
        public List<ProductType> ProductType { get; set; } = new List<ProductType>(); // Ensure it's initialized
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }


        //public SelectList ProductList { get; set; }
        //public List<ProductTypeDetail> ProductTypeDetail { get; set; } = new List<ProductTypeDetail>();

        public Product Product { get; set; }
        //public ProductType ProductType { get; set; }
        public PackagingBOM PackagingBOM { get; set; }
        public Dictionary<int, SelectList> PacSubitemDropdowns { get; set; } = new Dictionary<int, SelectList>();
    }
}