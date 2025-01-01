using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class FinalProductionViewModel
    {
       
        public virtual ICollection<SubItem> SubItems { get; set; } = new List<SubItem>();
        public IQueryable<Product> Products { get; set; }
        public BOM BOM { get; set; }
        public NewProduction NewProduction { get; set; }

        public Product Product { get; set; }
        public FinalProduction FinalProduction { get; set; }
    }
}