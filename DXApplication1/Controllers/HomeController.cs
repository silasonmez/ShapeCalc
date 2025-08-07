using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXApplication1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult GridViewPartial()
        {
            return PartialView("_GridViewPartial");
        }

        public ActionResult UserHome()
        {
            return View();
        }

    }
}