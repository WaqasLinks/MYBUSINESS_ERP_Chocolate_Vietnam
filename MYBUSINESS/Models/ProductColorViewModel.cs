using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ProductColorViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public List<Color> AvailableColors { get; set; } // List of colors from the Colors table
    }
    

}