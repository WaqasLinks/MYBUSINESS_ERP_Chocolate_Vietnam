using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PrintedPackagingReciverViewModel
    {
        public IQueryable<Supplier> Suppliers { get; set; } = Enumerable.Empty<Supplier>().AsQueryable();
        public Supplier Supplier { get; set; } = new Supplier();

        public PrintedPackaging PrintedPackaging { get; set; } = new PrintedPackaging();
        public PPReceiver PPReceiver { get; set; } = new PPReceiver();

        public List<PrintedPackagingDetail> PrintedPackagingDetail { get; set; } = new List<PrintedPackagingDetail>();
        public List<PPReciverDetail> PPReciverDetail { get; set; } = new List<PPReciverDetail>();

        public IQueryable<Product> Products { get; set; } = Enumerable.Empty<Product>().AsQueryable();
        public Product Product { get; set; } = new Product();

        // ✅ Constructor to prevent null reference errors
        public PrintedPackagingReciverViewModel()
        {
            Suppliers = Enumerable.Empty<Supplier>().AsQueryable();
            PrintedPackagingDetail = new List<PrintedPackagingDetail>();
            PPReciverDetail = new List<PPReciverDetail>();
            Products = Enumerable.Empty<Product>().AsQueryable();
        }
    }
}