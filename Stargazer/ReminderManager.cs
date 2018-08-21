using Stargazer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stargazer
{
    public static class ReminderManager
    {
        static ApplicationDbContext db = new ApplicationDbContext();

        public static void SendReminders()
        {
            List<Event> events = db.Events.Include("User").Where(e => e.Date > DateTime.Today).Where(e => e.ReminderSent == false).Where(e => e.User.GetReminders == true).ToList();
            
            foreach(Event selectedEvent in events)
            {
                TimeSpan daysToGo = selectedEvent.Date - DateTime.Today;
                if(daysToGo.Days < selectedEvent.User.ReminderTime)
                {
                    RequestManager.SendSmsAlert(selectedEvent.CosmicBody, selectedEvent.Location, selectedEvent.Date ,selectedEvent.User.PhoneNumber);
                }
            }

        }
    }
}