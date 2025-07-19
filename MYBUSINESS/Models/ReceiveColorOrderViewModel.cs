using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ReceiveColorOrderViewModel
    {
        public int OrderId { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string ReceivedCode { get; set; }
    }
}