//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MYBUSINESS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductDetail()
        {
            this.QuantityToProduces = new HashSet<QuantityToProduce>();
        }
    
        public decimal Id { get; set; }
        public Nullable<decimal> ProductId { get; set; }
        public string Shape { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public string RentId { get; set; }
        public Nullable<System.DateTime> RentStartDate { get; set; }
        public Nullable<System.DateTime> RentEndDate { get; set; }
        public Nullable<int> ProductionId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuantityToProduce> QuantityToProduces { get; set; }
        public virtual NewProduction NewProduction { get; set; }
        public virtual Product Product { get; set; }
    }
}
