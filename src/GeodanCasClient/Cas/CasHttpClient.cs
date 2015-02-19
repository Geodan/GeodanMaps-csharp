using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Geodan.Cloud.Client.Core.Exceptions;
using Geodan.Cloud.Client.Core.Extensions;

namespace Geodan.Cloud.Client.Core.Cas
{
    /// <summary>
    /// @time - 13-02-1015
    /// The Cas Service handles request against a cas secured service, there is no need to log in manually, this is by design (CAS)
    /// Request will be send to the service when it fails and a redirect to the login is found, a ticket will be requested and used 
    /// in an automatically created new request. The redirect url (CasLoginRedirectUrl) is set with the current login page, be aware 
    /// this url can change over time.
    /// 
    /// Currently the methods SendAsync aren't working with the automatically login yet because of "problems" with the the design of 
    /// HttpClient/HttpContent, to get these request to work in use the manual login function first. There is no known timeout for the 
    /// cas cookie, it is possible you need to login again after an unknown amount of time.
    /// </summary>
    public abstract class CasHttpClient : HttpClient, ICasHttpClient, INotifyPropertyChanged
    {
        private string _ticketServiceUrl;
        private string _serviceUrl;
        private string _password;
        private string _username;
        private string _errorString;
        private string _casLoginRedirectUrl;
        private bool _modAuthCas;

        public event CasLoginSuccessfulHandler CasLoginSuccessful;
        public event CasLoginFailedHandler CasLoginFailed;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <param name="username">cas username</param>
        /// <param name="password">cas password</param>
        /// <param name="casTicketServiceUrl">Url to the ticket service</param>
        /// <param name="serviceUrl">Url to the service</param>
        /// <param name="modAuthCas">If the service is protected by mod_auth_cas then 'ticket' should be used instead of 'SAMLart' in Url</param>
        protected CasHttpClient(string username, string password, string casTicketServiceUrl, string serviceUrl, bool modAuthCas = true)
        {
            Username = username;
            Password = password;
            TicketServiceUrl = casTicketServiceUrl;
            ServiceUrl = serviceUrl;
            ModAuthCas = modAuthCas;
            CasLoginRedirectUrl = "https://services.geodan.nl/cas/login";
        }

        # region properties

        public string TicketServiceUrl
        {
            get { return _ticketServiceUrl; }
            set
            {
                _ticketServiceUrl = value.RemoveTrailingSlash();
                OnPropertyChanged();
            }
        }

        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set
            {
                _serviceUrl = value.RemoveTrailingSlash();
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string ErrorString
        {
            get { return _errorString; }
            private set
            {
                _errorString = value;
                OnPropertyChanged();
            }
        }

        public bool ModAuthCas
        {
            get { return _modAuthCas; }
            set
            {
                _modAuthCas = value;
                OnPropertyChanged();
            }
        }

        public string CasLoginRedirectUrl
        {
            get { return _casLoginRedirectUrl; }
            set
            {
                _casLoginRedirectUrl = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// Manually login, not needed in normal circumstances
        /// CasLoginSuccessful fired when logged in
        /// CasLoginFailed fired when failed to login
        /// </summary>
        public async Task<bool> Login()
        {
            return await LoginAsync();
        }

        /// <summary>
        /// Logout of cas
        /// </summary>
        public async Task<bool> Logout()
        {
            var response = await base.GetAsync(string.Format("{0}/logout", ServiceUrl));
            return response.StatusCode == HttpStatusCode.OK;
        }

        private async Task<bool> LoginAsync()
        {
            var ticket = await GetServiceTicket(Username, Password, TicketServiceUrl, ServiceUrl);
            if (string.IsNullOrEmpty(ticket))
            {
                OnCasLoginFailed(ErrorString);
                return false;
            }

            var response = await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Get, new Uri(ServiceUrl), GetTicketString(ServiceUrl, ticket, ModAuthCas)));
            var serviceResponse = await response.Content.ReadAsStringAsync();
            if (serviceResponse == null)
            {
                OnCasLoginFailed(ErrorString);
                return false;
            }

            OnCasLoginSuccessful();
            return true;
        }

        private async Task<string> GetServiceTicket(string username, string password, string casTicketServiceUrl, string serviceUrl)
        {
            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("username", username), new KeyValuePair<string, string>("password", password) });
            DefaultRequestHeaders.ExpectContinue = false;
            var response = await base.PostAsync(casTicketServiceUrl, content);
            var location = response.Headers.FirstOrDefault(h => h.Key.Equals("location", StringComparison.OrdinalIgnoreCase));

