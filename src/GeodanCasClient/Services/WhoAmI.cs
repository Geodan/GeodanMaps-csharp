using System.Net;
using System.Threading.Tasks;
using Geodan.Cloud.Client.Core.Cas;
using Geodan.Cloud.Client.Core.Models;
using Newtonsoft.Json;

namespace Geodan.Cloud.Client.Core.Services
{
    public class WhoAmI : CasHttpClient
    {
        /// <summary>
        /// WhoAmI service for retrieving account information
        /// </summary>
        /// <param name="username">cas username</param>
        /// <param name="password">cas password</param>
        /// <param name="casTicketServiceUrl">Url to the ticket service</param>
        /// <param name="serviceUrl">Url to the service</param>     
        /// <param name="modAuthCas">If the service is protected by mod_auth_cas then 'ticket' should be used instead of 'SAMLart' in Url</param>
        public WhoAmI(string username, string password, string casTicketServiceUrl, string serviceUrl, bool modAuthCas = false)
            : base(username, password, casTicketServiceUrl, serviceUrl, modAuthCas)
        {
        }

        /// <summary>
        /// Get user/organisation information
        /// </summary>
        /// <returns>Response with UserInfo as Result</returns>
        public async Task<Response<UserInfo>> TellMe()
        {
            var requestUrl = string.Format("{0}", ServiceUrl);
            var response = await GetAsync(requestUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            var dsResponse = response.StatusCode == HttpStatusCode.OK ?
                Response<UserInfo>.CreateSuccessful(JsonConvert.DeserializeObject<UserInfo>(responseString), response.StatusCode) :
                Response<UserInfo>.CreateUnsuccessful(responseString, response.StatusCode);

            return dsResponse;
        }
    }
}
