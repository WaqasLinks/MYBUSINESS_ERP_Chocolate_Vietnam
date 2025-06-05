using Microsoft.AspNetCore.Mvc;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms; // For LocalReport, ReportDataSource, Warning
using System.Data.SqlClient;
using System.Windows.Media;
//using MyColor = MYBUSINESS.Models.Color;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace MYBUSINESS.Controllers
{
    public class PackagingProductionController : Controller
    {
        // GET: PackagingProduction
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private PackagingProductionViewModel packagingproductionViewModel = new PackagingProductionViewModel();
        // GET: Products
        public ActionResult Index()
        {
            var packagingProductions = db.PackagingProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(packagingProductions);
        }
        public ActionResult ColorIndex()
        {
            var packagingProductions = db.PackagingProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(packagingProductions);
        }

        public ActionResult ColorIndex4()
        {
            var packagingProductions = db.PackagingProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;

            return View(packagingProductions);
        }

        public ActionResult IndexComponents()
        {
            var packagingProductions = db.PackagingProductions
                           .OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            //var newProductions = db.NewProductions.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(packagingProductions);

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
        //public ActionResult Index1(int? productId)
        //{
        //    var query = from pc in db.PaperColors
        //                join pp in db.PackagingProductions on pc.PackagingProductionId equals pp.Id
        //                join bom in db.PPSubItems
        //                    on pc.ProductId equals bom.ParentProductId
        //                where bom.Quantity != null
        //                select new PackagingProductionViewModel
        //                {
        //                    PaperColorId = pc.Id,
        //                    ColorName = pc.Color,
        //                    ColorQuantity = (int)(pc.Quantity ?? 0m),
        //                    BOMQuantity = (int)(bom.Quantity ?? 0m),
        //                    PlannerQuantity = (int)((pc.Quantity ?? 0m) * (bom.Quantity ?? 0m)),
        //                    ProductId = (int)(pc.ProductId ?? 0),
        //                    SubItemId = (int)bom.Id
        //                };

        //    if (productId.HasValue)
        //    {
        //        query = query.Where(x => x.ProductId == productId.Value);
        //    }


        //    var model = query.ToList();
        //    return View(model);
        //}
        //public ActionResult Index1(int? productId)
        //{
        //    var query = from pc in db.PaperColors
        //                join pp in db.PackagingProductions on pc.PackagingProductionId equals pp.Id
        //                join bom in db.PPSubItems on pc.ProductId equals bom.ParentProductId
        //                where bom.Quantity != null
        //                join receipt in db.PPColorReceipts
        //                    on new { pcId = pc.Id, subItemId = bom.Id }
        //                    equals new { pcId = receipt.PaperColorId ?? 0, subItemId = receipt.PPSubItemId ?? 0 }
        //                    into receiptGroup
        //                from r in receiptGroup.DefaultIfEmpty()
        //                select new PackagingProductionViewModel
        //                {
        //                    PaperColorId = pc.Id,
        //                    ColorName = pc.Color,
        //                    // Explicitly convert nullable decimal to int using decimal? ?? 0m
        //                    ColorQuantity = (int)(pc.Quantity ?? 0m),  // Convert nullable decimal to int
        //                    BOMQuantity = (int)(bom.Quantity ?? 0m),   // Same for BOMQuantity
        //                    PlannerQuantity = (int)((pc.Quantity ?? 0m) * (bom.Quantity ?? 0m)),  // Handle nullable decimal multiplication
        //                    ProductId = (int)(pc.ProductId ?? 0),       // Handle nullable ProductId as int
        //                    SubItemId = (int)(bom.Id),             // Ensure bom.Id is cast to int
        //                    QuantityReceived = r != null ? (r.QuantityReceived ?? 0) : 0, // Handle nullable ints
        //                    ToReceived = r != null ? (r.ToReceived ?? 0) : 0, // Handle nullable ints
        //                    //ReceivedDate = r != null && r.ReceivedDate.HasValue ? r.ReceivedDate.Value.ToString("yyyy-MM-dd") : ""
        //                    ReceivedDate = r.ReceivedDate,

        //                };

        //    if (productId.HasValue)
        //    {
        //        query = query.Where(x => x.ProductId == productId.Value);
        //    }

        //    var model = query.ToList();
        //    return View(model);
        //}
        public ActionResult Index1(int? productId)
        {
            var query = from pc in db.PaperColors
                        join pp in db.PackagingProductions on pc.PackagingProductionId equals pp.Id
                        join bom in db.PPSubItems on pc.ProductId equals bom.ParentProductId
                        where bom.Quantity != null
                        join receipt in db.PPColorReceipts
                            on new { pcId = pc.Id, subItemId = bom.Id }
                            equals new
                            {
                                pcId = receipt.PaperColorId ?? 0,
                                subItemId = receipt.PPSubItemId ?? 0
                            }
                            into receiptGroup
                        from r in receiptGroup.DefaultIfEmpty()
                        select new PackagingProductionViewModel
                        {
                            PaperColorId = pc.Id,
                            ColorName = pc.Color,

                            // Convert nullable decimal to int
                            ColorQuantity = (int)(pc.Quantity ?? 0m),
                            BOMQuantity = (int)(bom.Quantity ?? 0m),

                            // Calculate Planner Quantity
                            PlannerQuantity = (int)((pc.Quantity ?? 0m) * (bom.Quantity ?? 0m)),

                            ProductId = (int)(pc.ProductId ?? 0),
                            SubItemId = (int)bom.Id,

                            // Handle nulls in QuantityReceived and ToReceived
                            QuantityReceived = r != null ? (r.QuantityReceived ?? 0) : 0,
                            ToReceived = r != null ? (r.ToReceived ?? 0) : 0,

                            // Use nullable DateTime directly
                            ReceivedDate = r.ReceivedDate,
                        };

            // Filter by productId if provided
            if (productId.HasValue)
            {
                query = query.Where(x => x.ProductId == productId.Value);
            }

            var model = query.ToList();
            return View(model);
        }
        //public ActionResult Index3(int? productId)
        public ActionResult Index3(int? packagingProductionId)

        {
            var query = from pc in db.PaperColors
                        join pp in db.PackagingProductions on pc.PackagingProductionId equals pp.Id
                        join bom in db.PPSubItems on pc.ProductId equals bom.ParentProductId
                        join product in db.Products on pc.ProductId equals product.Id // Add this join
                        where bom.Quantity != null
                        join receipt in db.PPColorReceipts
                            on new { pcId = pc.Id, subItemId = bom.Id }
                            equals new
                            {
                                pcId = receipt.PaperColorId ?? 0,
                                subItemId = receipt.PPSubItemId ?? 0
                            }
                            into receiptGroup
                        from r in receiptGroup.DefaultIfEmpty()
                        select new PackagingProductionViewModel
                        {
                            PaperColorId = pc.Id,
                            ColorName = pc.Color,

                            ColorQuantity = (int)(pc.Quantity ?? 0m),
                            BOMQuantity = (int)(bom.Quantity ?? 0m),
                            PlannerQuantity = (int)((pc.Quantity ?? 0m) * (bom.Quantity ?? 0m)),
                            ProductName = product.Name,
                            ProductId = (int)(pc.ProductId ?? 0),
                            SubItemId = (int)bom.Id,
                            PPSubItemId = (decimal)bom.Id, // ✅ THIS LINE ADDED
                            PackagingProductionId = pp.Id,

                            QuantityReceived = r != null ? (r.QuantityReceived ?? 0) : 0,
                            ToReceived = r != null ? (r.ToReceived ?? 0) : 0,
                            ReceivedDate = r.ReceivedDate,
                        };

            if (packagingProductionId.HasValue)
            {
                query = query.Where(x => x.PaperColorId != 0 && db.PaperColors
                    .Any(p => p.Id == x.PaperColorId && p.PackagingProductionId == packagingProductionId.Value));
            }

            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var startDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            ViewBag.StartDate = startDate;
            
            ViewBag.Colors = db.PaperColors.ToList(); // if PaperColor already matches the Color model

            var model = query.ToList();
            return View(model);
        }

        //   [HttpPost]

        //   [ValidateAntiForgeryToken]
        //   public ActionResult CreateColor(  
        //[Bind(Prefix = "CPReceipts", Include = "Id,PaperColorId,PPSubItemId,ReceivedDate,QuantityReceived,ToReceived")] List<CPReceipt> cpreceipts)
        //   {
        //       if (ModelState.IsValid)
        //       {
        //           db.CPReceipts.AddRange(cpreceipts);

        //           db.SaveChanges();
        //           return RedirectToAction("Index");

        //       }



        //       ViewBag.Suppliers = DAL.dbSuppliers;

        //       var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
        //       ViewBag.ProductList = new SelectList(productList, "Id", "Name");

        //       return View(packagingproductionViewModel);
        //   }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateColor(List<PPColorReceiptViewModel> cpreceipts)
        //{
        //    if (cpreceipts == null || !cpreceipts.Any())
        //    {
        //        return Json(new { success = false, message = "No data received" });
        //    }

        //    try
        //    {
        //        foreach (var item in cpreceipts)
        //        {
        //            var receipt = new CPReceipt
        //            {
        //                PaperColorId = item.PaperColorId,
        //                PPSubItemId = item.PPSubItemId,
        //                QuantityReceived = item.QuantityReceived,
        //                ToReceived = item.ToReceive,
        //                // If you are using ReceivedDate, uncomment and adjust the parsing logic
        //                // ReceivedDate = DateTime.TryParse(item.ReceivedDate, out var parsedDate)
        //                //                 ? parsedDate
        //                //                 : DateTime.Now
        //            };

        //            db.CPReceipts.Add(receipt);
        //        }

        //        db.SaveChanges();
        //        return Json(new { success = true, message = "Saved successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Error: {ex.Message}" });
        //    }
        //}
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateColor(List<PPColorReceiptViewModel> cpreceipts)
        {
            if (cpreceipts == null || !cpreceipts.Any())
            {
                return Json(new { success = false, message = "No data received" });
            }

            try
            {
                foreach (var item in cpreceipts)
                {
                    DateTime receivedDate;
                    if (!DateTime.TryParse(item.ReceivedDate, out receivedDate))
                    {
                        // Handle invalid date string (you can skip or assign a default value)
                        return Json(new { success = false, message = "Invalid date format" });
                    }


                    var receipt = new CPReceipt
                    {
                        PaperColorId = item.PaperColorId,
                        PPSubItemId = (int)item.PPSubItemId, // Assuming PPSubItemId is int in CPReceipt
                        QuantityReceived = item.QuantityReceived,
                        ReceivedDate = receivedDate,
                        // Uncomment below line only if your model supports it
                        // PlannerQuantity = item.PlannerQuantity,
                        ToReceived = item.ToReceive,
                        PackagingProductionId = item.PackagingProductionId
                    };

                    db.CPReceipts.Add(receipt);
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult Index2(int? productId)
        {
            var query = from pc in db.PaperColors
                        join pp in db.PackagingProductions on pc.PackagingProductionId equals pp.Id
                        join bom in db.PPSubItems
                            on pc.ProductId equals bom.ParentProductId
                        where bom.Quantity != null
                        select new PackagingProductionViewModel
                        {
                            PaperColorId = pc.Id,
                            ColorName = pc.Color,
                            ColorQuantity = (int)(pc.Quantity ?? 0m),
                            BOMQuantity = (int)(bom.Quantity ?? 0m),
                            PlannerQuantity = (int)((pc.Quantity ?? 0m) * (bom.Quantity ?? 0m)),
                            ProductId = (int)(pc.ProductId ?? 0),
                            SubItemId = (int)bom.Id
                        };

            if (productId.HasValue)
            {
                query = query.Where(x => x.ProductId == productId.Value);
            }


            var model = query.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create1(List<PPColorReceipt> PPColorReceipt)
        {
            if (ModelState.IsValid)
            {
                db.PPColorReceipts.AddRange(PPColorReceipt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Suppliers = DAL.dbSuppliers;

            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name");

            return View();
        }

        //public ActionResult EditCreateColor(int? packagingProductionId)
        //{
        //    var query = from pc in db.PaperColors
        //                join pp in db.PackagingProductions on pc.PackagingProductionId equals pp.Id
        //                join bom in db.PPSubItems on pc.ProductId equals bom.ParentProductId
        //                where bom.Quantity != null
        //                join receipt in db.PPColorReceipts
        //                    on new { pcId = pc.Id, subItemId = bom.Id }
        //                    equals new
        //                    {
        //                        pcId = receipt.PaperColorId ?? 0,
        //                        subItemId = receipt.PPSubItemId ?? 0
        //                    }
        //                    into receiptGroup
        //                from r in receiptGroup.DefaultIfEmpty()
        //                select new PackagingProductionViewModel
        //                {
        //                    PaperColorId = pc.Id,
        //                    ColorName = pc.Color,
        //                    ColorQuantity = (int)(pc.Quantity ?? 0m),
        //                    BOMQuantity = (int)(bom.Quantity ?? 0m),
        //                    PlannerQuantity = (int)((pc.Quantity ?? 0m) * (bom.Quantity ?? 0m)),
        //                    ProductId = (int)(pc.ProductId ?? 0),
        //                    SubItemId = (int)bom.Id,
        //                    PPSubItemId = (decimal)bom.Id,
        //                    QuantityReceived = r != null ? (r.QuantityReceived ?? 0) : 0,
        //                    ToReceived = r != null ? (r.ToReceived ?? 0) : 0,
        //                    ReceivedDate = r.ReceivedDate,
        //                };

        //    // Filter by productId (or other ID) in edit
        //    if (packagingProductionId.HasValue)
        //    {
        //        query = query.Where(x => x.PaperColorId != 0 && db.PaperColors
        //            .Any(p => p.Id == x.PaperColorId && p.PackagingProductionId == packagingProductionId.Value));
        //    }

        //    var model = query.ToList();
        //    return View(model);
        //}


        public ActionResult EditColor(int packagingProductionId)
        {
            var query = from r in db.CPReceipts
                        where r.PackagingProductionId == packagingProductionId
                        join pc in db.PaperColors on r.PaperColorId equals pc.Id
                        join bom in db.PPSubItems on r.PPSubItemId equals bom.Id
                        join pp in db.PackagingProductions on r.PackagingProductionId equals pp.Id
                        orderby r.ReceivedDate // Add ordering by date
                        select new PackagingProductionViewModel
                        {
                            PaperColorId = pc.Id,
                            ColorName = pc.Color,
                            ColorQuantity = (int)(pc.Quantity ?? 0),
                            BOMQuantity = (int)(bom.Quantity ?? 0),
                            PlannerQuantity = (int)((pc.Quantity ?? 0m) * (bom.Quantity ?? 0m)),
                            ProductId = (int)(pc.ProductId ?? 0),
                            SubItemId = (int)bom.Id,
                            PPSubItemId = (decimal)bom.Id,
                            PackagingProductionId = pp.Id,
                            QuantityReceived = r.QuantityReceived ?? 0,
                            ToReceived = r.ToReceived ?? 0,
                            ReceivedDate = r.ReceivedDate
                        };

            var model = query.ToList();

            var colors = model.Select(m => m.ColorName).Distinct().ToList();
            var colorPlannerQuantities = model.GroupBy(m => m.ColorName)
                                            .ToDictionary(g => g.Key, g => g.First().PlannerQuantity);

            // Calculate ToReceive per color (total planner - total received)
            var toReceiveDict = colors.ToDictionary(
                color => color,
                color => colorPlannerQuantities[color] - model.Where(m => m.ColorName == color).Sum(m => m.QuantityReceived)
            );

            ViewBag.Colors = colors;
            ViewBag.ColorPlannerQuantities = colorPlannerQuantities;
            ViewBag.ToReceiveDict = toReceiveDict;
            ViewBag.PackagingProductionId = packagingProductionId;

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditColor(List<PPColorReceiptViewModel> receiptData)
        {
            if (receiptData == null || !receiptData.Any())
            {
                return Json(new { success = false, message = "No data received" });
            }

            try
            {
                // Get packagingProductionId from first item (all should have same value)
                var packagingProductionId = receiptData.First().PackagingProductionId;

                // First remove all existing receipts for this production
                var existingReceipts = db.CPReceipts.Where(r => r.PackagingProductionId == packagingProductionId);
                db.CPReceipts.RemoveRange(existingReceipts);

                // Add the updated receipts
                foreach (var item in receiptData)
                {
                    DateTime receivedDate;
                    if (!DateTime.TryParse(item.ReceivedDate, out receivedDate))
                    {
                        return Json(new { success = false, message = "Invalid date format" });
                    }

                    var receipt = new CPReceipt
                    {
                        PaperColorId = item.PaperColorId,
                        PPSubItemId = item.PPSubItemId,
                        PackagingProductionId = packagingProductionId,
                        QuantityReceived = item.QuantityReceived,
                        ToReceived = item.ToReceive,
                        ReceivedDate = receivedDate
                    };

                    db.CPReceipts.Add(receipt);
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error saving changes: " + ex.Message
                });
            }
        }
        //public ActionResult FlatBoxStockSummary(int? packagingProductionId) Existing One
        //{
        //    // Get all receipts grouped by packaging production, product and color
        //    var stockData = from r in db.CPReceipts
        //                    join pc in db.PaperColors on r.PaperColorId equals pc.Id
        //                    join p in db.Products on pc.ProductId equals p.Id
        //                    join pp in db.PackagingProductions on r.PackagingProductionId equals pp.Id
        //                    where (packagingProductionId == null || r.PackagingProductionId == packagingProductionId)
        //                    group r by new
        //                    {
        //                        PackagingProductionId = pp.Id,
        //                        PackagingProductionName = pp.ProductName, // Assuming there's a Name property
        //                        ProductId = p.Id,
        //                        ProductName = p.Name,
        //                        ColorId = pc.Id,
        //                        ColorName = pc.Color
        //                    } into g
        //                    select new
        //                    {
        //                        g.Key.PackagingProductionId,
        //                        g.Key.PackagingProductionName,
        //                        g.Key.ProductId,
        //                        g.Key.ProductName,
        //                        g.Key.ColorId,
        //                        g.Key.ColorName,
        //                        TotalReceived = g.Sum(x => x.QuantityReceived) ?? 0
        //                    };

        //    // Group by PackagingProduction and Product
        //    var productionGroups = stockData.ToList()
        //        .GroupBy(x => new
        //        {
        //            x.PackagingProductionId,
        //            x.PackagingProductionName,
        //            x.ProductId,
        //            x.ProductName
        //        });

        //    var viewModel = productionGroups.Select(g => new FlatBoxStockViewModel
        //    {
        //        PackagingProductionId = g.Key.PackagingProductionId,
        //        ProductId = (int)g.Key.ProductId,
        //        ProductName = g.Key.ProductName,
        //        // Calculate complete boxes based on minimum color quantity
        //        TotalCompleteBoxes = g.Any() ? g.Min(x => x.TotalReceived) : 0,
        //        ColorComponents = g.Select(c => new ColorStockInfo
        //        {
        //            ColorId = c.ColorId,
        //            ColorName = c.ColorName,
        //            Quantity = c.TotalReceived
        //        }).ToList(),
        //        LastUpdated = DateTime.Now
        //    }).ToList();

        //    // Add summary data to ViewBag if needed
        //    ViewBag.PackagingProductions = db.PackagingProductions.ToList();
        //    if (packagingProductionId.HasValue)
        //    {
        //        ViewBag.SelectedProduction = db.PackagingProductions.Find(packagingProductionId.Value)?.ProductName;
        //    }

        //    return View(viewModel);
        //}

        [HttpGet]
        public ActionResult FlatBoxStockSummary(int? productId)
        {
            var stockData = from r in db.CPReceipts
                            join pc in db.PaperColors on r.PaperColorId equals pc.Id
                            join p in db.Products on pc.ProductId equals p.Id
                            where !productId.HasValue || p.Id == productId
                            group r by new
                            {
                                ProductId = p.Id,
                                ProductName = p.Name,
                                ColorName = pc.Color
                            } into g
                            select new
                            {
                                g.Key.ProductId,
                                g.Key.ProductName,
                                g.Key.ColorName,
                                TotalReceived = g.Sum(x => (int?)x.QuantityReceived) ?? 0
                            };

            var viewModel = new List<FlatBoxStockViewModel>();

            if (productId.HasValue)
            {
                // Get all unique color names for the selected product
                var allColors = db.PaperColors
                                  .Where(pc => pc.ProductId == productId)
                                  .Select(pc => pc.Color)
                                  .Distinct()
                                  .ToList();

                var summedQuantities = stockData.ToList();

                viewModel.Add(new FlatBoxStockViewModel
                {
                    ProductId = productId.Value,
                    ProductName = db.Products.Find(productId.Value)?.Name ?? "",
                    ColorComponents = allColors.Select(colorName => new ColorStockInfo
                    {
                        ColorId = 0, // optional or leave null, since we’re combining across IDs
                        ColorName = colorName,
                        Quantity = summedQuantities
                                    .Where(c => c.ColorName == colorName)
                                    .Sum(c => c.TotalReceived)
                    }).ToList(),
                    LastUpdated = DateTime.Now
                });
            }

            ViewBag.Products = db.Products.ToList();
            if (productId.HasValue)
            {
                ViewBag.SelectedProduct = db.Products.Find(productId.Value)?.Name;
            }

            return View(viewModel);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ProductStockDetail(List<FlatBoxStockViewModel> receipts)
        {
            if (receipts == null || !receipts.Any())
            {
                return Json(new { success = false, message = "No data received" });
            }

            try
            {
                foreach (var item in receipts)
                {
                    // Validate required fields
                    if (item.PaperColorId == 0 ||  item.PackagingProductionId == 0)
                    {
                        return Json(new { success = false, message = "Missing required fields" });
                    }

                    var stockDetail = new ProductStockDetail
                    {
                        PaperColorId = item.PaperColorId,
                        Quantity = item.Quantity,
                        PackagingProductionId = item.PackagingProductionId,
                        ProductId = item.ProductId,
                        DateManufactured = DateTime.Now
                    };

                    db.ProductStockDetails.Add(stockDetail);

                    // Optionally update the stock levels or other related entities
                    // var stockItem = db.StockItems.FirstOrDefault(s => s.ProductId == item.ProductId && s.ColorId == item.ColorId);
                    // if (stockItem != null) {
                    //     stockItem.Quantity += item.Quantity;
                    // }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return Json(new { success = false, message = "An error occurred while saving the production data." });
            }
        }

        public ActionResult Index4(int? packagingProductionId)
        {
            var query = from pc in db.PaperColors
                        join pp in db.PackagingProductions on pc.PackagingProductionId equals pp.Id
                        join bom in db.PPSubItems on pc.ProductId equals bom.ParentProductId
                        join product in db.Products on pc.ProductId equals product.Id
                        where bom.Quantity != null
                        join receipt in db.PPColorReceipts
                            on new { pcId = pc.Id, subItemId = bom.Id }
                            equals new
                            {
                                pcId = receipt.PaperColorId ?? 0,
                                subItemId = receipt.PPSubItemId ?? 0
                            }
                            into receiptGroup
                        from r in receiptGroup.DefaultIfEmpty()
                        let stockQty = db.ProductStockDetails
                            .Where(s => s.ProductId == pc.ProductId && s.PaperColorId == pc.Id)
                            .Select(s => s.Quantity)
                            .FirstOrDefault()
                        select new BoxProductionViewModel
                        {
                            PaperColorId = pc.Id,
                            ColorName = pc.Color,
                            ColorQuantity = (int)(pc.Quantity ?? 0m),
                            BOMQuantity = (int)(bom.Quantity ?? 0m),
                            StockQuantity = (int)(stockQty ?? 0m),
                            ProductName = product.Name,
                            ProductId = (int)(pc.ProductId ?? 0),
                            SubItemId = (int)bom.Id,
                            PPSubItemId = (decimal)bom.Id,
                            PackagingProductionId = pp.Id,
                            QuantityReceived = r != null ? (r.QuantityReceived ?? 0) : 0,
                            ToReceived = r != null ? (r.ToReceived ?? 0) : 0,
                            ReceivedDate = r != null ? r.ReceivedDate : null
                        };
            if (packagingProductionId.HasValue)
            {
                query = query.Where(x => x.PaperColorId != 0 && db.PaperColors
                    .Any(p => p.Id == x.PaperColorId && p.PackagingProductionId == packagingProductionId.Value));
            }

            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var startDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            ViewBag.StartDate = startDate;

            ViewBag.Colors = db.PaperColors.ToList();

            var model = query.ToList();
            return View(model);
            // Rest of the method remains the same...
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateIndex4(List<FinalBoxProductionViewModel> cpreceipts)
        {
            if (cpreceipts == null || !cpreceipts.Any())
            {
                return Json(new { success = false, message = "No data received" });
            }

            try
            {
                foreach (var item in cpreceipts)
                {
                    DateTime receivedDate;
                    if (!DateTime.TryParse(item.ReceivedDate, out receivedDate))
                    {
                        // Handle invalid date string (you can skip or assign a default value)
                        return Json(new { success = false, message = "Invalid date format" });
                    }


                    var receipt = new BOXProduction
                    {
                        PaperColorId = item.PaperColorId,
                        PPSubItemId = (int)item.PPSubItemId, // Assuming PPSubItemId is int in CPReceipt
                        QuantityReceived = item.QuantityReceived,
                        ReceivedDate = receivedDate,
                        // Uncomment below line only if your model supports it
                        // PlannerQuantity = item.PlannerQuantity,
                        ToReceived = item.ToReceive,
                        PackagingProductionId = item.PackagingProductionId
                    };

                    db.BOXProductions.Add(receipt);
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult EditIndex4(int packagingProductionId)
        {
            var query = from r in db.BOXProductions
                        where r.PackagingProductionId == packagingProductionId
                        join pc in db.PaperColors on r.PaperColorId equals pc.Id
                        join bom in db.PPSubItems on r.PPSubItemId equals bom.Id
                        join pp in db.PackagingProductions on r.PackagingProductionId equals pp.Id
                        orderby r.ReceivedDate // Add ordering by date
                        select new BoxProductionViewModel
                        {
                            PaperColorId = pc.Id,
                            ColorName = pc.Color,
                            ColorQuantity = (int)(pc.Quantity ?? 0),
                            BOMQuantity = (int)(bom.Quantity ?? 0),
                            PlannerQuantity = (int)((pc.Quantity ?? 0m) * (bom.Quantity ?? 0m)),
                            ProductId = (int)(pc.ProductId ?? 0),
                            SubItemId = (int)bom.Id,
                            PPSubItemId = (decimal)bom.Id,
                            PackagingProductionId = pp.Id,
                            QuantityReceived = r.QuantityReceived ?? 0,
                            ToReceived = r.ToReceived ?? 0,
                            ReceivedDate = r.ReceivedDate
                        };

            var model = query.ToList();

            var colors = model.Select(m => m.ColorName).Distinct().ToList();
            var colorPlannerQuantities = model.GroupBy(m => m.ColorName)
                                            .ToDictionary(g => g.Key, g => g.First().PlannerQuantity);

            // Calculate ToReceive per color (total planner - total received)
            var toReceiveDict = colors.ToDictionary(
                color => color,
                color => colorPlannerQuantities[color] - model.Where(m => m.ColorName == color).Sum(m => m.QuantityReceived)
            );

            ViewBag.Colors = colors;
            ViewBag.ColorPlannerQuantities = colorPlannerQuantities;
            ViewBag.ToReceiveDict = toReceiveDict;
            ViewBag.PackagingProductionId = packagingProductionId;

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditIndex4(List<FinalBoxProductionViewModel> receiptData)
        {
            if (receiptData == null || !receiptData.Any())
            {
                return Json(new { success = false, message = "No data received" });
            }

            try
            {
                // Get packagingProductionId from first item (all should have same value)
                var packagingProductionId = receiptData.First().PackagingProductionId;

                // First remove all existing receipts for this production
                var existingReceipts = db.CPReceipts.Where(r => r.PackagingProductionId == packagingProductionId);
                db.CPReceipts.RemoveRange(existingReceipts);

                // Add the updated receipts
                foreach (var item in receiptData)
                {
                    DateTime receivedDate;
                    if (!DateTime.TryParse(item.ReceivedDate, out receivedDate))
                    {
                        return Json(new { success = false, message = "Invalid date format" });
                    }

                    var receipt = new BOXProduction
                    {
                        PaperColorId = item.PaperColorId,
                        PPSubItemId = item.PPSubItemId,
                        PackagingProductionId = packagingProductionId,
                        QuantityReceived = item.QuantityReceived,
                        ToReceived = item.ToReceive,
                        ReceivedDate = receivedDate
                    };

                    db.BOXProductions.Add(receipt);
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error saving changes: " + ex.Message
                });
            }
        }

        public ActionResult EditProductStockDetail(int packagingProductionId)
        {
            // Validate input
            if (packagingProductionId <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the production first to ensure it exists
            var production = db.PackagingProductions
                .Include(pp => pp.Product)
                .FirstOrDefault(pp => pp.Id == packagingProductionId);

            if (production == null)
            {
                return HttpNotFound();
            }

            // Get the color order from the Create view's logic
            var createViewColorOrder = db.CPReceipts
                .Where(r => r.PackagingProductionId == packagingProductionId)
                .Join(db.PaperColors, r => r.PaperColorId, pc => pc.Id, (r, pc) => pc)
                .GroupBy(pc => new { pc.Id, pc.Color })
                .Select(g => new { g.Key.Id, g.Key.Color })
                .ToList()
                .Select(x => new { x.Id, x.Color })
                .Distinct()
                .ToList();

            // Get all stock details with related data
            var stockDetails = db.ProductStockDetails
                .Where(s => s.PackagingProductionId == packagingProductionId)
                .Include(s => s.PaperColor)
                .ToList();

            // Prepare view model with colors ordered to match Create view
            var viewModel = new FlatBoxStockViewModel
            {
                PackagingProductionId = packagingProductionId,
                ProductId = production.ProductId.HasValue ? (int)production.ProductId.Value : 0,
                ProductName = production.Product?.Name ?? "N/A",
                ColorComponents = createViewColorOrder
                    .Join(stockDetails,
                        co => co.Id,
                        sd => sd.PaperColorId,
                        (co, sd) => new ColorStockInfo
                        {
                            ColorId = sd.PaperColorId.HasValue ? (int)sd.PaperColorId.Value : 0,
                            ColorName = sd.PaperColor?.Color ?? "N/A",
                            Quantity = sd.Quantity.HasValue ? (int)sd.Quantity.Value : 0
                        })
                    .ToList(),
                LastUpdated = DateTime.Now
            };

            // For any colors in stockDetails not in createViewColorOrder (shouldn't happen)
            var missingColors = stockDetails
                .Where(sd => !createViewColorOrder.Any(co => co.Id == sd.PaperColorId))
                .Select(sd => new ColorStockInfo
                {
                    ColorId = sd.PaperColorId.HasValue ? (int)sd.PaperColorId.Value : 0,
                    ColorName = sd.PaperColor?.Color ?? "N/A",
                    Quantity = sd.Quantity.HasValue ? (int)sd.Quantity.Value : 0
                })
                .ToList();

            viewModel.ColorComponents.AddRange(missingColors);

            // Calculate complete boxes (minimum quantity across all colors)
            viewModel.TotalCompleteBoxes = viewModel.ColorComponents.Any()
                ? viewModel.ColorComponents.Min(c => c.Quantity)
                : 0;

            // Add production info to ViewBag
            ViewBag.PackagingProductionName = production.ProductName;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductStockDetail(FlatBoxStockViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate necessary ViewBag data if needed
                var production = db.PackagingProductions.Find(model.PackagingProductionId);
                ViewBag.PackagingProductionName = production?.ProductName;
                return View(model);
            }

            try
            {
                // Get existing stock details for this production
                var existingDetails = db.ProductStockDetails
                    .Where(s => s.PackagingProductionId == model.PackagingProductionId)
                    .ToList();

                // Process each color component
                foreach (var colorInfo in model.ColorComponents)
                {
                    var existingDetail = existingDetails.FirstOrDefault(s => s.PaperColorId == colorInfo.ColorId);

                    if (existingDetail != null)
                    {
                        // Update existing record
                        existingDetail.Quantity = colorInfo.Quantity;
                        existingDetail.DateManufactured = DateTime.Now;
                    }
                    else
                    {
                        // Create new record
                        var newDetail = new ProductStockDetail
                        {
                            PackagingProductionId = model.PackagingProductionId,
                            ProductId = model.ProductId,
                            PaperColorId = colorInfo.ColorId,
                            Quantity = colorInfo.Quantity,
                            DateManufactured = DateTime.Now
                        };
                        db.ProductStockDetails.Add(newDetail);
                    }
                }

                // Save changes
                db.SaveChanges();

                TempData["SuccessMessage"] = "Product stock details updated successfully";
                return RedirectToAction("FlatBoxStockSummary", new { packagingProductionId = model.PackagingProductionId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving: " + ex.Message);

                // Repopulate ViewBag data
                var production = db.PackagingProductions.Find(model.PackagingProductionId);
                ViewBag.PackagingProductionName = production?.ProductName;

                return View(model);
            }
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditColor(PPColorReceiptViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    try
        //    {
        //        var receipt = db.CPReceipts.FirstOrDefault(r => r.PaperColorId == model.PaperColorId && r.PPSubItemId == model.PPSubItemId);

        //        if (receipt == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        DateTime receivedDate;
        //        if (!DateTime.TryParse(model.ReceivedDate, out receivedDate))
        //        {
        //            return Json(new { success = false, message = "Invalid date format" });
        //        }

        //        // Update fields
        //        receipt.QuantityReceived = model.QuantityReceived;
        //        receipt.ToReceived = model.ToReceive;
        //        receipt.ReceivedDate = receivedDate;

        //        db.SaveChanges();

        //        return Json(new { success = true, message = "Record updated successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        [HttpPost]
        public ActionResult SaveReceipts(List<PPColorReceiptViewModel> receipts)
        {
            foreach (var receipt in receipts)
            {
                var entity = new PPColorReceipt
                {
                    PaperColorId = (int?)receipt.PaperColorId,
                    PPSubItemId = receipt.PPSubItemId,
                    QuantityReceived = receipt.QuantityReceived,
                    ReceivedDate = DateTime.Parse(receipt.ReceivedDate),
                    PannerQty = receipt.PlannerQuantity,
                    ToReceived = receipt.ToReceive,
                    
                };


                db.PPColorReceipts.Add(entity);
            }

            db.SaveChanges();

            return Json(new { success = true });
        }




        //public ActionResult Index1()
        //{
        //    var model = (from pc in db.PaperColors
        //                 join pp in db.PackagingProductions on pc.PackagingProductionId equals pp.Id
        //                 join bom in db.PPSubItems on pp.ProductId equals bom.ParentProductId into bomJoin
        //                 from bom in bomJoin.DefaultIfEmpty()
        //                 group bom by new { pc.Id, pc.Color, pc.Quantity } into g
        //                 select new PackagingProductionViewModel
        //                 {
        //                     PaperColorId = g.Key.Id,
        //                     ColorName = g.Key.Color,
        //                     ColorQuantity = (int)(g.Key.Quantity ?? 0),
        //                     BOMQuantity = g.Sum(x => x != null ? (x.Quantity ?? 0) : 0),
        //                     PlannerQuantity = (int)(g.Key.Quantity ?? 0) * g.Sum(x => x != null ? (x.Quantity ?? 0) : 0)
        //                 }).ToList();

        //    return View(model);
        //}

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
            decimal maxId = db.PackagingProductions.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;  // Passing the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers;  // Assuming this contains the supplier list

            var productType8And9 = db.Products
       .Where(p => p.PType == 8 || p.PType == 9)
       .Select(p => new
       {
           p.Id,
           p.Name
       })
       .ToList();

            ViewBag.ProductList = new SelectList(productType8And9, "Id", "Name");
           
            var productDetails = db.ProductDetails.ToList();

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

            var viewModel = new PackagingProductionViewModel
            {
                PackagingProduction = new PackagingProduction(),
                Products = DAL.dbProducts,
                SubItems = subItems,
                QuantityToProduce = new List<QuantityToProduce> { new QuantityToProduce() },
                ProductDetail = new List<ProductDetail> { new ProductDetail() },
                PaperColor = new List<PaperColor> { new PaperColor() },
                Color = DAL.dbColors.ToList() // ✅ load real color data here
            };
            ViewBag.Colors = db.Colors.ToList(); // ✅ Add this line
            ViewBag.Suppliers = DAL.dbSuppliers;
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

        //   [HttpPost] Existing
        //   [ValidateAntiForgeryToken]
        //   public ActionResult Create(
        //[Bind(Prefix = "PackagingProduction", Include = "Id,ProductName,Unit,ProductId,Quantity,Validate,PProdDate,Box,Supplier")] PackagingProduction packagingProduction,
        ////[Bind(Prefix = "SubItem", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItem> subItems,
        ////[Bind(Prefix = "SubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested,SubItemQty")] List<SubItemProduction> subItemProductions,
        //[Bind(Prefix = "PaperColor", Include = "Id,Color,Quantity,ProductId,PackagingProductionId,")] List<PaperColor> color)
        //   {
        //       if (ModelState.IsValid)
        //       {


        //           // Save the BOM and its SubItems
        //           db.PackagingProductions.Add(packagingProduction);


        //           //foreach (var item in subItems)
        //           //{
        //           //    item.ParentProductId = newProduction.ProductId;
        //           //    db.SubItems.Add(item);
        //           //}

        //           if (color != null)
        //           {
        //               foreach (var item in color)
        //               {
        //                   item.ProductId = packagingProduction.ProductId;
        //                   item.PackagingProductionId = packagingProduction.Id;
        //                   //item.Id = newProduction.Id;
        //                   db.PaperColors.Add(item);
        //                   //item.ProductId = newProduction.Id;
        //                   //db.QuantityToProduces.Add(item);

        //               }
        //           }

        //           db.SaveChanges();
        //           return RedirectToAction("Index");

        //       }



        //       ViewBag.Suppliers = DAL.dbSuppliers;

        //       var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
        //       ViewBag.ProductList = new SelectList(productList, "Id", "Name");

        //       return View(packagingproductionViewModel);
        //   }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
       [Bind(Prefix = "PackagingProduction", Include = "Id,ProductName,Unit,ProductId,Quantity,Validate,PProdDate,Box,Supplier")] PackagingProduction packagingProduction,
       [Bind(Prefix = "PaperColor", Include = "Id,Color,Quantity,ProductId,PackagingProductionId")] List<PaperColor> color)
        {
            if (ModelState.IsValid)
            {
                db.PackagingProductions.Add(packagingProduction);

                if (color != null)
                {
                    foreach (var item in color)
                    {
                        item.ProductId = packagingProduction.ProductId;
                        item.PackagingProductionId = packagingProduction.Id;
                        db.PaperColors.Add(item);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Rebuild the full view model for returning the view
            var viewModel = new PackagingProductionViewModel
            {
                PackagingProduction = packagingProduction,
                Products = DAL.dbProducts,
                SubItems = db.Products
                    .Select(p => new { p.Id, p.Stock })
                    .AsEnumerable()
                    .Select(p => new SubItem
                    {
                        ProductId = p.Id,
                        AvailableInventory = p.Stock,
                        QuantitytoPrepare = 0
                    }).ToList(),
                QuantityToProduce = new List<QuantityToProduce> { new QuantityToProduce() },
                ProductDetail = new List<ProductDetail> { new ProductDetail() },
                PaperColor = color ?? new List<PaperColor>(),
                Color = DAL.dbColors.ToList()
            };

            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Suppliers = DAL.dbSuppliers;

            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name");

            return View(viewModel);
        }



        // public ActionResult Edit(decimal? id)
        // {
        //     if (id == null)
        //     {
        //         return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //     }

        //     var newProduction = db.NewProductions
        //                           .Include(np => np.Products) // Include related Products
        //                           .FirstOrDefault(np => np.Id == id);

        //     if (newProduction == null)
        //     {
        //         return HttpNotFound();
        //     }
        //     var products = db.Products
        //.Where(p => p.Manufacturable == true)
        //.Select(p => new
        //{
        //    Value = p.Id.ToString(),
        //    Text = p.Name
        //})
        //.ToList();

        //     ViewBag.ProductList = new SelectList(products, "Value", "Text", newProduction.ProductId); // Pre-select the current product

        //     // Fetch SubItems associated with this NewProduction
        //     var subItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList();

        //     // Prepare ViewModel
        //     var viewModel = new NewProductionViewModel
        //     {
        //         NewProduction = newProduction,
        //         Products = db.Products, // Include all products
        //         SubItems = subItems // Include fetched SubItems
        //     };

        //     // Populate ViewBag with Product List
        //     var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
        //     ViewBag.ProductList = new SelectList(productList, "Id", "Name", newProduction.ProductName);

        //     return View(viewModel);
        // }
        public ActionResult Edit(decimal? id, bool readonlyMode = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch the NewProduction entity with related Products and other details
            var packagingProduction = db.PackagingProductions
                                   .Include(np => np.Product) // Include related Products
                                   .FirstOrDefault(np => np.Id == id);
            ViewBag.ProductList = new SelectList(db.Products, "Id", "Name", packagingProduction.ProductId);

            if (packagingProduction == null)
            {
                return HttpNotFound();
            }
            string productType;
            switch (packagingProduction.Product.PType)
            {
                case 1:
                    productType = "Variable";
                    break;
                case 2:
                    productType = "Excess";
                    break;
                case 3:
                    productType = "ByProduct";
                    break;
                default:
                    productType = "Unknown";
                    break;
            }

            var paperColors = db.PaperColors
    .Where(x => x.PackagingProductionId == packagingProduction.Id)
    .ToList();

            var productType8And9 = db.Products
        .Where(p => p.PType == 8 || p.PType == 9)
        .Select(p => new
        {
            p.Id,
            p.Name
        })
        .ToList();

            ViewBag.ProductList = new SelectList(productType8And9, "Id", "Name");
            ViewBag.Colors = db.Colors.ToList(); // ✅ Add this line
            ViewBag.Suppliers = DAL.dbSuppliers; 
            ViewBag.ReadonlyMode = readonlyMode;
            // Prepare ViewModel (including SubItems if needed)
            var viewModel = new PackagingProductionViewModel
            {
                PackagingProduction = packagingProduction,
                Products = db.Products,
                SubItemProduction = db.SubItemProductions.Where(x => x.NewProductionId == packagingProduction.Id).ToList(),
                PaperColor = paperColors, // <-- Assign the real list here
                Color = db.Colors.ToList()
            };

            return View(viewModel);
        }





        //   [HttpPost]
        //   [ValidateAntiForgeryToken]
        //   public ActionResult Edit(NewProductionViewModel model,
        //[Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,QuantityToProduce,ProductId")] NewProduction newProduction,
        ////[Bind(Prefix = "SubItem", Include = "Id,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItem> subItems,
        //[Bind(Prefix = "QuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,ProductId")] List<QuantityToProduce> quantityToProduces)

        //       {
        //       if (ModelState.IsValid)
        //           {
        //               // Fetch the existing NewProduction entity
        //               var existingProduction = db.NewProductions.Find(model.NewProduction.Id);
        //               var existingQuantityToProduces = db.QuantityToProduces.Where(x => x.ProductId == newProduction.Id).ToList();
        //               db.QuantityToProduces.RemoveRange(existingQuantityToProduces);
        //               if (existingProduction == null)
        //           {
        //               return HttpNotFound();
        //           }

        //           // Update NewProduction fields
        //           existingProduction.ProductionDate = model.NewProduction.ProductionDate;
        //           existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.NewProduction.Id)?.Name;
        //           existingProduction.Unit = model.NewProduction.Unit;
        //           existingProduction.QuantityToProduces = model.NewProduction.QuantityToProduces;

        //           db.Entry(existingProduction).State = EntityState.Modified;

        //           // Delete existing SubItems
        //           //var delSubItems = db.SubItems.Where(x => x.ParentProductId == newProduction.Id).ToList();
        //           //db.SubItems.RemoveRange(delSubItems);

        //           // Add new SubItems
        //           //foreach (var item in subItems)
        //           //{
        //           //    item.ParentProductId = existingProduction.Id;
        //           //}
        //           //db.SubItems.AddRange(subItems);
        //           if (quantityToProduces != null)
        //           {
        //               foreach (var item in quantityToProduces)
        //               {
        //                   item.ProductId = newProduction.Id;
        //                   db.QuantityToProduces.Add(item);
        //               }
        //           }
        //      //     Save changes to the database
        //               db.SaveChanges();
        //           return RedirectToAction("Index");
        //       }

        //       // Repopulate ViewBag on failure
        //       var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
        //       ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.NewProduction.ProductName);

        //       return View(model);
        //   }

        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Edit(NewProductionViewModel model,
        //[Bind(Prefix = "NewProduction", Include = "Id,ProductionDate,ProductName,Unit,ProductId")] NewProduction newProduction,
        //[Bind(Prefix = "QuantityToProduce", Include = "Id,ProductionQty,BOMId,ProductDetailId,Shape,CalculatedWeight,ProductId")] List<QuantityToProduce> quantityToProduces)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // Fetch the existing NewProduction entity
        //            var existingProduction = db.NewProductions.Find(model.NewProduction.Id);

        //            // Ensure the NewProduction entity exists
        //            if (existingProduction == null)
        //            {
        //                return HttpNotFound();
        //            }

        //            // Update NewProduction fields
        //            existingProduction.ProductionDate = model.NewProduction.ProductionDate;
        //            existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.NewProduction.ProductId)?.Name;
        //            existingProduction.Unit = model.NewProduction.Unit;
        //            existingProduction.ProductId = model.NewProduction.ProductId;

        //            // Update QuantityToProduce for the NewProduction
        //            // First, remove the existing QuantityToProduce entries related to this production
        //            var existingQuantityToProduces = db.QuantityToProduces.Where(x => x.ProductId == existingProduction.ProductId).ToList();
        //            db.QuantityToProduces.RemoveRange(existingQuantityToProduces);

        //            // Add new QuantityToProduce entries
        //            if (quantityToProduces != null && quantityToProduces.Count > 0)
        //            {
        //                foreach (var item in quantityToProduces)
        //                {
        //                    item.ProductId = existingProduction.ProductId; // Ensure ProductId is set to the correct value
        //                    item.NewProductionId = existingProduction.Id; // Ensure QuantityToProduce is linked to the correct NewProduction
        //                    db.QuantityToProduces.Add(item);
        //                }
        //            }

        //            // Save the changes to the database
        //            db.Entry(existingProduction).State = EntityState.Modified;
        //            db.SaveChanges();

        //            // Redirect to the index page after saving changes
        //            return RedirectToAction("Index");
        //        }

        //        // Repopulate ViewBag on failure (when ModelState is not valid)
        //        var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
        //        ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.NewProduction.ProductId);

        //        // Return the view with the model data on failure
        //        return View(model);
        //    }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
    PackagingProductionViewModel model,
   [Bind(Prefix = "PackagingProduction", Include = "Id,ProductName,Unit,ProductId,Quantity,Validate,PProdDate,Box")] PackagingProduction packagingProduction,
[Bind(Prefix = "PaperColor", Include = "Id,Color,Quantity,ProductId,PackagingProductionId,")] List<PaperColor> color)
        //[Bind(Prefix = "SubItemProduction", Include = "Id,ParentProductId,ProductId,Quantity,Unit,AvailableInventory,QuantitytoPrepare,QuantityRequested")] List<SubItemProduction> subItemProductions)

        {

            {
                // Fetch the existing NewProduction entity
                var existingProduction = db.PackagingProductions.Find(model.PackagingProduction.Id);

                // Ensure the NewProduction entity exists
                if (existingProduction == null)
                {
                    return HttpNotFound();
                }

                // Update NewProduction fields
                existingProduction.PProdDate = model.PackagingProduction.PProdDate;
                existingProduction.ProductName = db.Products.FirstOrDefault(p => p.Id == model.PackagingProduction.ProductId)?.Name;
                existingProduction.Unit = model.PackagingProduction.Unit;
                existingProduction.ProductId = model.PackagingProduction.ProductId;

                // Remove existing QuantityToProduce entries for the current NewProduction
                var existingQuantityToProduces = db.PaperColors
                    .Where(x => x.PackagingProductionId == existingProduction.Id)
                    .ToList();
                db.PaperColors.RemoveRange(existingQuantityToProduces);

                // Add new QuantityToProduce entries
                if (color != null && color.Count > 0)
                {
                    foreach (var item in color)
                    {
                        item.ProductId = existingProduction.ProductId; // Update ProductId
                        item.PackagingProductionId = existingProduction.Id;  // Associate with the current NewProduction
                        db.PaperColors.Add(item);
                    }
                }

                // Save the changes to the database
                db.Entry(existingProduction).State = EntityState.Modified;

                db.SaveChanges();

                // Redirect to the index page after saving changes
                return RedirectToAction("Index");
            }

            // Repopulate ViewBag on failure (when ModelState is not valid)
            var productList = db.Products.Select(p => new { p.Id, p.Name }).ToList();
            ViewBag.ProductList = new SelectList(productList, "Id", "Name", model.NewProduction.ProductId);

            // Return the view with the model data on failure
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
        public JsonResult GetProductUnit(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                return Json(new { unit = product.Unit }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { unit = "N/A" }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetSubItemDetails(int productId)
        //{
        //    // Retrieve sub-item details for the given product ID
        //    var subItems = db.SubItems
        //        .Where(s => s.ParentProductId == productId)
        //        .Select(s => new
        //        {
        //            s.Id,
        //            ProductName = s.Product.Name, // Assuming a navigation property for Product
        //            s.Quantity,
        //            s.Unit,
        //             Manufacturable = s.Product.Manufacturable
        //            /* s.Shape*/ // If Shape is part of the SubItem model
        //        })
        //        .ToList();

        //    return Json(subItems, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetSubItemDetails(int productId)
        {
            // Retrieve sub-item details for the given product ID
            //var subItems = db.SubItems
            //    .Where(s => s.ParentProductId == productId)

            //    .Select(s => new
            //    {
            //        s.Id,
            //        ProductId = s.Product.Id,
            //        ProductName = s.Product.Name, // Assuming a navigation property for Product
            //        //s.Product.PType,
            //        s.Quantity,
            //        s.Unit,


            //        //s.Product.Manufacturable
            //        manufacturable = s.Product.Manufacturable, // Include the Manufacturable property from the Product table
            //        PType = s.Product.PType == 1 ? "Variable" :
            //        s.Product.PType == 2 ? "Excess" :
            //        s.Product.PType == 3 ? "ByProduct" : "Unknown", // Default case
            //        VariableProduct = s.Product.VarProdParentId,
            //    })
            //    .ToList();
            var subItems = db.SubItems
    .Include(s => s.Product) // Ensure Product is loaded
    .Where(s => s.ParentProductId == productId)
    .Select(s => new
    {
        s.Id,
        ProductId = s.Product != null ? s.Product.Id : 0, // Prevent null reference errors
        ProductName = s.Product != null ? s.Product.Name : "Unknown",
        s.Quantity,
        s.Unit,
        manufacturable = s.Product != null && s.Product.PType == 4, // Prevent null reference errors
        Ingredient = s.Product != null && s.Product.PType == 5,
        IntermediaryIngredient = s.Product != null && s.Product.PType == 6,
        FinalProduct = s.Product != null && s.Product.PType == 4,
        PType = s.Product != null ?
            (s.Product.PType == 1 ? "Variable" :
             s.Product.PType == 2 ? "Excess" :
             s.Product.PType == 3 ? "ByProduct" : "Unknown")
            : "Unknown",
        VariableProduct = s.Product != null ? s.Product.VarProdParentId : (int?)null,
    })
    .ToList();


            return Json(subItems, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult ValidateStock(List<FinalProductionViewModel> updates)
        //{
        //    Console.WriteLine("ValidateStock method hit!");
        //                return Json(new { message = "Stock validated!", success = true });
        //}

        [HttpPost]
        public JsonResult ValidateStock(List<FinalProductionViewModel> LstProductionVM)
        {
            if (LstProductionVM == null || !LstProductionVM.Any())
            {
                return Json(new { message = "No updates received.", success = false });
            }

            foreach (FinalProductionViewModel productionVM in LstProductionVM.ToList())
            {
                Product product = db.Products.FirstOrDefault(p => p.Id == productionVM.ProductId);
                product.Stock -= productionVM.CalculatedValue;
                db.Entry(product).Property(x => x.Stock).IsModified = true;

                //var production = db.NewProductions.FirstOrDefault(n => n.Id == productionVM.ProductionId);
            }
            db.SaveChanges();
            //FinalProductionViewModel production= new FinalProductionViewModel ();
            foreach (FinalProductionViewModel productionVM in LstProductionVM.ToList())
            {
                //Console.WriteLine($"Received Update - ProductId: {production.ProductId}, Name: {production.Name}, CalculatedValue: {production.CalculatedValue}");

                // Find the product by ProductId
                var product = db.Products.FirstOrDefault(p => p.Id == productionVM.ProductId);
                var production = db.NewProductions.FirstOrDefault(n => n.Id == productionVM.ProductionId);

                if (product != null && production != null)
                {
                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

                    // ✅ Attach Product and Production to track changes
                    db.Products.Attach(product);
                    db.NewProductions.Attach(production);

                    // Perform stock subtraction (Stock - CalculatedValue)
                    product.Stock -= productionVM.CalculatedValue;

                    // Ensure stock does not go negative
                    if (product.Stock < 0)
                    {
                        product.Stock = 0;
                    }

                    // ✅ Explicitly mark properties as modified
                    db.Entry(product).Property(x => x.Stock).IsModified = true;

                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");

                    // ✅ Mark the production as validated
                    production.Validate = true;
                    db.Entry(production).Property(x => x.Validate).IsModified = true;
                }
                else
                {
                    Console.WriteLine($"Product '{productionVM.Name}' not found in the database.");
                    return Json(new { message = $"Product '{productionVM.Name}' not found.", success = false });
                }
            }

            // ✅ Save all changes at once
            var changes = db.SaveChanges();
            Console.WriteLine($"Changes Saved: {changes}");

            if (changes > 0)
            {
                return Json(new { message = "Stock updated & Production validated successfully.", success = true });
            }
            else
            {
                return Json(new { message = "No changes detected in the database.", success = false });
            }
        }

        [HttpPost]
        public JsonResult ValidateCStock(List<FinalProductionViewModel> updates)
        {
            if (updates == null || !updates.Any())
            {
                return Json(new { message = "No updates received.", success = false });
            }

            foreach (var prod in updates)
            {
                Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, CalculatedValue: {prod.CalculatedValue}");

                // Find the product by ProductId
                var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);

                if (product != null)
                {
                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

                    // Perform stock subtraction (Stock - CalculatedValue)
                    product.Stock += prod.CalculatedValue;

                    // Ensure stock does not go negative
                    if (product.Stock < 0)
                    {
                        product.Stock = 0;
                    }

                    // Mark property as modified
                    db.Entry(product).Property(x => x.Stock).IsModified = true;

                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
                }
                else
                {
                    Console.WriteLine($"Product '{prod.Name}' not found in the database.");
                    return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
                }
            }

            // Save all changes at once
            db.SaveChanges();

            return Json(new { message = "Stock updated successfully.", success = true });
        }
        [HttpPost]
        public JsonResult IncreasedStock(List<FinalProductionViewModel> updates)
        {
            if (updates == null || !updates.Any())
            {
                return Json(new { message = "No updates received.", success = false });
            }

            foreach (var prod in updates)
            {
                Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, CalculatedValue: {prod.CalculatedValue}");

                // Find the product by ProductId
                var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);

                if (product != null)
                {
                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

                    // Perform stock subtraction (Stock - CalculatedValue)
                    product.Stock += prod.CalculatedValue;

                    // Ensure stock does not go negative
                    if (product.Stock < 0)
                    {
                        product.Stock = 0;
                    }

                    // Mark property as modified
                    db.Entry(product).Property(x => x.Stock).IsModified = true;

                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
                }
                else
                {
                    Console.WriteLine($"Product '{prod.Name}' not found in the database.");
                    return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
                }
            }

            // Save all changes at once
            db.SaveChanges();

            return Json(new { message = "Stock updated successfully.", success = true });
        }

        [HttpPost]
        public JsonResult EIncreasedStock(List<FinalProductionViewModel> updates)
        {
            if (updates == null || !updates.Any())
            {
                return Json(new { message = "No updates received.", success = false });
            }

            foreach (var prod in updates)
            {
                Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, CalculatedValue: {prod.CalculatedValue}");

                // Find the product by ProductId
                var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);
                var production = db.PostProductions.FirstOrDefault(n => n.Id == prod.ProductionId);
                if (product != null && production != null)
                {
                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

                    // Perform stock subtraction (Stock - CalculatedValue)
                    product.Stock += prod.CalculatedValue;

                    // Ensure stock does not go negative
                    if (product.Stock < 0)
                    {
                        product.Stock = 0;
                    }

                    // Mark property as modified
                    db.Entry(product).Property(x => x.Stock).IsModified = true;
                    production.Validate = true;
                    db.Entry(production).Property(x => x.Validate).IsModified = true;
                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
                }
                else
                {
                    Console.WriteLine($"Product '{prod.Name}' not found in the database.");
                    return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
                }
            }

            // Save all changes at once
            db.SaveChanges();

            return Json(new { message = "Stock updated successfully.", success = true });
        }

        //public JsonResult GetSubItemDetails(int productId)
        //{
        //    // Fetch sub-item details including weight from ProductDetails table
        //    var subItems = db.SubItems
        //        .Where(s => s.ParentProductId == productId)
        //        .Join(
        //            db.ProductDetails, // Join with ProductDetails
        //            subItem => subItem.ProductId,
        //            productDetail => productDetail.ProductId,
        //            (subItem, productDetail) => new
        //            {
        //                subItem.Id,
        //                ProductName = subItem.Product.Name, // Assuming a navigation property for Product
        //                subItem.Quantity,
        //                subItem.Unit,
        //                Weight = productDetail.Weight // Fetch weight from ProductDetails
        //            })
        //        .ToList();

        //    return Json(subItems, JsonRequestBehavior.AllowGet);
        //}



        //public JsonResult GetSubItemDetails(int parentProductId)
        //{
        //    System.Diagnostics.Debug.WriteLine($"Received ParentProductId: {parentProductId}");

        //    var subItems = db.SubItems
        //        .Where(s => s.ParentProductId == parentProductId)
        //        .Include(s => s.Product) // Load navigation property if needed
        //        .Select(s => new
        //        {
        //            s.Id,
        //            ProductName = s.Product.Name,
        //            s.ProductId,
        //            s.Quantity,
        //            s.Unit
        //        })
        //        .ToList();

        //    System.Diagnostics.Debug.WriteLine($"SubItems Count: {subItems.Count}");

        //    return Json(subItems, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult ValidateStock(List<FinalProductionViewModel> updates)
        //{

        //    foreach (var update in updates)
        //    {
        //        // Find the product by its Name
        //        var product = db.Products.FirstOrDefault(p => p.Id == update.Product.Id);
        //        if (product != null)
        //        {
        //            // Subtract the calculated value from the stock
        //            product.Stock -= update.SubtractValue;

        //            // Ensure stock does not go negative
        //            if (product.Stock < 0)
        //                product.Stock = 0;

        //            // Save changes directly to the database
        //        }
        //    }

        //    db.SaveChanges();

        //    return Json(new { message = "Stock updated successfully" });
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult ValidateStock(List<FinalProductionViewModel> updates)
        //{
        //       if (updates == null || !updates.Any())
        //   {
        //        return Json(new { message = "No updates received.", success = false });
        //    }

        //            foreach (var prod in updates)
        //            {
        //                Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, SubtractValue: {prod.SubtractValue}");

        //                // Find the product by ProductId instead of Name for accuracy
        //                var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);

        //                if (product != null)
        //                {
        //                    Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

        //                    // Subtract stock safely
        //                    product.Stock -= prod.SubtractValue;

        //                    // Ensure stock does not go negative
        //                    if (product.Stock < 0)
        //                    {
        //                        product.Stock = 0;
        //                    }

        //        //            // Mark property as modified
        //                    db.Entry(product).Property(x => x.Stock).IsModified = true;

        //                    Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
        //                }
        //                else
        //                {
        //                    Console.WriteLine($"Product '{prod.Name}' not found in the database.");
        //                    return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
        //                }
        //            }

        //        //    // Save all changes at once
        //            db.SaveChanges();

        //            return Json(new { message = "Stock updated successfully.", success = true });
        //}
        // [HttpPost]
        //public JsonResult ValidateStock(List<FinalProductionViewModel> updates)
        //{
        //    if (updates == null || !updates.Any())
        //    {
        //        return Json(new { message = "No updates received.", success = false });
        //    }

        //    foreach (var prod in updates)
        //    {
        //        Console.WriteLine($"Received Update - ProductId: {prod.ProductId}, Name: {prod.Name}, SubtractValue: {prod.SubtractValue}");

        //        // Find the product by ProductId instead of Name for accuracy
        //        var product = db.Products.FirstOrDefault(p => p.Id == prod.ProductId);

        //        if (product != null)
        //        {
        //            Console.WriteLine($"Before Update - Product: {product.Name}, Stock: {product.Stock}");

        //            // Subtract stock safely
        //            product.Stock -= prod.SubtractValue;

        //            // Ensure stock does not go negative
        //            if (product.Stock < 0)
        //            {
        //                product.Stock = 0;
        //            }

        //            // Mark property as modified
        //            db.Entry(product).Property(x => x.Stock).IsModified = true;

        //            Console.WriteLine($"After Update - Product: {product.Name}, Stock: {product.Stock}");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Product '{prod.Name}' not found in the database.");
        //            return Json(new { message = $"Product '{prod.Name}' not found.", success = false });
        //        }
        //    }

        //    // Save all changes at once
        //    db.SaveChanges();

        //    return Json(new { message = "Stock updated successfully.", success = true });
        //}

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ValidateProduction(int id)
        {
            try
            {
                var production = db.PackagingProductions.Find(id);
                if (production == null)
                {
                    return Json(new { success = false, message = "Production record not found." });
                }

                // Check if already validated - handle nullable Validate field
                if (production.Validate.GetValueOrDefault())
                {
                    return Json(new { success = false, message = "This production is already validated." });
                }

                // Mark as validated
                production.Validate = true;
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Production validated successfully!",
                    redirectUrl = Url.Action("Index", "PackagingProduction")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error validating production: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult PrintOrderPdf(int orderId)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Sale_ReceiptPaperOrder.rdlc");

            // Get data from the new stored procedure
            var data = db.Database.SqlQuery<PaperOrderViewModel>(
                "EXEC [dbo].[sp_GetPackagingProductionDetails] @PackagingProductionId",
                new SqlParameter("@PackagingProductionId", orderId)).ToList();

            if (!data.Any())
            {
                return Content("No data found for this production order.");
            }

            // Add data source to report
            ReportDataSource reportDataSource = new ReportDataSource(
                "DataSet1", // This should match your RDLC dataset name
                data);
            localReport.DataSources.Add(reportDataSource);

            // Set parameters if your report requires any
            // localReport.SetParameters(new ReportParameter("OrderId", orderId.ToString()));

            // Render report
            string mimeType, encoding, fileNameExtension;
            string[] streams;
            Warning[] warnings;

            byte[] renderedBytes = localReport.Render(
                "PDF",
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings
            );

            return File(renderedBytes, mimeType, $"Production_Order_{orderId}.pdf");
        }
        [HttpGet]
        public ActionResult FinalExcess()
        {
            decimal maxId = db.NewProductions.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;  // Passing the suggested BOM ID to the view
            ViewBag.Suppliers = DAL.dbSuppliers;  // Assuming this contains the supplier list

            var products = db.Products
       .Where(p => p.PType == 4)
       .Select(p => new
       {
           Value = p.Id.ToString(),  // ID of the product
           Text = p.Name            // Name of the product
       })
       .ToList();

            ViewBag.ProductList = new SelectList(products, "Value", "Text"); // Pass as a SelectList
            var productDetails = db.ProductDetails.ToList();

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
                SubItems = subItems,
                QuantityToProduce = new List<QuantityToProduce> { new QuantityToProduce() },
                ProductDetail = new List<ProductDetail> { new ProductDetail() },
            };

            return View(viewModel);
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