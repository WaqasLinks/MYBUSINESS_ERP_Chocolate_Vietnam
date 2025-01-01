using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

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
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(DAL.dbNewProductions.Where(x => x.SubItems.Count() == 0).ToList());
        }
        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(DAL.dbNewProductions.Where(x => x.SubItems.Count() > 0).ToList());

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
                    SubItems = subItems
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
        public ActionResult Create(NewProductionViewModel model,
     [Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,QuantityToProduce")] NewProduction newProduction,
     [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItem> subItems)
        {
            if (ModelState.IsValid)
            {
                // Set QuantityToProduce explicitly if needed
                newProduction.QuantityToProduce = model.NewProduction.QuantityToProduce;
            
                foreach (var item in subItems)
                {
                    var selectedProduct = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (selectedProduct != null)
                    {
                        
                        //item.QuantityRequested = item.QuantityRequested ?? 0; // Handle null values
                        item.AvailableInventory = selectedProduct.Stock;
                        item.QuantitytoPrepare = item.QuantitytoPrepare ?? 0;/*Math.Max(0m, item.QuantityRequested.GetValueOrDefault() - item.AvailableInventory.GetValueOrDefault());*/
                    }

                    // Assign ParentProductId
                    item.ParentProductId = newProduction.Id;
                }

                model.NewProduction.SubItems = subItems;

                db.NewProductions.Add(newProduction);
                db.SubItems.AddRange(subItems);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

        

        ViewBag.Suppliers = DAL.dbSuppliers;

            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name");

            return View(newProduction);
        }



        public ActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newProduction = db.NewProductions
                                  .Include(np => np.Products) // Include related Products
                                  .FirstOrDefault(np => np.Id == id);

            if (newProduction == null)
            {
                return HttpNotFound();
            }

            // Fetch SubItems associated with this NewProduction
            var subItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList();

            // Prepare ViewModel
            var viewModel = new NewProductionViewModel
            {
                NewProduction = newProduction,
                Products = db.Products, // Include all products
                SubItems = subItems // Include fetched SubItems
            };

            // Populate ViewBag with Product List
            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name", newProduction.ProductName);

            return View(viewModel);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NewProductionViewModel model,
     [Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,QuantityToProduce")] NewProduction newProduction,
     [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing NewProduction entity
                var existingProduction = db.NewProductions.Find(model.NewProduction.Id);

                if (existingProduction == null)
                {
                    return HttpNotFound();
                }

                // Update NewProduction fields
                existingProduction.ProductionDate = model.NewProduction.ProductionDate;
                existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.NewProduction.Id)?.Name;
                existingProduction.Unit = model.NewProduction.Unit;
                existingProduction.QuantityToProduce = model.NewProduction.QuantityToProduce;

                db.Entry(existingProduction).State = EntityState.Modified;

                // Delete existing SubItems
                var delSubItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList();
                db.SubItems.RemoveRange(delSubItems);

                // Add new SubItems
                foreach (var item in subItems)
                {
                    item.ParentProductId = existingProduction.Id;
                }
                db.SubItems.AddRange(subItems);

                // Save changes to the database
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Repopulate ViewBag on failure
            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.NewProduction.ProductName);

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