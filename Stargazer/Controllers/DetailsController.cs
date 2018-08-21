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

        public ActionResult Map(string address, double magnitude)
        {
            Dictionary<string, double> coordinates = RequestManager.GeocodeAddress(address);
            List<ViewingPlace> viewingPlaces = GetViewingPlaces(coordinates["latitude"], coordinates["longitude"], magnitude);
            ViewBag.GoogleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=" + Keyring.GoogleMapsKey + "&callback=initMap";
            return PartialView("_Map", viewingPlaces);
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

        public ActionResult PictureUpload()
        {
            return View("PictureUpload");
        }

        [HttpPost]
        public ActionResult UploadPicture(HttpPostedFileBase file, string cosmicBody)
        {
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/"), fileName);
                file.SaveAs(path);

                var imageProperties = RequestManager.PostImgur(path);
                Picture picture = new Picture();

            }
            

            return RedirectToAction("Index");
        }
    }
}