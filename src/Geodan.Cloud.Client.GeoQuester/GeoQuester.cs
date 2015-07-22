using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Geodan.Cloud.Client.Core.Cas;
using Geodan.Cloud.Client.Core.Models;
using Geodan.Cloud.Client.GeoQuester.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace Geodan.Cloud.Client.GeoQuester
{
    public class GeoQuester: CasHttpClient
    {
        private const string Api = "api";
        private DocumentService.DocumentService _documentService;

        public GeoQuester(string username, string password, string casTicketServiceUrl, string serviceUrl, string documentServiceUrl, bool modAuthCas = true) : base(username, password, casTicketServiceUrl, serviceUrl, modAuthCas)
        {
            _documentService = new DocumentService.DocumentService(username, password, casTicketServiceUrl, documentServiceUrl, modAuthCas);
        }

        /// <summary>
        /// Checks if feature intersects with a layer shape
        /// </summary>
        /// <param name="layer">Name of the layer</param>
        /// <param name="organisation">Name of the organisation</param>
        /// <param name="location">String with GeoJson of feature</param>
        /// <param name="buffer">Buffer around the given location</param>
        /// <param name="outputFormat"></param>
        /// <param name="maxFeatures">Maximum of features to be returned</param>
        /// <returns></returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<IsOverlapResult>> IsOverlap(string layer, string organisation, string location, double buffer = 0,
            string outputFormat = "yesno", double maxFeatures= 11)
        {
            var requestUrl = string.Format("{0}/{1}?layer={2}&organisation={3}&buffer={4}&outputFormat={5}&maxFeatures={6}", ServiceUrl, Api, layer, organisation,buffer,outputFormat, maxFeatures);

            var req = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(location, Encoding.UTF8, "application/json")
            };

            var response = await SendAsync(req);
            var responseString = await response.Content.ReadAsStringAsync();

            var dsResponse = response.StatusCode == HttpStatusCode.OK 
                ? Response<IsOverlapResult>.CreateSuccessful(JsonConvert.DeserializeObject<IsOverlapResult>(responseString), response.StatusCode) 
                : Response<IsOverlapResult>.CreateUnsuccessful(responseString, response.StatusCode);
            return dsResponse;
        }

        /// <summary>
        /// Get all layers for specific client
        /// </summary>
        /// <param name="account">Account name</param>
        /// <param name="service">Name of the account</param>
        /// <param name="documentName">Name of the document</param>
        /// <returns>List of all Layers for specified account</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<List<Layer>>> GetAllLayers(string account, string service, string documentName)
        {
            var response = await _documentService.GetDocumentData(account, service, documentName);
            Response<List<Layer>> dsResponse;

            if (response.Success)
            {
                var reader = new StreamReader(response.Result);
                var text = reader.ReadToEnd();
                var layers = JsonConvert.DeserializeObject<List<Layer>>(text);
                var filter = layers.Where(l => l.Account.Equals(account)).ToList();
                dsResponse = Response<List<Layer>>.CreateSuccessful(filter, response.HttpStatusCode);
            }
            else
            {
                dsResponse = Response<List<Layer>>.CreateUnsuccessful(response.Error, response.HttpStatusCode);
            }
            return dsResponse;
        }

    }
}
