using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MYBUSINESS.Models
{
    public class InvoiceDetail
    {
        public int? tchat { get; set; }
        public int? stt_rec0 { get; set; }
        public string inv_itemCode { get; set; }
        public string inv_itemName { get; set; }
        public string inv_unitCode { get; set; }
        public int? inv_quantity { get; set; }
        public decimal? inv_unitPrice { get; set; }
        public decimal inv_discountPercentage { get; set; }
        public decimal inv_discountAmount { get; set; }
        public decimal inv_TotalAmountWithoutVat { get; set; }
        public int ma_thue { get; set; }
        public decimal inv_vatAmount { get; set; }
        public decimal inv_TotalAmount { get; set; }
    }
    public class InvoiceDetailsWrapper
    {
        public List<InvoiceDetail> data { get; set; }
    }
    public class Invoice
    {
        public string inv_invoiceSeries { get; set; }
        public string inv_invoiceIssuedDate { get; set; }
        public string inv_currencyCode { get; set; }
        public decimal inv_exchangeRate { get; set; }
        public string so_benh_an { get; set; }
        public string inv_buyerDisplayName { get; set; }
        public string inv_buyerLegalName { get; set; }
        public string inv_buyerTaxCode { get; set; }
        public string inv_buyerAddressLine { get; set; }
        public string inv_buyerEmail { get; set; }
        public string inv_paymentMethodName { get; set; }
        public decimal? inv_discountAmount { get; set; }
        public decimal inv_TotalAmountWithoutVat { get; set; }
        public decimal inv_vatAmount { get; set; }
        public decimal inv_TotalAmount { get; set; }
        public string key_api { get; set; }
        public List<InvoiceDetailsWrapper> details { get; set; }
    }

    public class InvoiceRequest
    {
        public int editmode { get; set; }
        public List<Invoice> data { get; set; }
    }
}
