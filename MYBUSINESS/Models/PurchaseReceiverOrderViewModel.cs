using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    //public class PurchaseOrderViewModel
    //{
    //    public IQueryable<Supplier> Suppliers { get; set; }
    //    public Supplier Supplier { get; set; }
    //    public PO PurchaseOrder { get; set; }
    //    public List<POD> PurchaseOrderDetail { get; set; }
    //    public IQueryable<Product> Products { get; set; }
    //    public IEnumerable<IGrouping<string, Product>> ProductListCategory
    //    {
    //        get
    //        {
    //            // Group products by Category property
    //            return Products.GroupBy(p => p.Category);
    //        }
    //    }
    //    public Product Product { get; set; }
    //    //public List<FundingSource> FundingSources { get; set; }

    //}
    public class PurchaseReceiverOrderViewModel
    {
        public IQueryable<Supplier> Suppliers { get; set; }
        public Supplier Supplier { get; set; }
        public PO PurchaseOrder { get; set; }
        public List<POD> PurchaseOrderDetail { get; set; }

        public List<PODReciver> PurchaseReceiverOrderDetail { get; set; }
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