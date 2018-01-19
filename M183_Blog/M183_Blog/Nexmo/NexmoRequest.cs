using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace M183_Blog.Nexmo
{
    public class NexmoRequest
    {
        public string from { get; set; }
        public string text { get; set; }
        public string to { get; set; }
        public string api_key { get; set; }
        public string api_secret { get; set; }

        public void Send()
        {
            var client = new HttpClient();
            var url = @"https://rest.nexmo.com/sms/json";

            var json = JsonConvert.SerializeObject(new NexmoRequest
            {
                from = "test123",
                text = "test",
                to = "+41795592648",
                api_key = "",
                api_secret = ""
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync(url, content).Result;
        }
    }
}