using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    [Authorize(Roles = "Admin,Manager,User")]
    public class InventoryController : Controller
    {
        private BusinessContext db = new BusinessContext();


        public ActionResult Index1()
        {
            
            var inventory = db.Inventories.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList().ToList();
            return View(inventory);
            //return View(DAL.dbBOMs.Where(x => x.SubItems.Count() == 0).ToList());
        }
        // GET: Inventory
        //public ActionResult Index()
        //{
        //    var productsData = db.Products.ToList(); // filter if needed

        //    var products = productsData.Select(p => new ProductInventoryItem
        //    {
        //        Id = (int)p.Id,
        //        Name = p.Name,
        //        CurrentStock = p.Stock ?? 0
        //    }).ToList();

        //    var viewModel = new InventoryViewModel
        //    {
        //        Products = products
        //    };

        //    ViewBag.ProductList = new SelectList(db.Products, "Id", "Name");

        //    return View(viewModel);
        //}

        public ActionResult Index()
        {
            // Create a dictionary for product types
            var productTypes = new Dictionary<int, string>
        {
            {0, "All Products"},
            {1, "Variable Product"},
            {2, "Excess Product"},
            {3, "ByProduct"},
            {4, "Finished Product"},
            {5, "Ingredient"},
            {6, "Intermediary Ingredient"},
            {7, "Merchandise"},
            {8, "Flat Packaging"},
            {9, "Finish Packaging"}
        };

            // Get all products initially
            var productsData = db.Products.ToList();

            var products = productsData.Select(p => new ProductInventoryItem
            {
                Id = (int)p.Id,
                Name = p.Name,
                CurrentStock = p.Stock ?? 0,
                ProductType = p.PType ?? 0
            }).ToList();

            var viewModel = new InventoryViewModel
            {
                Products = products,
                ProductTypes = new SelectList(productTypes, "Key", "Value")
            };

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult FilterByProductType(int productType)
        {
            var productsData = db.Products.AsQueryable();

            if (productType > 0) // if not "All Products"
            {
                productsData = productsData.Where(p => p.PType == productType);
            }

            var products = productsData.ToList().Select(p => new
            {
                Id = (int)p.Id,
                Name = p.Name,
                CurrentStock = p.Stock ?? 0,
                ProductType = p.PType ?? 0
            }).ToList();

            return Json(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Save(ProductInventoryItem item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = db.Products.Find(item.Id);
                    if (product != null)
                    {
                        product.Stock = item.PhysicalQuantity;
                        product.UpdateDate = DateTime.Now;

                        var inventory = new Inventory
                        {
                            ProductId = item.Id,
                            CurrentStock = (int?)item.CurrentStock,
                            PhysicalQuantity = (int?)item.PhysicalQuantity,
                            Determination = (int?)item.Determination
                        };

                        db.Inventories.Add(inventory);
                        db.SaveChanges();
                    }

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
            }

            return Json(new { success = false, error = "Invalid model state" });
        }

        // ... rest of your existing code ...







        // POST: Inventory/Save
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Save(ProductInventoryItem item)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var product = await db.Products.FindAsync(item.Id);
        //        if (product != null)
        //        {
        //            product.Stock = item.PhysicalQuantity;
        //            product.UpdateDate = DateTime.Now;

        //            var inventory = new Inventory
        //            {
        //                ProductId = item.Id,
        //                CurrentStock = (int?)item.CurrentStock,
        //                PhysicalQuantity = (int?)item.PhysicalQuantity,
        //                Determination = (int?)item.Determination
        //            };

        //            db.Inventories.Add(inventory);
        //            await db.SaveChangesAsync();
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View("Index");
        //}



        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var inventory = await db.Inventories.FindAsync(id);

            if (inventory == null)
            {
                return HttpNotFound();
            }

            // Get the product associated with this inventory
            var product = await db.Products.FindAsync(inventory.ProductId);

            // Create view model
            var viewModel = new InventoryViewModel
            {
                Products = new List<ProductInventoryItem>
        {
            new ProductInventoryItem
            {
                Id = (int) inventory.ProductId,
                Name = product?.Name ?? "Unknown Product",
                CurrentStock = inventory.CurrentStock ?? 0,
                PhysicalQuantity = inventory.PhysicalQuantity ?? 0,
                Determination = inventory.Determination ?? 0
            }
        }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(InventoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Since we're only editing one product in this view
                var item = model.Products.FirstOrDefault();

                if (item != null)
                {
                    // Update the product stock
                    var product = await db.Products.FindAsync(item.Id);
                    if (product != null)
                    {
                        product.Stock = item.PhysicalQuantity;
                        product.UpdateDate = DateTime.Now;
                    }

                    // Find the inventory record to update
                    var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.ProductId == item.Id);
                    if (inventory != null)
                    {
                        inventory.CurrentStock = (int)item.CurrentStock;
                        inventory.PhysicalQuantity = (int)item.PhysicalQuantity;
                        inventory.Determination = (int)(item.PhysicalQuantity - item.CurrentStock);

                        db.Entry(inventory).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index1");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public JsonResult GetProductsByPType(int ptype)
        {
            var products = db.Products
                            .Where(p => p.PType == ptype)
                            .Select(p => new
                            {
                                Id = p.Id,
                                Name = p.Name
                            }).ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
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