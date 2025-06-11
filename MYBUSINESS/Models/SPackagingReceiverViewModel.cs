using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class SPackagingReceiverViewModel
    {
        public IQueryable<Supplier> Suppliers { get; set; } = Enumerable.Empty<Supplier>().AsQueryable();
        public Supplier Supplier { get; set; } = new Supplier();

        public SPackaging SPackaging { get; set; } = new SPackaging();
        public SPackgingReceiver SPackgingReceiver { get; set; } = new SPackgingReceiver();

        public List<SPackagingDetail> SPackagingDetail { get; set; } = new List<SPackagingDetail>();
        public List<SPDReceiver> SPDReceiver { get; set; } = new List<SPDReceiver>();

        public IQueryable<Product> Products { get; set; } = Enumerable.Empty<Product>().AsQueryable();
        public Product Product { get; set; } = new Product();

        // ✅ Constructor to prevent null reference errors
        public SPackagingReceiverViewModel()
        {
            Suppliers = Enumerable.Empty<Supplier>().AsQueryable();
            SPackagingDetail = new List<SPackagingDetail>();
            SPDReceiver = new List<SPDReceiver>();
            Products = Enumerable.Empty<Product>().AsQueryable();
        }
    }
}