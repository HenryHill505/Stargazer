using Newtonsoft.Json.Linq;
using Stargazer.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Stargazer
{
    public static class RequestManager
    {
        static string PictureOfTheDayService = "https://api.nasa.gov/planetary/apod?api_key=" + Keyring.NASAKey;
        static string NearEarthCometsService = "https://data.nasa.gov/resource/2vr3-k9wn.json";
        static string DatastroLightPollution = "https://www.datastro.eu/api/records/1.0/search/?dataset=imageserver&sort=-dist&facet=localdate&facet=utdate&facet=limitingmag&facet=cloudcover&facet=constellation&facet=country&facet=filename";
        static string GooglePlaces = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=";
        static string GoogleGeocode = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        static string ImgurImage = "https://api.imgur.com/3/image/";
        static string ImgurUpload = "https://api.imgur.com/3/upload.json";
        static string AstropicalStars = "http://www.astropical.space/astrodb/api.php?table=stars&format=json";
        static string OpenWeatherForecast = "https://api.openweathermap.org/data/2.5/forecast";

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
        //private static async Task<JObject> GetPictureOfTheDayJSON()
        //{
        //    HttpClient client = new HttpClient();
        //    var response = await client.GetAsync(PictureOfTheDayService).ConfigureAwait(false);
        //    string responseBody = await response.Content.ReadAsStringAsync();
        //    JObject json = JObject.Parse(responseBody);
        //    return json;
           
        //}

        public static string GetPictureOfTheDayUrl()
        {
            //JObject json = GetPictureOfTheDayJSON().Result;
            JObject json = GetJsonObject(PictureOfTheDayService).Result;
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

        //private static async Task<JArray> GetCometJSON()
        //{
        //    HttpClient client = new HttpClient();
        //    var response = await client.GetAsync(NearEarthCometsService).ConfigureAwait(false);
        //    string responseBody = await response.Content.ReadAsStringAsync();
        //    JArray json = JArray.Parse(responseBody);
        //    return json;
        //}

        public static List<Comet> GetCometList()
        {
            List<Comet> comets = new List<Comet>();

            //JArray json = GetCometJSON().Result;
            JArray json = GetJsonArray(NearEarthCometsService).Result;
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
        
        public static async Task<string> GetImgur(string imageHash)
        {
            string url = ImgurImage + imageHash;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Client-ID " + Keyring.ImgurClientId);
            var response = await client.GetAsync(url).ConfigureAwait(false);
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            string imageUrl = (string)json["data"]["link"];
            return imageUrl;
        }

        public static Dictionary<string, string> PostImgur(string imagePath)
        {
            string url = ImgurUpload;
            Dictionary<string, string> imageProperties = new Dictionary<string, string>();
            using (var client = new WebClient())
            {
                client.Headers.Add("Authorization", "Client-ID " + Keyring.ImgurClientId);
                var values = new NameValueCollection
                {
                    {"key", Keyring.ImgurClientId },
                    {"image", Convert.ToBase64String(File.ReadAllBytes(imagePath)) }
                };

                var response = client.UploadValues(url, values);
                string result = System.Text.Encoding.UTF8.GetString(response);
                JObject json = JObject.Parse(result);
                string deleteHash = (string)json["data"]["deletehash"];
                string imageUrl = (string)json["data"]["link"];
                //imageUrl = imageUrl.Remove(0, 19);
                //imageUrl = "https://api.imgur.com/3/image" + imageUrl;

                imageProperties.Add("imageUrl", imageUrl);
                imageProperties.Add("deleteHash", deleteHash);

                //var imageProperties = new { imageUrl, deletehash };
                return imageProperties;
            }

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

        public static List<Star> GetStars()
        {
            List<Star> stars = new List<Star>();
            string url = AstropicalStars;
            JObject json = GetJsonObject(url).Result;
            int starCount = json["hipstars"].Count();

            for(int i = 0; i < starCount; i++)
            {
                stars.Add(new Star() { name = (string)json["hipstars"][i]["name"], magnitude = (double)json["hipstars"][i]["mag"], declination = (double)json["hipstars"][i]["de"], rightAscension = (double)json["hipstars"][i]["ra"] });
            }

            return stars;
        }

        public static Dictionary<string, string> GetWeatherForecast(double latitude, double longitude, string dateString)
        {
            Dictionary<string, string> weatherResults = new Dictionary<string, string>();
            string url = OpenWeatherForecast+"?lat=" + latitude + "&lon=" + longitude + "&APPID=" + Keyring.OpenWeatherKey;
            JObject json = GetJsonObject(url).Result;
            int resultCount = json["list"].Count();

            string description;
            string clouds;
            
            for (int i = 0; i < resultCount; i++)
            {
                string resultDate = (string)json["list"][i]["dt_txt"];
                if (resultDate.Contains(dateString) && resultDate.Contains("15:00:00"))
                {
                     description = (string)json["list"][i]["weather"][0]["description"];
                     clouds = (string)json["list"][i]["clouds"]["all"];

                    weatherResults.Add("description", description);
                    weatherResults.Add("clouds", clouds);

                    return weatherResults;
                }
            }
            description = (string)json["list"][resultCount-1]["weather"][0]["description"];
            clouds = (string)json["list"][resultCount-1]["clouds"]["all"];

            weatherResults.Add("description", description);
            weatherResults.Add("clouds", clouds);

            return weatherResults;
        }

        public static void SendSmsAlert(string bodyName, string location, DateTime date, string toNumber)
        {
            string accountSid = Keyring.TwilioSID;
            string authToken = Keyring.TwilioAuthToken;
            TwilioClient.Init(accountSid, authToken);

            string messageContent = $"REMINDER: You've made an event to view {bodyName} at {location} on {date}"; 

            var to = new PhoneNumber(toNumber);
            var message = MessageResource.Create(
                to,
                from: new PhoneNumber("+19013905586"),
                body: messageContent);
        }
    }
}