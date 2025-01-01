using MYBUSINESS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LMSMYBUSINESS.Models
{
    public class LMSFilter : ActionFilterAttribute
    {
        private BusinessContext db;// = new BusinessContext();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var url = filterContext.HttpContext.Request.Url;
            string newUrl=string.Empty;
            var urlLst = url.ToString().Split('/');
            for (int i = 3; i < urlLst.Length; i++) 
            {
                newUrl += urlLst[i];
            }
            string CurrentController = HttpContext.Current.Request.RequestContext.RouteData.Values["Controller"].ToString();
            string CurrentAction = HttpContext.Current.Request.RequestContext.RouteData.Values["Action"].ToString();
            //string paramValue =HttpContext.Current.Request.RequestContext.RouteData.Values["IsReturn"];

            //string paramVal = HttpContext.Current.Request.Params["IsReturn"];

            Employee CurrentUser = (Employee)HttpContext.Current.Session["CurrentUser"];
            if (HttpContext.Current.Session["CurrentUser"] != null && CurrentUser.EmployeeTypeId == 1)
            {

                //List<string> urlpath;
                //bool flg = false;
                db = new BusinessContext();
                List<UserAuthorization> LstUsrAuth = db.UserAuthorizations.ToList();
                //var usrAth= db.UserAuthorizations.FirstOrDefault(x => x.UrlPath.Replace("/","") == newUrl);
                var usrAth = db.UserAuthorizations.FirstOrDefault(x => newUrl.Contains( x.UrlPath.Replace("/", "")));
                if (usrAth == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "UserAuthorization" }, { "action", "UnAuthorized" } });
                    return;
                }
                if (usrAth != null && usrAth.Authorize == false) 
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "UserAuthorization" }, { "action", "UnAuthorized" } });
                    return;
                }


                //foreach (UserAuthorization usrAuth in LstUsrAuth)
                //{
                //    urlpath = usrAuth.UrlPath.Split('/').ToList();
                //    if (CurrentController == urlpath[0] && CurrentAction == urlpath[1] && usrAuth.Authorize == true)
                //    {
                //        flg = true; break;
                //    }
                //}
                //if (flg == false) { return; }
            }


            if (HttpContext.Current.Session["CurrentUser"] != null)
            {
                //Employee emp = (Employee)HttpContext.Current.Session["CurrentUser"];
                //when all rights will enter in database. then this function will work correctly 



                //Right rt = db.Rights.FirstOrDefault(r => r.EmployeeId == emp.Id && r.Controller == CurrentController && r.Action == CurrentAction && r.Allowed == true);

                //if (rt == null && CurrentController != "NotAllowed")
                //{
                //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "NotAllowed" }, { "action", "UnAuthorized" } });
                //    //////base.OnActionExecuting(filterContext);
                //}

                return;
            }
            if (HttpContext.Current.Session["CurrentUser"] == null && CurrentController == "UserManagement" && CurrentAction == "Login")
            {
                //Let things happend automatically.
                //return;
                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "UserManagement" }, { "action", "Login" } });
                return;
            }
            if (HttpContext.Current.Session["CurrentUser"] == null || CurrentController == "UserManagement" && CurrentAction == "Login")
            {
                //Let things happend automatically.
                //return;
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "UserManagement" }, { "action", "Login" } });
                return;
            }


            //if (CurrentController == "NotAllowed" && CurrentAction == "UnAuthorized")
            //{
            //    //base.OnActionExecuting(filterContext);
            //}

            //else
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "NotAllowed" }, { "action", "UnAuthorized" } });
            //    //base.OnActionExecuting(filterContext);
            //}


        }

    }
}