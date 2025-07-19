using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class PackagingBOMController : Controller
    {
        // GET: PackagingBOM
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private PacakgingBOMViewModel packagingbomViewModel = new PacakgingBOMViewModel();
        // GET: Products

        [Authorize(Roles = "Admin,Technical Manager")]
        public ActionResult Index(int? pType = null)
        {
            var productTypes = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "VariableProduct" },
        new SelectListItem { Value = "2", Text = "ExcessProduct" },
        new SelectListItem { Value = "3", Text = "ByProduct" },
        new SelectListItem { Value = "4", Text = "FinishedProduct" },
        new SelectListItem { Value = "5", Text = "IngredientProduct" },
        new SelectListItem { Value = "6", Text = "IntermedataryProduct" },
        new SelectListItem { Value = "7", Text = "Merchendise" }
    };
            ViewBag.ProductTypes = productTypes;
            ViewBag.CurrentPType = pType;
            ViewBag.Suppliers = DAL.dbSuppliers;
            var query = db.PackagingBOMs.OrderByDescending(p => p.Id).AsQueryable();

            if (pType != null)
            {
                query = query.Where(x => x.Product.PType == pType);
            }

            var bom = query.ToList();
            return View(bom);
            //return View(DAL.dbBOMs.Where(x => x.SubItems.Count() == 0).ToList());
        }
        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var bom = db.PackagingBOMs.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            return View(bom);
            //return View(DAL.dbBOMs.Where(x => x.SubItems.Count() > 0).ToList());

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
        [Authorize(Roles = "Admin,Technical Manager")]
        public ActionResult Create()
        {
            // Get the next available BOM ID (if needed)
            decimal maxId = db.PackagingBOMs.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId; // Pass the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers; // Assuming this contains the supplier list

            // Get products without BOM
            var productsWithoutBOM = db.Products
                .Where(p => (p.PType == 4 || p.PType == 6) && !db.PackagingBOMs.Any(b => b.ProductId == p.Id)) // Filter out products with existing BOMs
                .Select(p => new { Value = p.Id.ToString(), Text = p.Name }) // Prepare for SelectList
                .ToList();

            // Pass the filtered product list to the view
            ViewBag.ProductList = new SelectList(productsWithoutBOM, "Value", "Text");
            var excessProducts = db.Products
        .Where(p => p.PType == 6 || p.PType == 5)
        .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
        .ToList();
            ViewBag.ExcessProductList = new SelectList(excessProducts, "Value", "Text");

            // Get Products with PType = 3 (ByProduct)
            var byProducts = db.Products
                .Where(p => p.PType == 3)
                .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
                .ToList();
            ViewBag.ByProductList = new SelectList(byProducts, "Value", "Text");


            // Initialize the BOM and SubItem list in the ViewModel
            PacakgingBOMViewModel pacakgingbomViewModel = new PacakgingBOMViewModel
            {
                PackagingBOM = new PackagingBOM
                {
                    Saleable = true,
                    Purchasable = true,
                    Manufacturable = true
                },
                PacSubitem = new List<PacSubitem> { new PacSubitem() }, // Initialize SubItem list with one SubItem object
                PackagingColor = new List<PackagingColor> { new PackagingColor() },
                ProductType = new List<ProductType> { new ProductType() },
                ProductDetail = new List<ProductDetail> { new ProductDetail() }, // Initialize ProductDetail if needed
                Products = DAL.dbProducts // Assuming this contains product data
            };

            return View(pacakgingbomViewModel); // Passing the ViewModel to the view
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "PackagingBOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,ProductId,ProductName,CreateDate,UpdateDate,Unit,Quantity")] PackagingBOM bom,
                                   [Bind(Prefix = "PacSubitem", Include = "Id,ProductId,Quantity,Unit,ProductType")] List<PacSubitem> subItems,
                                   [Bind(Prefix = "PackagingColor", Include = "Id,ProductId,Quantity,Unit,ProductType,Color")] List<PackagingColor> packagingcolors)
        {

            if (ModelState.IsValid)
            {
                bom.CreateDate = DateTime.Now;
                bom.UpdateDate = DateTime.Now;
                var product = db.Products.FirstOrDefault(p => p.Id == bom.ProductId);

                if (product != null)
                {
                    bom.ProductName = product.Name; // Assign the ProductName
                }
                // Debugging output
                Console.WriteLine($"BOM Unit: {bom.Unit}");

                // Save the BOM and its SubItems
                db.PackagingBOMs.Add(bom);

                foreach (var item in subItems)
                {
                    item.ParentProductId = bom.ProductId;
                    item.PackagingBOMId = bom.Id;
                    db.PacSubitems.Add(item);
                }
                foreach (var item in packagingcolors)
                {
                    item.ParentProductId = bom.ProductId;
                    item.BOMId = bom.Id;
                    db.PackagingColors.Add(item);
                }
                //foreach (var item in productTypes)
                //{
                //    item.ParentProductId = bom.ProductId;
                //    item.BOMId = bom.Id;
                //    db.ProductTypes.Add(item);
                //}
                //if (productTypeDetails!=null)
                //{
                //    foreach (var item in productTypeDetails)
                //    {
                //        item.ProductId = bom.Id;
                //        db.ProductTypeDetails.Add(item);

                //    }
                //}

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(bom);
        }






        // GET: Products/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PackagingBOM bom = db.PackagingBOMs.Find(id);
            if (bom == null)
            {
                return HttpNotFound();
            }

            // Fetch products without BOM (filtered by PType 4)
            var productsWithoutBOM = db.Products
                .Where(p => (p.PType == 4 || p.PType == 6) && (!db.PackagingBOMs.Any(b => b.ProductId == p.Id) || p.Id == bom.ProductId))
                .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
                .ToList();

            ViewBag.ProductList = new SelectList(productsWithoutBOM, "Value", "Text", bom.ProductId);

            // Fetch SubItems related to this BOM
            var subItemProducts = db.PacSubitems
                .Where(s => s.PackagingBOMId == bom.Id && s.ProductId != null)  // Filter out NULL ProductId
                .Include(s => s.Product) // Eager load related Product
                .Select(s => new { Value = s.ProductId.ToString(), Text = s.Product.Name }) // Select Product Name
                .ToList();

            ViewBag.SubItemProductList = new SelectList(subItemProducts, "Value", "Text", bom.ProductId);

            var colorProducts = db.PackagingColors
                .Where(s => s.BOMId == bom.Id && s.ProductId != null)  // Filter out NULL ProductId
                .Include(s => s.Product) // Eager load related Product
                .Select(s => new { Value = s.ProductId.ToString(), Text = s.Product.Name }) // Select Product Name
                .ToList();

            ViewBag.ColorProductList = new SelectList(colorProducts, "Value", "Text", bom.ProductId);

            // Fetch all products based on PType
            var ptypeProducts = db.Products
                .Where(p => p.PType == 1 || p.PType == 6 || p.PType == 3)
                .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
                .ToList();

            ViewBag.PTypeProductList = new SelectList(ptypeProducts, "Value", "Text");

            // Pass the BOM model and related data
            var pacakgingbomViewModel = new PacakgingBOMViewModel
            {
                PackagingBOM = bom,
                Products = DAL.dbProducts,
                PacSubitem = db.PacSubitems.Where(x => x.PackagingBOMId == bom.Id).ToList(),
                PackagingColor = db.PackagingColors.Where(x => x.BOMId == bom.Id).ToList(),
                ProductType = db.ProductTypes.Where(x => x.BOMId == bom.Id).ToList()
            };

            return View(pacakgingbomViewModel);
        }
        [HttpGet]
        public JsonResult GetColors()
        {
            var colors = db.Colors
                           .Select(c => new
                           {
                               Id = c.Id,
                               ColorName = c.ColorName,
                               ColorCode = c.ColorCode
                           })
                           .ToList();

            return Json(colors, JsonRequestBehavior.AllowGet);
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
    [Bind(Prefix = "PackagingBOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,ProductName,ProductId,Unit,CreateDate,UpdateDate,Quantity")] PackagingBOM bom,
    [Bind(Prefix = "PacSubitem", Include = "Id,ProductId,Quantity,Unit")] List<PacSubitem> subItems,
    [Bind(Prefix = "PackagingColor", Include = "Id,ProductId,Quantity,Unit,ProductType,Color")] List<PackagingColor> packagingcolors)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.FirstOrDefault(p => p.Id == bom.ProductId);
                if (product != null)
                {
                    bom.ProductName = product.Name; // Assign the ProductName
                }
                // Remove existing SubItems for the BOM
                var existingSubItems = db.PacSubitems.Where(x => x.PackagingBOMId == bom.Id).ToList();
                var existingProductType = db.PackagingColors.Where(x => x.BOMId == bom.Id).ToList();
                db.PacSubitems.RemoveRange(existingSubItems);
                db.PackagingColors.RemoveRange(existingProductType);
                // Update the BOM record
                db.Entry(bom).State = EntityState.Modified;

                if (subItems != null && subItems.Count > 0)
                {
                    // Add new SubItems
                    foreach (var item in subItems)
                    {
                        item.ParentProductId = bom.ProductId; // Set ParentProductId for each SubItem
                        item.PackagingBOMId = bom.Id;                  // Set BOMId for each SubItem
                        db.PacSubitems.Add(item);
                    }
                }
                if (packagingcolors != null && packagingcolors.Count > 0)
                {
                    // Add new SubItems
                    foreach (var item in packagingcolors)
                    {
                        item.ParentProductId = bom.ProductId; // Set ParentProductId for each SubItem
                        item.BOMId = bom.Id;                  // Set BOMId for each SubItem
                        db.PackagingColors.Add(item);
                    }
                }
                db.SaveChanges(); // Commit the changes to the database
                return RedirectToAction("Index");
            }

            // Reinitialize view data for the dropdowns and other dependencies
            ViewBag.Suppliers = DAL.dbSuppliers;
            ViewBag.ProductList = new SelectList(db.Products, "Id", "Name", bom.ProductId);
            return View(bom);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //public JsonResult GetProductUnit(int id)
        //{
        //    var product = db.Products.FirstOrDefault(p => p.Id == id);
        //    if (product != null)
        //    {
        //        return Json(new { unit = product.Unit }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { unit = "N/A" }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetProductUnit(int id)
        //{
        //    var product = db.Products.FirstOrDefault(p => p.Id == id);

        //    if (product != null)
        //    {
        //        // Fetch the related product's name using VarProdParentId
        //        var variableProductName = db.Products
        //            .Where(p => p.Id == product.VarProdParentId)  // Find the product by ID
        //            .Select(p => p.Name)  // Select its Name
        //            .FirstOrDefault() ?? "N/A"; // If not found, return "N/A"

        //        return Json(new
        //        {
        //            unit = product.Unit,
        //            variableProductName = variableProductName // Return the product name instead of ID
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { unit = "N/A", variableProductName = "N/A" }, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetProductUnit(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                // Fetch the related product's details using VarProdParentId
                var variableProduct = db.Products
                    .Where(p => p.Id == product.VarProdParentId)  // Find the product by ID
                    .Select(p => new { p.Name, p.Unit })  // Select Name and Unit
                    .FirstOrDefault();

                return Json(new
                {
                    unit = product.Unit, // Unit of selected product
                    variableProductName = variableProduct?.Name ?? "N/A", // Name of the variable product
                    variableProductUnit = variableProduct?.Unit ?? "N/A" // Unit of the variable product
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { unit = "N/A", variableProductName = "N/A", variableProductUnit = "N/A" }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetProductDetails(int productId)
        {
            try
            {
                var productDetails = db.ProductDetails
                    .Where(pd => pd.ProductId == productId)
                    .Select(pd => new
                    {
                        pd.Id,
                        pd.Shape,
                        pd.Weight
                    }).ToList();

                if (!productDetails.Any())
                {
                    return Json(new { message = "No product details found" }, JsonRequestBehavior.AllowGet);
                }

                return Json(productDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log exception
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetQuantityToProduction(int productId)
        {
            try
            {
                var productDetails = db.QuantityToProduces
                    .Where(pd => pd.ProductId == productId)
                    .Select(pd => new
                    {
                        pd.Id,
                        pd.Shape,
                        pd.ProductionQty,
                        pd.CalculatedWeight
                    }).ToList();

                if (!productDetails.Any())
                {
                    return Json(new { message = "No product details found" }, JsonRequestBehavior.AllowGet);
                }

                return Json(productDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log exception
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        //public JsonResult GetVariableProduct(int productId)
        //{
        //    var variableProduct = db.Products // Change `Product` to `Products`
        //        .Where(p => p.VarProdParentId == productId)
        //        .Select(p => new { p.Id, p.Name, p.Unit})
        //        .FirstOrDefault();

        //    if (variableProduct != null)
        //    {
        //        return Json(variableProduct, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { error = "No variable product found" }, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetVariableProduct(int productId)
        {
            var variableProducts = db.Products // Change `Product` to `Products`
                .Where(p => p.VarProdParentId == productId)
                .Select(p => new { p.Id, p.Name, p.Unit })
                .ToList(); // Convert to List

            //if (variableProducts.Any()) // Check if there are any products
            //{
            return Json(variableProducts, JsonRequestBehavior.AllowGet);
            //}
            //return Json(new { error = "No variable products found" }, JsonRequestBehavior.AllowGet);
        }

    }
}