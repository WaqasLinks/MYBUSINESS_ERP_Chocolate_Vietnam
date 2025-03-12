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
        public int? stt { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string dvtinh { get; set; }
        public int? sluong { get; set; }
        public decimal? dgia { get; set; }
        public decimal tlckhau { get; set; }
        public decimal stckhau { get; set; }
        public decimal thtien { get; set; }
        public decimal tgtcthue { get; set; }
        public int tsuat { get; set; }
        public decimal tthue { get; set; }
        public decimal tgtthue { get; set; }
        public decimal ttcktmai { get; set; }
        public decimal tgtien { get; set; }
        public decimal tgtttbso { get; set; }
    }
    public class InvoiceDetailsWrapper
    {
        public List<InvoiceDetail> data { get; set; }
    }
    public class Invoice
    {
        public string khieu { get; set; }
        public string tdlap { get; set; }
        public string dvtte { get; set; }
        public decimal tgia { get; set; }
        public string sdhang { get; set; }
        public string tnmua { get; set; }
        public string ten { get; set; }
        public string mst { get; set; }
        public string dchi { get; set; }
        public string Email { get; set; }
        public string htttoan { get; set; }
        public decimal? stckhau { get; set; }
        public decimal thtien { get; set; }
        public decimal tgtcthue { get; set; }
        public decimal tthue { get; set; }
        public decimal tgtthue { get; set; }
        public decimal tgtttbso { get; set; }
        public decimal tgtien { get; set; }
        public string key_API { get; set; }
        public decimal ttcktmai { get; set; }
        public List<InvoiceDetailsWrapper> details { get; set; }
    }

    public class InvoiceRequest
    {
        public int editmode { get; set; }
        public List<Invoice> data { get; set; }
    }
}
