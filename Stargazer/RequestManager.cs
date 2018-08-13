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
        public static async Task<string> GetPictureOfTheDay()
        {
            string url = "https://api.nasa.gov/planetary/apod?api_key=" + Keyring.NASAKey;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);
            string picurl = (string)json["url"];
            return picurl;
        }
    }
}