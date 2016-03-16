using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Geodan.Cloud.Client.Core.Cas;
using Geodan.Cloud.Client.Core.Models;
using Geodan.Cloud.Client.DocumentService.Models;
using Newtonsoft.Json;
using Version = Geodan.Cloud.Client.DocumentService.Models.Version;

namespace Geodan.Cloud.Client.DocumentService
{
    public class Api : CasHttpClient
    {
        private const string ApiEndpoint = "api";
        private const string Data = "data";
        private const string Version = "version";

        public Api(string username, string password, string casTicketServiceUrl, string serviceUrl, bool modAuthCas = true) :
            base(username, password, casTicketServiceUrl, serviceUrl, modAuthCas)
        {
        }

        /// <summary>
        /// Get all documents for specified account
        /// </summary>
        /// <param name="account">Name of the account</param>
        /// <param name="pageNr">Page number you want to retrieve</param>
        /// <param name="pageSize">Size of the page you want to retrieve. Maximum value is 2147483647</param>
        /// <param name="sort">Sort order</param>        
        /// <returns>List of all DataDocuments for specified account</returns>        
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<List<DataDocument>>> GetAllDocumentsForAccount(string account, int pageNr = 0, int pageSize = 2147483647, Sort sort = Sort.Asc)
        {
            return await GetAllDocuments(pageNr, pageSize, sort, account);
        }

        /// <summary>
        /// Get all documents for specified account
        /// </summary>
        /// <param name="account">Name of the account</param>
        /// <param name="service">Name of the service</param>
        /// <param name="pageNr">Page number you want to retrieve</param>
        /// <param name="pageSize">Size of the page you want to retrieve. Maximum value is 2147483647</param>
        /// <param name="sort">Sort order</param>        
        /// <returns>List of all DataDocuments for specified account</returns>        
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<List<DataDocument>>> GetAllDocumentsForAccountAndService(string account, string service, int pageNr = 0, int pageSize = 2147483647, Sort sort = Sort.Asc)
        {
            return await GetAllDocuments(pageNr, pageSize, sort, account, service);
        }

        /// <summary>
        /// Returns a list of documents. Can be queried with paging parameters.
        /// </summary>
        /// <param name="pageNr">Page number you want to retrieve</param>
        /// <param name="pageSize">Size of the page you want to retrieve. Maximum value is 2147483647</param>
        /// <param name="sort">Sort order</param>
        /// <param name="account">when specified the request only returns account specific documents</param>
        /// <param name="service">Name of the service</param>
        /// <returns>List of all public DataDocuments</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<List<DataDocument>>> GetAllDocuments(int pageNr = 0, int pageSize = 2147483647, Sort sort = Sort.Asc, string account = null, string service = null)
        {
            var accountSpecific = string.IsNullOrEmpty(account) ? "" : $"/{account}";
            var serviceSpecific = string.IsNullOrEmpty(service) ? "" : $"/{service}";
            var requestUrl = $"{ServiceUrl}/{ApiEndpoint}{accountSpecific}{serviceSpecific}?pageNr={pageNr}&pageSize={pageSize}&sort={sort.ToString().ToUpper()}";
            var response = await GetAsync(requestUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            Response<List<DataDocument>> dsResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                dsResponse = Response<List<DataDocument>>.CreateSuccessful(JsonConvert.DeserializeObject<List<DataDocument>>(responseString), response.StatusCode);
            }
            else
            {
                dsResponse = Response<List<DataDocument>>.CreateUnsuccessful(responseString, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.NotFound)
                    dsResponse.Error = "No documents found";
            }

            return dsResponse;
        }

        /// <summary>
        /// Find a specified document
        /// </summary>
        /// <param name="account">Name of the account</param>
        /// <param name="service">Name of the service</param>
        /// <param name="name">Name of the document</param>
        /// <returns>Found DataDocument including data</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<List<DataDocument>>> GetDocument(string account, string service, string name)
        {
            var requestUrl = $"{ServiceUrl}/{ApiEndpoint}/{account}/{service}/{name}";
            var response = await GetAsync(requestUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            Response<List<DataDocument>> dsResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                dsResponse = Response<List<DataDocument>>.CreateSuccessful(JsonConvert.DeserializeObject<List<DataDocument>>(responseString), response.StatusCode);
            }
            else
            {
                dsResponse = Response<List<DataDocument>>.CreateUnsuccessful(responseString, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.NotFound) //404  
                    dsResponse.Error = "Document not found";
            }

            return dsResponse;
        }

        /// <summary>
        /// Streams the data of specified document (string) with in document specified contenttype
        /// </summary>
        /// <param name="account">Name of the account</param>
        /// <param name="service">Name of the service</param>
        /// <param name="name">Name of the document</param>
        /// <returns>Document data as stream</returns>        
        public async Task<Response<Stream>> GetDocumentData(string account, string service, string name)
        {
            var requestUrl = $"{ServiceUrl}/{ApiEndpoint}/{Data}/{account}/{service}/{name}";
            var response = await GetAsync(requestUrl);
            var stream = await response.Content.ReadAsStreamAsync();

            Response<Stream> dsResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                dsResponse = Response<Stream>.CreateSuccessful(stream, response.StatusCode);
            }
            else
            {
                dsResponse = Response<Stream>.CreateUnsuccessful(await response.Content.ReadAsStringAsync(), response.StatusCode);
                if (response.StatusCode == HttpStatusCode.NoContent)
                    dsResponse.Error = "Document contains no data";

                if (response.StatusCode == HttpStatusCode.NotFound)
                    dsResponse.Error = "Document not found";

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    dsResponse.Error = "Error creating document";

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                    dsResponse.Error = "Error encoding document";
            }

            return dsResponse;
        }

