using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CongressoApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: DashboardUser/Home
        public ActionResult Dashboard()
        {

            return View();
        }
        // GET: DashboardAdmin/Home
        public ActionResult DashboardAdmin()
        {

            return View();
        }
    }
}