            if (location.Key == null)
            {
                ErrorString = "key 'location' not found in response headers";
                return null;
            }

            if (!location.Value.Any())
            {
                ErrorString = "key 'location' has no values";
                return null;
            }

            var uri = new Uri(location.Value.ElementAt(0));
            var tgtId = uri.Segments.ElementAt(uri.Segments.Count() - 1);

            content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("service", serviceUrl) });
            response = await base.PostAsync(casTicketServiceUrl + "/" + tgtId, content);

            return await response.Content.ReadAsStringAsync();
        }

        private static string GetTicketString(string serviceUrl, string ticket, bool modAuthCas)
        {
            var urlElement = "SAMLart";
            if (modAuthCas) urlElement = "ticket";
            var ticketSTring = string.Format("TARGET={0}/&{2}={1}", serviceUrl, ticket, urlElement);

            return ticketSTring;
        }

        private async Task<HttpResponseMessage> RequestAsync(Func<string, Task<HttpResponseMessage>> method)
        {
            var httpTask = method(null);
            if (httpTask == null)
                throw new RequestNotSupportedException();

            var data = await httpTask;

            if (data.RequestMessage.RequestUri != null && data.RequestMessage.RequestUri.ToString().Contains(CasLoginRedirectUrl))
            {
                var ticket = await GetServiceTicket(Username, Password, TicketServiceUrl, ServiceUrl);
                if (string.IsNullOrEmpty(ticket))
                {
                    OnCasLoginFailed(ErrorString);
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent(string.Format("CAS: Unable to get ticket, {0}", ErrorString))
                    };
                }

                var httpTask2 = method(GetTicketString(ServiceUrl, ticket, ModAuthCas));
                data = await httpTask2;

                if (data.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("CAS: Unauthorized")
                    };
                }

                OnCasLoginSuccessful();
            }

            return data;
        }

        /// <summary>
        /// Create a new Uri if a ticket is required with the request
        /// </summary>
        private static Uri AlterUriForTicket(Uri uri, string ticketString)
        {
            if (string.IsNullOrEmpty(ticketString))
                return uri;

            var oldUri = uri.ToString();
            var newUri = oldUri.Contains("?") ?
               string.Format("{0}&{1}", oldUri, ticketString) :
               string.Format("{0}?{1}", oldUri, ticketString);

            return new Uri(newUri);
        }

        protected virtual void OnCasLoginSuccessful()
        {
            ErrorString = null;
            var handler = CasLoginSuccessful;
            if (handler != null) handler(this);
        }

        protected virtual void OnCasLoginFailed(string error)
        {
            var handler = CasLoginFailed;
            if (handler != null) handler(this, error);
        }

        #region HttpTaskCreation

        private Task<HttpResponseMessage> CreateHttpTask(HttpMethod method, Uri requestUri, string ticketString)
        {
            if (method == HttpMethod.Get)
                return base.GetAsync(AlterUriForTicket(requestUri, ticketString));
            if (method == HttpMethod.Delete)
                return base.DeleteAsync(AlterUriForTicket(requestUri, ticketString));

            return null;
        }

        private Task<HttpResponseMessage> CreateHttpTask(HttpMethod method, Uri requestUri, CancellationToken cancellationToken, string ticketString)
        {
            if (method == HttpMethod.Get)
                return base.GetAsync(AlterUriForTicket(requestUri, ticketString), cancellationToken);
            if (method == HttpMethod.Delete)
                return base.DeleteAsync(AlterUriForTicket(requestUri, ticketString), cancellationToken);

            return null;
        }

        private Task<HttpResponseMessage> CreateHttpTask(HttpMethod method, Uri requestUri, HttpCompletionOption completionOption, string ticketString)
        {
            if (method == HttpMethod.Get)
                return base.GetAsync(AlterUriForTicket(requestUri, ticketString), completionOption);

            return null;
        }

        private Task<HttpResponseMessage> CreateHttpTask(HttpMethod method, Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken, string ticketString)
        {
            if (method == HttpMethod.Get)
                return base.GetAsync(AlterUriForTicket(requestUri, ticketString), completionOption, cancellationToken);

            return null;
        }

        private Task<HttpResponseMessage> CreateHttpTask(HttpMethod method, Uri requestUri, Func<HttpContent> content, Dictionary<string, string> headers, string ticketString)
        {
            var req = new HttpRequestMessage(method, AlterUriForTicket(requestUri, ticketString)) { Content = content() };
            if (headers != null)
                foreach (var header in headers)
                    req.Headers.Add(header.Key, header.Value);

            return base.SendAsync(req);
        }

        private Task<HttpResponseMessage> CreateHttpTask(HttpMethod method, Uri requestUri, Func<HttpContent> content, string ticketString)
        {
            if (method == HttpMethod.Put)
                return base.PutAsync(AlterUriForTicket(requestUri, ticketString), content());
            if (method == HttpMethod.Post)
                return base.PostAsync(AlterUriForTicket(requestUri, ticketString), content());

            return null;
        }

        private Task<HttpResponseMessage> CreateHttpTask(HttpMethod method, Uri requestUri, Func<HttpContent> content, CancellationToken cancellationToken, string ticketString)
        {
            if (method == HttpMethod.Put)
                return base.PutAsync(AlterUriForTicket(requestUri, ticketString), content(), cancellationToken);
            if (method == HttpMethod.Post)
                return base.PostAsync(AlterUriForTicket(requestUri, ticketString), content(), cancellationToken);

            return null;
        }

        #endregion

        #region HttpWrappers

        #region WrappersDeleteAsync

        public async new Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return await DeleteAsync(new Uri(requestUri));
        }

        public async new Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken)
        {
            return await DeleteAsync(new Uri(requestUri), cancellationToken);
        }

        public async new Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Delete, requestUri, ticketReq));
        }

        public async new Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Delete, requestUri, cancellationToken, ticketReq));
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri, Func<HttpContent> content, Dictionary<string, string> headers = null)
        {
            return await DeleteAsync(new Uri(requestUri), content, headers);
        }

        public async Task<HttpResponseMessage> DeleteAsync(Uri requestUri, Func<HttpContent> content, Dictionary<string, string> headers = null)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Delete, requestUri, content, headers, ticketReq));
        }


        #endregion

        #region WrappersGetAsync

        public async new Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await GetAsync(new Uri(requestUri));
        }

        public async new Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
        {
            return await GetAsync(new Uri(requestUri), completionOption);
        }

        public async new Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await GetAsync(new Uri(requestUri), completionOption, cancellationToken);
        }

        public async new Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
        {
            return await GetAsync(new Uri(requestUri), cancellationToken);
        }

        public async new Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Get, requestUri, ticketReq));
        }

        public async new Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Get, requestUri, completionOption, ticketReq));
        }

        public async new Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Get, requestUri, completionOption, cancellationToken, ticketReq));
        }

        public async new Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Get, requestUri, cancellationToken, ticketReq));
        }

        #endregion

        #region WrappersGetByteArrayAsync

        public async new Task<byte[]> GetByteArrayAsync(string requestUri)
        {
            return await GetByteArrayAsync(new Uri(requestUri));
        }

        public async new Task<byte[]> GetByteArrayAsync(Uri requestUri)
        {
            var response = await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Get, requestUri, ticketReq));
            return await response.Content.ReadAsByteArrayAsync();
        }

        #endregion

        #region WrappersGetStreamAsync

        public async new Task<Stream> GetStreamAsync(string requestUri)
        {
            return await GetStreamAsync(new Uri(requestUri));
        }

        public async new Task<Stream> GetStreamAsync(Uri requestUri)
        {
            var response = await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Get, requestUri, ticketReq));
            return await response.Content.ReadAsStreamAsync();
        }

        #endregion

        #region WrappersGetStringAsync

        public async new Task<string> GetStringAsync(string requestUri)
        {
            return await GetStringAsync(new Uri(requestUri));
        }

        public async new Task<string> GetStringAsync(Uri requestUri)
        {
            var response = await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Get, requestUri, ticketReq));
            return await response.Content.ReadAsStringAsync();
        }

        #endregion

        #region WrappersPostAsync

        [Obsolete("Use PutAsync(string, Func<HttpContent>) instead", true)]
        public new Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            throw new NotImplementedException("Obsolete");
        }

        [Obsolete("Use PutAsync(string, Func<HttpContent>, CancellationToken) instead", true)]
        public new Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Obsolete");
        }

        [Obsolete("Use PutAsync(Uri, Func<HttpContent>) instead", true)]
        public new Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            throw new NotImplementedException("Obsolete");
        }

        [Obsolete("Use PutAsync(Uri, Func<HttpContent>, CancellationToken) instead", true)]
        public new Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Obsolete");
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Func<HttpContent> content)
        {
            return await PostAsync(new Uri(requestUri), content);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Func<HttpContent> content, CancellationToken cancellationToken)
        {
            return await PostAsync(new Uri(requestUri), content, cancellationToken);
        }

        public async Task<HttpResponseMessage> PostAsync(Uri requestUri, Func<HttpContent> content)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Post, requestUri, content, ticketReq));
        }
        public async Task<HttpResponseMessage> PostAsync(Uri requestUri, Func<HttpContent> content, CancellationToken cancellationToken)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Post, requestUri, content, cancellationToken, ticketReq));
        }

        #endregion

        #region WrappersPutAsync

        [Obsolete("Use PutAsync(string, Func<HttpContent>) instead", true)]
        public new Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            throw new NotImplementedException("Obsolete");
        }

        [Obsolete("Use PutAsync(string, Func<HttpContent>, CancellationToken) instead", true)]
        public new Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Obsolete");
        }

        [Obsolete("Use PutAsync(Uri, Func<HttpContent>) instead", true)]
        public new Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
        {
            throw new NotImplementedException("Obsolete");
        }

        [Obsolete("Use PutAsync(Uri, Func<HttpContent>, CancellationToken) instead", true)]
        public new Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Obsolete");
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, Func<HttpContent> content)
        {
            return await PutAsync(new Uri(requestUri), content);
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, Func<HttpContent> content, CancellationToken cancellationToken)
        {
            return await PutAsync(new Uri(requestUri), content, cancellationToken);
        }

        public async Task<HttpResponseMessage> PutAsync(Uri requestUri, Func<HttpContent> content)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Put, requestUri, content, ticketReq));
        }

        public async Task<HttpResponseMessage> PutAsync(Uri requestUri, Func<HttpContent> content, CancellationToken cancellationToken)
        {
            return await RequestAsync(ticketReq => CreateHttpTask(HttpMethod.Put, requestUri, content, cancellationToken, ticketReq));
        }

        #endregion

        #region WrappersSendAsync

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await base.SendAsync(request);
        }

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return await base.SendAsync(request, completionOption);
        }

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, completionOption, cancellationToken);
        }

        #endregion

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}