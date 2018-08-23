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
            List<Star> stars = RequestManager.GetStars();
            return View(stars);
        }

        public ActionResult GetComets()
        {
            List<Comet> comets = RequestManager.GetCometList();
            return PartialView("_Comets", comets);
        }

        public ActionResult GetStars()
        {
            List<Star> stars = RequestManager.GetStars();
            return PartialView("_Stars", stars);
        }
    }
}