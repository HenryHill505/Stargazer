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
    }
}