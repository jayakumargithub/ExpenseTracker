using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ExpenseTracker.Web.Helper
{
    public static class ExpenseTrackerHttpClient
    {
        public static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ExpenseTrackerConstants.ExpenseTrackerAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("applicaiton/json"));

            return client;
        }
    }
}