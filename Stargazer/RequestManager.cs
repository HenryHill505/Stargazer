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
        public static async Task<JObject> GetPictureOfTheDayJSON()
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

        public static async Task<JArray> GetCometJSON()
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
    }
}