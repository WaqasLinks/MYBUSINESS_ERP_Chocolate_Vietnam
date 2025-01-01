using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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

            return View(DAL.dbBOMs.Where(x => x.SubItems.Count() == 0).ToList());
        }
        public ActionResult IndexComponents()
        {
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(DAL.dbBOMs.Where(x => x.SubItems.Count() > 0).ToList());

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
            ViewBag.SuggestedId = maxId;  // Passing the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers;  // Assuming this contains the supplier list

            var products = db.Products
        .Where(p => p.Manufacturable == true)
        .Select(p => new
        {
            Value = p.Id.ToString(),  // ID of the product
            Text = p.Name            // Name of the product
        })
        .ToList();

            ViewBag.ProductList = new SelectList(products, "Value", "Text"); // Pass as a SelectList

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
                Products = DAL.dbProducts  // Assuming this contains product data
            };


            return View(bomViewModel);  // Passing the ViewModel to the view
        }





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
        public ActionResult Create([Bind(Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,ProductId,CreateDate,UpdateDate,Unit")] BOM bom,
                                   [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit")] List<SubItem> subItems)
        {
            if (ModelState.IsValid)
            {
                // Debugging output
                Console.WriteLine($"BOM Unit: {bom.Unit}");

                // Save the BOM and its SubItems
                db.BOMs.Add(bom);

                foreach (var item in subItems)
                {
                    item.ParentProductId = bom.Id;
                    db.SubItems.Add(item);
                }

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

            ViewBag.ProductList = new SelectList(
 db.Products,           // Source collection
 "Id",                  // Value field (ProductId)
 "Name",                // Display field (ProductName)
 bom.ProductId          // Selected value (current ProductId)
);


            var subItems = db.SubItems.Where(x => x.ParentProductId == bom.Id).ToList();
      
            bomViewModel.BOM = bom;
            bomViewModel.Products = DAL.dbProducts;
            bomViewModel.SubItem = db.SubItems.Where(x => x.ParentProductId == bom.Id).ToList();

            return View(bomViewModel);
            //return View(product);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
    [Bind(Prefix = "BOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,ProductionProcessCateogry,ProductionProcessDescription,Unit,CreateDate,UpdateDate")] BOM bom,
    [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit")] List<SubItem> subItems)
        {
            if (ModelState.IsValid)
            {
                // Remove existing SubItems for the BOM
                var existingSubItems = db.SubItems.Where(x => x.ParentProductId == bom.Id).ToList();
                db.SubItems.RemoveRange(existingSubItems);

                // Update the BOM record
                db.Entry(bom).State = EntityState.Modified;

                // Add the new SubItems
                foreach (var item in subItems)
                {
                    item.ParentProductId = bom.Id;
                    db.SubItems.Add(item);
                }

                // Save changes
                db.SaveChanges();
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
        public JsonResult GetProductUnit(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                return Json(new { unit = product.Unit }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { unit = "N/A" }, JsonRequestBehavior.AllowGet);
        }
       

    }
}
