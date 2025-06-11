using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ProductionDetailsViewModel
    {
        public class MainProductionInfo
        {
            public int Id { get; set; }
            public DateTime ProductionDate { get; set; }
            public int ProductId { get; set; }
            public string MainProductName { get; set; }
            public string Unit { get; set; }
            public decimal TotalQuantity { get; set; }
        }

        // Quantity to produce details
        public class QuantityToProduce
        {
            public int Id { get; set; }
            public string Shape { get; set; }
            public decimal Quantity { get; set; }
            public decimal Total { get; set; }
            public string Unit { get; set; }
        }

        // Main ingredients
        public class MainIngredient
        {
            public int Id { get; set; }
            public string IngredientName { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public decimal SubItemQty { get; set; }
        }

        public MainProductionInfo ProductionInfo { get; set; }
        public List<QuantityToProduce> QuantitiesToProduce { get; set; }
        public List<MainIngredient> Ingredients { get; set; }
    }
}