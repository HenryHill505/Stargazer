﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stargazer.Models;

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
            List<LightPoint> viewingPoints = RequestManager.GetLightPollutionData(latitude, longitude, 6000000, magnitude);
            List<ViewingPlace> viewingPlaces = RequestManager.GetNearbyViewingPlaces(viewingPoints);
            return viewingPlaces;
        }

        public ActionResult Map(double latitude, double longitude, double magnitude)
        {
            List<ViewingPlace> viewingPlaces = GetViewingPlaces(latitude, longitude, magnitude);
            ViewBag.GoogleMapsUrl = "https://maps.googleapis.com/maps/api/js?key=" + Keyring.GoogleMapsKey + "&callback=initMap";
            return PartialView("_Map", viewingPlaces);
        }

        public void CreateEvent(string userId, string date, string location, string cosmicBody)
        {
            try
            {
                Event newEvent = new Event();
                newEvent.UserId = userId;
                newEvent.Date = DateTime.Parse(date);
                newEvent.Location = location;
                newEvent.CosmicBody = cosmicBody;
            }

            catch
            {

            }
        }

        private ActionResult GetCometDetails(string identifier)
        {
            Comet comet = RequestManager.GetCometDetails(identifier);
            
            return View("Comet", comet);
        }


    }
}