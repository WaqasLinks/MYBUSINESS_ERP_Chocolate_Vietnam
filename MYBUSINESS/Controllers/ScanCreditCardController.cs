using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
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
    [Authorize(Roles = "Admin,Accountant,Shop general manager,Shop")]
    public class ScanCreditCardController : Controller
    {
        // GET: ScanCreditCard
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private ScanCreditCardViewModel scancreditCardViewModel = new ScanCreditCardViewModel();
        // GET: Products

        [Authorize(Roles = "Admin,Accountant,Shop general manager")]
        public ActionResult Index()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var shopmanage = db.ScanCreditCards.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList().ToList();
            return View(shopmanage);
            //return View(DAL.dbBOMs.Where(x => x.SubItems.Count() == 0).ToList());
        }
        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var shopmanage = db.ScanCreditCards.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            return View(shopmanage);
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

        [Authorize(Roles = "Admin,Shop general manager,Shop")]
        public ActionResult Create()
        {
            // Get the next available BOM ID (if needed)
            decimal maxId = db.ScanCreditCards.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;
            maxId += 1;
            ViewBag.SuggestedId = maxId; // Pass the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers; // Assuming this contains the supplier list

            // Get products without BOM
            // Get all stores and show their Name
            var stores = db.Stores
                .Select(s => new { Value = s.Id.ToString(), Text = s.Name }) // Use Store Name instead of Product Name
                .ToList();

            // Assign to ViewBag for dropdown
            ViewBag.StoreList = new SelectList(stores, "Value", "Text");
            ScanCreditCardViewModel scancreditCardViewModel = new ScanCreditCardViewModel
            {
                ScanCreditCard = new ScanCreditCard(),
                // Initialize ProductDetail if needed
                Products = DAL.dbProducts // Assuming this contains product data
            };


            // Initialize the BOM and SubItem list in the ViewModel


            return View(scancreditCardViewModel); // Passing the ViewModel to the view
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,StoreId,Note,Date")] ScanCreditCard scancreditCard)
        {
            if (ModelState.IsValid)
            {
                db.ScanCreditCards.Add(scancreditCard);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // If we get here, there was a validation error - rebuild the view model
            var stores = db.Stores
                .Select(s => new { Value = s.Id.ToString(), Text = s.Name })
                .ToList();

            var viewModel = new ScanCreditCardViewModel
            {
                ScanCreditCard = scancreditCard,
                Products = DAL.dbProducts
            };

            ViewBag.StoreList = new SelectList(stores, "Value", "Text");
            return View(viewModel);
        }






        // GET: Products/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ScanCreditCard scancreditCard = db.ScanCreditCards.Find(id);
            if (scancreditCard == null)
            {
                return HttpNotFound();
            }

            var stores = db.Stores
        .Select(s => new { Value = s.Id.ToString(), Text = s.Name })
        .ToList();

            ViewBag.StoreList = new SelectList(stores, "Value", "Text", scancreditCard.StoreId.ToString());


            var scancreditCardViewModel = new ScanCreditCardViewModel
            {
                ScanCreditCard = scancreditCard,
                Products = DAL.dbProducts,

            };

            return View(scancreditCardViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,StoreId,Note,Date,Amount")] ScanCreditCard scancreditCard)
        {
            if (ModelState.IsValid)
            {
                // Update ScanBankDeposit
                db.Entry(scancreditCard).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Re-populate Store dropdown for the view
            var stores = db.Stores
                .Select(s => new { Value = s.Id.ToString(), Text = s.Name })
                .ToList();
            ViewBag.StoreList = new SelectList(stores, "Value", "Text", scancreditCard.StoreId.ToString());

            // Re-populate products if needed (for the ViewModel)
            var scancreditCardViewModel = new ScanCreditCardViewModel
            {
                ScanCreditCard = scancreditCard,
                Products = DAL.dbProducts
            };

            return View(scancreditCardViewModel);
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

            //if (variableProducts.Any()) // Check if there are any products
            //{
                return Json(variableProducts, JsonRequestBehavior.AllowGet);
            //}
            //return Json(new { error = "No variable products found" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public ActionResult UploadImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string uploadDir = Server.MapPath("~/UploadedImages");

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                string filePath = Path.Combine(uploadDir, fileName);
                file.SaveAs(filePath);

                string imageUrl = Url.Content("~/UploadedImages/" + fileName);
                return Content(imageUrl); // send back the image URL
            }

            return new HttpStatusCodeResult(400, "Upload failed");
        }
    }
}