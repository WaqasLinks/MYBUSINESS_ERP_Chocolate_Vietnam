using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class BankDepositReportViewModel
    {
        public int Id { get; set; }
        public DateTime ClosingDate { get; set; }
        public decimal ClosingBalance { get; set; }
        public string ClosingCurrencyDetail { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StorePhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CurrencyName { get; set; }
        public int Qty { get; set; }

    }
}