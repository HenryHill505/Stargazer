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

        public ActionResult GetCometDetails(string identifier)
        {
            Comet comet = RequestManager.GetCometDetails(identifier);
            return View("Comet", comet);
        }
    }
}