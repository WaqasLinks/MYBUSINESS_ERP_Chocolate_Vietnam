using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MYBUSINESS.Controllers
{
    [Authorize(Roles = "Admin,Manager,User")]
    public class UploadingController : Controller
    {
        // GET: Uploading
        public ActionResult Index()
        {
            return View();
        }
    }
}