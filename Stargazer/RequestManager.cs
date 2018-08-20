using Newtonsoft.Json.Linq;
using Stargazer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Stargazer
{
    public static class RequestManager
    {
        static string PictureOfTheDayService = "https://api.nasa.gov/planetary/apod?api_key=" + Keyring.NASAKey;
        static string NearEarthCometsService = "https://data.nasa.gov/resource/2vr3-k9wn.json";
        static string DatastroLightPollution = "https://www.datastro.eu/api/records/1.0/search/?dataset=imageserver&sort=-dist&facet=localdate&facet=utdate&facet=limitingmag&facet=cloudcover&facet=constellation&facet=country&facet=filename";
        static string GooglePlaces = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=";
        static string GoogleGeocode = "https://maps.googleapis.com/maps/api/geocode/json?address=";

        public static Dictionary<string, double> GeocodeAddress(string address)
        {
            address = address.Replace(" ", "+");
            string url = GoogleGeocode + address + "&key=" + Keyring.GoogleMapsKey;
            JObject json = GetJsonObject(url).Result;
            double latitude = (double)json["results"][0]["geometry"]["location"]["lat"];
            double longitude = (double)json["results"][0]["geometry"]["location"]["lng"];
            Dictionary<string, double> coordinates = new Dictionary<string, double>();
            coordinates.Add("latitude", latitude);
            coordinates.Add("longitude", longitude);
            return coordinates;
        }
        private static async Task<JObject> GetPictureOfTheDayJSON()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(PictureOfTheDayService).ConfigureAwait(false);
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);
            return json;
           
        }

        public static string GetPictureOfTheDayUrl()
        {
            JObject json = GetPictureOfTheDayJSON().Result;
            return (string)json["url"];
        }

        private static async Task<JObject> GetJsonObject(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);
            return json;
        }

        private static async Task<JArray> GetJsonArray(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url).ConfigureAwait(false);
            string responseBody = await response.Content.ReadAsStringAsync();
            JArray json = JArray.Parse(responseBody);
            return json;
        }

        private static async Task<JArray> GetCometJSON()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(NearEarthCometsService).ConfigureAwait(false);
            string responseBody = await response.Content.ReadAsStringAsync();
            JArray json = JArray.Parse(responseBody);
            return json;
        }

        public static List<Comet> GetCometList()
        {
            List<Comet> comets = new List<Comet>();

            JArray json = GetCometJSON().Result;
            int cometCount = json.Count;
            for (int i = 0; i < cometCount; i++)
            {
                if (json[i]["h_mag"] != null)
                {
                    comets.Add(new Comet() { name = (string)json[i]["designation"], magnitude = (double)json[i]["h_mag"] });
                }
            }

            return comets;
        }

        public static Comet GetCometDetails(string identifier)
        {
            string queryUrl = NearEarthCometsService + "?designation=" + identifier;
            queryUrl = queryUrl.Replace(" ", "%20");
            JArray cometJson = GetJsonArray(queryUrl).Result;
            Comet comet = new Comet() { name = (string)cometJson[0]["designation"], magnitude = (double)cometJson[0]["h_mag"] };
            return comet;
        } 

        public static List<LightPoint> GetLightPollutionData(double latitude, double longitude, double distance, double magnitude)
        {
            List<LightPoint> lightPoints = new List<LightPoint>();
            string queryUrl = DatastroLightPollution + "&geofilter.distance=" + latitude + "," + longitude + "," + distance;
            JObject json = GetJsonObject(queryUrl).Result;
            int resultsCount = json["records"].Count();
            for (int i = 0; i < resultsCount ; i++)
            {
                if ((double)json["records"][i]["fields"]["limitingmag"] >= magnitude)
                {
                    lightPoints.Add(new LightPoint() {latitude = (double)json["records"][i]["fields"]["coord"][0], longitude = (double)json["records"][i]["fields"]["coord"][1], limitingMagnitude = (double)json["records"][i]["fields"]["limitingmag"] });
                }
            }
            return lightPoints;
        }

        public static List<ViewingPlace> GetNearbyViewingPlaces(List<LightPoint> viewingPoints)
        {
            List<ViewingPlace> places = new List<ViewingPlace>();
            int searchRadius = 200000;
            foreach (LightPoint viewingPoint in viewingPoints)
            {
                string queryUrl = GooglePlaces + viewingPoint.latitude + ", " + viewingPoint.longitude + "&radius=" + searchRadius + "&type=park" + "&key=" + Keyring.GoogleMapsKey;
                JObject json = GetJsonObject(queryUrl).Result;

                //if (json["results"][0]["types"].Contains("park
                //Should add an if statement to handle json is empty
                places.Add(new ViewingPlace() { name = (string)json["results"][0]["name"], latitude = (double)json["results"][0]["geometry"]["location"]["lat"], longitude = (double)json["results"][0]["geometry"]["location"]["lng"], vicinity = (string)json["results"][0]["vicinity"] });
            }
            return places;
        }

        public static Object GetWeatherForecast(double latitude, double longitude)
        {
            
            string url = "https://api.openweathermap.org/data/2.5/forecast?lat=" + latitude + "&lon=" + longitude + "&APPID=" + Keyring.OpenWeatherKey;
            JObject json = GetJsonObject(url).Result;
            string description = (string)json["list"][0]["weather"]["description"];
            double clouds = (double)json["list"][0]["clouds"]["all"];

            var result = new { description = description, clouds = clouds};

            return result;
        }
    }
}