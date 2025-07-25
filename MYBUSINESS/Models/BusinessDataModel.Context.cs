﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class BusinessContext : DbContext
    {
        public BusinessContext()
            : base("name=BusinessContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<BOM> BOMs { get; set; }
        public virtual DbSet<BOXProduction> BOXProductions { get; set; }
        public virtual DbSet<BusinessInfo> BusinessInfoes { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<CPReceipt> CPReceipts { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<ExpenseDetail> ExpenseDetails { get; set; }
        public virtual DbSet<FinalProduction> FinalProductions { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<LoanDetail> LoanDetails { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<MetaDetaDescription> MetaDetaDescriptions { get; set; }
        public virtual DbSet<MyBusinessInfo> MyBusinessInfoes { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<PackagingColored> PackagingColoreds { get; set; }
        public virtual DbSet<PackagingSubItem> PackagingSubItems { get; set; }
        public virtual DbSet<PaperColor> PaperColors { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PO> POes { get; set; }
        public virtual DbSet<POD> PODs { get; set; }
        public virtual DbSet<PODReciver> PODRecivers { get; set; }
        public virtual DbSet<PPBOM> PPBOMs { get; set; }
        public virtual DbSet<PPColorReceipt> PPColorReceipts { get; set; }
        public virtual DbSet<PPReceiver> PPReceivers { get; set; }
        public virtual DbSet<PPReciverDetail> PPReciverDetails { get; set; }
        public virtual DbSet<PPSubItem> PPSubItems { get; set; }
        public virtual DbSet<PrintedPackaging> PrintedPackagings { get; set; }
        public virtual DbSet<PrintedPackagingDetail> PrintedPackagingDetails { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductDetail> ProductDetails { get; set; }
        public virtual DbSet<ProductionDetail> ProductionDetails { get; set; }
        public virtual DbSet<ProductionDetail1> ProductionDetails1 { get; set; }
        public virtual DbSet<ProductionOrder> ProductionOrders { get; set; }
        public virtual DbSet<ProductionOrderDetail> ProductionOrderDetails { get; set; }
        public virtual DbSet<ProductStockDetail> ProductStockDetails { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<QuantityToProduce> QuantityToProduces { get; set; }
        public virtual DbSet<Rent> Rents { get; set; }
        public virtual DbSet<RentDetail> RentDetails { get; set; }
        public virtual DbSet<ScanBankDeposit> ScanBankDeposits { get; set; }
        public virtual DbSet<ScanMoneyInput> ScanMoneyInputs { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceDetail> ServiceDetails { get; set; }
        public virtual DbSet<ShopManage> ShopManages { get; set; }
        public virtual DbSet<SOD> SODs { get; set; }
        public virtual DbSet<SPackaging> SPackagings { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<StoreOrderReceipt> StoreOrderReceipts { get; set; }
        public virtual DbSet<StoreProduct> StoreProducts { get; set; }
        public virtual DbSet<SubItem> SubItems { get; set; }
        public virtual DbSet<SubItemProduction> SubItemProductions { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<UserAuthorization> UserAuthorizations { get; set; }
        public virtual DbSet<PTypeDesc> PTypeDescs { get; set; }
        public virtual DbSet<SPackagingDetail> SPackagingDetails { get; set; }
        public virtual DbSet<SPackgingReceiver> SPackgingReceivers { get; set; }
        public virtual DbSet<SPDReceiver> SPDReceivers { get; set; }
        public virtual DbSet<PPNewProduction> PPNewProductions { get; set; }
        public virtual DbSet<PPQuantityToProduce> PPQuantityToProduces { get; set; }
        public virtual DbSet<PPSubItemProduction> PPSubItemProductions { get; set; }
        public virtual DbSet<PPPostProduction> PPPostProductions { get; set; }
        public virtual DbSet<DailyBalanceVnd> DailyBalanceVnds { get; set; }
        public virtual DbSet<NewProduction> NewProductions { get; set; }
        public virtual DbSet<PostProduction> PostProductions { get; set; }
        public virtual DbSet<PackagingProduction> PackagingProductions { get; set; }
        public virtual DbSet<OrderPProduct> OrderPProducts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<ScanCreditCard> ScanCreditCards { get; set; }
        public virtual DbSet<StoreOrderReceiptPP> StoreOrderReceiptPPs { get; set; }
        public virtual DbSet<SO> SOes { get; set; }
        public virtual DbSet<PackagingBOM> PackagingBOMs { get; set; }
        public virtual DbSet<PackagingColor> PackagingColors { get; set; }
        public virtual DbSet<PacSubitem> PacSubitems { get; set; }
        public virtual DbSet<PacQuantityToProduce> PacQuantityToProduces { get; set; }
        public virtual DbSet<PacColor> PacColors { get; set; }
        public virtual DbSet<PacSubItemProduction> PacSubItemProductions { get; set; }
        public virtual DbSet<PacProduction> PacProductions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<POReciver> PORecivers { get; set; }
        public virtual DbSet<OrderItemPProduct> OrderItemPProducts { get; set; }
        public virtual DbSet<OrderColorPProduct> OrderColorPProducts { get; set; }
        public virtual DbSet<OrderItemColorPProduct> OrderItemColorPProducts { get; set; }
        public virtual DbSet<StoreColorOrderReceiptPP> StoreColorOrderReceiptPPs { get; set; }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual ObjectResult<spExpenseReport_Result> spExpenseReport(string purchaseOrderID)
        {
            var purchaseOrderIDParameter = purchaseOrderID != null ?
                new ObjectParameter("PurchaseOrderID", purchaseOrderID) :
                new ObjectParameter("PurchaseOrderID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spExpenseReport_Result>("spExpenseReport", purchaseOrderIDParameter);
        }
    
        public virtual ObjectResult<spLoanReport_Result> spLoanReport(string purchaseOrderID)
        {
            var purchaseOrderIDParameter = purchaseOrderID != null ?
                new ObjectParameter("PurchaseOrderID", purchaseOrderID) :
                new ObjectParameter("PurchaseOrderID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spLoanReport_Result>("spLoanReport", purchaseOrderIDParameter);
        }
    
        public virtual ObjectResult<spRentReport_Result> spRentReport(string saleOrderID)
        {
            var saleOrderIDParameter = saleOrderID != null ?
                new ObjectParameter("SaleOrderID", saleOrderID) :
                new ObjectParameter("SaleOrderID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spRentReport_Result>("spRentReport", saleOrderIDParameter);
        }
    
        public virtual ObjectResult<spServiceReport_Result> spServiceReport(string saleOrderID)
        {
            var saleOrderIDParameter = saleOrderID != null ?
                new ObjectParameter("SaleOrderID", saleOrderID) :
                new ObjectParameter("SaleOrderID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spServiceReport_Result>("spServiceReport", saleOrderIDParameter);
        }
    
        public virtual ObjectResult<spTest_Result> spTest(string param1)
        {
            var param1Parameter = param1 != null ?
                new ObjectParameter("Param1", param1) :
                new ObjectParameter("Param1", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spTest_Result>("spTest", param1Parameter);
        }
    
        public virtual ObjectResult<spBankDepositReport_Result> spBankDepositReport(Nullable<int> bankDepositId)
        {
            var bankDepositIdParameter = bankDepositId.HasValue ?
                new ObjectParameter("BankDepositId", bankDepositId) :
                new ObjectParameter("BankDepositId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spBankDepositReport_Result>("spBankDepositReport", bankDepositIdParameter);
        }
    
        public virtual ObjectResult<spGetOrderProductDetails_Result> spGetOrderProductDetails(Nullable<int> orderId)
        {
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spGetOrderProductDetails_Result>("spGetOrderProductDetails", orderIdParameter);
        }
    
        public virtual ObjectResult<spSOCustomerReport_Result> spSOCustomerReport(string saleOrderID)
        {
            var saleOrderIDParameter = saleOrderID != null ?
                new ObjectParameter("SaleOrderID", saleOrderID) :
                new ObjectParameter("SaleOrderID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spSOCustomerReport_Result>("spSOCustomerReport", saleOrderIDParameter);
        }
    
        public virtual ObjectResult<spPOReport_Result> spPOReport(string purchaseOrderID)
        {
            var purchaseOrderIDParameter = purchaseOrderID != null ?
                new ObjectParameter("PurchaseOrderID", purchaseOrderID) :
                new ObjectParameter("PurchaseOrderID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spPOReport_Result>("spPOReport", purchaseOrderIDParameter);
        }
    
        public virtual ObjectResult<spGetOrderPProductDetails_Result> spGetOrderPProductDetails(Nullable<int> orderId)
        {
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spGetOrderPProductDetails_Result>("spGetOrderPProductDetails", orderIdParameter);
        }
    
        public virtual ObjectResult<GetPackagingProductionColorDetails_Result> GetPackagingProductionColorDetails(Nullable<int> packagingProductionId)
        {
            var packagingProductionIdParameter = packagingProductionId.HasValue ?
                new ObjectParameter("PackagingProductionId", packagingProductionId) :
                new ObjectParameter("PackagingProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPackagingProductionColorDetails_Result>("GetPackagingProductionColorDetails", packagingProductionIdParameter);
        }
    
        public virtual ObjectResult<GetPackagingProductionColorDetailsCombined_Result> GetPackagingProductionColorDetailsCombined(Nullable<int> packagingProductionId)
        {
            var packagingProductionIdParameter = packagingProductionId.HasValue ?
                new ObjectParameter("PackagingProductionId", packagingProductionId) :
                new ObjectParameter("PackagingProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPackagingProductionColorDetailsCombined_Result>("GetPackagingProductionColorDetailsCombined", packagingProductionIdParameter);
        }
    
        public virtual ObjectResult<GetColorQuantitiesByProductionId_Result> GetColorQuantitiesByProductionId(Nullable<int> packagingProductionId)
        {
            var packagingProductionIdParameter = packagingProductionId.HasValue ?
                new ObjectParameter("PackagingProductionId", packagingProductionId) :
                new ObjectParameter("PackagingProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetColorQuantitiesByProductionId_Result>("GetColorQuantitiesByProductionId", packagingProductionIdParameter);
        }
    
        public virtual ObjectResult<spSOReport_Result> spSOReport(string saleOrderID)
        {
            var saleOrderIDParameter = saleOrderID != null ?
                new ObjectParameter("SaleOrderID", saleOrderID) :
                new ObjectParameter("SaleOrderID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spSOReport_Result>("spSOReport", saleOrderIDParameter);
        }
    
        public virtual int GetColoredPackagingById(Nullable<int> orderId)
        {
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetColoredPackagingById", orderIdParameter);
        }
    
        public virtual int GetChocolateProductionById(Nullable<int> productionId)
        {
            var productionIdParameter = productionId.HasValue ?
                new ObjectParameter("ProductionId", productionId) :
                new ObjectParameter("ProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetChocolateProductionById", productionIdParameter);
        }
    
        public virtual ObjectResult<GetChocolateProductionHeader_Result> GetChocolateProductionHeader(Nullable<int> productionId)
        {
            var productionIdParameter = productionId.HasValue ?
                new ObjectParameter("ProductionId", productionId) :
                new ObjectParameter("ProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetChocolateProductionHeader_Result>("GetChocolateProductionHeader", productionIdParameter);
        }
    
        public virtual ObjectResult<GetChocolateProductionIngredients_Result> GetChocolateProductionIngredients(Nullable<int> productionId)
        {
            var productionIdParameter = productionId.HasValue ?
                new ObjectParameter("ProductionId", productionId) :
                new ObjectParameter("ProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetChocolateProductionIngredients_Result>("GetChocolateProductionIngredients", productionIdParameter);
        }
    
        public virtual ObjectResult<GetChocolateProductionShapes_Result> GetChocolateProductionShapes(Nullable<int> productionId)
        {
            var productionIdParameter = productionId.HasValue ?
                new ObjectParameter("ProductionId", productionId) :
                new ObjectParameter("ProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetChocolateProductionShapes_Result>("GetChocolateProductionShapes", productionIdParameter);
        }
    
        public virtual ObjectResult<GetPostChocolateProductionHeader_Result> GetPostChocolateProductionHeader(Nullable<int> postProductionId)
        {
            var postProductionIdParameter = postProductionId.HasValue ?
                new ObjectParameter("PostProductionId", postProductionId) :
                new ObjectParameter("PostProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPostChocolateProductionHeader_Result>("GetPostChocolateProductionHeader", postProductionIdParameter);
        }
    
        public virtual ObjectResult<GetPostChocolateProductionIngredients_Result> GetPostChocolateProductionIngredients(Nullable<int> postProductionId)
        {
            var postProductionIdParameter = postProductionId.HasValue ?
                new ObjectParameter("PostProductionId", postProductionId) :
                new ObjectParameter("PostProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPostChocolateProductionIngredients_Result>("GetPostChocolateProductionIngredients", postProductionIdParameter);
        }
    
        public virtual ObjectResult<GetPostChocolateProductionShapes_Result> GetPostChocolateProductionShapes(Nullable<int> postProductionId)
        {
            var postProductionIdParameter = postProductionId.HasValue ?
                new ObjectParameter("PostProductionId", postProductionId) :
                new ObjectParameter("PostProductionId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetPostChocolateProductionShapes_Result>("GetPostChocolateProductionShapes", postProductionIdParameter);
        }
    }
}
