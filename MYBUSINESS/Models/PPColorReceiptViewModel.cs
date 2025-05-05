using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class PPColorReceiptViewModel
    {
        public int PaperColorId { get; set; }
        public decimal PPSubItemId { get; set; }
        public string ColorName { get; set; }
        public int QuantityReceived { get; set; }
        public string ReceivedDate { get; set; }
        public int PlannerQuantity { get; set; }
        public int ToReceive { get; set; }
        public int PackagingProductionId { get; set; }
    }
}