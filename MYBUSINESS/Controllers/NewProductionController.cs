using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace MYBUSINESS.Controllers
{
    public class NewProductionController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private NewProductionViewModel newproductionViewModel = new NewProductionViewModel();
        // GET: Products
        public ActionResult Index()
        {
            var newProductions = db.NewProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(newProductions);
        }
        public ActionResult IndexComponents()
        {
            var newProductions = db.NewProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(newProductions);

        }

        public ActionResult SearchData(string suppId)
        {
            if (suppId.Trim() == string.Empty)
            {

                return PartialView("_SelectedProducts", DAL.dbProducts.OrderBy(i => i.Id).ToList());

            }
            else
            {
                int intSuppId = Int32.Parse(suppId.Trim());

                IQueryable<Product> selectedProducts = null;
                //selectedProducts = db.Products.Where(p => p.SupplierId == intSuppId);
                return PartialView("_SelectedProducts", selectedProducts.OrderBy(i => i.Id).ToList());

            }

        }

        //public ActionResult Create()
        //{
        //    //int maxId = db.Products.Max(p => p.Id);
        //    decimal maxId = db.NewProductions.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
        //    maxId += 1;
        //    ViewBag.SuggestedId = maxId;
        //    ViewBag.Suppliers = DAL.dbSuppliers;
        //    BOM bom = new BOM();

        //    newproductionViewModel.NewProduction = new NewProduction();  // Create an instance of NewProduction

        //    //newproductionViewModel.NewProduction = NewProduction;
        //    newproductionViewModel.Products = DAL.dbProducts;
        //    return View(newproductionViewModel);
        //}
        ////[HttpPost]
        ////[ValidateAntiForgeryToken]
        ////public ActionResult Create([Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,QuantityToProduce")] NewProduction newProduction, [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        ////{

        ////    if (ModelState.IsValid)
        ////    {
        ////        db.NewProductions.Add(NewProduction);
        ////        foreach (var item in subItems) item.ParentProductId = newProduction.Id;
        ////        db.SubItems.AddRange(subItems);


        ////        db.SaveChanges();
        ////        return RedirectToAction("Index");
        ////    }
        ////    ViewBag.Suppliers = DAL.dbSuppliers;
        ////    return View(newProduction);
        ////}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,QuantityToProduce")] NewProduction newProduction, [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Create an instance of NewProduction
        //        var newProduction1 = new NewProduction
        //        {
        //            ProductionDate = newProduction.ProductionDate,
        //            ProductName = newProduction.ProductName,
        //            Unit = newProduction.Unit,
        //            QuantityToProduce = newProduction.QuantityToProduce
        //        };

        //        db.NewProductions.Add(newProduction);

        //        foreach (var item in subItems)
        //            item.ParentProductId = newProduction.Id;

        //        db.SubItems.AddRange(subItems);

        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Suppliers = DAL.dbSuppliers;
        //    return View(newProduction);
        //}
        //public ActionResult Create()
        //{
        //    decimal maxId = (db.NewProductions?.Max(p => p == null ? 0 : p.Id) ?? 0) + 1;

        //    ViewBag.SuggestedId = maxId;
        //    ViewBag.Suppliers = DAL.dbSuppliers;

        //    var productList = db.Products
        //                        .Select(p => new { p.Id, p.Name, p.Stock })
        //                        .ToList();

        //    ViewBag.ProductList = new SelectList(productList, "Id", "Name");

        //    var viewModel = new NewProductionViewModel
        //    {
        //        NewProduction = new NewProduction(),
        //        Products = DAL.dbProducts ?? Enumerable.Empty<Product>().AsQueryable(),
        //        SubItems = db.Products.Select(p => new SubItem
        //        {
        //            ProductId = p.Id,
        //            AvailableInventory = p.Stock, // Assign stock to AvailableInventory
        //            QuantitytoPrepare = 0 // Initial value, will be updated later
        //        }).ToList()
        //    };

        //    return View(viewModel);
        //}

            [HttpGet]
            public ActionResult Create()
            {
                decimal maxId = db.NewProductions.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                maxId += 1;
                ViewBag.SuggestedId = maxId;  // Passing the suggested BOM ID to the view
                ViewBag.Suppliers = DAL.dbSuppliers;  // Assuming this contains the supplier list

            var products = db.Products
       .Where(p => p.PType == 4)
       .Select(p => new
       {
           Value = p.Id.ToString(),  // ID of the product
           Text = p.Name            // Name of the product
       })
       .ToList();

            ViewBag.ProductList = new SelectList(products, "Value", "Text"); // Pass as a SelectList
            var productDetails = db.ProductDetails.ToList();

            var remarks = db.BOMs
    .Select(b => new
    {
        Value = b.Remarks,           // Key for dropdown
        Text = b.Remarks,            // Display text
        ProductName = b.ProductName, // Related product name
        Unit = b.Product.Unit    // Related time unit or unit field
    })
    .ToList();

            ViewBag.Remarks = new SelectList(remarks, "Value", "Text");
            ViewBag.RemarksData = remarks;


            var productList = db.Products
                                    .Select(p => new
                                    {
                                        p.Id,
                                        p.Name,
                                        p.Stock,
                                        p.Remarks, // Fetch remarks from BOM
                                        p.Category,   // Fetch stock for AvailableInventory
                                    })
                                    .ToList();

                ViewBag.ProductList = new SelectList(productList, "Id", "Name");

                // Fetch data and then map to SubItem outside the LINQ query
                var subItems = db.Products
                                 .Select(p => new
                                 {
                                     ProductId = p.Id,
                                     AvailableInventory = p.Stock
                                 })
                                 .AsEnumerable() // Move query execution to memory
                                 .Select(p => new SubItem
                                 {
                                     ProductId = p.ProductId,
                                     AvailableInventory = p.AvailableInventory,
                                     QuantitytoPrepare = 0 // Initial value
                                 })
                                 .ToList();
           
            var viewModel = new NewProductionViewModel
                {
                    NewProduction = new NewProduction(),
                    Products = DAL.dbProducts,
                    SubItems = subItems,
                    QuantityToProduce = new List<QuantityToProduce> { new QuantityToProduce() },
                    ProductDetail = new List<ProductDetail> { new ProductDetail() },
                };

                return View(viewModel);
            }




        //   [HttpPost]
        //   [ValidateAntiForgeryToken]
        //   public ActionResult Create(NewProductionViewModel model,
        //       [Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,QuantityToProduce")] NewProduction newProduction,
        //[Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        //   {
        //       if (ModelState.IsValid)
        //       {
        //           var selectedProduct = db.Products.FirstOrDefault(p => p.Id == model.NewProduction.Id);

        //           if (selectedProduct != null)
        //           {
        //               model.NewProduction.ProductName = selectedProduct.Name;
        //           }

        //           model.NewProduction.SubItems = new List<SubItem>();

        //           db.NewProductions.Add(model.NewProduction);

        //           foreach (var item in subItems) item.ParentProductId = newProduction.Id;
        //           db.SubItems.AddRange(subItems);

        //           db.SaveChanges();
        //           return RedirectToAction("Index");
        //       }

        //       ViewBag.Suppliers = DAL.dbSuppliers;

        //       var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
        //       ViewBag.ProductList = new SelectList(productList, "Id", "Name");

        //       return View(model);
        //   }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
     [Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,ProductId")] NewProduction newProduction,
     //[Bind(Prefix = "SubItem", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItem> subItems,
     [Bind(Prefix = "SubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested,SubItemQty")] List<SubItemProduction> subItemProductions,
     [Bind(Prefix = "QuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId,Weight")] List<QuantityToProduce> quantityToProduces)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.Find(newProduction.ProductId);
                if (product != null)
                {
                    newProduction.ProductName = product.Name;
                }

                // Save the BOM and its SubItems
                db.NewProductions.Add(newProduction);

                if (subItemProductions != null)
                {
                    foreach (var item in subItemProductions)
                    {
                        item.ParentProductId = newProduction.ProductId;
                        item.NewProductionId = newProduction.Id;
                        db.SubItemProductions.Add(item);
                    }
                }
                    //foreach (var item in subItems)
                    //{
                    //    item.ParentProductId = newProduction.ProductId;
                    //    db.SubItems.Add(item);
                    //}

                    if (quantityToProduces != null)
                {
                    foreach (var item in quantityToProduces)
                    {
                        item.ProductId = newProduction.ProductId;
                        item.NewProductionId = newProduction.Id;
                        //item.Id = newProduction.Id;
                        db.QuantityToProduces.Add(item);
                        //item.ProductId = newProduction.Id;
                        //db.QuantityToProduces.Add(item);

                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");

            }

        

        ViewBag.Suppliers = DAL.dbSuppliers;

            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name");

            return View(newproductionViewModel);
        }



        // public ActionResult Edit(decimal? id)
        // {
        //     if (id == null)
        //     {
        //         return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //     }

        //     var newProduction = db.NewProductions
        //                           .Include(np => np.Products) // Include related Products
        //                           .FirstOrDefault(np => np.Id == id);

        //     if (newProduction == null)
        //     {
        //         return HttpNotFound();
        //     }
        //     var products = db.Products
        //.Where(p => p.Manufacturable == true)
        //.Select(p => new
        //{
        //    Value = p.Id.ToString(),
        //    Text = p.Name
        //})
        //.ToList();

        //     ViewBag.ProductList = new SelectList(products, "Value", "Text", newProduction.ProductId); // Pre-select the current product

        //     // Fetch SubItems associated with this NewProduction
        //     var subItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList();

        //     // Prepare ViewModel
        //     var viewModel = new NewProductionViewModel
        //     {
        //         NewProduction = newProduction,
        //         Products = db.Products, // Include all products
        //         SubItems = subItems // Include fetched SubItems
        //     };

        //     // Populate ViewBag with Product List
        //     var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
        //     ViewBag.ProductList = new SelectList(productList, "Id", "Name", newProduction.ProductName);

        //     return View(viewModel);
        // }
        public ActionResult Edit(decimal? id, bool readonlyMode = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch the NewProduction entity with related Products and other details
            var newProduction = db.NewProductions
                                   .Include(np => np.Product) // Include related Products
                                   .FirstOrDefault(np => np.Id == id);


            if (newProduction == null)
            {
                return HttpNotFound();
            }
            string productType;
            switch (newProduction.Product.PType)
            {
                case 1:
                    productType = "Variable";
                    break;
                case 2:
                    productType = "Excess";
                    break;
                case 3:
                    productType = "ByProduct";
                    break;
                default:
                    productType = "Unknown";
                    break;
            }

            var quantityToProduce = db.QuantityToProduces
.Where(x => x.NewProductionId == newProduction.Id)
.ToList();




            var totalWeight = quantityToProduce.Sum(x => x.CalculatedWeight) ?? 0;




            //var subItems = db.SubItems.Where(s => s.ParentProductId == newProduction.ProductId).ToList();
            var subItemProductions = db.SubItemProductions.Where(s => s.NewProductionId == newProduction.Id).ToList();
            Console.WriteLine(newProduction.ProductionDate); // Debugging line
                                                             // Get the list of products
            var products = db.Products
        .Where(p => p.PType == 4)
        .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
        .ToList();

            // Pre-select the current product in the dropdown
            ViewBag.ProductList = new SelectList(products, "Value", "Text", newProduction.ProductId);

            ViewBag.ReadonlyMode = readonlyMode;
            // Prepare ViewModel (including SubItems if needed)
            var viewModel = new NewProductionViewModel
            {
                //SubItems = subItems,
                SubItemProduction = db.SubItemProductions.Where(x => x.NewProductionId == newProduction.Id).ToList(),
                NewProduction = newProduction,
                Products = db.Products, // Include all products
                /* SubItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList(), */// Get SubItems associated with this NewProduction
                TotalWeight = totalWeight,                                                                                    //QuantityToProduce = db.QuantityToProduces.Where(x => x.ProductId == newProduction.Id).ToList()
                QuantityToProduce = quantityToProduce
            };

            return View(viewModel);
        }





        //   [HttpPost]
        //   [ValidateAntiForgeryToken]
        //   public ActionResult Edit(NewProductionViewModel model,
        //[Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,QuantityToProduce,ProductId")] NewProduction newProduction,
        ////[Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItem> subItems,
        //[Bind(Prefix = "QuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,ProductId")] List<QuantityToProduce> quantityToProduces)

        //       {
        //       if (ModelState.IsValid)
        //           {
        //               // Fetch the existing NewProduction entity
        //               var existingProduction = db.NewProductions.Find(model.NewProduction.Id);
        //               var existingQuantityToProduces = db.QuantityToProduces.Where(x => x.ProductId == newProduction.Id).ToList();
        //               db.QuantityToProduces.RemoveRange(existingQuantityToProduces);
        //               if (existingProduction == null)
        //           {
        //               return HttpNotFound();
        //           }

        //           // Update NewProduction fields
        //           existingProduction.ProductionDate = model.NewProduction.ProductionDate;
        //           existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.NewProduction.Id)?.Name;
        //           existingProduction.Unit = model.NewProduction.Unit;
        //           existingProduction.QuantityToProduces = model.NewProduction.QuantityToProduces;

        //           db.Entry(existingProduction).State = EntityState.Modified;

        //           // Delete existing SubItems
        //           //var delSubItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList();
        //           //db.SubItems.RemoveRange(delSubItems);

        //           // Add new SubItems
        //           //foreach (var item in subItems)
        //           //{
        //           //    item.ParentProductId = existingProduction.Id;
        //           //}
        //           //db.SubItems.AddRange(subItems);
        //           if (quantityToProduces != null)
        //           {
        //               foreach (var item in quantityToProduces)
        //               {
        //                   item.ProductId = newProduction.Id;
        //                   db.QuantityToProduces.Add(item);
        //               }
        //           }
        //      //     Save changes to the database
        //               db.SaveChanges();
        //           return RedirectToAction("Index");
        //       }

        //       // Repopulate ViewBag on failure
        //       var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
        //       ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.NewProduction.ProductName);

        //       return View(model);
        //   }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Edit(NewProductionViewModel model,
        //[Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,ProductId")] NewProduction newProduction,
        //[Bind(Prefix = "QuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId")] List<QuantityToProduce> quantityToProduces)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // Fetch the existing NewProduction entity
        //            var existingProduction = db.NewProductions.Find(model.NewProduction.Id);

        //            // Ensure the NewProduction entity exists
        //            if (existingProduction == null)
        //            {
        //                return HttpNotFound();
        //            }

        //            // Update NewProduction fields
        //            existingProduction.ProductionDate = model.NewProduction.ProductionDate;
        //            existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.NewProduction.ProductId)?.Name;
        //            existingProduction.Unit = model.NewProduction.Unit;
        //            existingProduction.ProductId = model.NewProduction.ProductId;

        //            // Update QuantityToProduce for the NewProduction
        //            // First, remove the existing QuantityToProduce entries related to this production
        //            var existingQuantityToProduces = db.QuantityToProduces.Where(x => x.ProductId == existingProduction.ProductId).ToList();
        //            db.QuantityToProduces.RemoveRange(existingQuantityToProduces);

        //            // Add new QuantityToProduce entries
        //            if (quantityToProduces != null && quantityToProduces.Count > 0)
        //            {
        //                foreach (var item in quantityToProduces)
        //                {
        //                    item.ProductId = existingProduction.ProductId; // Ensure ProductId is set to the correct value
        //                    item.NewProductionId = existingProduction.Id; // Ensure QuantityToProduce is linked to the correct NewProduction
        //                    db.QuantityToProduces.Add(item);
        //                }
        //            }

        //            // Save the changes to the database
        //            db.Entry(existingProduction).State = EntityState.Modified;
        //            db.SaveChanges();

        //            // Redirect to the index page after saving changes
        //            return RedirectToAction("Index");
        //        }

        //        // Repopulate ViewBag on failure (when ModelState is not valid)
        //        var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
        //        ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.NewProduction.ProductId);

        //        // Return the view with the model data on failure
        //        return View(model);
        //    }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
    NewProductionViewModel model,
   [Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,ProductId,Quantity,Validate")] NewProduction newProduction,
    [Bind(Prefix = "QuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId")] List<QuantityToProduce> quantityToProduces,
    [Bind(Prefix = "SubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItemProduction> subItemProductions)

        {
            
            {
                // Fetch the existing NewProduction entity
                var existingProduction = db.NewProductions.Find(model.NewProduction.Id);

                // Ensure the NewProduction entity exists
                if (existingProduction == null)
                {
                    return HttpNotFound();
                }

                // Update NewProduction fields
                existingProduction.ProductionDate = model.NewProduction.ProductionDate;
                existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.NewProduction.ProductId)?.Name;
                existingProduction.Unit = model.NewProduction.Unit;
                existingProduction.ProductId = model.NewProduction.ProductId;
             
                // Remove existing QuantityToProduce entries for the current NewProduction
                var existingQuantityToProduces = db.QuantityToProduces
                    .Where(x => x.NewProductionId == existingProduction.Id)
                    .ToList();
                db.QuantityToProduces.RemoveRange(existingQuantityToProduces);
              
                // Add new QuantityToProduce entries
                if (quantityToProduces != null && quantityToProduces.Count > 0)
                {
                    foreach (var item in quantityToProduces)
                    {
                        item.ProductId = existingProduction.ProductId; // Update ProductId
                        item.NewProductionId = existingProduction.Id;  // Associate with the current NewProduction
                        db.QuantityToProduces.Add(item);
                    }
                }
               
                // Save the changes to the database
                db.Entry(existingProduction).State = EntityState.Modified;
              
                db.SaveChanges();

                // Redirect to the index page after saving changes
                return RedirectToAction("Index");
            }

            // Repopulate ViewBag on failure (when ModelState is not valid)
            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.NewProduction.ProductId);

            // Return the view with the model data on failure
            return View(model);
        }





        // GET: Products/Delete/5
        public ActionResult Delete(decimal id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }

            // POST: Products/Delete/5
            [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Product product = db.Products.Find(id);
            bool isPresent = false;
            if (db.PODs.FirstOrDefault(x => x.ProductId == id) != null || db.SODs.FirstOrDefault(x => x.ProductId == id) != null)
            {
                isPresent = true;
            }

            if (isPresent == false)
            {
                db.Products.Remove(product);
            }
            else
            {
                product.Status = "D";
                db.Entry(product).Property(x => x.Status).IsModified = true;

            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult GetProductUnit(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                return Json(new { unit = product.Unit }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { unit = "N/A" }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetSubItemDetails(int productId)
        //{
        //    // Retrieve sub-item details for the given product ID
        //    var subItems = db.SubItems
        //        .Where(s => s.ParentProductId == productId)
        //        .Select(s => new
        //        {
        //            s.Id,
        //            ProductName = s.Product.Name, // Assuming a navigation property for Product
        //            s.Quantity,
        //            s.Unit,
        //             Manufacturable = s.Product.Manufacturable
        //            /* s.Shape*/ // If Shape is part of the SubItem model
        //        })
        //        .ToList();

        //    return Json(subItems, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetSubItemDetails(int productId)
        {
            // Retrieve sub-item details for the given product ID
            //var subItems = db.SubItems
            //    .Where(s => s.ParentProductId == productId)

            //    .Select(s => new
            //    {
            //        s.Id,
            //        ProductId = s.Product.Id,
            //        ProductName = s.Product.Name, // Assuming a navigation property for Product
            //        //s.Product.PType,
            //        s.Quantity,
            //        s.Unit,


            //        //s.Product.Manufacturable
            //        manufacturable = s.Product.Manufacturable, // Include the Manufacturable property from the Product table
            //        PType = s.Product.PType == 1 ? "Variable" :
            //        s.Product.PType == 2 ? "Excess" :
            //        s.Product.PType == 3 ? "ByProduct" : "Unknown", // Default case
            //        VariableProduct = s.Product.VarProdParentId,
            //    })
            //    .ToList();
            var subItems = db.SubItems
    .Include(s => s.Product) // Ensure Product is loaded
    .Where(s => s.ParentProductId == productId)
    .Select(s => new
    {
        s.Id,
        ProductId = s.Product != null ? s.Product.Id : 0, // Prevent null reference errors
        ProductName = s.Product != null ? s.Product.Name : "Unknown",
        s.Quantity,
        s.Unit,
        manufacturable = s.Product != null && s.Product.PType == 4, // Prevent null reference errors
        Ingredient = s.Product != null && s.Product.PType == 5,
        IntermediaryIngredient = s.Product != null && s.Product.PType == 6,
        FinalProduct = s.Product != null && s.Product.PType == 4,
        PType = s.Product != null ?
            (s.Product.PType == 1 ? "Variable" :
             s.Product.PType == 2 ? "Excess" :
             s.Product.PType == 3 ? "ByProduct" : "Unknown")
            : "Unknown",
        VariableProduct = s.Product != null ? s.Product.VarProdParentId : (int?)null,
    })
    .ToList();


            return Json(subItems, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult ValidateStock(List<FinalProductionViewModel> updates)
        //{
        //    Console.WriteLine("ValidateStock method hit!");
        //                return Json(new { message = "Stock validated!", success = true });
        //}

        [HttpPost]
        public JsonResult ValidateStock(List<FinalProductionViewModel> LstProductionVM)
        {
            if (LstProductionVM == null || !LstProductionVM.Any())
            {
                return Json(new { message = "No updates received.", success = false });
            }

            foreach (FinalProductionViewModel productionVM in LstProductionVM.ToList())
            {
                Product product = db.Products.FirstOrDefault(p => p.Id == productionVM.ProductId);
                product.Stock -= productionVM.CalculatedValue;
                db.Entry(product).Property(x => x.Stock).IsModified = true;
                
                //var production = db.NewProductions.FirstOrDefault(n => n.Id == productionVM.ProductionId);
            }
            db.SaveChanges();
            //FinalProductionViewModel production= new FinalProductionViewModel ();
            foreach (FinalProductionViewModel productionVM in LstProductionVM.ToList())
            {
                //Console.WriteLine($"Received Update - ProductId: {production.ProductId}, Name: {production.Name}, CalculatedValue: {production.CalculatedValue}");

                // Find the product by ProductId
                var product = db.Products.FirstOrDefault(p => p.Id == productionVM.ProductId);
                var production = db.NewProductions.FirstOrDefault(n => n.Id == productionVM.ProductionId);

                if (product != null && production != null)
                {
                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

                    // ✅ Attach Product and Production to track changes
                    db.Products.Attach(product);
                    db.NewProductions.Attach(production);

                    // Perform stock subtraction (Stock - CalculatedValue)
                    product.Stock -= productionVM.CalculatedValue;

                    // Ensure stock does not go negative
                    if (product.Stock < 0)
                    {
                        product.Stock = 0;
                    }

                    // ✅ Explicitly mark properties as modified
                    db.Entry(product).Property(x => x.Stock).IsModified = true;

                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");

                    // ✅ Mark the production as validated
                    production.Validate = true;
                    db.Entry(production).Property(x => x.Validate).IsModified = true;
                }
                else
                {
                    Console.WriteLine($"Product '{productionVM.Name}' not found in the database.");
                    return Json(new { message = $"Product '{productionVM.Name}' not found.", success = false });
                }
            }

            // ✅ Save all changes at once
            var changes = db.SaveChanges();
            Console.WriteLine($"Changes Saved: {changes}");

            if (changes > 0)
            {
                return Json(new { message = "Stock updated & Production validated successfully.", success = true });
            }
            else
            {
                return Json(new { message = "No changes detected in the database.", success = false });
            }
        }

        [HttpPost]
        public JsonResult ValidateCStock(List<FinalProductionViewModel> updates)
        {
            if (updates == null || !updates.Any())
            {
                return Json(new { message = "No updates received.", success = false });
            }

            foreach (var prod in updates)
            {
                Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, CalculatedValue: {prod.CalculatedValue}");

                // Find the product by ProductId
                var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);

                if (product != null)
                {
                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

                    // Perform stock subtraction (Stock - CalculatedValue)
                    product.Stock += prod.CalculatedValue;

                    // Ensure stock does not go negative
                    if (product.Stock < 0)
                    {
                        product.Stock = 0;
                    }

                    // Mark property as modified
                    db.Entry(product).Property(x => x.Stock).IsModified = true;

                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
                }
                else
                {
                    Console.WriteLine($"Product '{prod.Name}' not found in the database.");
                    return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
                }
            }

            // Save all changes at once
            db.SaveChanges();

            return Json(new { message = "Stock updated successfully.", success = true });
        }
        [HttpPost]
        public JsonResult IncreasedStock(List<FinalProductionViewModel> updates)
        {
            if (updates == null || !updates.Any())
            {
                return Json(new { message = "No updates received.", success = false });
            }

            foreach (var prod in updates)
            {
                Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, CalculatedValue: {prod.CalculatedValue}");

                // Find the product by ProductId
                var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);

                if (product != null)
                {
                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

                    // Perform stock subtraction (Stock - CalculatedValue)
                    product.Stock += prod.CalculatedValue;

                    // Ensure stock does not go negative
                    if (product.Stock < 0)
                    {
                        product.Stock = 0;
                    }

                    // Mark property as modified
                    db.Entry(product).Property(x => x.Stock).IsModified = true;

                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
                }
                else
                {
                    Console.WriteLine($"Product '{prod.Name}' not found in the database.");
                    return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
                }
            }

            // Save all changes at once
            db.SaveChanges();

            return Json(new { message = "Stock updated successfully.", success = true });
        }

        [HttpPost]
        public JsonResult EIncreasedStock(List<FinalProductionViewModel> updates)
        {
            if (updates == null || !updates.Any())
            {
                return Json(new { message = "No updates received.", success = false });
            }

            foreach (var prod in updates)
            {
                Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, CalculatedValue: {prod.CalculatedValue}");

                // Find the product by ProductId
                var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);
                var production = db.PostProductions.FirstOrDefault(n => n.Id == prod.ProductionId);
                if (product != null && production != null)
                {
                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

                    // Perform stock subtraction (Stock - CalculatedValue)
                    product.Stock += prod.CalculatedValue;

                    // Ensure stock does not go negative
                    if (product.Stock < 0)
                    {
                        product.Stock = 0;
                    }

                    // Mark property as modified
                    db.Entry(product).Property(x => x.Stock).IsModified = true;
                    production.Validate = true;
                    db.Entry(production).Property(x => x.Validate).IsModified = true;
                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
                }
                else
                {
                    Console.WriteLine($"Product '{prod.Name}' not found in the database.");
                    return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
                }
            }

            // Save all changes at once
            db.SaveChanges();

            return Json(new { message = "Stock updated successfully.", success = true });
        }

        //public JsonResult GetSubItemDetails(int productId)
        //{
        //    // Fetch sub-item details including weight from ProductDetails table
        //    var subItems = db.SubItems
        //        .Where(s => s.ParentProductId == productId)
        //        .Join(
        //            db.ProductDetails, // Join with ProductDetails
        //            subItem => subItem.ProductId,
        //            productDetail => productDetail.ProductId,
        //            (subItem, productDetail) => new
        //            {
        //                subItem.Id,
        //                ProductName = subItem.Product.Name, // Assuming a navigation property for Product
        //                subItem.Quantity,
        //                subItem.Unit,
        //                Weight = productDetail.Weight // Fetch weight from ProductDetails
        //            })
        //        .ToList();

        //    return Json(subItems, JsonRequestBehavior.AllowGet);
        //}



        //public JsonResult GetSubItemDetails(int parentProductId)
        //{
        //    System.Diagnostics.Debug.WriteLine($"Received ParentProductId: {parentProductId}");

        //    var subItems = db.SubItems
        //        .Where(s => s.ParentProductId == parentProductId)
        //        .Include(s => s.Product) // Load navigation property if needed
        //        .Select(s => new
        //        {
        //            s.Id,
        //            ProductName = s.Product.Name,
        //            s.ProductId,
        //            s.Quantity,
        //            s.Unit
        //        })
        //        .ToList();

        //    System.Diagnostics.Debug.WriteLine($"SubItems Count: {subItems.Count}");

        //    return Json(subItems, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult ValidateStock(List<FinalProductionViewModel> updates)
        //{

        //    foreach (var update in updates)
        //    {
        //        // Find the product by its Name
        //        var product = db.Products.FirstOrDefault(p => p.Id == update.Product.Id);
        //        if (product != null)
        //        {
        //            // Subtract the calculated value from the stock
        //            product.Stock -= update.SubtractValue;

        //            // Ensure stock does not go negative
        //            if (product.Stock < 0)
        //                product.Stock = 0;

        //            // Save changes directly to the database
        //        }
        //    }

        //    db.SaveChanges();

        //    return Json(new { message = "Stock updated successfully" });
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult ValidateStock(List<FinalProductionViewModel> updates)
        //{
        //       if (updates == null || !updates.Any())
        //   {
        //        return Json(new { message = "No updates received.", success = false });
        //    }

        //            foreach (var prod in updates)
        //            {
        //                Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, SubtractValue: {prod.SubtractValue}");

        //                // Find the product by ProductId instead of Name for accuracy
        //                var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);

        //                if (product != null)
        //                {
        //                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

        //                    // Subtract stock safely
        //                    product.Stock -= prod.SubtractValue;

        //                    // Ensure stock does not go negative
        //                    if (product.Stock < 0)
        //                    {
        //                        product.Stock = 0;
        //                    }

        //        //            // Mark property as modified
        //                    db.Entry(product).Property(x => x.Stock).IsModified = true;

        //                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
        //                }
        //                else
        //                {
        //                    Console.WriteLine($"Product '{prod.Name}' not found in the database.");
        //                    return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
        //                }
        //            }

        //        //    // Save all changes at once
        //            db.SaveChanges();

        //            return Json(new { message = "Stock updated successfully.", success = true });
        //}
        // [HttpPost]
        //public JsonResult ValidateStock(List<FinalProductionViewModel> updates)
        //{
        //    if (updates == null || !updates.Any())
        //    {
        //        return Json(new { message = "No updates received.", success = false });
        //    }

        //    foreach (var prod in updates)
        //    {
        //        Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, SubtractValue: {prod.SubtractValue}");

        //        // Find the product by ProductId instead of Name for accuracy
        //        var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);

        //        if (product != null)
        //        {
        //            Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

        //            // Subtract stock safely
        //            product.Stock -= prod.SubtractValue;

        //            // Ensure stock does not go negative
        //            if (product.Stock < 0)
        //            {
        //                product.Stock = 0;
        //            }

        //            // Mark property as modified
        //            db.Entry(product).Property(x => x.Stock).IsModified = true;

        //            Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Product '{prod.Name}' not found in the database.");
        //            return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
        //        }
        //    }

        //    // Save all changes at once
        //    db.SaveChanges();

        //    return Json(new { message = "Stock updated successfully.", success = true });
        //}

        [HttpGet]
        public ActionResult FinalExcess()
        {
            decimal maxId = db.NewProductions.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;  // Passing the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers;  // Assuming this contains the supplier list

            var products = db.Products
       .Where(p => p.PType == 4)
       .Select(p => new
       {
           Value = p.Id.ToString(),  // ID of the product
           Text = p.Name            // Name of the product
       })
       .ToList();

            ViewBag.ProductList = new SelectList(products, "Value", "Text"); // Pass as a SelectList
            var productDetails = db.ProductDetails.ToList();

            var remarks = db.BOMs
    .Select(b => new
    {
        Value = b.Remarks,           // Key for dropdown
        Text = b.Remarks,            // Display text
        ProductName = b.ProductName, // Related product name
        Unit = b.Product.Unit    // Related time unit or unit field
    })
    .ToList();

            ViewBag.Remarks = new SelectList(remarks, "Value", "Text");
            ViewBag.RemarksData = remarks;


            var productList = db.Products
                                    .Select(p => new
                                    {
                                        p.Id,
                                        p.Name,
                                        p.Stock,
                                        p.Remarks, // Fetch remarks from BOM
                                        p.Category,   // Fetch stock for AvailableInventory
                                    })
                                    .ToList();

            ViewBag.ProductList = new SelectList(productList, "Id", "Name");

            // Fetch data and then map to SubItem outside the LINQ query
            var subItems = db.Products
                             .Select(p => new
                             {
                                 ProductId = p.Id,
                                 AvailableInventory = p.Stock
                             })
                             .AsEnumerable() // Move query execution to memory
                             .Select(p => new SubItem
                             {
                                 ProductId = p.ProductId,
                                 AvailableInventory = p.AvailableInventory,
                                 QuantitytoPrepare = 0 // Initial value
                             })
                             .ToList();

            var viewModel = new NewProductionViewModel
            {
                NewProduction = new NewProduction(),
                Products = DAL.dbProducts,
                SubItems = subItems,
                QuantityToProduce = new List<QuantityToProduce> { new QuantityToProduce() },
                ProductDetail = new List<ProductDetail> { new ProductDetail() },
            };

            return View(viewModel);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}