using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class StoreBalanceViewModel
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal ClosingBalance { get; set; }
        public DateTime? ClosingDate { get; set; }
        public int Quantity { get; set; }
        public int VNDQuantity { get; set; }
        public int USDQuantity { get; set; }
        public int JPYQuantity { get; set; }
        public string CurrencyName { get; set; }
    }
}