        /// <summary>
        /// Get the current service version, API created for DocumentService Version "1.0.0.3-SNAPSHOT"        
        /// </summary>
        /// <returns>DocumentService version</returns>
        public async Task<Response<Version>> GetVersion()
        {
            var requestUrl = $"{ServiceUrl}/{ApiEndpoint}/{Version}";
            var response = await GetAsync(requestUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            return response.StatusCode == HttpStatusCode.OK ?
                Response<Version>.CreateSuccessful(JsonConvert.DeserializeObject<Version>(responseString), response.StatusCode) :
                Response<Version>.CreateUnsuccessful(responseString, response.StatusCode);
        }

        /// <summary>
        /// Updates an exisiting document based on account, service and name. If not found, new document will be created.
        /// </summary>
        /// <param name="dataDocument"></param>        
        public async Task<Response<bool>> CreateOrUpdateDocument(DataDocument dataDocument)
        {
            var requestUrl = $"{ServiceUrl}/{ApiEndpoint}";            
            var response = await PutAsync(requestUrl, () => CreateJsonStringContent(dataDocument));
            var responseString = await response.Content.ReadAsStringAsync();
            var dsResponse = response.StatusCode == HttpStatusCode.Created ?
                Response<bool>.CreateSuccessful(true, response.StatusCode) :
                Response<bool>.CreateUnsuccessful(responseString, response.StatusCode);

            return dsResponse;
        }

        /// <summary>
        /// Create a new document
        /// </summary>
        /// <param name="dataDocument">DataDocument to create</param>
        /// <returns>Newly created document</returns>
        /// <exception cref="JsonSerializationException">Thrown when response could not be parsed</exception>
        public async Task<Response<DataDocument>> CreateNewDocument(DataDocument dataDocument)
        {
            var requestUrl = $"{ServiceUrl}/{ApiEndpoint}";

            var response = await PostAsync(requestUrl, () => CreateJsonStringContent(dataDocument));
            var responseString = await response.Content.ReadAsStringAsync();
            Response<DataDocument> dsResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                dsResponse = Response<DataDocument>.CreateSuccessful(JsonConvert.DeserializeObject<DataDocument>(responseString), response.StatusCode);
            }
            else
            {
                dsResponse = Response<DataDocument>.CreateUnsuccessful(responseString, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.Conflict)
                    dsResponse.Error = "Document already exists";
            }

            return dsResponse;
        }

        /// <summary>
        /// Create a new document using mulitpart file as content
        /// </summary>
        /// <param name="dataDocument"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<Response<DataDocument>> CreateNewDocument(DataDocument dataDocument, MultipartFile file)
        {
            var requestUrl = $"{ServiceUrl}/{ApiEndpoint}";
            var response = await PostAsync(requestUrl, () => CreateMultipartContent(dataDocument, file));
            var responseString = await response.Content.ReadAsStringAsync();

            Response<DataDocument> dsResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                dsResponse = Response<DataDocument>.CreateSuccessful(JsonConvert.DeserializeObject<DataDocument>(responseString), response.StatusCode);
            }
            else
            {
                dsResponse = Response<DataDocument>.CreateUnsuccessful(responseString, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.Conflict)
                    dsResponse.Error = "Document already exists";
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                    dsResponse.Error = "Error encoding document";
            }

            return dsResponse;
        }

        /// <summary>
        /// Delete document from service, service currently only respond with NoContent, removal not guaranteed
        /// </summary>
        /// <param name="dataDocument"></param>
        /// <returns>Response of type bool</returns>
        public async Task<Response<bool>> DeleteDocument(DataDocument dataDocument)
        {
            var requestUrl = $"{ServiceUrl}/{ApiEndpoint}";
            var response = await DeleteAsync(requestUrl, () => CreateJsonStringContent(dataDocument));
            var responseString = await response.Content.ReadAsStringAsync();

            return response.StatusCode == HttpStatusCode.NoContent ?
                Response<bool>.CreateSuccessful(true, response.StatusCode) :
                Response<bool>.CreateUnsuccessful(responseString, response.StatusCode);
        }

        /// <summary>
        /// Delete document from service, service currently only respond with NoContent, removal not guaranteed
        /// </summary>
        /// <param name="account">Name of the account</param>
        /// <param name="service">Name of the service</param>
        /// <param name="name">Name of the document</param>
        /// <returns>Response of type bool</returns>
        public async Task<Response<bool>> DeleteDocument(string account, string service, string name)
        {
            var requestUrl = $"{ServiceUrl}/{ApiEndpoint}/{account}/{service}/{name}";
            var response = await DeleteAsync(requestUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            return response.StatusCode == HttpStatusCode.NoContent ?
               Response<bool>.CreateSuccessful(true, response.StatusCode) :
               Response<bool>.CreateUnsuccessful(responseString, response.StatusCode);
        }

        private static StringContent CreateJsonStringContent(object o)
        {
            var json = JsonConvert.SerializeObject(o, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static MultipartFormDataContent CreateMultipartContent(DataDocument dataDocument, MultipartFile mpf)
        {
            var content = new MultipartFormDataContent
            {
                {
                    new StringContent(JsonConvert.SerializeObject(dataDocument)), "jsonDocument"
                }
            };

            var streamCopy = new MemoryStream();
            mpf.Data.Seek(0, SeekOrigin.Begin);
            mpf.Data.CopyTo(streamCopy);
            streamCopy.Seek(0, SeekOrigin.Begin);

            var file = new StreamContent(streamCopy);
            file.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                FileName = mpf.Filename,
                Name = "file"
            };

            if (!string.IsNullOrEmpty(dataDocument.Type))
                file.Headers.Add("Content-Type", dataDocument.Type);

            content.Add(file, "file");

            return content;
        }
    }
}
