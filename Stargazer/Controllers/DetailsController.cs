using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Stargazer.Controllers
{
    public class DetailsController : Controller
    {
        // GET: Details
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDetails(string identifier, string type)
        {
            switch (type)
            {
                case "comet":
                    return GetCometDetails(identifier);
                default:
                    return Index();
            }
        }

        private ActionResult GetCometDetails(string identifier)
        {
            Comet comet = RequestManager.GetCometDetails(identifier);
            ViewBag.GoogleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=" + Keyring.GoogleMapsKey;
            return View("Comet", comet);
        }
    }
}