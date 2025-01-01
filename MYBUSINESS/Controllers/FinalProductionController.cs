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
    public class FinalProductionController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private FinalProductionViewModel finalProductionViewModel = new FinalProductionViewModel();
        // GET: Products
        public ActionResult Index()
        {

            var finalProduction = DAL.dbFinalProductions.Where(x => x.SubItems.Count() == 0).ToList();
            return View(finalProduction);
        }




        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(DAL.dbFinalProductions.Where(x => x.SubItems.Count() > 0).ToList());

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
        public ActionResult Create()
        {
            decimal maxId = (db.FinalProductions?.Max(p => p == null ? 0 : p.Id) ?? 0) + 1;

            ViewBag.SuggestedId = maxId;
            ViewBag.Suppliers = DAL.dbSuppliers;

            var productList = db.Products
                                .Select(p => new { p.Id, p.Name })
                                .ToList();

            ViewBag.ProductList = new SelectList(productList, "Id", "Name");

            var viewModel = new FinalProductionViewModel
            {
                FinalProduction = new FinalProduction(),
                Products = DAL.dbProducts ?? Enumerable.Empty<Product>().AsQueryable()
            };

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FinalProductionViewModel model,
            [Bind(Prefix = "FinalProduction", Include = "Id,Date,ProductName,Unit,QuantityToProduce")] FinalProduction finalProduction,
     [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        {
            if (ModelState.IsValid)
            {
                var selectedProduct = db.Products.FirstOrDefault(p => p.Id == model.FinalProduction.Id);

                if (selectedProduct != null)
                {
                    model.FinalProduction.ProductName = selectedProduct.Name;
                }

                model.FinalProduction.SubItems = new List<SubItem>();

                db.FinalProductions.Add(model.FinalProduction);

                foreach (var item in subItems) item.ParentProductId = finalProduction.Id;
                db.SubItems.AddRange(subItems);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Suppliers = DAL.dbSuppliers;

            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name");

            return View(model);
        }



        // GET: NewProductions/Edit/5
        public ActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Find the NewProduction entity by ID
            var finalProduction = db.FinalProductions
                                  .Include(np => np.Product) // Include related products if necessary
                                  .FirstOrDefault(np => np.Id == id);

            
            if (finalProduction == null)
            {
                return HttpNotFound();
            }

            // Populate ViewBag for dropdown list
            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name", finalProduction.ProductName);

            // Prepare the ViewModel
            var viewModel = new FinalProductionViewModel
            {
                FinalProduction = finalProduction,
                Products = db.Products,
                SubItems = db.SubItems.Where(x => x.ParentProductId == finalProduction.Id).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FinalProductionViewModel model,
     [Bind(Prefix = "NewProduction", Include = "Id,Date,ProductName,Unit,QuantityToProduce")] FinalProduction finalProduction,
     [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing NewProduction entity
                var existingProduction = db.FinalProductions.Find(model.FinalProduction.Id);

                if (existingProduction == null)
                {
                    return HttpNotFound();
                }

                // Update NewProduction fields
                existingProduction.Date = model.FinalProduction.Date;
                existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.FinalProduction.Id)?.Name;
                existingProduction.Unit = model.FinalProduction.Unit;
                existingProduction.QuantityToProduce = model.FinalProduction.QuantityToProduce;

                db.Entry(existingProduction).State = EntityState.Modified;

                // Delete existing SubItems
                //var delSubItems = db.SubItems.Where(x => x.ParentProductId == finalProduction.Id).ToList();
                //db.SubItems.RemoveRange(delSubItems);

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
            ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.FinalProduction.ProductName);

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