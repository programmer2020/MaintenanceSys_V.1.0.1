using maintenanceApplication.customeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace maintenanceApplication.Controllers
{
    //[BasicAttributes]
    [Authorize]
    //[OutputCache(Duration = 50, Location = System.Web.UI.OutputCacheLocation.Client)]
    //[OutputCache(Duration = 50, Location = System.Web.UI.OutputCacheLocation.Server)]
    [OutputCache(Duration = 0  , NoStore = true)]

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sections()
        {
            return View ();
        }

        //[Authorize(Users = "admin@admin.com")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //[Authorize(Roles = "admin")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}