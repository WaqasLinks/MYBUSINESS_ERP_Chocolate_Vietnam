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
    public class PPPostProductionController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private PPPostProductionViewModel pppostproductionViewModel = new PPPostProductionViewModel();
        // GET: PPPostProduction
        public ActionResult Index()
        {
            var postProductions = db.PPPostProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(postProductions);
        }
        public ActionResult IndexComponents()
        {
            var postProductions = db.PPPostProductions
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

        [HttpGet]
        public ActionResult Create(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            decimal maxId = db.PPPostProductions.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;  // Passing the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers;  // Assuming this contains the supplier list

            // Fetch the NewProduction entity with related Products and other details
            var newProduction = db.PPNewProductions
                                   .Include(np => np.Product) // Include related Products
                                   .FirstOrDefault(np => np.Id == id);


            if (newProduction == null)
            {
                return HttpNotFound();
            }
            var quantityToProduce = db.PPQuantityToProduces
.Where(x => x.NewProductionId == newProduction.Id)
.ToList();

            var totalWeight = quantityToProduce.Sum(x => x.CalculatedWeight) ?? 0;




            //var subItems = db.SubItems.Where(s => s.ParentProductId == newProduction.ProductId).ToList();
            var subItemProductions = db.PPSubItemProductions.Where(s => s.NewProductionId == newProduction.Id).ToList();
            Console.WriteLine(newProduction.ProductionDate); // Debugging line
            // Get the list of products
            var products = db.Products
    .Where(p => p.PType == 4)
    .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
    .ToList();

            // Pre-select the current product in the dropdown
            ViewBag.ProductList = new SelectList(products, "Value", "Text", newProduction.ProductId);


            // Prepare ViewModel (including SubItems if needed)
            var viewModel = new PPPostProductionViewModel
            {
                //SubItems = subItems,
                PPSubItemProduction = db.PPSubItemProductions.Where(x => x.NewProductionId == newProduction.Id).ToList(),
                PPNewProduction = newProduction,
                PPPostProduction = new PPPostProduction
                {
                    Unit = newProduction.Unit // <-- Make sure to copy this value into the correct object
                },
                Products = db.Products, // Include all products
                /* SubItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList(), */// Get SubItems associated with this NewProduction
                TotalWeight = totalWeight,                                                                                    //QuantityToProduce = db.QuantityToProduces.Where(x => x.ProductId == newProduction.Id).ToList()
                PPQuantityToProduce = quantityToProduce
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
[Bind(Prefix = "PPPostProduction", Include = "Id,ProductionDate,ProductName,Unit,Quantity,ProductId,ProductionId,Note")] PPPostProduction postProduction,
//[Bind(Prefix = "SubItem", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItem> subItems,
[Bind(Prefix = "PPSubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<PPSubItemProduction> subItemProductions,
[Bind(Prefix = "PPQuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId")] List<PPQuantityToProduce> quantityToProduces)
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
                db.PPPostProductions.Add(postProduction);

                if (subItemProductions != null)
                {
                    foreach (var item in subItemProductions)
                    {
                        item.ParentProductId = postProduction.ProductId;
                        item.PostProductionId = postProduction.Id;
                        db.PPSubItemProductions.Add(item);
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

            return View(pppostproductionViewModel);
        }

        public ActionResult Edit(decimal? id, bool readonlyMode = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch the NewProduction entity with related Products and other details
            var postProduction = db.PPPostProductions
                                   .Include(np => np.Product) // Include related Products
                                   .FirstOrDefault(np => np.Id == id);


            if (postProduction == null)
            {
                return HttpNotFound();
            }
            var quantityToProduce = db.PPQuantityToProduces
      .Where(x => x.PostProductionId == postProduction.Id) // Correct filtering
      .ToList();

            var totalWeight = quantityToProduce.Sum(x => x.CalculatedWeight) ?? 0;




            //var subItems = db.SubItems.Where(s => s.ParentProductId == newProduction.ProductId).ToList();
            //var subItemProductions = db.SubItemProductions.Where(s => s.PostProductionId == postProduction.Id).ToList();
            var subItemProductions = db.PPSubItemProductions
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
            var viewModel = new PPPostProductionViewModel
            {
                //SubItems = subItems,
                PPSubItemProduction = db.PPSubItemProductions.Where(x => x.PostProductionId == postProduction.Id).ToList(),
                PPPostProduction = postProduction,
                Products = db.Products, // Include all products
                /* SubItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList(), */// Get SubItems associated with this NewProduction
                TotalWeight = totalWeight,                                                                                    //QuantityToProduce = db.QuantityToProduces.Where(x => x.ProductId == newProduction.Id).ToList()
                PPQuantityToProduce = quantityToProduce

            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
 PPPostProductionViewModel model,
 [Bind(Prefix = "PPPostProduction", Include = "Id,ProductionDate,ProductName,Unit,ProductId,Quantity,ProductionId,Note")] PPPostProduction postProduction,
 [Bind(Prefix = "PPQuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId")] List<PPQuantityToProduce> quantityToProduces,
 [Bind(Prefix = "PPSubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<PPSubItemProduction> subItemProductions)

        {

            {
                // Fetch the existing NewProduction entity
                var existingProduction = db.PPPostProductions.Find(model.PPPostProduction.Id);

                // Ensure the NewProduction entity exists
                if (existingProduction == null)
                {
                    return HttpNotFound();
                }

                // Update NewProduction fields
                existingProduction.ProductionDate = model.PPPostProduction.ProductionDate;
                existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.PPPostProduction.ProductId)?.Name;
                existingProduction.Unit = model.PPPostProduction.Unit;
                existingProduction.ProductId = model.PPPostProduction.ProductId;

                // Remove existing QuantityToProduce entries for the current NewProduction
                var existingQuantityToProduces = db.PPQuantityToProduces
                    .Where(x => x.NewProductionId == existingProduction.Id)
                    .ToList();
                db.PPQuantityToProduces.RemoveRange(existingQuantityToProduces);

                // Add new QuantityToProduce entries
                if (quantityToProduces != null && quantityToProduces.Count > 0)
                {
                    foreach (var item in quantityToProduces)
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
                    manufacturable = s.Product.PType == 4, // Include the Manufacturable property from the Product table
                    PType = s.Product.PType == 1 ? "Variable" :
                    s.Product.PType == 2 ? "Excess" :
                    s.Product.PType == 3 ? "ByProduct" : "Unknown", // Default case
                    VariableProduct = s.Product.VarProdParentId,
                })
                .ToList();

            return Json(subItems, JsonRequestBehavior.AllowGet);
        }

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

        [HttpGet]
        public ActionResult FinalExcess()
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