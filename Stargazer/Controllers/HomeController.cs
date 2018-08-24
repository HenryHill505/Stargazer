using Stargazer.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;


namespace Stargazer.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext(); 

        public ActionResult Index()
        {
            return RedirectToAction("HomePage");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult HomePage()
        {
            var picUrl = RequestManager.GetPictureOfTheDayUrl();
            ViewBag.picUrl = picUrl;
            List<Event> events = db.Events.Where(e => e.Date > DateTime.Today).Take(50).OrderBy(e => e.Date).ToList();
            return View("HomePage", events);
        }

        public ActionResult EventDetails(int Id)
        {
            Event selectedEvent = db.Events.Include("User").Where(e => e.Id == Id).FirstOrDefault();
            return View("EventDetails", selectedEvent);
        }

        public string AddEvent(string body, DateTime date, string location)
        {
            //Probably better to do this with a junction table
            string userId = User.Identity.GetUserId();
            Event newEvent = new Event() { UserId = userId, Date = date, Location = location, CosmicBody = body };
            db.Events.Add(newEvent);
            db.SaveChanges();
            return "success";
        }

        public ActionResult DeleteEvent(int id)
        {
            Event selectedEvent = db.Events.Where(e => e.Id == id).FirstOrDefault();
            db.Events.Remove(selectedEvent);
            db.SaveChanges();
            return RedirectToAction("HomePage");
        }
    }
}