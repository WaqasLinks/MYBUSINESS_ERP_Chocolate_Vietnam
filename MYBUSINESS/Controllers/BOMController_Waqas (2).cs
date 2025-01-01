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
            //int maxId = db.Products.Max(p => p.Id);
            decimal maxId = db.BOMs.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;
            ViewBag.Suppliers = DAL.dbSuppliers;
            BOM bom = new BOM();


            bom.Saleable = true;
            bom.Purchasable = true;
            bom.Manufacturable = true;
            bomViewModel.BOM = bom;
            bomViewModel.Products = DAL.dbProducts;
            return View(bomViewModel);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //public ActionResult Create([Bind(Prefix = "Customer", Include = "Name,Address")] Customer Customer, [Bind(Prefix = "SaleOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,CustomerId,Remarks,Remarks2,PaymentMethod,PaymentDetail,SaleReturn,BankAccountId,Date")] SO sO, [Bind(Prefix = "SaleOrderDetail", Include = "ProductId,SalePrice,Quantity,SaleType,PerPack,IsPack,Product")] List<SOD> sOD, FormCollection collection)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "BOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,CreateDate,UpdateDate")] BOM bom, [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        {

            if (ModelState.IsValid)
            {
                db.BOMs.Add(bom);


                foreach (var item in subItems) item.ParentProductId = bom.Id;
                db.SubItems.AddRange(subItems);


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


            bomViewModel.BOM = bom;
            bomViewModel.Products = DAL.dbProducts;
            bomViewModel.SubItems = db.SubItems.Where(x => x.ParentProductId == bom.Id).ToList();

            return View(bomViewModel);
            //return View(product);
        }
        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Prefix = "BOM", Include = "Id,Name,Remarks,Saleable,Purchasable,Manufacturable,ShelfLife,TimeUnit,CreateDate,UpdateDate")] BOM bom, [Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity")] List<SubItem> subItems)
        //public ActionResult Edit([Bind(Include = "Id,Name,PurchasePrice,SalePrice,WholeSalePrice,Stock,Saleable,PerPack,IsService,ShowIn,BarCode")] Product product)
        {

            //del old subitems
            List<SubItem> delSubItems = db.SubItems.Where(x => x.ParentProductId == bom.Id).ToList();

            db.SubItems.RemoveRange(delSubItems);

            if (ModelState.IsValid)
            {
                db.Entry(bom).State = EntityState.Modified;


                foreach (var item in subItems) item.ParentProductId = bom.Id;
                db.SubItems.AddRange(subItems);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Suppliers = DAL.dbSuppliers;
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
    }
}
