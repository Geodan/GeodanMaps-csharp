using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Geodan.Cloud.Client.Core.Models;
using Geodan.Cloud.Client.Routing.Extensions;
using Geodan.Cloud.Client.Routing.Models;
using Geodan.Cloud.Client.Routing.RequestParams;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Routing
{
    public class Api : HttpClient
    {
        public string TspEndpoint { get; set; }         = "tsp";
        public string RouteEndpoint { get; set; }       = "route";
        public string IsochroneEndpoint { get; set; }   = "isochrone";
        public string BatchRouteEndpoint { get; set; }  = "batchroute";
        public string ApiKey { get; set; }
        public string ServiceUrl { get; set; }

        public Api(string serviceUrl, string apiKey)
        {
            ServiceUrl = serviceUrl;
            ApiKey = apiKey;
        }

        /// <summary>
        /// Calculate a route
        /// </summary>
        /// <param name="fromcoordx">Start coordinate X</param>
        /// <param name="fromcoordy">Start coordinate Y</param>
        /// <param name="tocoordx">End coordinate X</param>
        /// <param name="tocoordy">End coordinate Y</param>
        /// <param name="srs">Input SRS (Default: EPSG:4326)</param>
        /// <param name="networkType">Which routenetwork to use. (Default: auto)</param>
        /// <param name="calcMode">Which calculation mode to use. (Default: time)</param>
        /// <param name="returnType">Return type, timedistance = time and distance info, coords = timedistance info + actual route. (Default: timedistance)</param>
        /// <param name="format">In which unit to return time and distance (Default: min-km)</param>
        /// <returns>Route response</returns>
        public async Task<Response<Route>> GetRoute(double fromcoordx, double fromcoordy, double tocoordx, double tocoordy, string srs = "epsg:4326", 
            NetworkType networkType = NetworkType.auto, CalcMode calcMode = CalcMode.time, RouteReturnType returnType = RouteReturnType.timedistance, 
            UnitFormatType format = UnitFormatType.minkm)
        {
            var requestUrl = $"{ServiceUrl}/{RouteEndpoint}?" +
                             $"fromcoordx={fromcoordx}&" +
                             $"fromcoordy={fromcoordy}&" +
                             $"tocoordx={tocoordx}&" +
                             $"tocoordy={tocoordy}&" +
                             $"srs={srs}&" +
                             $"networktype={networkType}&" +
                             $"calcMode={calcMode}&" +
                             $"returntype={returnType}&" +
                             $"format={format.GetValue()}&" +
                             $"outputformat=json";
            AppendServiceKey(ref requestUrl);

            var response = await GetAsync(requestUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            Response<Route> routeResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var featureCollection = JsonConvert.DeserializeObject<RoutingFeatureCollection>(responseString);
                var route = new Route(featureCollection, format);
                routeResponse = Response<Route>.CreateSuccessful(route, response.StatusCode);
            }
            else
            {
                routeResponse = Response<Route>.CreateUnsuccessful(responseString, response.StatusCode);
            }

            return routeResponse;
        }

        /// <summary>
        /// Get multiple routes in one request
        /// </summary>
        /// <param name="batchLocations">list of from to locations</param>
        /// <param name="srs">Input SRS (Default: EPSG:4326)</param>
        /// <param name="networkType">Which routenetwork to use. (Default: auto)</param>
        /// <param name="calcMode">Which calculation mode to use. (Default: time)</param>
        /// <param name="returnType">Return type, timedistance = time and distance info, coords = timedistance info + actual route. (Default: timedistance)</param>
        /// <param name="format">In which unit to return time and distance (Default: min-km)</param>
        /// <returns></returns>
        public async Task<Response<Route>> GetBatchRoute(List<BatchLocation> batchLocations, string srs = "epsg:4326", NetworkType networkType = NetworkType.auto, 
            CalcMode calcMode = CalcMode.time, RouteReturnType returnType = RouteReturnType.timedistance, UnitFormatType format = UnitFormatType.minkm)
        {
            var requestUrl = $"{ServiceUrl}/{BatchRouteEndpoint}?" +
                             $"srs={srs}&" +
                             $"networktype={networkType}&" +
                             $"calcMode={calcMode}&" +
                             $"returntype={returnType}&" +
                             $"format={format.GetValue()}&" +
                             $"outputformat=json";
            AppendServiceKey(ref requestUrl);

            var response = await PostAsync(requestUrl, CreateRouteLocationsStringContent(batchLocations));
            var responseString = await response.Content.ReadAsStringAsync();
            Response<Route> batchResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var featureCollection = JsonConvert.DeserializeObject<RoutingFeatureCollection>(responseString);
                var route = new Route(featureCollection, format);
                batchResponse = Response<Route>.CreateSuccessful(route, response.StatusCode);
            }
            else
            {
                batchResponse = Response<Route>.CreateUnsuccessful(responseString, response.StatusCode);
            }

            return batchResponse;
        }

        //ToDo: Implement
        public async Task<Response<Route>> GetIsochrone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tspLocations"></param>
        /// <param name="srs"></param>
        /// <param name="networkType"></param>
        /// <param name="calcMode"></param>
        /// <param name="tspMode">Open (no fixed start or end location), OpenEnd (first tspLocations = start, end open), OpenStart (open start location, last tspLocations is end), StartEnd (first tspLocations = start, last tspLocations = end), Round (first tspLocations is start and end)</param>
        /// <param name="identifier">returns same identifier in result</param>
        /// <returns></returns>
        public async Task<Response<TspResponse>> GetTsp(List<TspLocation> tspLocations, string srs = "epsg:4326", NetworkType networkType = NetworkType.auto, 
            CalcMode calcMode = CalcMode.time, TspMode tspMode = TspMode.open, int? identifier = null)
        {
            var requestUrl = $"{ServiceUrl}/{TspEndpoint}?" +
                             $"networktype={networkType}&" +
                             $"calcMode={calcMode}&" +
                             $"tspMode={tspMode}&" +
                             $"srs={srs}";
            
            requestUrl = identifier.HasValue ? $"{requestUrl}&Identifier={identifier.Value}" : $"{requestUrl}&Identifier=1";
            AppendServiceKey(ref requestUrl);

            var response = await PostAsync(requestUrl, CreateTspLocationsStringContent(tspLocations));
            var responseString = await response.Content.ReadAsStringAsync();
            var tspResponse = response.StatusCode == HttpStatusCode.OK ? Response<TspResponse>.CreateSuccessful(JsonConvert.DeserializeObject<TspResponse>(responseString), response.StatusCode) : 
                Response<TspResponse>.CreateUnsuccessful(responseString, response.StatusCode);

            return tspResponse;
        }

        private static StringContent CreateRouteLocationsStringContent(List<BatchLocation> batchLocations)
        {
            return new StringContent(JsonConvert.SerializeObject(batchLocations), Encoding.UTF8, "application/json");
        }

        private static StringContent CreateTspLocationsStringContent(List<TspLocation> tspLocations)
        {
            return new StringContent(JsonConvert.SerializeObject(tspLocations), Encoding.UTF8, "application/json");
        }

        private void AppendServiceKey(ref string url)
        {
            url = url.Contains("?") ? $"{url}&apikey={ApiKey}" : $"{url}?apikey={ApiKey}";
        }
    }
}
