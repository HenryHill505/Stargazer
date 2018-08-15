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

        public static void GetLightPollutionData(double latitude, double longitude, double distance, double magnitude)
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
        }
    }
}