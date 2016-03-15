using System.Net;
using System.Threading.Tasks;
using Geodan.Cloud.Client.Core.Cas;
using Geodan.Cloud.Client.Core.Models;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Routing
{
    public class Routing : CasHttpClient
    {
        public string RouteApi { get; set; } = "route";

        public Routing(string username, string password, string casTicketServiceUrl, string serviceUrl, bool modAuthCas = true) :
            base(username, password, casTicketServiceUrl, serviceUrl, modAuthCas)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of all DataDocuments for specified account</returns>        
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<string>> Test()
        {
            var requestUrl = $"{ServiceUrl}/{RouteApi}?fromcoordx=4.91311&fromcoordy=52.34232&tocoordx=5.29969&tocoordy=51.69176&srs=epsg:4326&routetype=cost&returntype=coords&outputformat=json";
            var response = await GetAsync(requestUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            Response<string> dsResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                dsResponse = Response<string>.CreateSuccessful(responseString, response.StatusCode);
            }
            else
            {
                dsResponse = Response<string>.CreateUnsuccessful(responseString, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.NotFound)
                    dsResponse.Error = "No documents found";
            }

            return dsResponse;
        }
    }
}
