using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    
    public class PurchaseReciverOrderViewModel
    {
        public IQueryable<Supplier> Suppliers { get; set; }
        public Supplier Supplier { get; set; }
        //public PO PurchaseOrder { get; set; }
        public PO PurchaseOrder { get; set; }
        public POReciver PurchaseOrderReciver  { get; set; }
        public List<POD> PurchaseOrderDetail { get; set; }
        public List<PODReciver> PurchaseReciverOrderDetail { get; set; }
        public IQueryable<Product> Products { get; set; }

        public Product Product { get; set; }
    }
}