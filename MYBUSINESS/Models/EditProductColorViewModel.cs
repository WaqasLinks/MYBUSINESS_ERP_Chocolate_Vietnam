using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class EditProductColorViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public List<ColorWithQuantity> AvailableColors { get; set; }
    }
    public class ColorWithQuantity
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public decimal Quantity { get; set; }
    }
}