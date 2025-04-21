using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class DashboardStatsViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? StoreId { get; set; } // Add StoreId property
        public decimal TotalSalesWithoutVAT { get; set; }
        public decimal TotalSalesWithVAT { get; set; }

        public List<ProductSaleInfoViewModel> ProductSales { get; set; }
    }
    }