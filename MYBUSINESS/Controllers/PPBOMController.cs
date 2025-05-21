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
    public class PPBOMController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private PPBOMViewModel ppBOMViewModel = new PPBOMViewModel();
        // GET: Products

        public ActionResult Index()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var ppbom = db.PPBOMs.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList().ToList();
            return View(ppbom);
            //return View(DAL.dbBOMs.Where(x => x.SubItems.Count() == 0).ToList());
        }
        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var ppbom = db.PPBOMs.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            return View(ppbom);
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

        public ActionResult Create()
        {
            // Get the next available BOM ID (if needed)
            decimal maxId = db.PPBOMs.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId; // Pass the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers; // Assuming this contains the supplier list

            // Get products without BOM
            var productsWithoutBOM = db.Products
                .Where(p => (p.PType == 8 || p.PType == 9) /*&& !db.PPBOMs.Any(b => b.ProductId == p.Id)*/) // Filter out products with existing BOMs
                .Select(p => new { Value = p.Id.ToString(), Text = p.Name }) // Prepare for SelectList
                .ToList();

            // Pass the filtered product list to the view
            ViewBag.ProductList = new SelectList(productsWithoutBOM, "Value", "Text");

            // Get color list from Colors table
            var colorList = db.Colors
                .Select(c => new { Value = c.Id.ToString(), Text = c.ColorName })
                .ToList();

            // Pass to ViewBag
            ViewBag.ColorList = new SelectList(colorList, "Value", "Text");



            var excessProducts = db.Products
        .Where(p => p.PType == 2)
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
            PPBOMViewModel ppBOMViewModel = new PPBOMViewModel
            {
                PPBOM = new PPBOM
                {
                    Saleable = true,
                    Purchasable = true,
                    Manufacturable = true
                },
                PPSubItem = new List<PPSubItem> { new PPSubItem() }, // Initialize SubItem list with one SubItem object
                ProductType = new List<ProductType> { new ProductType() },
                ProductDetail = new List<ProductDetail> { new ProductDetail() }, // Initialize ProductDetail if needed
                Products = DAL.dbProducts // Assuming this contains product data
            };

            return View(ppBOMViewModel); // Passing the ViewModel to the view
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "Id,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,CreateDate,UpdateDate,ProductionProcessCateogry,ProductionProcessDescription,ProductName,ProductId,Unit")] PPBOM ppbom,
    [Bind(Prefix = "PPSubItem", Include = "Id,ParentProductId,ProductId,Quantity,PPBOMId,Unit,AvailableInventory,QuantityRequested,QuantitytoPrepare,ProductType")] List<PPSubItem> ppsubItems)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.FirstOrDefault(p => p.Id == ppbom.ProductId);

                if (product != null)
                {
                    ppbom.ProductName = product.Name;
                }

                // First save the PPBOM
                db.PPBOMs.Add(ppbom);
                db.SaveChanges(); // Now ppbom.Id is generated

                // Then add the SubItems with the generated PPBOM Id
                foreach (var item in ppsubItems)
                {
                    item.ParentProductId = ppbom.ProductId;
                    item.PPBOMId = ppbom.Id;
                    db.PPSubItems.Add(item);
                }

                db.SaveChanges(); // Save the child records

                return RedirectToAction("Index");
            }

            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(ppbom);
        }







        // GET: Products/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PPBOM bom = db.PPBOMs.Find(id);
            if (bom == null)
            {
                return HttpNotFound();
            }

            // Fetch products without BOM (filtered by PType 4)
            var productsWithoutBOM = db.Products
                .Where(p => p.PType == 8 || p.PType == 9)
                .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
                .ToList();

            ViewBag.ProductList = new SelectList(productsWithoutBOM, "Value", "Text", bom.ProductId);

            // Fetch SubItems related to this BOM
            var subItemProducts = db.PPSubItems
                .Where(s => s.PPBOMId == bom.Id && s.ProductId != null)  // Filter out NULL ProductId
                .Include(s => s.Product) // Eager load related Product
                .Select(s => new { Value = s.ProductId.ToString(), Text = s.Product.Name }) // Select Product Name
                .ToList();

            ViewBag.SubItemProductList = new SelectList(subItemProducts, "Value", "Text", bom.ProductId);

            // Fetch all products based on PType
            var ptypeProducts = db.Products
                .Where(p => p.PType == 1 || p.PType == 2 || p.PType == 3)
                .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
                .ToList();

            ViewBag.PTypeProductList = new SelectList(ptypeProducts, "Value", "Text");

            // Pass the BOM model and related data
            var ppbomViewModel = new PPBOMViewModel
            {
                PPBOM = bom,
                Products = DAL.dbProducts,
                PPSubItem = db.PPSubItems.Where(x => x.PPBOMId == bom.Id).ToList(),
                ProductType = db.ProductTypes.Where(x => x.BOMId == bom.Id).ToList()
            };

            return View(ppbomViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
    [Bind(Prefix = "PPBOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,ProductName,ProductId,Unit,CreateDate,UpdateDate")] PPBOM bom,
    [Bind(Prefix = "PPSubItem", Include = "Id,ProductId,Quantity,Unit")] List<PPSubItem> subItems)
    
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.FirstOrDefault(p => p.Id == bom.ProductId);
                if (product != null)
                {
                    bom.ProductName = product.Name; // Assign the ProductName
                }
                // Remove existing SubItems for the BOM
                var existingSubItems = db.PPSubItems.Where(x => x.PPBOMId == bom.Id).ToList();
                
                db.PPSubItems.RemoveRange(existingSubItems);
                
                // Update the BOM record
                db.Entry(bom).State = EntityState.Modified;

                if (subItems != null && subItems.Count > 0)
                {
                    // Add new SubItems
                    foreach (var item in subItems)
                    {
                        item.ParentProductId = bom.ProductId; // Set ParentProductId for each SubItem
                        item.PPBOMId = bom.Id;                  // Set BOMId for each SubItem
                        db.PPSubItems.Add(item);
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
            PPBOM ppbom = db.PPBOMs.Find(id);
            if (ppbom == null)
            {
                return HttpNotFound();
            }
            return View(ppbom);
        }

        //// POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(decimal id)
        //{
        //    PPBOM ppbom = db.PPBOMs.Find(id);
        //    bool isPresent = false;
        //    if (db.PODs.FirstOrDefault(x => x.ProductId == id) != null || db.SODs.FirstOrDefault(x => x.ProductId == id) != null)
        //    {
        //        isPresent = true;
        //    }

        //    if (isPresent == false)
        //    {
        //        db.PPBOMs.Remove(ppbom);
        //    }
        //    else
        //    {
        //        ppbom.Status = "D";
        //        db.Entry(ppbom).Property(x => x.Status).IsModified = true;

        //    }
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            PPBOM ppbom = db.PPBOMs.Find(id);

            if (ppbom == null)
            {
                return HttpNotFound();
            }

            // First check if used in PODs or SODs
            bool isPresent = db.PODs.Any(x => x.ProductId == id) ||
                            db.SODs.Any(x => x.ProductId == id);

            if (!isPresent)
            {
                // Delete all related PPSubItems first
                var subItems = db.PPSubItems.Where(x => x.PPBOMId == id).ToList();
                db.PPSubItems.RemoveRange(subItems);

                // Then delete the PPBOM
                db.PPBOMs.Remove(ppbom);
            }
            else
            {
                ppbom.Status = "D";
                db.Entry(ppbom).Property(x => x.Status).IsModified = true;
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



        //[HttpGet]
        //public JsonResult GetProductDetails(int productId)
        //{
        //    var productDetails = db.ProductDetails
        //        .Where(pd => pd.ProductId == productId) // Filter by selected product
        //        .Select(pd => new
        //        {
        //            pd.Id,
        //            pd.Shape,
        //            pd.Weight
        //        }).ToList();

        //    return Json(productDetails, JsonRequestBehavior.AllowGet);
        //}

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

            if (variableProducts.Any()) // Check if there are any products
            {
                return Json(variableProducts, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = "No variable products found" }, JsonRequestBehavior.AllowGet);
        }


        //[HttpGet]
        //public JsonResult GetProductDetails(int productId)
        //{
        //    // Fetch all product details for the given ProductId
        //    var productDetails = db.ProductDetails
        //        .Where(p => p.ProductId == productId)
        //        .Select(p => new
        //        {
        //            p.Id,
        //            p.Shape,
        //            p.Weight
        //        })
        //        .ToList();

        //    // Return the data as JSON
        //    return Json(productDetails);
        //}


    }
}