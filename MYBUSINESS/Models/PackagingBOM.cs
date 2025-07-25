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
    
    public partial class PackagingBOM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PackagingBOM()
        {
            this.PackagingColors = new HashSet<PackagingColor>();
            this.PacSubitems = new HashSet<PacSubitem>();
            this.PacColors = new HashSet<PacColor>();
            this.PacSubItemProductions = new HashSet<PacSubItemProduction>();
        }
    
        public int Id { get; set; }
        public string Remarks { get; set; }
        public bool Saleable { get; set; }
        public bool Purchasable { get; set; }
        public bool Manufacturable { get; set; }
        public Nullable<int> ShelfLife { get; set; }
        public string TimeUnit { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string ProductionProcessCateogry { get; set; }
        public string ProductionProcessDescription { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> ProductId { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PackagingColor> PackagingColors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PacSubitem> PacSubitems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PacColor> PacColors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PacSubItemProduction> PacSubItemProductions { get; set; }
        public virtual Product Product { get; set; }
    }
}
