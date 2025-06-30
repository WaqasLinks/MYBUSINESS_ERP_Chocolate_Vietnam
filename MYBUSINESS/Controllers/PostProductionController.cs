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
    [Authorize(Roles = "Admin,Technical Manager,Chocolate Production manager")]
    public class PostProductionController : Controller
    {
        // GET: PostProduction
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private PostProductionViewModel postProductionViewModel = new PostProductionViewModel();

        [Authorize(Roles = "Admin,Technical Manager,Chocolate Production manager")]
        // GET: Products

        public ActionResult Index()
        {
            var postProductions = db.PostProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(postProductions);
        }
        public ActionResult IndexComponents()
        {
            var postProductions = db.PostProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(postProductions);

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
            public ActionResult Create(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            decimal maxId = db.PostProductions.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;  // Passing the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers;  // Assuming this contains the supplier list

            // Fetch the NewProduction entity with related Products and other details
            var newProduction = db.NewProductions
                                   .Include(np => np.Product) // Include related Products
                                   .FirstOrDefault(np => np.Id == id);


            if (newProduction == null)
            {
                return HttpNotFound();
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


            // Prepare ViewModel (including SubItems if needed)
            var viewModel = new PostProductionViewModel
            {
                //SubItems = subItems,
                SubItemProduction = db.SubItemProductions.Where(x => x.NewProductionId == newProduction.Id).ToList(),
                NewProduction = newProduction,
                PostProduction = new PostProduction
                {
                    Unit = newProduction.Unit // <-- Make sure to copy this value into the correct object
                },
                Products = db.Products, // Include all products
                /* SubItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList(), */// Get SubItems associated with this NewProduction
                TotalWeight = totalWeight,                                                                                    //QuantityToProduce = db.QuantityToProduces.Where(x => x.ProductId == newProduction.Id).ToList()
                QuantityToProduce = quantityToProduce
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
     [Bind(Prefix = "PostProduction", Include = "Id,ProductionDate,ProductName,Unit,Quantity,ProductId,ProductionId,Note")] PostProduction postProduction,
     //[Bind(Prefix = "SubItem", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItem> subItems,
     [Bind(Prefix = "SubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItemProduction> subItemProductions,
     [Bind(Prefix = "QuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId")] List<QuantityToProduce> quantityToProduces)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.Find(postProduction.ProductId);
                if (product != null)
                {
                    postProduction.ProductName = product.Name;
                }


                postProduction.ProductionId = postProduction.ProductionId;
                // Save the BOM and its SubItems
                db.PostProductions.Add(postProduction);

                if (subItemProductions != null)
                {
                    foreach (var item in subItemProductions)
                    {
                        item.ParentProductId = postProduction.ProductId;
                        item.PostProductionId = postProduction.Id;
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
                        item.ProductId = postProduction.ProductId;
                        item.PostProductionId = postProduction.Id;
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

            return View(postProductionViewModel);
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
            var postProduction = db.PostProductions
                                   .Include(np => np.Product) // Include related Products
                                   .FirstOrDefault(np => np.Id == id);


            if (postProduction == null)
            {
                return HttpNotFound();
            }
          var quantityToProduce = db.QuantityToProduces
    .Where(x => x.PostProductionId == postProduction.Id) // Correct filtering
    .ToList();

            var totalWeight = quantityToProduce.Sum(x => x.CalculatedWeight) ?? 0;




            //var subItems = db.SubItems.Where(s => s.ParentProductId == newProduction.ProductId).ToList();
            //var subItemProductions = db.SubItemProductions.Where(s => s.PostProductionId == postProduction.Id).ToList();
            var subItemProductions = db.SubItemProductions
                           .Include(s => s.Product)  // Ensure Product is loaded
                           .Where(s => s.PostProductionId == postProduction.Id)
                           .ToList();

            Console.WriteLine(postProduction.ProductionDate); // Debugging line
            // Get the list of products
            var products = db.Products
                             .Where(p => p.PType == 4)
                             .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
                             .ToList();

            // Pre-select the current product in the dropdown
            ViewBag.ProductList = new SelectList(products, "Value", "Text", postProduction.ProductId);
            ViewBag.ReadonlyMode = readonlyMode;
            // Prepare ViewModel (including SubItems if needed)
            var viewModel = new PostProductionViewModel
            {
                //SubItems = subItems,
                SubItemProduction = db.SubItemProductions.Where(x => x.PostProductionId == postProduction.Id).ToList(),
                PostProduction = postProduction,
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
    PostProductionViewModel model,
    [Bind(Prefix = "PostProduction", Include = "Id,ProductionDate,ProductName,Unit,ProductId,Quantity,ProductionId,Note")] PostProduction postProduction,
    [Bind(Prefix = "QuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId")] List<QuantityToProduce> quantityToProduces,
    [Bind(Prefix = "SubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit")] List<SubItemProduction> subItemProductions)
        {
            
            {
                // Fetch the existing PostProduction entity
                var existingProduction = db.PostProductions.Find(model.PostProduction.Id);

                if (existingProduction == null)
                {
                    return HttpNotFound();
                }

                // Update PostProduction fields
                existingProduction.ProductionDate = model.PostProduction.ProductionDate;
                existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.PostProduction.ProductId)?.Name;
                existingProduction.Unit = model.PostProduction.Unit;
                existingProduction.ProductId = model.PostProduction.ProductId;

                // Handle QuantityToProduce updates
                var existingQuantityToProduces = db.QuantityToProduces
                    .Where(x => x.PostProductionId == existingProduction.Id)
                    .ToList();
                db.QuantityToProduces.RemoveRange(existingQuantityToProduces);

                if (quantityToProduces != null && quantityToProduces.Count > 0)
                {
                    foreach (var item in quantityToProduces)
                    {
                        item.ProductId = existingProduction.ProductId;
                        item.PostProductionId = existingProduction.Id;
                        db.QuantityToProduces.Add(item);
                    }
                }

                // Handle SubItemProduction updates
                if (subItemProductions != null && subItemProductions.Count > 0)
                {
                    foreach (var item in subItemProductions)
                    {
                        var existingItem = db.SubItemProductions.Find(item.Id);
                        if (existingItem != null)
                        {
                            // Update only the fields that can be modified
                            existingItem.Quantity = item.Quantity;
                            existingItem.Unit = item.Unit;
                            db.Entry(existingItem).State = EntityState.Modified;
                        }
                    }
                }

                db.Entry(existingProduction).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Repopulate ViewBag on failure
            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.PostProduction.ProductId);

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
            var subItems = db.SubItems
                .Where(s => s.ParentProductId == productId)
                
                .Select(s => new
                {
                    s.Id,
                    ProductId = s.Product.Id,
                    ProductName = s.Product.Name, // Assuming a navigation property for Product
                    //s.Product.PType,
                    s.Quantity,
                    s.Unit,
                    
                    
                    //s.Product.Manufacturable
                    manufacturable = s.Product.PType==4, // Include the Manufacturable property from the Product table
                    PType = s.Product.PType == 1 ? "Variable" :
                    s.Product.PType == 2 ? "Excess" :
                    s.Product.PType == 3 ? "ByProduct" : "Unknown", // Default case
                    VariableProduct = s.Product.VarProdParentId,
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
        public JsonResult ValidateStock(List<FinalProductionViewModel> updates)
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
                    product.Stock -= prod.CalculatedValue;

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