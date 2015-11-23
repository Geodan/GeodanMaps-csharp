using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Geodan.Cloud.Client.Core
{
    public static class SimpleCasClient
    {
        public static HttpClient CreateLoggedInClient(string username, string password, string casTicketServiceUrl,
            string serviceUrl)
        {
            string dummyTicket;
            string serviceRespnose;
            return CreateLoggedInClient(username, password, casTicketServiceUrl, serviceUrl, out dummyTicket, out serviceRespnose);
        }

        public static HttpClient CreateLoggedInClient(string username, string password, string casTicketServiceUrl,
            string serviceUrl, out string dummyTicket, out string loginResponse, bool modAuthCas = true)
        {
            var httpClient = new HttpClient();
            dummyTicket = GetServiceTicket(httpClient, username, password, casTicketServiceUrl, serviceUrl);
            loginResponse = CallServiceWithTicket(httpClient, serviceUrl, dummyTicket, modAuthCas);
            return httpClient;
        }

        private static string GetServiceTicket(HttpClient httpClient, string username, string password, string casTicketServiceUrl, string serviceUrl)
        {
            var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("username",  username),
                new KeyValuePair<string, string>("password", password)});

            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            //request for ticket granting ticket (tgt) at the cas ticket service
            var response = httpClient.PostAsync(casTicketServiceUrl, content).Result;
            // tgt is in the location header
            var location = response.Headers.FirstOrDefault(h => h.Key.Equals("location", StringComparison.OrdinalIgnoreCase));
            if (location.Key == null) throw new KeyNotFoundException("key 'location' not found in response headers");
            if (!location.Value.Any()) throw new InvalidOperationException("key 'location' has no values");
            var uri = new Uri(location.Value.ElementAt(0));
            var tgtId = uri.Segments.ElementAt(uri.Segments.Count() - 1);
            content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("service", serviceUrl)
            });
            // request for service ticket
            response = httpClient.PostAsync(casTicketServiceUrl + "/" + tgtId, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="serviceUrl"></param>
        /// <param name="ticket"></param>
        /// <param name="modAuthCas">If the service is protected by mod_auth_cas then 'ticket' should be used instead of 'SAMLart' in Url</param>
        /// <returns></returns>
        private static string CallServiceWithTicket(HttpClient httpClient, string serviceUrl, string ticket, bool modAuthCas)
        {
            var urlElement = "SAMLart";
            if (modAuthCas) urlElement = "ticket";
            var url = string.Format("{0}/?TARGET={0}/&{2}={1}", serviceUrl, ticket, urlElement);
            var response = httpClient.GetAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// casServiceUrl must not have a trailing slash.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="casServiceUrl"></param>
        /// <returns></returns>
        public static string Logout(HttpClient httpClient, string casServiceUrl)
        {
            var response = httpClient.GetAsync(casServiceUrl + "/logout").Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}