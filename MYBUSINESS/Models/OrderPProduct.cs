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
    
    public partial class OrderPProduct
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderPProduct()
        {
            this.OrderItemPProducts = new HashSet<OrderItemPProduct>();
        }
    
        public int Id { get; set; }
        public Nullable<int> StoreId { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<bool> Validate { get; set; }
    
        public virtual Store Store { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItemPProduct> OrderItemPProducts { get; set; }
    }
}
