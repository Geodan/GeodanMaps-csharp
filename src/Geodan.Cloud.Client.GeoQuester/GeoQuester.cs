using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Geodan.Cloud.Client.Core.Models;
using Geodan.Cloud.Client.GeoQuester.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace Geodan.Cloud.Client.GeoQuester
{
    public class GeoQuester : HttpClient
    {
        private string ServiceUrl { get; set; }
        private string ServiceKey { get; set; }

        public GeoQuester(string serviceUrl, string serviceKey)
        {
            ServiceUrl = serviceUrl[serviceUrl.Length - 1].Equals('/') ? serviceUrl.Substring(0, serviceUrl.Length - 1) : serviceUrl;
            ServiceKey = serviceKey;
        }

        /// <summary>
        /// Checks if feature intersects with a layer shape
        /// </summary>        
        /// <param name="organisation">Name of the organisation</param>
        /// <param name="configurationName">Name of the configuration</param>
        /// <param name="geoJson">String with GeoJson of feature</param>
        /// <param name="buffer">Buffer around the given location</param>
        /// <returns></returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<IntersectResult>> Intersects(string organisation, string configurationName, string geoJson, double buffer = 0)
        {
            var requestUrl = string.Format("{0}/intersects/{1}/{2}{3}geometry={4}&buffer={5}", ServiceUrl, organisation, configurationName, ServiceUrl.Contains("?") ? "&" : "?", WebUtility.UrlEncode(geoJson), buffer);
            requestUrl = AppendServiceKey(requestUrl);

            var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var response = await SendAsync(req);
            var responseString = await response.Content.ReadAsStringAsync();            

            var dsResponse = response.StatusCode == HttpStatusCode.OK
                ? Response<IntersectResult>.CreateSuccessful(JsonConvert.DeserializeObject<IntersectResult>(responseString), response.StatusCode)
                : Response<IntersectResult>.CreateUnsuccessful(responseString, response.StatusCode);

            return dsResponse;
        }

        /// <summary>
        /// Get all configurations for specific client
        /// </summary>
        /// <param name="account">Account name</param>        
        /// <returns>List of all configs and layers for specified account</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<List<Configuration>>> GetConfigurations(string account)
        {
            var requestUrl = string.Format("{0}/configurations/{1}", ServiceUrl, account);
            requestUrl = AppendServiceKey(requestUrl);
            var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);            

            var response = await SendAsync(req);
            var responseString = await response.Content.ReadAsStringAsync();

            var dsResponse = response.StatusCode == HttpStatusCode.OK
                ? Response<List<Configuration>>.CreateSuccessful(JsonConvert.DeserializeObject<List<Configuration>>(responseString), response.StatusCode)
                : Response<List<Configuration>>.CreateUnsuccessful(responseString, response.StatusCode);

            return dsResponse;
        }

        /// <summary>
        /// Get all configurations for specific client
        /// </summary>
        /// <param name="account">Account name</param>
        /// <param name="configname">The config to get</param>
        /// <returns>List of all Layers for specified config</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<List<Configuration>>> GetConfiguration(string account, string configname)
        {
            var requestUrl = string.Format("{0}/configurations/{1}", ServiceUrl, account);
            requestUrl = AppendServiceKey(requestUrl);
            var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var response = await SendAsync(req);
            var responseString = await response.Content.ReadAsStringAsync();

            var dsResponse = response.StatusCode == HttpStatusCode.OK
                ? Response<List<Configuration>>.CreateSuccessful(JsonConvert.DeserializeObject<List<Configuration>>(responseString), response.StatusCode)
                : Response<List<Configuration>>.CreateUnsuccessful(responseString, response.StatusCode);

            return dsResponse;
        }

        private string AppendServiceKey(string url)
        {
            return url.Contains("?") ? string.Format("{0}&servicekey={1}", url, ServiceKey) : string.Format("{0}?servicekey={1}", url, ServiceKey);
        }
    }
}
