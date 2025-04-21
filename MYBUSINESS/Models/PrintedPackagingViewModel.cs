using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PrintedPackagingViewModel
    {
        public IQueryable<Supplier> Suppliers { get; set; }
        public Supplier Supplier { get; set; }
        public PrintedPackaging PrintedPackaging { get; set; }
        public List<PrintedPackagingDetail> PrintedPackagingDetail { get; set; }
        public IQueryable<Product> Products { get; set; }

      
        public Product Product { get; set; }
    }
}