using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class DailyCashSummaryViewModel
    {
        public string StoreName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal CashSales { get; set; }
        public decimal MoneyInput { get; set; }
        public decimal BankDeposit { get; set; }
        public decimal ActualClosingBalance { get; set; }

        public decimal CreditCardSales { get; set; }
        public decimal UploadedCreditCardAmount { get; set; }

        public decimal CreditCardDifference => UploadedCreditCardAmount - CreditCardSales;

        public bool IsCreditCardMatched => CreditCardDifference == 0;
    }
}