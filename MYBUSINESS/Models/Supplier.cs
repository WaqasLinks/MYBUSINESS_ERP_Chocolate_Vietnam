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
    
    public partial class Supplier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Supplier()
        {
            this.Expenses = new HashSet<Expense>();
            this.Loans = new HashSet<Loan>();
            this.POes = new HashSet<PO>();
            this.PPReceivers = new HashSet<PPReceiver>();
            this.PrintedPackagings = new HashSet<PrintedPackaging>();
            this.SPackagings = new HashSet<SPackaging>();
            this.SPackgingReceivers = new HashSet<SPackgingReceiver>();
            this.PORecivers = new HashSet<POReciver>();
        }
    
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string bizId { get; set; }
        public Nullable<bool> IsCreditor { get; set; }
        public Nullable<int> Type { get; set; }
        public string Status { get; set; }
        public Nullable<int> StoreId { get; set; }
    
        public virtual BusinessInfo BusinessInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Expense> Expenses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Loan> Loans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PO> POes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PPReceiver> PPReceivers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrintedPackaging> PrintedPackagings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SPackaging> SPackagings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SPackgingReceiver> SPackgingReceivers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POReciver> PORecivers { get; set; }
    }
}
