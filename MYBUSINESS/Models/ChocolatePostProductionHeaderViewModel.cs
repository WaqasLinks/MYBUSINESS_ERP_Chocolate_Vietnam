using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ChocolatePostProductionHeaderViewModel
    {
        public int ProductionId { get; set; }
        public string FormattedDate { get; set; }  // Changed to string to match SP
        public string MainProductName { get; set; }
        public decimal TotalWeight { get; set; }
    }
}