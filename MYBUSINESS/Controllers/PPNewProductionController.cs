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
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;

namespace MYBUSINESS.Controllers
{
    public class PPNewProductionController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private PPNewProductionViewModel ppnewproductionViewModel = new PPNewProductionViewModel();
        // GET: PPNewProduction
        public ActionResult Index()
        {
            var ppnewProductions = db.PPNewProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(ppnewProductions);
        }
        public ActionResult IndexComponents()
        {
            var ppnewProductions = db.PPNewProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(ppnewProductions);

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

        [HttpGet]
        public ActionResult Create()
        {
            decimal maxId = db.PPNewProductions.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
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

            var viewModel = new PPNewProductionViewModel
            {
                PPNewProduction = new PPNewProduction(),
                Products = DAL.dbProducts,
                SubItems = subItems,
                PPQuantityToProduce = new List<PPQuantityToProduce> { new PPQuantityToProduce() },
                ProductDetail = new List<ProductDetail> { new ProductDetail() },
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
[Bind(Prefix = "PPNewProduction", Include = "Id,ProductionDate,ProductName,Unit,ProductId")] PPNewProduction ppnewProduction,
//[Bind(Prefix = "SubItem", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItem> subItems,
[Bind(Prefix = "PPSubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested,SubItemQty")] List<PPSubItemProduction> ppsubItemProductions,
[Bind(Prefix = "PPQuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId,Weight")] List<PPQuantityToProduce> ppquantityToProduces)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.Find(ppnewProduction.ProductId);
                if (product != null)
                {
                    ppnewProduction.ProductName = product.Name;
                }

                // Save the BOM and its SubItems
                db.PPNewProductions.Add(ppnewProduction);

                if (ppsubItemProductions != null)
                {
                    foreach (var item in ppsubItemProductions)
                    {
                        item.ParentProductId = ppnewProduction.ProductId;
                        item.NewProductionId = ppnewProduction.Id;
                        db.PPSubItemProductions.Add(item);
                    }
                }
                //foreach (var item in subItems)
                //{
                //    item.ParentProductId = newProduction.ProductId;
                //    db.SubItems.Add(item);
                //}

                if (ppquantityToProduces != null)
                {
                    foreach (var item in ppquantityToProduces)
                    {
                        item.ProductId = ppnewProduction.ProductId;
                        item.NewProductionId = ppnewProduction.Id;
                        //item.Id = newProduction.Id;
                        db.PPQuantityToProduces.Add(item);
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

            return View(ppnewproductionViewModel);
        }


        public ActionResult Edit(decimal? id, bool readonlyMode = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch the NewProduction entity with related Products and other details
            var ppnewProduction = db.PPNewProductions
                                   .Include(np => np.Product) // Include related Products
                                   .FirstOrDefault(np => np.Id == id);


            if (ppnewProduction == null)
            {
                return HttpNotFound();
            }
            string productType;
            switch (ppnewProduction.Product.PType)
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

            var ppquantityToProduce = db.PPQuantityToProduces
.Where(x => x.NewProductionId == ppnewProduction.Id)
.ToList();




            var totalWeight = ppquantityToProduce.Sum(x => x.CalculatedWeight) ?? 0;




            //var subItems = db.SubItems.Where(s => s.ParentProductId == newProduction.ProductId).ToList();
            var ppsubItemProductions = db.PPSubItemProductions.Where(s => s.NewProductionId == ppnewProduction.Id).ToList();
            Console.WriteLine(ppnewProduction.ProductionDate); // Debugging line
                                                               // Get the list of products
            var products = db.Products
        .Where(p => p.PType == 4)
        .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
        .ToList();

            // Pre-select the current product in the dropdown
            ViewBag.ProductList = new SelectList(products, "Value", "Text", ppnewProduction.ProductId);

            ViewBag.ReadonlyMode = readonlyMode;
            // Prepare ViewModel (including SubItems if needed)
            var viewModel = new PPNewProductionViewModel
            {
                //SubItems = subItems,
                PPSubItemProductions = db.PPSubItemProductions.Where(x => x.NewProductionId == ppnewProduction.Id).ToList(),
                PPNewProduction = ppnewProduction,
                Products = db.Products, // Include all products
                /* SubItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList(), */// Get SubItems associated with this NewProduction
                TotalWeight = totalWeight,                                                                                    //QuantityToProduce = db.QuantityToProduces.Where(x => x.ProductId == newProduction.Id).ToList()
                PPQuantityToProduce = ppquantityToProduce
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
PPNewProductionViewModel model,
[Bind(Prefix = "PPNewProduction", Include = "Id,ProductionDate,ProductName,Unit,ProductId,Quantity,Validate")] PPNewProduction ppnewProduction,
[Bind(Prefix = "PPQuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId")] List<PPQuantityToProduce> ppquantityToProduces,
[Bind(Prefix = "PPSubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<PPSubItemProduction> ppsubItemProductions)

        {

            {
                // Fetch the existing NewProduction entity
                var existingProduction = db.PPNewProductions.Find(model.PPNewProduction.Id);

                // Ensure the NewProduction entity exists
                if (existingProduction == null)
                {
                    return HttpNotFound();
                }

                // Update NewProduction fields
                existingProduction.ProductionDate = model.PPNewProduction.ProductionDate;
                existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.PPNewProduction.ProductId)?.Name;
                existingProduction.Unit = model.PPNewProduction.Unit;
                existingProduction.ProductId = model.PPNewProduction.ProductId;

                // Remove existing QuantityToProduce entries for the current NewProduction
                var existingQuantityToProduces = db.PPQuantityToProduces
                    .Where(x => x.NewProductionId == existingProduction.Id)
                    .ToList();
                db.PPQuantityToProduces.RemoveRange(existingQuantityToProduces);

                // Add new QuantityToProduce entries
                if (ppquantityToProduces != null && ppquantityToProduces.Count > 0)
                {
                    foreach (var item in ppquantityToProduces)
                    {
                        item.ProductId = existingProduction.ProductId; // Update ProductId
                        item.NewProductionId = existingProduction.Id;  // Associate with the current NewProduction
                        db.PPQuantityToProduces.Add(item);
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
            ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.PPNewProduction.ProductId);

            // Return the view with the model data on failure
            return View(model);
        }

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
                var production = db.PPNewProductions.FirstOrDefault(n => n.Id == productionVM.ProductionId);

                if (product != null && production != null)
                {
                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

                    // ✅ Attach Product and Production to track changes
                    db.Products.Attach(product);
                    db.PPNewProductions.Attach(production);

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

    }
}