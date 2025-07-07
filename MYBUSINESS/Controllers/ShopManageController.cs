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
    [Authorize(Roles = "Admin,Shop general manager,Shop")]
    public class ShopManageController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private ScanBankDepostViewModel scanbankDepostViewModel = new ScanBankDepostViewModel();
        // GET: Products


        [Authorize(Roles = "Admin,Shop general manager")]
        public ActionResult Index()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var shopmanage = db.ScanBankDeposits.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList().ToList();
            return View(shopmanage);
            //return View(DAL.dbBOMs.Where(x => x.SubItems.Count() == 0).ToList());
        }
        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var shopmanage = db.ScanBankDeposits.OrderByDescending(p => p.Id) // Sorting by Id in descending order
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
            decimal maxId = db.ScanBankDeposits.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;
            maxId += 1;
            ViewBag.SuggestedId = maxId; // Pass the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers; // Assuming this contains the supplier list

            // Get products without BOM
            // Get all stores and show their Name
            //var stores = db.Stores
            //    .Select(s => new { Value = s.Id.ToString(), Text = s.Name }) // Use Store Name instead of Product Name
            //    .ToList();

            //// Assign to ViewBag for dropdown
            //ViewBag.StoreList = new SelectList(stores, "Value", "Text");
            var storeData = (from s in db.Stores
                             join sm in db.ShopManages
                             on s.Id equals sm.ShoreId into storeGroup
                             from sg in storeGroup.OrderByDescending(x => x.Date).Take(1).DefaultIfEmpty()
                             select new
                             {
                                 StoreId = s.Id,
                                 StoreName = s.StoreShortName,
                                 Balance = sg != null ? sg.Balance : 0,
                                 Date = sg != null ? sg.Date : (DateTime?)null
                             }).ToList();

            // Step 2: Format the text in C# (outside the DB query)
            var stores = storeData.Select(s => new
            {
                Value = s.StoreId.ToString(),
                Text = s.StoreName +
                       (s.Date != null
                           ? $" - Balance: {s.Balance} - Date: {s.Date.Value.ToString("yyyy-MM-dd")}"
                           : " - No Balance Info")
            }).ToList();

            // Step 3: Assign to ViewBag for dropdown
            ViewBag.StoreList = new SelectList(stores, "Value", "Text"); ScanBankDepostViewModel scanBankDepostViewModel = new ScanBankDepostViewModel
            {
                ScanBankDeposit = DAL.dbScanBankDeposit.FirstOrDefault(),
                // Initialize ProductDetail if needed
                Products = DAL.dbProducts // Assuming this contains product data
            };


            // Initialize the BOM and SubItem list in the ViewModel


            return View(scanbankDepostViewModel); // Passing the ViewModel to the view
        }


        // public ActionResult Create()
        // {
        //     // Get the next available BOM ID (if needed)
        //     decimal maxId = db.BOMs.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
        //     maxId += 1;
        //     ViewBag.SuggestedId = maxId;  // Passing the suggested BOM ID to the view
        //     ViewBag.Suppliers = DAL.dbSuppliers;  // Assuming this contains the supplier list


        //     var productsWithoutBOM = db.Products
        //.Where(p => p.Manufacturable == true && !db.BOMs.Any(b => b.ProductId == p.Id))
        //.Select(p => new { Value = p.Id.ToString(), Text = p.Name })
        //.ToList();

        //     // Pass to the view
        //     ViewBag.ProductList = new SelectList(productsWithoutBOM, "Value", "Text");
        //     //    var products = db.Products
        //     //.Where(p => p.Manufacturable == true)
        //     //.Select(p => new
        //     //{
        //     //    Value = p.Id.ToString(),  // ID of the product
        //     //    Text = p.Name            // Name of the product
        //     //})
        //     //.ToList();

        //     //    ViewBag.ProductList = new SelectList(products, "Value", "Text"); // Pass as a SelectList

        //     var productDetails = db.ProductDetails.ToList();

        //     var productList = db.Products
        //                            .Select(p => new
        //                            {
        //                                p.Id,
        //                                p.Name,
        //                                p.Stock,
        //                                p.Remarks, // Fetch remarks from BOM
        //                                p.Category,   // Fetch stock for AvailableInventory
        //                            })
        //                            .ToList();

        //     ViewBag.ProductList = new SelectList(productList, "Id", "Name");


        //     // Initialize the BOM and SubItem list in the ViewModel
        //     BOMViewModel bomViewModel = new BOMViewModel
        //     {
        //         BOM = new BOM
        //         {
        //             Saleable = true,
        //             Purchasable = true,
        //             Manufacturable = true
        //         },
        //         SubItem = new List<SubItem> { new SubItem() }, // Initialize SubItem list with one SubItem object
        //         //ProductTypeDetail = new List<ProductTypeDetail> { new ProductTypeDetail() },
        //         ProductDetail = new List<ProductDetail> { new ProductDetail() },
        //         Products = DAL.dbProducts, // Assuming this contains product data
        //         //ProductList = new SelectList(products, "Id", "Name"),
        //         //ProductDetail = new List<ProductDetail>()
        //     };


        //     return View(bomViewModel);  // Passing the ViewModel to the view


        // }





        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //public ActionResult Create([Bind(Prefix = "Customer", Include = "Name,Address")] Customer Customer, [Bind(Prefix = "SaleOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,CustomerId,Remarks,Remarks2,PaymentMethod,PaymentDetail,SaleReturn,BankAccountId,Date")] SO sO, [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,SalePrice,Quantity,SaleType,PerPack,IsPack,Product")] List<SOD> sOD, FormCollection collection)

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Prefix = "BOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,CreateDate,UpdateDate")] BOM bom,
        //[Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit")] List<SubItem> subItems)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Save the BOM and its SubItems
        //        db.BOMs.Add(bom);

        //        foreach (var item in subItems)
        //        {
        //            item.ParentProductId = bom.Id;
        //            db.SubItems.Add(item); // Save the subitem, including the Unit value
        //        }
        //        db.SaveChanges();

        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Suppliers = DAL.dbSuppliers;  // if needed, populate this for the Create view
        //    return View(bom);  // return BOM if model state is not valid
        //}

        //       [HttpPost]
        //       [ValidateAntiForgeryToken]
        //       public ActionResult Create([Bind(Prefix = "BOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,ProductId,CreateDate,UpdateDate")] BOM bom,
        //[Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit")] List<SubItem> subItems)
        //       {
        //           if (ModelState.IsValid)
        //           {
        //               // Debugging output
        //               foreach (var item in subItems)
        //               {
        //                   Console.WriteLine($"ProductId: {item.ProductId}, Unit: {item.Unit}, Quantity: {item.Quantity}");
        //               }

        //               // Save the BOM and its SubItems
        //               db.BOMs.Add(bom);

        //               foreach (var item in subItems)
        //               {
        //                   item.ParentProductId = bom.Id;
        //                   db.SubItems.Add(item); // Save the subitem, including the Unit value
        //               }
        //               db.SaveChanges();

        //               return RedirectToAction("Index");
        //           }

        //           ViewBag.Suppliers = DAL.dbSuppliers;
        //           return View(bom);
        //       }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // This disables request validation for this action
        public ActionResult Create([Bind(Include = "Id,StoreId,Note")] ScanBankDeposit scanBankDeposit)

        {
            if (ModelState.IsValid)
            {

                var shop = db.ScanBankDeposits.FirstOrDefault(p => p.Id == scanBankDeposit.StoreId);


                db.ScanBankDeposits.Add(scanBankDeposit);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(scanBankDeposit);
        }






        // GET: Products/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ScanBankDeposit scanBankDeposit = db.ScanBankDeposits.Find(id);
            if (scanBankDeposit == null)
            {
                return HttpNotFound();
            }

            var stores = db.Stores
        .Select(s => new { Value = s.Id.ToString(), Text = s.Name })
        .ToList();

            ViewBag.StoreList = new SelectList(stores, "Value", "Text", scanBankDeposit.StoreId.ToString());


            var scanbankDepostViewModel = new ScanBankDepostViewModel
            {
               ScanBankDeposit  = scanBankDeposit,
                Products = DAL.dbProducts,
                
            };

            return View(scanbankDepostViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,StoreId,Note")] ScanBankDeposit scanBankDeposit)
        {
            if (ModelState.IsValid)
            {
                // Update ScanBankDeposit
                db.Entry(scanBankDeposit).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Re-populate Store dropdown for the view
            var stores = db.Stores
                .Select(s => new { Value = s.Id.ToString(), Text = s.Name })
                .ToList();
            ViewBag.StoreList = new SelectList(stores, "Value", "Text", scanBankDeposit.StoreId.ToString());

            // Re-populate products if needed (for the ViewModel)
            var scanbankDepostViewModel = new ScanBankDepostViewModel
            {
                ScanBankDeposit = scanBankDeposit,
                Products = DAL.dbProducts
            };

            return View(scanbankDepostViewModel);
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