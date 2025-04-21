using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PPBOMViewModel
    {
        public List<PPSubItem> PPSubItem { get; set; } = new List<PPSubItem>(); // Ensure it's initialized
        public List<ProductType> ProductType { get; set; } = new List<ProductType>(); // Ensure it's initialized
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }


        //public SelectList ProductList { get; set; }
        //public List<ProductTypeDetail> ProductTypeDetail { get; set; } = new List<ProductTypeDetail>();

        public Product Product { get; set; }
        //public ProductType ProductType { get; set; }
        public PPBOM PPBOM { get; set; }
        //public Dictionary<int, SelectList> SubItemDropdowns { get; set; } = new Dictionary<int, SelectList>();
    }
}