using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stargazer.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace Stargazer.Controllers
{
    public class DetailsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Details
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult GetDetails(string identifier, string type)
        //{
        //    switch (type)
        //    {
        //        case "comet":
        //            return GetCometDetails(identifier);
        //        default:
        //            return Index();
        //    }
        //}

        public ActionResult GetDetails(string identifier, string type, CosmicBody cosmicBody)
        {
            switch (type)
            {
                case "comet":
                    return GetCometDetails(identifier);
                //case "star":
                //    return GetStarDetails(cosmicBody);
                default:
                    return Index();
            }
        }

        public ActionResult GetStarDetails(string name, double magnitude, double declination, double rightAscension)
        {
            Star star = new Star() { name = name, magnitude = magnitude, declination = declination, rightAscension = rightAscension };
            CosmicBodyViewModel viewModel = GetCosmicBodyViewModel(star);
            return View("Star", viewModel);
        }

        public CosmicBodyViewModel GetCosmicBodyViewModel(CosmicBody body)
        {
            List<Event> events = db.Events.Where(e => e.CosmicBody == body.name).Where(e => e.Date > DateTime.Today).ToList();
            CosmicBodyViewModel viewModel = new CosmicBodyViewModel() { body = body, events = events };

            string userId = User.Identity.GetUserId();
            viewModel.userAddress = db.Users.Where(u => u.Id == userId).Select(u => u.Address).FirstOrDefault();
            return viewModel;
        }

        public List<ViewingPlace> GetViewingPlaces(double latitude, double longitude, double magnitude)
        {
            List<LightPoint> viewingPoints = RequestManager.GetLightPollutionData(latitude, longitude, 6000000, magnitude);
            List<ViewingPlace> viewingPlaces = RequestManager.GetNearbyViewingPlaces(viewingPoints);
            return viewingPlaces;
        }

        //public ActionResult Map(double latitude, double longitude, double magnitude)
        //{
        //    List<ViewingPlace> viewingPlaces = GetViewingPlaces(latitude, longitude, magnitude);
        //    ViewBag.GoogleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=" + Keyring.GoogleMapsKey + "&callback=initMap";
        //    return PartialView("_Map", viewingPlaces);
        //}

        public ActionResult MapComet(string address, double magnitude)
        {
            Dictionary<string, double> coordinates = RequestManager.GeocodeAddress(address);
            List<ViewingPlace> viewingPlaces = GetViewingPlaces(coordinates["latitude"], coordinates["longitude"], magnitude);
            ViewBag.GoogleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=" + Keyring.GoogleMapsKey + "&callback=initMap";
            return PartialView("_Map", viewingPlaces);
        }

        public ActionResult MapStar(string address, double magnitude, double declination, string priority)
        {
            try
            {
                Dictionary<string, double> coordinates = RequestManager.GeocodeAddress(address);

                if (priority == "view") { coordinates["latitude"] = declination; }

                if (!StarCalculator.isStarVisible(coordinates["latitude"], declination))
                {
                    coordinates["latitude"] = StarCalculator.getVisibleLatitude(coordinates["latitude"], declination);
                }
                List<ViewingPlace> viewingPlaces = GetViewingPlaces(coordinates["latitude"], coordinates["longitude"], magnitude);
                ViewBag.GoogleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=" + Keyring.GoogleMapsKey + "&callback=initMap";
                return PartialView("_Map", viewingPlaces);
            }
            catch
            {
                return PartialView("_MapError","Home");
            }
        }

        public ActionResult GetEventPartial()
        {
            return PartialView("_CreateEvent");
        }

        public string CreateEvent(string userId, string date, string location, string cosmicBody)
        {
            try
            {
                Event newEvent = new Event();
                newEvent.UserId = userId;
                newEvent.Date = DateTime.Parse(date);
                newEvent.Location = location;
                newEvent.CosmicBody = cosmicBody;
                db.Events.Add(newEvent);
                db.SaveChanges();
                return "Event created successfully";
            }

            catch
            {
                return "Event was not created. Check to make all fields have a valid format";
            }
        }

        private ActionResult GetCometDetails(string identifier)
        {
            Comet comet = RequestManager.GetCometDetails(identifier);
            List<Event> events = db.Events.Where(e => e.CosmicBody == comet.name).Where(e => e.Date > DateTime.Today).ToList();
            CosmicBodyViewModel viewModel = new CosmicBodyViewModel() { body = comet, events = events };

            string userId = User.Identity.GetUserId();
            viewModel.userAddress = db.Users.Where(u => u.Id == userId).Select(u => u.Address).FirstOrDefault();
            
            return View("Comet", viewModel);
        }

        [HttpGet]
        public ActionResult GetPictures(string bodyName)
        {
            List<Picture> pictures = db.Pictures.Where(p => p.BodyName == bodyName).ToList();
            return PartialView("_Pictures", pictures);
        }

        public ActionResult PictureUpload(string bodyName)
        {
            List<string> test = new List<string>();
            test.Add(bodyName);
            return View("PictureUpload", test);
        }

        [HttpPost]
        public ActionResult UploadPicture(HttpPostedFileBase file, string cosmicBody)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/"), fileName);
                    file.SaveAs(path);

                    var imageProperties = RequestManager.PostImgur(path);
                    Picture picture = new Picture() { DeleteHash = imageProperties["deleteHash"], Link = imageProperties["imageUrl"], BodyName = cosmicBody, UserId = User.Identity.GetUserId() };
                    db.Pictures.Add(picture);
                    db.SaveChanges();
                }

                return RedirectToAction("Search", "Search");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetWeather(string address, string date)
        {
            try
            {
                DateTime chosenDate = DateTime.Parse(date);

                if ((chosenDate - DateTime.Now).Days > 5)
                {
                    return PartialView("_WeatherUnavailable");
                }

                Dictionary<string, double> coordinates = RequestManager.GeocodeAddress(address);

                string dateString = $"{chosenDate.Year}-";
                if (chosenDate.Month < 10) { dateString += "0"; }
                dateString = $"{dateString}{chosenDate.Month}-{chosenDate.Day}";

                Dictionary<string, string> weatherResults = RequestManager.GetWeatherForecast(coordinates["latitude"], coordinates["longitude"], dateString);
                return PartialView("_Weather", weatherResults);
            }
            catch
            {
                return View("Error");
            }
        }

        public ActionResult GetUploadPartial(string bodyName)
        {
            return PartialView("_UploadPicture", bodyName);
        }
    }
}