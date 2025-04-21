using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class SPackagingViewModel
    {
        public IQueryable<Supplier> Suppliers { get; set; }
        public Supplier Supplier { get; set; }
        public SPackaging SPackaging { get; set; }
        public List<SPackagingDetail> SPackagingDetail { get; set; }
        public IQueryable<Product> Products { get; set; }

        // Group products by Category
        //public IEnumerable<Product> ProductListCategory
        //{
        //    get
        //    {
        //        return Products.AsEnumerable();
        //    }
        //} 
        //public IEnumerable<Product> ProductListCategory
        //{
        //    get; set;

        //}
        public Product Product { get; set; }
    }
}