using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Geodan.Cloud.Client.Core.Models;
using Geodan.Cloud.Client.GeoQuester.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace Geodan.Cloud.Client.GeoQuester
{
    /// <summary>
    /// Supports Service 1.0
    /// </summary>
    public class Api : HttpClient
    {
        private string ServiceUrl { get; }
        private string ServiceKey { get; }

        public Api(string serviceUrl, string serviceKey)
        {
            ServiceUrl = serviceUrl[serviceUrl.Length - 1].Equals('/') ? serviceUrl.Substring(0, serviceUrl.Length - 1) : serviceUrl;
            ServiceKey = serviceKey;
        }

        /// <summary>
        /// Checks if geometry intersects for each layer in configuration, for large geometries use IntersectsPost
        /// </summary>        
        /// <param name="organisation">Name of the organisation</param>
        /// <param name="configurationName">Name of the configuration</param>
        /// <param name="geoJson">String with GeoJson of feature</param>
        /// <param name="buffer">Buffer around the given location</param>
        /// <returns>IntersectResult</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<IntersectResult>> IntersectsGet(string organisation, string configurationName, string geoJson, double buffer = 0)
        {
            var requestUrl = $"{ServiceUrl}/intersects/{organisation}/{configurationName}{(ServiceUrl.Contains("?") ? "&" : "?")}geometry={WebUtility.UrlEncode(geoJson)}&buffer={buffer}";
            requestUrl = AppendServiceKey(requestUrl);

            Response<IntersectResult> gqResponse;

            if (requestUrl.Length < 2000)
            {
                var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                var response = await SendAsync(req);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    gqResponse = Response<IntersectResult>.CreateSuccessful(JsonConvert.DeserializeObject<IntersectResult>(responseString), response.StatusCode);
                }
                else
                {
                    gqResponse = Response<IntersectResult>.CreateUnsuccessful(responseString, response.StatusCode);
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        gqResponse.Error = "Some parameters are missing or invalid";
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                        gqResponse.Error = "It is forbidden to request this resource";
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        gqResponse.Error = "The requested GeoQuestConfiguration is not found.";
                }
            }
            else
            {
                gqResponse = Response<IntersectResult>.CreateUnsuccessful("Geometry for intersecting is too large to process", HttpStatusCode.NotImplemented);
                gqResponse.Error = "Geomtry too large";
            }

            return gqResponse;
        }

        /// <summary>
        /// Checks if geometry intersects for each layer in configuration
        /// </summary>        
        /// <param name="organisation">Name of the organisation</param>
        /// <param name="configurationName">Name of the configuration</param>
        /// <param name="geoJson">String with GeoJson of feature</param>
        /// <param name="buffer">Buffer around the given location</param>
        /// <returns>IntersectResult</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<IntersectResult>> IntersectsPost(string organisation, string configurationName, string geoJson, double buffer = 0)
        {
            var requestUrl = $"{ServiceUrl}/intersects/{organisation}/{configurationName}";
            requestUrl = AppendServiceKey(requestUrl);

            Response<IntersectResult> gqResponse;

            var content = new MultipartFormDataContent
            {
                {new StringContent(geoJson), "geometry"},
                {new StringContent(buffer.ToString()), "buffer"}
            };

            var response = await PostAsync(requestUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                gqResponse = Response<IntersectResult>.CreateSuccessful(JsonConvert.DeserializeObject<IntersectResult>(responseString), response.StatusCode);
            }
            else
            {
                gqResponse = Response<IntersectResult>.CreateUnsuccessful(responseString, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    gqResponse.Error = "Some parameters are missing or invalid";
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    gqResponse.Error = "It is forbidden to request this resource";
                if (response.StatusCode == HttpStatusCode.NotFound)
                    gqResponse.Error = "The requested GeoQuestConfiguration is not found.";
            }

            return gqResponse;
        }

        /// <summary>
        /// Checks if geometry intersects for each layer in configuration, method IntersectingFeaturesPost is p
        /// </summary>        
        /// <param name="organisation">Name of the organisation</param>
        /// <param name="configurationName">Name of the configuration</param>
        /// <param name="geoJson">String with GeoJson of feature</param>
        /// <param name="maxFeatures"></param>
        /// <param name="buffer">Buffer around the given location</param>
        /// <param name="outputProperties"></param>
        /// <returns>IntersectFeatureResult</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<IntersectFeatureResult>> IntersectingFeaturesGet(string organisation, string configurationName, string geoJson, IntersectOutputProperties outputProperties, int maxFeatures = 10, double buffer = 0)
        {            
            var requestUrl = $"{ServiceUrl}/intersects/{organisation}/{configurationName}/features{(ServiceUrl.Contains("?") ? "&" : "?")}geometry={WebUtility.UrlEncode(geoJson)}&buffer={buffer}&maxFeatures={maxFeatures}&outputProperties={outputProperties.GetStringValue()}";
            requestUrl = AppendServiceKey(requestUrl);

            Response<IntersectFeatureResult> gqResponse;

            if (requestUrl.Length < 2000)
            {
                var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                var response = await SendAsync(req);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    gqResponse = Response<IntersectFeatureResult>.CreateSuccessful(JsonConvert.DeserializeObject<IntersectFeatureResult>(responseString), response.StatusCode);
                }
                else
                {
                    gqResponse = Response<IntersectFeatureResult>.CreateUnsuccessful(responseString, response.StatusCode);
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        gqResponse.Error = "Some parameters are missing or invalid";
                    if (response.StatusCode == HttpStatusCode.Forbidden)
                        gqResponse.Error = "It is forbidden to request this resource";
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        gqResponse.Error = "The requested GeoQuestConfiguration is not found.";
                }
            }
            else
            {
                gqResponse = Response<IntersectFeatureResult>.CreateUnsuccessful("Geometry for intersecting is too large to process", HttpStatusCode.NotImplemented);
                gqResponse.Error = "Geomtry too large";
            }

            return gqResponse;
        }

        /// <summary>
        /// Checks if geometry intersects for each layer in configurations
        /// </summary>        
        /// <param name="organisation">Name of the organisation</param>
        /// <param name="configurationName">Name of the configuration</param>
        /// <param name="geoJson">String with GeoJson of feature</param>
        /// <param name="maxFeatures"></param>
        /// <param name="buffer">Buffer around the given location</param>
        /// <param name="outputProperties"></param>
        /// <returns>IntersectFeatureResult</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<IntersectFeatureResult>> IntersectingFeaturesPost(string organisation, string configurationName, string geoJson, IntersectOutputProperties outputProperties, int maxFeatures = 10, double buffer = 0)
        {
            var requestUrl = $"{ServiceUrl}/intersects/{organisation}/{configurationName}";
            requestUrl = AppendServiceKey(requestUrl);

            Response<IntersectFeatureResult> gqResponse;

            var content = new MultipartFormDataContent
            {
                {new StringContent(geoJson), "geometry"},
                {new StringContent(outputProperties.GetStringValue()), "outputProperties"},
                {new StringContent(buffer.ToString()), "buffer"},
                {new StringContent(maxFeatures.ToString()), "maxFeatures"}
            };

            var response = await PostAsync(requestUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                gqResponse = Response<IntersectFeatureResult>.CreateSuccessful(JsonConvert.DeserializeObject<IntersectFeatureResult>(responseString), response.StatusCode);
            }
            else
            {
                gqResponse = Response<IntersectFeatureResult>.CreateUnsuccessful(responseString, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    gqResponse.Error = "Some parameters are missing or invalid";
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    gqResponse.Error = "It is forbidden to request this resource";
                if (response.StatusCode == HttpStatusCode.NotFound)
                    gqResponse.Error = "The requested GeoQuestConfiguration is not found.";
            }

            return gqResponse;
        }

        /// <summary>
        /// Get all configurations for specific client
        /// </summary>
        /// <param name="account">Account name</param>        
        /// <returns>List of all configs for specified account</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<List<Configuration>>> GetConfigurations(string account)
        {
            var requestUrl = $"{ServiceUrl}/configurations/{account}";
            requestUrl = AppendServiceKey(requestUrl);
            var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var response = await SendAsync(req);
            var responseString = await response.Content.ReadAsStringAsync();

            Response<List<Configuration>> gqResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                gqResponse = Response<List<Configuration>>.CreateSuccessful(JsonConvert.DeserializeObject<List<Configuration>>(responseString), response.StatusCode);
            }
            else
            {
                gqResponse = Response<List<Configuration>>.CreateUnsuccessful(responseString, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    gqResponse.Error = "It is forbidden to request this resource";
            }

            return gqResponse;
        }

        /// <summary>
        /// Get specific configuration
        /// </summary>
        /// <param name="account">Account name</param>
        /// <param name="configname">The config to get</param>
        /// <returns>Specified config</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<Configuration>> GetConfiguration(string account, string configname)
        {
            var requestUrl = $"{ServiceUrl}/configurations/{account}/{configname}";
            requestUrl = AppendServiceKey(requestUrl);
            var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var response = await SendAsync(req);
            var responseString = await response.Content.ReadAsStringAsync();

            Response<Configuration> gqResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                gqResponse = Response<Configuration>.CreateSuccessful(JsonConvert.DeserializeObject<Configuration>(responseString), response.StatusCode);
            }
            else
            {
                gqResponse = Response<Configuration>.CreateUnsuccessful(responseString, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    gqResponse.Error = "It is forbidden to request this resource";
                if (response.StatusCode == HttpStatusCode.NotFound)
                    gqResponse.Error = "The requested query configuration is not found.";
            }

            return gqResponse;
        }

        private string AppendServiceKey(string url)
        {
            return url.Contains("?") ? $"{url}&servicekey={ServiceKey}" : $"{url}?servicekey={ServiceKey}";
        }
    }
}
