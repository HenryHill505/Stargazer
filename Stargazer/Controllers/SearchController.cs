using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Stargazer.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult GetComets()
        {
            List<Comet> comets = RequestManager.GetCometList();
            return PartialView("_Comets", comets);
        }
    }
}