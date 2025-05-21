using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using MyColor = MYBUSINESS.Models.Color;

namespace MYBUSINESS.Models
{
    public class PackagingProductionViewModel
    {
        public virtual ICollection<SubItem> SubItems { get; set; } = new List<SubItem>();
        public List<QuantityToProduce> QuantityToProduce { get; set; } = new List<QuantityToProduce>();
        public List<PaperColor> PaperColor { get; set; } = new List<PaperColor>();
        public List<CPReceipt> CPReceipt { get; set; } = new List<CPReceipt>();
        public List<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();
        public IQueryable<Product> Products { get; set; }
        public SubItem SubItem { get; set; }
        public virtual ICollection<SubItemProduction> SubItemProduction { get; set; } = new List<SubItemProduction>();
        public SubItemProduction SubItemProductions { get; set; }
        public Product Product { get; set; }

        public NewProduction NewProduction { get; set; }
        public Color Colors { get; set; }
        public List<Color> Color { get; set; } = new List<Color>();
        public List<PPColorReceipt> PPColorReceipt { get; set; } = new List<PPColorReceipt>();
        public PackagingProduction PackagingProduction { get; set; }
        public decimal TotalWeight { get; set; }
        public Dictionary<int, decimal> CalculatedSubitems { get; set; } = new Dictionary<int, decimal>();
        //public decimal calculatedsubitem { get; set; }

        public decimal CalculatedValue { get; set; }
        public string ProductType { get; set; } // ✅ New Property for PType
        public int BOMQuantity { get; set; }      // From PPSubItem
        public int ColorQuantity { get; set; }    // From PaperColor

        public int PlannerQuantity { get; set; }
        public int ProductId { get; set; } // from PaperColor.ProductId
        public decimal SubItemId { get; set; } // from PPSubItem.Id (optional, for next page logic)

        public int? InsertQuantityReceived { get; set; }
        //public int? ToReceived => InsertQuantityReceived.HasValue ? InsertQuantityReceived - PlannerQuantity : null;
        public string DateReceived { get; set; }
        public int PaperColorId { get; set; }
        public string ColorName { get; set; }
        public decimal PPSubItemId { get; set; }
        public int QuantityReceived { get; set; }
        public int ToReceived { get; set; }
        public string ProductName { get; set; } // Add this property
        public DateTime? ReceivedDate { get; set; } // Original date
        public string ReceivedDateFormatted { get; set; } // For display
        public int PackagingProductionId { get; set; }

    }


}