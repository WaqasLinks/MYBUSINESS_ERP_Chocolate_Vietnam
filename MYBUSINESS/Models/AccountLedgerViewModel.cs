using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class AccountLedgerViewModel
    {
        public string Id { get; set; }
        public Nullable<decimal> Serial { get; set; }
        public Nullable<decimal> OrderAmountDiscounted { get; set; }
        public decimal BillPaid { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<decimal> PrevBalance { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<bool> IsSale { get; set; }
        public Nullable<bool> IsReturn { get; set; }
        public Nullable<bool> CashIn { get; set; }
        public string PersonName { get; set; }


    }
}