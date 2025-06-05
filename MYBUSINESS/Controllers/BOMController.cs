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
    public class BOMController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private BOMViewModel bomViewModel = new BOMViewModel();
        // GET: Products

        public ActionResult Index()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var bom = db.BOMs.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList().ToList();
            return View(bom);
            //return View(DAL.dbBOMs.Where(x => x.SubItems.Count() == 0).ToList());
        }
        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            var bom = db.BOMs.OrderByDescending(p => p.Id) // Sorting by Id in descending order
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
            decimal maxId = db.BOMs.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId; // Pass the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers; // Assuming this contains the supplier list

            // Get products without BOM
            var productsWithoutBOM = db.Products
                .Where(p => (p.PType == 4 || p.PType == 6) && !db.BOMs.Any(b => b.ProductId == p.Id)) // Filter out products with existing BOMs
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
            BOMViewModel bomViewModel = new BOMViewModel
            {
                BOM = new BOM
                {
                    Saleable = true,
                    Purchasable = true,
                    Manufacturable = true
                },
                SubItem = new List<SubItem> { new SubItem() }, // Initialize SubItem list with one SubItem object
                ProductType = new List<ProductType> { new ProductType() },
                ProductDetail = new List<ProductDetail> { new ProductDetail() }, // Initialize ProductDetail if needed
                Products = DAL.dbProducts // Assuming this contains product data
            };

            return View(bomViewModel); // Passing the ViewModel to the view
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
        public ActionResult Create([Bind(Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,ProductId,ProductName,CreateDate,UpdateDate,Unit")] BOM bom,
                                   [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit,ProductType")] List<SubItem> subItems)
        //[Bind(Prefix = "ProductType", Include = "Id,ParentProductId,ProductId,Quantity,BOMId,Unit")] List<ProductType> productTypes)

        //[Bind(Prefix = "ProductTypeDetail", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,ProductId")] List<ProductTypeDetail> productTypeDetails)
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
                db.BOMs.Add(bom);

                foreach (var item in subItems)
                {
                    item.ParentProductId = bom.ProductId;
                    item.BOMId = bom.Id;
                    db.SubItems.Add(item);
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

            BOM bom = db.BOMs.Find(id);
            if (bom == null)
            {
                return HttpNotFound();
            }

            // Fetch products without BOM (filtered by PType 4)
            var productsWithoutBOM = db.Products
                .Where(p => (p.PType == 4 || p.PType == 6) && (!db.BOMs.Any(b => b.ProductId == p.Id) || p.Id == bom.ProductId))
                .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
                .ToList();

            ViewBag.ProductList = new SelectList(productsWithoutBOM, "Value", "Text", bom.ProductId);

            // Fetch SubItems related to this BOM
            var subItemProducts = db.SubItems
                .Where(s => s.BOMId == bom.Id && s.ProductId != null)  // Filter out NULL ProductId
                .Include(s => s.Product) // Eager load related Product
                .Select(s => new { Value = s.ProductId.ToString(), Text = s.Product.Name  }) // Select Product Name
                .ToList();

            ViewBag.SubItemProductList = new SelectList(subItemProducts, "Value", "Text", bom.ProductId);

            // Fetch all products based on PType
            var ptypeProducts = db.Products
                .Where(p => p.PType == 1 || p.PType == 2 || p.PType == 3)
                .Select(p => new { Value = p.Id.ToString(), Text = p.Name })
                .ToList();

            ViewBag.PTypeProductList = new SelectList(ptypeProducts, "Value", "Text");

            // Pass the BOM model and related data
            var bomViewModel = new BOMViewModel
            {
                BOM = bom,
                Products = DAL.dbProducts,
                SubItem = db.SubItems.Where(x => x.BOMId == bom.Id).ToList(),
                ProductType = db.ProductTypes.Where(x => x.BOMId == bom.Id).ToList()
            };

            return View(bomViewModel);
        }



        //public ActionResult Edit(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    // Fetch the BOM record
        //    BOM bom = db.BOMs.Find(id);
        //    if (bom == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    // Fetch the list of products with manufacturable = true
        //    var products = db.Products
        //        .Where(p => p.Manufacturable == true)
        //        .Select(p => new
        //        {
        //            Value = p.Id.ToString(), // Ensure ID is a string
        //            Text = p.Name
        //        })
        //        .ToList();

        //    // Pass the products to ViewBag with bom.ProductId pre-selected
        //    ViewBag.ProductList = new SelectList(products, "Value", "Text", bom.ProductId.ToString());

        //    // Create and populate the BOMViewModel
        //    BOMViewModel bomViewModel = new BOMViewModel
        //    {
        //        BOM = bom,
        //        Products = DAL.dbProducts,
        //        SubItem = db.SubItems.Where(x => x.ParentProductId == bom.Id).ToList(),
        //        //ProductTypeDetail = db.ProductTypeDetails.Where(x => x.ProductId == bom.Id).ToList()
        //    };

        //    // Debugging information
        //    Debug.WriteLine($"Selected Product ID: {bom.ProductId}");
        //    Debug.WriteLine($"ViewBag.ProductList: {ViewBag.ProductList}");

        //    return View(bomViewModel);
        //}



        // POST: Products/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Prefix = "BOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,Unit,CreateDate,UpdateDate")] BOM bom,
        //[Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit")] List<SubItem> subItems)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        List<SubItem> delSubItems = db.SubItems.Where(x => x.ParentProductId == bom.Id).ToList();
        //        db.SubItems.RemoveRange(delSubItems);

        //        if (ModelState.IsValid)
        //        {
        //            db.Entry(bom).State = EntityState.Modified;


        //            foreach (var item in subItems)
        //            {
        //                item.ParentProductId = bom.Id;
        //                db.SubItems.Add(item);
        //            }

        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        ViewBag.Suppliers = DAL.dbSuppliers;
        //        return View(bom);
        //    }




        //ViewBag.Suppliers = DAL.dbSuppliers;
        //    return View(bomViewModel);
        //}

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Edit(
        //[Bind(Prefix = "BOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,Unit,CreateDate,UpdateDate")] BOM bom,
        //[Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit")] List<SubItem> subItems)
        ////[Bind(Prefix = "ProductTypeDetail", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,ProductId")] List<ProductTypeDetail> productTypeDetails)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // Remove existing SubItems for the BOM
        //            var existingSubItems = db.SubItems.Where(x => x.ParentProductId == bom.Id).ToList();
        //            //var existingProductTypeDetails = db.ProductTypeDetails.Where(x => x.ProductId == bom.Id).ToList();
        //            db.SubItems.RemoveRange(existingSubItems);
        //            //db.ProductTypeDetails.RemoveRange(existingProductTypeDetails);
        //            // Update the BOM record
        //            db.Entry(bom).State = EntityState.Modified;

        //            if (subItems != null)
        //            {
        //                foreach (var item in subItems)
        //                {
        //                    item.ParentProductId = bom.ProductId;
        //                    item.BOMId = bom.Id;
        //                    db.SubItems.Add(item);
        //                }
        //            }
        //            // Add the new SubItems


        //            //if (productTypeDetails != null)
        //            //{
        //            //    foreach (var item in productTypeDetails)
        //            //    {
        //            //        item.ProductId = bom.Id;
        //            //        db.ProductTypeDetails.Add(item);
        //            //    }
        //            //}


        //            // Save changes
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }

        //        // Reinitialize view data for the dropdowns and other dependencies
        //        ViewBag.Suppliers = DAL.dbSuppliers;
        //        ViewBag.ProductList = new SelectList(db.Products, "Id", "Name", bom.ProductId);
        //        return View(bom);
        //    }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
    [Bind(Prefix = "BOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,ProductName,ProductId,Unit,CreateDate,UpdateDate")] BOM bom,
    [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit")] List<SubItem> subItems,
    [Bind(Prefix = "ProductType", Include = "Id,ParentProductId,ProductId,Quantity,BOMId,Unit")] List<ProductType> productTypes)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.FirstOrDefault(p => p.Id == bom.ProductId);
                if (product != null)
                {
                    bom.ProductName = product.Name; // Assign the ProductName
                }
                // Remove existing SubItems for the BOM
                var existingSubItems = db.SubItems.Where(x => x.BOMId == bom.Id).ToList();
                var existingProductType = db.ProductTypes.Where(x => x.BOMId == bom.Id).ToList();
                db.SubItems.RemoveRange(existingSubItems);
                db.ProductTypes.RemoveRange(existingProductType);
                // Update the BOM record
                db.Entry(bom).State = EntityState.Modified;

                if (subItems != null && subItems.Count > 0)
                {
                    // Add new SubItems
                    foreach (var item in subItems)
                    {
                        item.ParentProductId = bom.ProductId; // Set ParentProductId for each SubItem
                        item.BOMId = bom.Id;                  // Set BOMId for each SubItem
                        db.SubItems.Add(item);
                    }
                }
                if (productTypes != null && productTypes.Count > 0)
                {
                    // Add new SubItems
                    foreach (var item in productTypes)
                    {
                        item.ParentProductId = bom.ProductId; // Set ParentProductId for each SubItem
                        item.BOMId = bom.Id;                  // Set BOMId for each SubItem
                        db.ProductTypes.Add(item);
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
