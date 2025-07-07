using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;
using System.IO;
using System.Web.WebSockets;
using MYBUSINESS.CustomClasses;
using MYBUSINESS.Models;
using System.Runtime.InteropServices.ComTypes;
using System.Net.Mail;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace MYBUSINESS.Controllers
{
    [Authorize(Roles = "Admin,Technical Manager,stock staff,stock manager")]
    public class StoreOrderController : Controller
    {
        private BusinessContext db = new BusinessContext();
        public const string LeavON_Email = "postmaster@phevasoft.com";
        public const string LeavON_Password = "l5wA0w3_[w7";

        [Authorize(Roles = "Admin,Technical Manager,stock staff,stock manager")]
        public ActionResult IndexOrderPP()
        {            
            var orderitemspp = db.OrderPProducts.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList().ToList();
            return View(orderitemspp);
            //return View(DAL.dbBOMs.Where(x => x.SubItems.Count() == 0).ToList());
        }

        [Authorize(Roles = "Admin,Technical Manager,stock staff,stock manager")]
        public ActionResult Index()
        {
            var orders = db.Orders
                .Include(o => o.Store)
                .OrderByDescending(p => p.Id)
                .ToList();

            // Create a dictionary to store which orders have been received
            var receivedOrderIds = db.StoreOrderReceipts
                .Select(ro => ro.OrderId)
                .Distinct()
                .ToHashSet();

            ViewBag.ReceivedOrderIds = receivedOrderIds;

            return View(orders);
        }
        public ActionResult IndexComponents()
        {   
            var orderitems = db.Orders.OrderByDescending(p => p.Id) // Sorting by Id in descending order
                           .ToList();
            return View(orderitems);
            
        }

        [Authorize(Roles = "Admin,Technical Manager,stock staff,stock manager")]
        public ActionResult CreateOrder()
        {
            var finishProducts = db.Products.Where(p => p.PType == 4).ToList();
            var stores = db.Stores.ToList(); // Assuming you have a Stores table

            var groupedProducts = finishProducts
                .GroupBy(p => p.Category)
                .Select(g => new ProductCategoryViewModel
                {
                    CategoryName = g.Key,
                    Products = g.Select(p => new ProductOrderViewModel
                    {
                        ProductId = (int)p.Id,
                        Name = p.Name,
                        Unit = p.Unit
                    }).ToList()
                }).ToList();

            ViewBag.Stores = new SelectList(stores, "Id", "StoreShortName"); // Assuming Store has Id and Name properties
            return View(groupedProducts);
        }

        [Authorize(Roles = "Admin,Technical Manager,stock staff,stock manager")]
        public ActionResult ReceiveOrder(int orderId)
        {
            var order = db.Orders.Include(o => o.Store).FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            var model = new ReceiveOrderViewModel
            {
                OrderId = order.Id,
                StoreName = order.Store?.Name,
                StoreId = order.StoreId ?? 0 // or any default/fallback value

            };

            return View(model);
        }

        public ActionResult ReceiveOrderPP(int orderId)
        {
            var order = db.OrderPProducts.Include(o => o.Store).FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            var model = new ReceiveOrderViewModel
            {
                OrderId = order.Id,
                StoreName = order.Store?.Name,
                StoreId = order.StoreId ?? 0 // or any default/fallback value

            };

            return View(model);
        }
        [Authorize(Roles = "Admin,Technical Manager,stock staff,stock manager")]
        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult ReceiveOrder(ReceiveOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check for existing receipt to prevent duplicates
            var existingReceipt = db.StoreOrderReceipts.FirstOrDefault(r => r.OrderId == model.OrderId);
            if (existingReceipt != null)
            {
                ModelState.AddModelError("", "This order has already been received.");
                return View(model);
            }

            // Begin transaction to ensure atomicity
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Step 1: Save the receipt
                    var receipt = new StoreOrderReceipt
                    {
                        OrderId = model.OrderId,
                        StoreId = model.StoreId,
                        UniqueCode = model.ReceivedCode
                    };
                    db.StoreOrderReceipts.Add(receipt);

                    // Step 2: Fetch Order Items
                    var orderItems = db.OrderItems.Where(oi => oi.OrderId == model.OrderId).ToList();

                    // Step 3: Update product stock
                    foreach (var item in orderItems)
                    {
                        var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            product.Stock += item.Quantity;
                        }
                    }

                    // Step 4: Save all changes
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["SuccessMessage"] = "Order received and stock updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Error receiving order: " + ex.Message);
                    return View(model);
                }
            }
        }

        [ValidateAntiForgeryToken]
        public ActionResult ReceiveOrderPPPost(ReceiveOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check for existing receipt to prevent duplicates
            var existingReceipt = db.StoreOrderReceiptPPs.FirstOrDefault(r => r.OrderId == model.OrderId);
            if (existingReceipt != null)
            {
                ModelState.AddModelError("", "This order has already been received.");
                return View(model);
            }

            // Begin transaction to ensure atomicity
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Step 1: Save the receipt
                    var receipt = new StoreOrderReceiptPP
                    {
                        OrderId = model.OrderId,
                        StoreId = model.StoreId,
                        UniqueCode = model.ReceivedCode
                    };
                    db.StoreOrderReceiptPPs.Add(receipt);

                    // Step 2: Fetch Order Items
                    var orderItems = db.OrderItemPProducts.Where(oi => oi.OrderId == model.OrderId).ToList();

                    // Step 3: Update product stock
                    foreach (var item in orderItems)
                    {
                        var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            product.Stock += item.Quantity;
                        }
                    }

                    // Step 4: Save all changes
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["SuccessMessage"] = "Order received and stock updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Error receiving order: " + ex.Message);
                    return View(model);
                }
            }
        }


        public static void SendOrderNotificationEmail(Order order, string receiverEmail)
        {
            const string smtpServer = "mail5017.site4now.net";
            const int smtpPort = 587;
            const string smtpUsername = "postmaster@phevasoft.com";
            const string smtpPassword = "l5wA0w3_[w7"; // Replace with actual password
            const string fromEmail = "postmaster@phevasoft.com";
            const string fromName = "Phevasoft Orders";

            try
            {
                // 1. Configure security settings
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // 2. Bypass certificate validation (temporary for testing)
                ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, errors) => true;

                // 3. Create email with essential headers
                var mail = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = $"New Order Created - Order ID: {order.Id}",
                    Body = $"Dear Manager,\n\nA new store order has been created.\n\n" +
                           $"Order ID: {order.Id}\n" +
                           $"Store ID: {order.StoreId}\n" +
                           $"Order Date: {order.OrderDate}\n\n" +
                           $"Please check the system for more details.\n\n" +
                           $"This is a system-generated email, please do not reply.",
                    IsBodyHtml = false
                };
                mail.To.Add(receiverEmail);

                // Critical headers to pass spam filter
                mail.Headers.Add("X-Mailer", "ASP.NET MVC");
                mail.Headers.Add("X-Auto-Response-Suppress", "All");
                mail.Headers.Add("Auto-Submitted", "auto-generated");

                // 4. Configure SMTP client with EXACT settings
                using (var smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Timeout = 30000; // 30 seconds

                    // 5. Send with detailed error handling
                    smtp.Send(mail);
                }
            }
            catch (SmtpException smtpEx)
            {
                // Special handling for SMTP-specific errors
                string errorDetails = $"[{DateTime.Now}] SMTP Error (Status: {smtpEx.StatusCode}):\n" +
                                    $"Message: {smtpEx.Message}\n" +
                                    $"Full Response: {GetFullSmtpResponse(smtpEx)}\n";

                LogError(errorDetails);
                throw new ApplicationException("Failed to send order notification email", smtpEx);
            }
            catch (Exception ex)
            {
                LogError($"[{DateTime.Now}] General Error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback = null;
            }
        }

        private static string GetFullSmtpResponse(SmtpException ex)
        {
            try
            {
                var responseField = typeof(SmtpException).GetField("_statusResponse",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                return responseField?.GetValue(ex)?.ToString() ?? "No response available";
            }
            catch
            {
                return "Unable to retrieve full SMTP response";
            }
        }

        private static void LogError(string message)
        {
            try
            {
                string logPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/email_errors.log");
                System.IO.File.AppendAllText(logPath, message + "\n\n");

                // Also log to Windows Event Viewer
                System.Diagnostics.EventLog.WriteEntry(
                    "Application",
                    $"Email Error: {message}",
                    System.Diagnostics.EventLogEntryType.Error);
            }
            catch { /* Ensure logging doesn't throw */ }
        }

        public ActionResult CreatePackagingOrder()
        {
            var packagingProducts = db.Products.Where(p => p.PType == 8 || p.PType == 9).ToList();
            var stores = db.Stores.ToList();

            // Make sure to assign the SelectList to ViewBag.Stores
            ViewBag.Stores = new SelectList(stores, "Id", "StoreShortName");

            return View(packagingProducts);
        }
        
        [HttpPost]
        public ActionResult SubmitOrder(List<ProductCategoryViewModel> categories, int storeId)
        {
            // Filter out items where Quantity is null or <= 0
            var itemsToSave = categories
        .SelectMany(c => c.Products)
        .Where(p => p.Quantity.HasValue && p.Quantity > 0)
        .Select(p => new OrderItem
        {
            ProductId = p.ProductId,
            Quantity = p.Quantity.Value
        })
        .ToList();

            if (!itemsToSave.Any())
            {
                TempData["ErrorMessage"] = "No valid products selected (quantity must be > 0).";
                return RedirectToAction("CreateOrder");
            }

            var order = new Order
            {
                StoreId = storeId, // Use the selected store ID
                OrderDate = DateTime.Now
            };

            db.Orders.Add(order);
            db.SaveChanges(); // Save order first to get ID

            // Only save items with Quantity > 0
            foreach (var item in itemsToSave)
            {
                db.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            db.SaveChanges();

            // Send email notification (unchanged)
            string receiverEmail = "zeeshannaveed893@gmail.com";
            SendOrderNotificationEmail(order, receiverEmail);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SubmitOrderPP(List<OrderItemPProduct> orderitem, int storeId)
        {
            // Filter out items where Quantity is null or <= 0
            var itemsToSave = orderitem?
                .Where(item => item.Quantity > 0)
                .ToList() ?? new List<OrderItemPProduct>();

            if (!itemsToSave.Any())
            {
                TempData["ErrorMessage"] = "No valid products selected (quantity must be > 0).";
                return RedirectToAction("CreateOrderPP"); // Make sure this matches your create action name
            }

            var order = new OrderPProduct
            {
                StoreId = storeId, // Use the passed storeId parameter instead of hardcoded value
                OrderDate = DateTime.Now
            };

            db.OrderPProducts.Add(order);
            db.SaveChanges(); // Save order first to get ID

            // Only save items with Quantity > 0
            foreach (var item in itemsToSave)
            {
                db.OrderItemPProducts.Add(new OrderItemPProduct
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            db.SaveChanges();

            string receiverEmail = "zeeshannaveed893@gmail.com";
            SendOrderNotificationEmail(order, receiverEmail);

            return RedirectToAction("IndexOrderPP");
        }
        // Updated email method
        private void SendOrderNotificationEmail(OrderPProduct order, string email)
        {
            // Your email sending logic here
            // Use order.StoreId, order.OrderDate etc.
        }

        [HttpGet]
        public ActionResult PrintOrderPdf(int orderId)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Sale_ReceiptOrder.rdlc");

            // Get data from stored procedure
            var data = db.Database.SqlQuery<OrderReceiptViewModel>(
                "EXEC spGetOrderProductDetails @OrderId",
                new SqlParameter("@OrderId", orderId)).ToList();

            if (!data.Any())
            {
                return Content("No data found.");
            }

            // Add data source to report
            //ReportDataSource reportDataSource = new ReportDataSource(
            //    "spGetOrderProductDetailsDataSet",
            //    data);
            ReportDataSource reportDataSource = new ReportDataSource(
    "DataSet1",
    data);
            localReport.DataSources.Add(reportDataSource);

            // ✅ Set required report parameter
            //localReport.SetParameters(new ReportParameter("OrderId", orderId.ToString()));

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

            return File(renderedBytes, mimeType, $"Order_Receipt_{orderId}.pdf");
        }
        [HttpGet]
        public ActionResult PrintOrderPPdf(int orderId)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Sale_ReceiptOrderPP.rdlc");

            // Get data from stored procedure
            var data = db.Database.SqlQuery<OrderReceiptViewModel>(
                "EXEC spGetOrderPProductDetails @OrderId",
                new SqlParameter("@OrderId", orderId)).ToList();

            if (!data.Any())
            {
                return Content("No data found.");
            }

            // Add data source to report
            //ReportDataSource reportDataSource = new ReportDataSource(
            //    "spGetOrderProductDetailsDataSet",
            //    data);
            ReportDataSource reportDataSource = new ReportDataSource(
    "DataSet1",
    data);
            localReport.DataSources.Add(reportDataSource);

            // ✅ Set required report parameter
            //localReport.SetParameters(new ReportParameter("OrderId", orderId.ToString()));

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

            return File(renderedBytes, mimeType, $"Order_Receipt_{orderId}.pdf");
        }

        public ActionResult EditOrderPP(int orderId, bool? readonlyMode = false)
        {
            var orderItems = db.OrderItemPProducts
                               .Where(oi => oi.OrderId == orderId)
                               .ToList();

            if (!orderItems.Any())
            {
                return HttpNotFound("Order Items not found.");
            }

            // ✅ Set ViewBag.Stores like you did in CreatePackagingOrder
            var stores = db.Stores.ToList();
            ViewBag.Stores = new SelectList(stores, "Id", "StoreShortName");

            // ✅ Set current store ID to show selected store in readonly mode
            ViewBag.CurrentStoreId = db.Stores
                                       .Where(o => o.Id == orderId)
                                       .Select(o => o.Id)
                                       .FirstOrDefault();

            ViewBag.ReadonlyMode = readonlyMode;

            return View(orderItems);
        }




        public ActionResult Edit(int orderId, bool? readonlyMode = false)
        {
            // Get the order first to know which store it belongs to
            var order = db.Orders.Include(o => o.Store).FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return HttpNotFound("Order not found.");
            }

            // If order is validated, force readonly mode
            if (order.Validate == true)
            {
                readonlyMode = true;
            }

            var orderItems = db.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .ToList();

            if (!orderItems.Any())
            {
                return HttpNotFound("Order Items not found.");
            }

            var grouped = orderItems
                .GroupBy(oi => oi.Product.Category)
                .Select(g => new EditOrderCategoryViewModel
                {
                    CategoryName = g.Key,
                    Items = g.Select(oi => new OrderItemViewModel
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId.HasValue ? (int)oi.ProductId.Value : 0,
                        OrderId = oi.OrderId ?? 0,
                        Quantity = oi.Quantity.HasValue ? (int)oi.Quantity.Value : 0,
                        ProductName = oi.Product.Name,
                        Unit = oi.Product.Unit
                    }).ToList()
                }).ToList();

            // Get all stores for dropdown
            var stores = db.Stores.ToList();
            ViewBag.Stores = new SelectList(stores, "Id", "Name", order.StoreId);
            ViewBag.ReadonlyMode = readonlyMode;
            ViewBag.CurrentStoreId = order.StoreId;
            ViewBag.OrderId = orderId;
            ViewBag.IsValidated = order.Validate;

            return View(grouped);
        }

        //[HttpPost]
        //public ActionResult Edit(List<OrderItem> updatedItems)
        //{
        //    if (updatedItems == null || !updatedItems.Any())
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    foreach (var updatedItem in updatedItems)
        //    {
        //        var existingItem = db.OrderItems.Find(updatedItem.Id);
        //        if (existingItem != null)
        //        {
        //            existingItem.Quantity = updatedItem.Quantity;
        //            // If needed, allow updating ProductId too
        //        }
        //    }

        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public ActionResult Edit(List<OrderItem> items, int storeId)
        {
            if (items == null || !items.Any())
            {
                return RedirectToAction("Index");
            }

            // Get the first item to find the order ID
            var firstItem = items.First();
            var order = db.Orders.Find(firstItem.OrderId);

            if (order != null)
            {
                // Update store if changed
                if (order.StoreId != storeId)
                {
                    order.StoreId = storeId;
                    db.Entry(order).State = EntityState.Modified;
                }
            }

            foreach (var updatedItem in items)
            {
                var existingItem = db.OrderItems.Find(updatedItem.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity = updatedItem.Quantity;
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult EditOrderPP(List<OrderItemPProduct> updatedItems)
        {
            if (updatedItems == null || !updatedItems.Any())
            {
                return RedirectToAction("Index");
            }

            foreach (var updatedItem in updatedItems)
            {
                var existingItem = db.OrderItemPProducts.Find(updatedItem.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity = updatedItem.Quantity;
                    // If needed, allow updating ProductId too
                }
            }

            db.SaveChanges();
            return RedirectToAction("IndexOrderPP");
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Validation(List<OrderItemsViewModel> LstProductionVM)
        {
            if (LstProductionVM == null || !LstProductionVM.Any())
            {
                return Json(new { success = false, message = "No items to validate." });
            }

            int orderId = LstProductionVM.First().OrderId;

            var order = db.Orders.Find(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            foreach (var item in LstProductionVM)
            {
                var product = db.Products.Find(item.ProductId);
                var orderItem = db.OrderItems.FirstOrDefault(oi => oi.Id == item.Id);

                if (product != null && orderItem != null)
                {
                    if (product.Stock < item.Quantity)
                    {
                        return Json(new { success = false, message = $"Insufficient stock for {product.Name}." });
                    }

                    // Subtract quantity from product stock
                    product.Stock -= item.Quantity;

                    // Update quantity if needed
                    orderItem.Quantity = item.Quantity;
                }
            }

            // Mark order as validated
            order.Validate = true;
            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Order validated successfully!",
                redirectUrl = Url.Action("Index", "StoreOrder")
            });
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ValidationPP(List<OrderItemsViewModel> LstProductionVM)
        {
            if (LstProductionVM == null || !LstProductionVM.Any())
            {
                return Json(new { success = false, message = "No items to validate." });
            }

            int orderId = LstProductionVM.First().OrderId;

            // Get the parent order (from OrderPProducts table)
            var order = db.OrderPProducts.Find(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            // Check if order is already validated
            if (order.Validate == true)
            {
                return Json(new { success = false, message = "Order is already validated." });
            }

            foreach (var item in LstProductionVM)
            {
                var product = db.Products.Find(item.ProductId);
                var orderItem = db.OrderItemPProducts.FirstOrDefault(oi => oi.Id == item.Id);

                if (product != null && orderItem != null)
                {
                    // For packaging products, you might want different validation logic
                    if (product.PType == 4) // Assuming 4 is packaging products type
                    {
                        if (product.Stock < item.Quantity)
                        {
                            return Json(new
                            {
                                success = false,
                                message = $"Insufficient stock for {product.Name}. Available: {product.Stock}, Requested: {item.Quantity}"
                            });
                        }

                        // Subtract quantity from product stock
                        product.Stock -= item.Quantity;
                    }

                    // Update quantity if needed
                    orderItem.Quantity = item.Quantity;
                }
                else
                {
                    return Json(new { success = false, message = "Product or order item not found." });
                }
            }

            // Mark order as validated
            order.Validate = true;
            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Order validated successfully!",
                redirectUrl = Url.Action("IndexOrderPP", "StoreOrder")
            });
        }


        public static void TestEmail()
        {
            try
            {
                var mail = new MailMessage();
                var smtp = new SmtpClient("mail.phevasoft.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("postmaster@phevasoft.com", "yourpassword"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                mail.From = new MailAddress("postmaster@phevasoft.com");
                mail.To.Add("zeeshannaveed893@gmail.com");
                mail.Subject = "TEST EMAIL";
                mail.Body = "This is a test email sent at " + DateTime.Now;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                // Log detailed error
            }
        }


        private string BuildErrorMessage(SmtpException ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"SMTP Error at {DateTime.Now}");
            sb.AppendLine($"Status: {ex.StatusCode}");
            sb.AppendLine($"Message: {ex.Message}");

            if (ex.InnerException != null)
            {
                sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
            }

            sb.AppendLine($"Stack Trace: {ex.StackTrace}");
            return sb.ToString();
        }

        //private void LogError(string message)
        //{
        //    try
        //    {
        //        string logDir = Server.MapPath("~/App_Data/Logs");
        //        if (!Directory.Exists(logDir))
        //        {
        //            Directory.CreateDirectory(logDir);
        //        }

        //        string logPath = Path.Combine(logDir, "email_errors.log");
        //        //File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n\n");
        //    }
        //    catch
        //    {
        //        // Silent fail - we don't want logging failures to interrupt the main flow
        //    }
        //}

        [HttpPost]
        public async Task<JsonResult> GenerateInvoice(int orderId, string authToken)
        {
            try
            {
                // Get the order details
                var order = db.Orders.Include(o => o.Store).FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                {
                    return Json(new { Success = false, Message = "Order not found" });
                }

                // Get order items
                var orderItems = db.OrderItems
                                  .Where(oi => oi.OrderId == orderId)
                                  .Include(oi => oi.Product)
                                  .ToList();

                // Create a dummy customer (modify as needed)
                var customer = new Customer
                {
                    Name = order.Store?.Name ?? "Default Customer",
                    Email = order.Store?.Email ?? "default@example.com",
                    //TaxCode = order.Store?.VatNumber ?? "0000000000"
                };

                // Call the API
                var response = await AddWebServiceCustomerDetails(authToken, customer, order, orderItems, order.Store);

                return Json(new
                {
                    Success = response.Success,
                    Message = response.Message,
                    Data = response.Data,
                    PdfUrl = response.Data?.PdfUrl
                });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error: " + ex.Message });
            }
        }

        public async Task<ActionResult> DownloadInvoicePdf(string invoiceId)
        {
            try
            {
                var pdfUrl = $"https://4001137910.minvoice.com.vn/api/InvoiceApi78/PrintInvoice?id={invoiceId}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(pdfUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        return File(stream, "application/pdf", $"Invoice_{invoiceId}.pdf");
                    }
                    return Content("Failed to download invoice PDF");
                }
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }
        public async Task<JsonResult> LoginToWebServiceAsync()
        {
            try
            {
                // Enforce TLS 1.2 or TLS 1.3
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                using (var client = new HttpClient())
                {
                    // Define the login endpoint URL (replace with the actual API endpoint)
                    string url = "https://4001137910.minvoice.com.vn/api/Account/Login"; // Replace with the actual endpoint

                    // Create the request body with the necessary parameters
                    var requestBody = new
                    {
                        //username = "PHEVA",
                        username = "SOFT",
                        password = "l_}4+G9c%VE0",
                        //password = "2BM@g0J%5sguJ@",
                        ma_dvcs = "VP"
                    };

                    // Convert the request body to JSON format
                    string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

                    // Set up the HttpClient and request headers
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                    // Send a POST request with the JSON request body to get the token
                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response body as a string
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Check if the response is in JSON format
                        if (response.Content.Headers.ContentType?.MediaType == "application/json")
                        {
                            // Parse the response JSON to extract the token
                            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                            string token = jsonResponse.token;

                            if (!string.IsNullOrEmpty(token))
                            {
                                // Store the token (e.g., in session or other storage)
                                Session["AuthToken"] = token;

                                // Success message or subsequent requests
                                return Json(new { Success = true, Token = token });
                            }
                            else
                            {
                                // Handle case where token is null or empty
                                return Json(new { Success = false, Message = "Token is null or empty" });
                            }
                        }
                        else
                        {
                            // Log or handle non-JSON response
                            return Json(new { Success = false, Message = "Unexpected response format: " + responseBody });
                        }
                    }
                    else
                    {
                        // Handle error responses
                        string errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new { Success = false, Message = $"Failed to log in. Status code: {response.StatusCode}. Response: {errorContent}" });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                System.Diagnostics.Debug.WriteLine("Exception occurred in LoginToWebService: " + ex.Message);
                return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }
        }
        public async Task<WebServiceResponse> AddWebServiceCustomerDetails(string authToken, Customer cust, Order order, List<OrderItem> orderItemDetails, Store store)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                //return Json(new { Success = false, Message = "Invalid token" });
            }

            // Check for null references in the input parameters
            if (cust == null)
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                //return Json(new { Success = false, Message = "Customer information is missing." });
            }

            if (order == null)
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                //return Json(new { Success = false, Message = "Sale order information is missing." });
            }

            if (orderItemDetails == null || orderItemDetails.Count == 0)
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                //return Json(new { Success = false, Message = "Sale order details are missing." });
            }

            try
            {
                var tax = 8;/*db.MyBusinessInfoes.TaxInPercent*/
                //var soId = db.SODs.Select(d => d.SOId== saleOrder.Id).FirstOrDefault();
                // var dbsaleOrderDetails = db.SODs
                // .Include(d => d.Product)
                // .Where(d => d.SOId == saleOrder.Id)
                //.ToList();

                foreach (OrderItem sod in orderItemDetails)
                {
                    sod.Product = db.Products.FirstOrDefault(x => x.Id == sod.ProductId);
                }

                string url = "https://4001137910.minvoice.com.vn/api/InvoiceApi78/SaveV2"; //Real url working test env 
                //string url = "https://0106026495-998.minvoice.pro/api/InvoiceApi780/Save"; //To check if webservice down / do not respond

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                    var getCurrentYear = DateTime.Now.Year; // Gets the current year (e.g., 2024)
                    var lastTwoDigits = getCurrentYear % 100; // Extracts the last two digits (e.g., 24)
                    var lastTwoDigitsString = lastTwoDigits.ToString("D2");

                    // Safely create the request model
                    //var invoiceDetails = saleOrderDetails
                    //    .Where(detail => detail != null && detail.Product != null)
                    //    .Select(detail => new InvoiceDetail
                    //    {
                    //        tchat = 1,
                    //        stt_rec0 = 4,// detail.SODId ?? 0,
                    //        inv_itemCode = "CB005",//detail.Product != null ? "CB001" : "", // Default value if Product is null
                    //        inv_itemName = "Chocolatebox001", //detail.Product?.Name ?? "Unknown Product", // Default value if Product.Name is null
                    //        inv_unitCode = "Box",//detail.Product != null ? "Box/Pack" : "Unknown",
                    //        inv_quantity = 1,//detail.Quantity ?? 0,
                    //        inv_unitPrice = 12000, //detail.SalePrice ?? 0m,
                    //        inv_discountPercentage = 0,
                    //        inv_discountAmount = 0,
                    //        inv_TotalAmountWithoutVat = 120000, //saleOrder.BillAmount,//saleOrder.BillAmount ?? 0 // Assuming BillAmount is required
                    //        ma_thue = 8,
                    //        inv_vatAmount = 9600,
                    //        inv_TotalAmount = 129600
                    //    })
                    //    .ToList();

                    var invoiceDetails = orderItemDetails//dbsaleOrderDetails
                        .Where(detail => detail != null && detail.ProductId != null &&
                    detail.Product.WholeSalePrice.HasValue)
                        .Select((detail, index) => new InvoiceDetail

                        {

                            tchat = 1,
                            stt = index + 1, // Ensure dynamic property exists
                            //stt = (int?)detail.ProductId,
                            /*ma = detail.Product?.EInvoicePCode?? "",*/ // Handle null Product gracefully
                            ma = detail.Product.EInvoicePCode ?? "",
                            ten = detail.Product?.Name ?? "",
                            dvtinh = detail.Product?.Unit ?? "", // Fix: Use null-conditional operator
                            sluong = (int)detail.Quantity, // Ensure Quantity is not null
                            dgia = detail.Product.WholeSalePrice ?? 0, // Ensure SalePrice is not null
                            tlckhau = 0,
                            ttcktmai = 0,
                            stckhau = 0,
                            thtien = (decimal)((detail.Quantity) * (detail.Product.WholeSalePrice)), // Ensure null handling
                            tsuat = tax, // Tax code
                            tthue = (decimal)((detail.Quantity) * (detail.Product.WholeSalePrice) * tax / 100m), // Ensure null handling
                            tgtien = (decimal)(((detail.Quantity) * (detail.Product.WholeSalePrice)) + ((detail.Quantity ?? 1) * (detail.Product.WholeSalePrice) * tax) / 100),

                        })
                       .ToList();
                    var totalAmountWithoutVat = invoiceDetails.Sum(d => d.thtien);
                    var totalVatAmount = invoiceDetails.Sum(d => d.tthue);
                    var totalAmountWithVat = invoiceDetails.Sum(d => d.tgtien);
                    // Ensure saleOrder properties are not null before using them
                    var invoice = new Invoice
                    {
                        //inv_invoiceSeries = saleOrder.InvoiceSeries ?? "1C24MPE", // Default value if InvoiceSeries is null
                        khieu = $"1C{lastTwoDigitsString}TES",//"1C24MPE", // Default value if InvoiceSeries is null Invoice symobol fo receipt
                        tdlap = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd HH:mm:ss"),//saleOrder.InvoiceIssuedDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddDays(13).ToString("yyyy-MM-dd"),
                        dvtte = "VND",//saleOrder.CurrencyCode ?? "VND",
                        tgia = 1,//saleOrder.ExchangeRate ?? 1,
                        //sdhang = saleOrder.Id,//"HN-20242309-002", //saleOrder.SOSerial ?? "HN-20241509-001",
                        //sdhang = $"{saleOrder.Id}", /*-{DateTime.UtcNow.Ticks}*/
                        sdhang = $"{order.Id}-{DateTime.UtcNow.Ticks}",
                        //sdhang = $"{saleOrder.Id}",
                        //tnmua = cust.Name,
                        ten = store.CompanyName,
                        mst = store.CompanyVatNumber,/*saleOrder.SOSerial.ToString()*///"0401485182",//cust.TaxCode ?? "0401485182",
                        dchi = store.CompanyAddress,
                        //inv_buyerEmail = cust.Email ?? "unknown@example.com",
                        email = store.Email,
                        htttoan = "TM/CK",
                        //ttcktmai = (decimal)order.Discount,
                        tgtcthue = totalAmountWithoutVat,/*(decimal)order.Product.WholeSalePrice / (1 + tax / 100),*///saleOrder.TotalAmountWithoutVat ?? 0,
                        tgtthue = totalVatAmount/*(decimal)(order.SaleOrderAmount * tax / 100m)*/,

                        tgtttbso = totalAmountWithVat/*(decimal)order.SaleOrderAmountWithVaT*/,//saleOrder.BillPaid ?? 0, = saleOrder.BillAmount,//saleOrder.BillPaid ?? 0, = saleOrder.BillAmount,//saleOrder.BillPaid ?? 0,
                        /*key_API = saleOrder.Id,*///"HN-20242309-002",//saleOrder.ApiKey ?? "HN-20241509-001",

                        details = new List<InvoiceDetailsWrapper> { new InvoiceDetailsWrapper { data = invoiceDetails } }
                    };

                    var requestBody = new InvoiceRequest
                    {
                        editmode = 1,
                        data = new List<Invoice> { invoice }
                    };

                    string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

                    // Sending the request to the web service
                    //HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json"));
                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, Encoding.UTF8, "application/json"));


                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        if (response.Content.Headers.ContentType.MediaType == "application/json")
                        {
                            try
                            {
                                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

                                // Check if deserialization failed
                                if (jsonResponse == null)
                                {
                                    return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                                    //return Json(new { Success = false, Message = "Failed to parse the response." });
                                }
                                //StaticDto.Data = jsonResponse;
                                // Check if 'code' property exists and is not null
                                if (jsonResponse?.code != null && jsonResponse.code == "00")
                                {
                                    string invoiceId = jsonResponse.data?.hoadon68_id;
                                    string pdfUrl = $"https://4001137910.minvoice.com.vn/api/InvoiceApi78/PrintInvoice?id={invoiceId}";
                                    return new WebServiceResponse
                                    {
                                        Success = true,
                                        Message = jsonResponse.message,
                                        Data = /*jsonResponse*/ new
                                        {
                                            InvoiceData = jsonResponse,
                                            PdfUrl = pdfUrl,
                                            InvoiceNumber = jsonResponse.data?.shdon
                                        }
                                    };
                                }
                                else
                                {
                                    return new WebServiceResponse
                                    {
                                        Success = false,
                                        Message = "Unexpected response code.",
                                        Data = jsonResponse
                                    };
                                }
                                //if (jsonResponse?.code != null && jsonResponse.code == "00")
                                //{
                                //    // Store the serialized JSON string in TempData
                                //    string jsonResponseString = JsonConvert.SerializeObject(jsonResponse);
                                //    //Session["JsonResponseWebservice"] = jsonResponseString;
                                //    TempData["JsonResponseWebservicess"] = jsonResponseString;
                                //    //ViewBag.LLLL = TempData["JsonResponseWebservicess"];

                                //    // Retrieve the TempData value as a dynamic object
                                //    dynamic tempDataResponse = JsonConvert.DeserializeObject(TempData["JsonResponseWebservicess"].ToString());
                                //    //StaticDto.Data = tempDataResponse;
                                //    //_jsonResponseWebservice = JsonConvert.DeserializeObject(responseBody);

                                //    // Now you can use tempDataResponse
                                //    return Json(new { Success = true, Message = jsonResponse.message, Data = tempDataResponse });
                                //}
                                //else
                                //{
                                //    return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
                                //}
                                //dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                                //// Check if deserialization failed
                                //if (jsonResponse == null)
                                //{
                                //    return Json(new { Success = false, Message = "Failed to parse the response." });
                                //}

                                //// Check if 'code' property exists and is not null
                                //if (jsonResponse?.code != null && jsonResponse.code == "00")
                                //{
                                //    string jsonResponseString = JsonConvert.SerializeObject(jsonResponse);
                                //    // Store the serialized JSON string in TempData
                                //    TempData["JsonResponseWebservice"] = jsonResponseString;
                                //    // Optional: If you want to retrieve it as a dynamic object later
                                //    dynamic tempDataResponse = JsonConvert.DeserializeObject(TempData["JsonResponseWebservice"].ToString());
                                //    //string jsonResponseString = JsonConvert.SerializeObject(jsonResponse);
                                //    //TempData["JsonResponseWebservice"] = jsonResponse;
                                //    //string abc =(string) TempData["JsonResponseWebservice"];
                                //    //TempData["_JsonResponseWebservice"] = TempData["JsonResponseWebservice"];
                                //    return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse });
                                //    //return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
                                //}
                                //else
                                //{
                                //    return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
                                //}

                                //if (jsonResponse.code == "00")
                                //{
                                //    // Store jsonResponse in TempData
                                //    Session["JsonResponse"] = jsonResponse;
                                //    return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
                                //}
                                //else
                                //{
                                //    return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
                                //}
                            }
                            catch (JsonException jsonEx)
                            {
                                return new WebServiceResponse { Success = false, Message = "Request failed with status code: " + response.StatusCode };
                                // return Json(new { Success = false, Message = "Error parsing JSON response.", Error = jsonEx.Message });
                            }
                        }
                        else
                        {
                            return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                            //return Json(new { Success = false, Message = "Received non-JSON response.", Response = responseBody });
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        TempData["WebserviceDownError"] = "Sever is down VAT Invoice cannot print at this time";
                        return new WebServiceResponse { Success = false, Message = "An error occurred: " };
                        //return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return new WebServiceResponse { Success = false, Message = "An error occurred: " + ex.Message };
                //return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }


            //if (string.IsNullOrEmpty(authToken))
            //{
            //    return Json(new { Success = false, Message = "Invalid token" });
            //}

            //try
            //{
            //    string url = "https://0106026495-998.minvoice.pro/api/InvoiceApi78/Save";

            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            //        // Creating the request model
            //        var invoiceDetails = saleOrderDetails.Select(detail => new InvoiceDetail
            //        {
            //            tchat = 1,
            //            stt_rec0 = detail.SODId ?? 0, // Use null-coalescing operator to handle null values
            //            inv_itemCode = "CB001",//detail.Product != null ? "CB001" : "", // Provide a default value if Product is null
            //            inv_itemName = detail.Product?.Name ?? "Unknown Product", // Use null-conditional operator and provide a default value
            //            inv_unitCode = detail.Product != null ? "Box/Pack" : "Unknown", // Check if Product is not null
            //            inv_quantity = detail.Quantity ?? 0, // Provide a default value if Quantity is null
            //            inv_unitPrice = detail.PurchasePrice ?? 0m, // Provide a default value if PurchasePrice is null
            //            inv_discountPercentage = 0, // Detail-specific value or default
            //            inv_discountAmount = 20, // Provide a fixed value or calculate if needed
            //            inv_TotalAmountWithoutVat = 220000,//saleOrder.BillAmount ?? 0, // Provide a default value if BillAmount is null
            //            ma_thue = 8, // Detail-specific value
            //            inv_vatAmount = 0, // Detail-specific value
            //            inv_TotalAmount = 0 // Detail-specific value
            //        }).ToList();

            //        var invoice = new Invoice
            //        {
            //            inv_invoiceSeries = "1C24MPE",
            //            inv_invoiceIssuedDate = DateTime.Now.AddDays(13).ToString("yyyy-MM-dd"),//DateTime.Now.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture), //saleOrder.InvoiceIssuedDate.ToString("yyyy-MM-dd"),
            //            inv_currencyCode = "VND",//saleOrder.CurrencyCode,
            //            inv_exchangeRate = 1,//saleOrder.ExchangeRate,
            //            so_benh_an = "HN-20241509-001", //saleOrder.SOSerial.ToString(),
            //            inv_buyerDisplayName = cust.Name,
            //            inv_buyerLegalName = cust.Name,
            //            inv_buyerTaxCode = "0401485182", //cust.TaxCode,
            //            inv_buyerAddressLine = cust.Address,
            //            inv_buyerEmail = cust.Email,
            //            inv_paymentMethodName = "TM/CK", //saleOrder.PaymentMethod,
            //            inv_discountAmount = saleOrder.Discount,
            //            inv_TotalAmountWithoutVat = 0,//saleOrder.TotalAmountWithoutVat,
            //            inv_vatAmount = 0,//saleOrder.VatAmount,
            //            inv_TotalAmount = saleOrder.BillPaid,
            //            key_api = "HN-20241509-001", //saleOrder.ApiKey,
            //            details = new List<InvoiceDetailsWrapper> { new InvoiceDetailsWrapper { data = invoiceDetails } }
            //        };

            //        var requestBody = new InvoiceRequest
            //        {
            //            editmode = 1,
            //            data = new List<Invoice> { invoice }
            //        };

            //        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

            //        // Sending the request to the web service
            //        HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json"));

            //        if (response.IsSuccessStatusCode)
            //        {
            //            string responseBody = await response.Content.ReadAsStringAsync();

            //            if (response.Content.Headers.ContentType.MediaType == "application/json")
            //            {
            //                try
            //                {
            //                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

            //                    // Check for the "code" field in the response to determine success
            //                    if (jsonResponse.code == "00")
            //                    {
            //                        return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
            //                    }
            //                    else
            //                    {
            //                        return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
            //                    }
            //                }
            //                catch (JsonException jsonEx)
            //                {
            //                    return Json(new { Success = false, Message = "Error parsing JSON response.", Error = jsonEx.Message });
            //                }
            //            }
            //            else
            //            {
            //                return Json(new { Success = false, Message = "Received non-JSON response.", Response = responseBody });
            //            }
            //        }
            //        else
            //        {
            //            string errorContent = await response.Content.ReadAsStringAsync();
            //            return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception or handle it as needed
            //    return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            //}



            //if (string.IsNullOrEmpty(authToken))
            //{
            //    return Json(new { Success = false, Message = "Invalid token" });
            //}

            //try
            //{
            //    string url = "https://0106026495-998.minvoice.pro/api/InvoiceApi78/Save"; // Endpoint to add customer details

            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            //        var requestBody = new
            //        {
            //            inv_invoiceSeries = "1C24MPE ",
            //            inv_invoiceIssuedDate = "2024-29-08",
            //            inv_currencyCode = "VND",
            //            inv_exchangeRate = 1,
            //            so_benh_an = "HN-20242908-001",
            //            inv_buyerDisplayName = "Mister XYZ",
            //            inv_buyerLegalName = "company A",
            //            inv_buyerTaxCode = "0401485182",
            //            inv_buyerAddressLine = "Company A address",
            //            inv_buyerEmail = "companya@gmail.com",
            //            inv_buyerBankAccount = "",
            //            inv_buyerBankName = "",
            //            inv_paymentMethodName = "TM/CK",
            //            inv_discountAmount = 0,
            //            inv_TotalAmountWithoutVat = 220000,
            //            inv_vatAmount = 17600,
            //            inv_TotalAmount = 237600,
            //            key_api = "HN-20242908-001",
            //        };

            //        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

            //        // Make sure to use await and async
            //        HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json"));

            //        if (response.IsSuccessStatusCode)
            //        {
            //            string responseBody = await response.Content.ReadAsStringAsync();

            //            if (response.Content.Headers.ContentType.MediaType == "application/json")
            //            {
            //                try
            //                {
            //                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

            //                    // Check for the "code" field in the response to determine success
            //                    if (jsonResponse.code == "00")
            //                    {
            //                        return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
            //                    }
            //                    else
            //                    {
            //                        return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
            //                    }
            //                }
            //                //catch (Exception ex)
            //                //{
            //                //    //log.Error("Error in ", ex);
            //                //}
            //                catch (JsonException jsonEx)
            //                {
            //                    return Json(new { Success = false, Message = "Error parsing JSON response.", Error = jsonEx.Message });
            //                }
            //            }
            //            else
            //            {
            //                return Json(new { Success = false, Message = "Received non-JSON response.", Response = responseBody });
            //            }
            //        }
            //        else
            //        {
            //            string errorContent = await response.Content.ReadAsStringAsync();
            //            return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception or handle it as needed
            //    return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            //}



            //if (string.IsNullOrEmpty(authToken))
            //{
            //    return Json(new { Success = false, Message = "Invalid token" });
            //}

            //try
            //{
            //    string url = "https://0106026495-998.minvoice.pro/api/InvoiceApi78/Save"; // Endpoint to add customer details
            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        // Correctly set the Authorization header with a space between "Bearer" and the token
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            //        var requestBody = new
            //        {
            //            // Map your customer properties here
            //            inv_invoiceIssuedDate = DateTime.UtcNow,
            //            inv_invoiceSeries = "1C24MPE",
            //            inv_paymentMethodName = "TM/CK",
            //            inv_buyerDisplayName = cust.Name,
            //            ma_dt = "Cust_00123_dt",
            //            inv_buyerLegalName = cust.Name,
            //            inv_buyerTaxCode = "inv_buyr_Tax_Code",
            //            inv_buyerAddressLine = cust.Address,
            //            inv_buyerEmail = cust.Email,
            //            amount_to_word = "Amount in word",
            //            inv_TotalAmount = 0.00,
            //            inv_discountAmount = 0.00,
            //            inv_vatAmount = 0.00,
            //            TotalAmountWithoutVat = 0.00,
            //            key_api = "Do not have",
            //            inv_itemCode = "#134 Item Code",
            //            inv_itemName = "item Name",
            //            inv_quantity = 11.22,
            //            inv_unitPrice = 22.00,
            //            inv_discountPercentage = 10.13,
            //            inv_TotalAmountWithoutVat = 2.13,
            //            ma_thue = 2.13,
            //        };

            //        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            //        HttpResponseMessage response = client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json")).Result;

            //        if (response.IsSuccessStatusCode)
            //        {
            //            // Read response content as string
            //            string responseBody = response.Content.ReadAsStringAsync().Result;

            //            // Deserialize the response to a dynamic object to check its structure
            //            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

            //            // Check for the "code" field in the response to determine success
            //            if (jsonResponse.code == "00")
            //            {
            //                return Json(new { Success = true, Message = jsonResponse.message, Data = jsonResponse.data });
            //            }
            //            else
            //            {
            //                return Json(new { Success = false, Message = "Unexpected response code.", Response = jsonResponse });
            //            }
            //        }
            //        else
            //        {
            //            string errorContent = response.Content.ReadAsStringAsync().Result;
            //            return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception or handle it as needed
            //    return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            //}
            //if (string.IsNullOrEmpty(authToken))
            //{
            //    return Json(new { Success = false, Message = "Invalid token" });
            //}

            //try
            //{
            //    string url = "https://hddt.minvoice.com.vn/api/InvoiceApi78/Save"; // Endpoint to add customer details
            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            //        var requestBody = new
            //        {
            //            // Map your customer properties here
            //            inv_invoiceIssuedDate = DateTime.UtcNow,
            //            inv_invoiceSeries= "1C24MPE",
            //            inv_paymentMethodName = "TM/CK",
            //            inv_buyerDisplayName = cust.Name,
            //            ma_dt = "Cust_00123_dt",
            //            inv_buyerLegalName = cust.Name,
            //            inv_buyerTaxCode = "inv_buyr_Tax_Code",
            //            inv_buyerAddressLine = cust.Address,
            //            inv_buyerEmail = cust.Email,
            //            amount_to_word="Amount in word",
            //            inv_TotalAmount=0.00,
            //            inv_discountAmount = 0.00,
            //            inv_vatAmount = 0.00,
            //            TotalAmountWithoutVat = 0.00,
            //            key_api = "Do not have",
            //            inv_itemCode = "#134 Item Code",
            //            inv_itemName = "item Name",
            //            inv_quantity = 11.22,
            //            inv_unitPrice = 22.00,
            //            inv_discountPercentage = 10.13,
            //            inv_TotalAmountWithoutVat = 2.13,
            //            ma_thue = 2.13,
            //        };

            //        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            //        HttpResponseMessage response = client.PostAsync(url, new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json")).Result;

            //        if (response.IsSuccessStatusCode)
            //        {
            //            return Json(new { Success = true, Message = "Customer details added successfully" });
            //        }
            //        else
            //        {
            //            string errorContent = response.Content.ReadAsStringAsync().Result;
            //            return Json(new { Success = false, Message = $"Failed to add customer details. Status code: {response.StatusCode}. Response: {errorContent}" });
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log or handle the exception as needed
            //    System.Diagnostics.Debug.WriteLine("Exception occurred in AddWebServiceCustomerDetails: " + ex.Message);
            //    return Json(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            //}

            //return Json(new { Success = true, Message = "" });
        }


    }
}
