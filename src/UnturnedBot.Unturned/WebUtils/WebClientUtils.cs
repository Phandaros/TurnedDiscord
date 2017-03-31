using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;

namespace UnturnedBot.Unturned.WebUtils
{
    static class WebClientUtils
    {
        private static WebClient wc = new WebClient() { BaseAddress = "http://localhost:3070/", Encoding = Encoding.UTF8};

        public static string Request(string endpoint = "", NameValueCollection queries = null)
        {
            queries = queries ?? new NameValueCollection();
            queries.Add("unturned", "true");

            wc.QueryString = queries;
            return wc.DownloadString(endpoint);
        }
    }
}
