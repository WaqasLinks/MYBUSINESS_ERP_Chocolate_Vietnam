using System.Collections.Generic;
using System.Web.Mvc;
using MYBUSINESS.Models;
using System.Configuration;
using MYBUSINESS.CustomClasses;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System;

namespace MYBUSINESS.Controllers
{
    public class ColorController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private ColorViewModel colorViewModel = new ColorViewModel();
        // GET: Products

        public ActionResult Index()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var bom = db.Colors.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList().ToList();
            return View(bom);
            //return View(DAL.dbBOMs.Where(x => x.SubItems.Count() == 0).ToList());
        }
        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var bom = db.Colors.OrderByDescending(p => p.Id) // Sorting by Id in descending order
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

        public ActionResult Create()
        {
            // Get the next available BOM ID (if needed)
            decimal maxId = db.Colors.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId; // Pass the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers; // Assuming this contains the supplier list

            // Get products without BOM

            ColorViewModel colorViewModel = new ColorViewModel
            {
                Color = new Color()
                              
            };

            return View(colorViewModel); // Passing the ViewModel to the view
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ColorName,ColorCode")] Color color)
                                   
        {
            if (ModelState.IsValid)
            {

                db.Colors.Add(color);

                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(color);
        }






        // GET: Products/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Color color = db.Colors.Find(id);
            if (color == null)
            {
                return HttpNotFound();
            }

            // Pass the BOM model and related data
            var colorViewModel = new ColorViewModel
            {
                Color = color,
                         };

            return View(colorViewModel);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Prefix = "Color", Include = "Id,ColorName,ColorCode")] Color color)
        {
            if (ModelState.IsValid)
            {
                db.Entry(color).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(color);
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
