using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class StoreViewModel
    {
        public int Id { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal OpeningBalanceDollars { get; set; }
        public decimal OpeningBalanceYens { get; set; }
        public string OpeningCurrencyDetail { get; set; }
        public string OpeningCurrencyDetailDollars { get; set; }
        public string OpeningCurrencyDetailYens { get; set; }
        public decimal ClosingBalance { get; set; }
        public string ClosingCurrencyDetail { get; set; }
        public int StoreId { get; set; }

    }


}