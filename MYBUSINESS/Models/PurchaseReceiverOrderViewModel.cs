using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
            public class PurchaseReciverOrderViewModel
    {
        public IQueryable<Supplier> Suppliers { get; set; } = Enumerable.Empty<Supplier>().AsQueryable();
        public Supplier Supplier { get; set; } = new Supplier();

        public PO PurchaseOrder { get; set; } = new PO();
        public POReciver PurchaseOrderReciver { get; set; } = new POReciver();

        public List<POD> PurchaseOrderDetail { get; set; } = new List<POD>();
        public List<PODReciver> PurchaseReciverOrderDetail { get; set; } = new List<PODReciver>();

        public IQueryable<Product> Products { get; set; } = Enumerable.Empty<Product>().AsQueryable();
        public Product Product { get; set; } = new Product();

        // ✅ Constructor to prevent null reference errors
        public PurchaseReciverOrderViewModel()
        {
            Suppliers = Enumerable.Empty<Supplier>().AsQueryable();
            PurchaseOrderDetail = new List<POD>();
            PurchaseReciverOrderDetail = new List<PODReciver>();
            Products = Enumerable.Empty<Product>().AsQueryable();
        }
    }
}
