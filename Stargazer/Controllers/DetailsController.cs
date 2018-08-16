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

        public List<ViewingPlace> GetViewingPlaces(double latitude, double longitude, double magnitude)
        {
            List<LightPoint> viewingPoints = RequestManager.GetLightPollutionData(latitude, longitude, 25000, magnitude);
            List<ViewingPlace> viewingPlaces = RequestManager.GetNearbyViewingPlaces(viewingPoints);
            return viewingPlaces;
        }

        public ActionResult Map()
        {
            ViewBag.GoogleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=" + Keyring.GoogleMapsKey + "&callback = initMap";
            return PartialView("_Map");
        }

        private ActionResult GetCometDetails(string identifier)
        {
            Comet comet = RequestManager.GetCometDetails(identifier);
            
            return View("Comet", comet);
        }
    }
}