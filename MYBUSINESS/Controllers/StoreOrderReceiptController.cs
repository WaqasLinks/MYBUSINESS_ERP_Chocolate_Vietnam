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
    [Authorize(Roles = "Admin,Manager,User")]
    public class StoreOrderReceiptController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private DAL DAL = new DAL();
        private StoreOrderReceiptViewModel storeOrderReceiptViewModel = new StoreOrderReceiptViewModel();
        // GET: StoreOrderReceipt
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            // Get the next available ID
            decimal maxId = db.StoreOrderReceipts.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;

            ViewBag.Suppliers = DAL.dbSuppliers;

            var storeData = (from s in db.Stores
                             join sm in db.ShopManages
                             on s.Id equals sm.ShoreId into storeGroup
                             from sg in storeGroup.OrderByDescending(x => x.Date).Take(1).DefaultIfEmpty()
                             select new
                             {
                                 StoreId = s.Id,
                                 StoreName = s.Name,
                                 Balance = sg != null ? sg.Balance : 0,
                                 Date = sg != null ? sg.Date : (DateTime?)null
                             }).ToList();

            var stores = storeData.Select(s => new
            {
                Value = s.StoreId.ToString(),
                Text = s.StoreName +
                       (s.Date != null
                           ? $" - Balance: {s.Balance} - Date: {s.Date.Value.ToString("yyyy-MM-dd")}"
                           : " - No Balance Info")
            }).ToList();

            ViewBag.StoreList = new SelectList(stores, "Value", "Text");

            StoreOrderReceiptViewModel viewModel = new StoreOrderReceiptViewModel
            {
                StoreOrderReceipt = new StoreOrderReceipt(),
                Products = DAL.dbProducts
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,StoreId,Note")] StoreOrderReceipt storeOrderReceipt)
        {
            if (ModelState.IsValid)
            {
                db.StoreOrderReceipts.Add(storeOrderReceipt);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Suppliers = DAL.dbSuppliers;
            return View(storeOrderReceipt);
        }
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StoreOrderReceipt storeOrderReceipt = db.StoreOrderReceipts.Find(id);
            if (storeOrderReceipt == null)
            {
                return HttpNotFound();
            }

            var stores = db.Stores
                .Select(s => new { Value = s.Id.ToString(), Text = s.Name })
                .ToList();

            ViewBag.StoreList = new SelectList(stores, "Value", "Text", storeOrderReceipt.StoreId.ToString());

            var viewModel = new StoreOrderReceiptViewModel
            {
                StoreOrderReceipt = storeOrderReceipt,
                Products = DAL.dbProducts,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,StoreId,Note")] StoreOrderReceipt storeOrderReceipt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(storeOrderReceipt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var stores = db.Stores
                .Select(s => new { Value = s.Id.ToString(), Text = s.Name })
                .ToList();
            ViewBag.StoreList = new SelectList(stores, "Value", "Text", storeOrderReceipt.StoreId.ToString());

            var viewModel = new StoreOrderReceiptViewModel
            {
                StoreOrderReceipt = storeOrderReceipt,
                Products = DAL.dbProducts
            };

            return View(viewModel);
        }
    }
}