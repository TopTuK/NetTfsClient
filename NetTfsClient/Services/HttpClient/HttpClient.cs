using NetTfsClient.Models.HttpClient;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services.HttpClient
{
    internal class HttpClient : IHttpClient
    {
        /// <summary>
        /// RestSharp IAuthenticator realisation for authenticate with personal access token
        /// Docs: https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page
        /// </summary>
        private class TfsPatAuthenticator : IAuthenticator
        {
            private readonly string _personalAccessToken;

            public TfsPatAuthenticator(string personalAccessToken)
            {
                _personalAccessToken = Encoding.UTF8.
                    GetString(Encoding.Default.GetBytes("Basic ")) +
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken))
                    );
            }

            public ValueTask Authenticate(IRestClient client, RestRequest request)
            {
                request.AddHeader("Authorization", _personalAccessToken);
                return ValueTask.CompletedTask;
            }
        }

        /// <summary>
        /// Basic IHttpResponse realization
        /// </summary>
        /// 
        private class RestHttpResponse : IHttpResponse
        {
            private RestResponse _restResponse;

            public RestHttpResponse(RestResponse restResponse)
            {
                _restResponse = restResponse;
            }
            public int StatusCode => (int) _restResponse.StatusCode;
            public bool IsSuccess => _restResponse.IsSuccessful;
            public bool HasError => !IsSuccess;

            public Uri? RequestUrl => _restResponse.ResponseUri;
            public string? Content => _restResponse.Content;

            public IReadOnlyDictionary<string, string?>? Headers => _restResponse
                .Headers
                ?.ToDictionary(param => param.Name!, param => param.Value?.ToString());
            public string? ContentType => _restResponse.ContentType;

            public IReadOnlyDictionary<string, string>? Cookies => _restResponse
                .Cookies
                ?.ToDictionary(cookie => cookie.Name, cookie => cookie.Value);
            public bool IsEmptyCookies => (Cookies == null) || (Cookies.Count == 0);
        }

        public Uri? BaseUrl 
        { 
            get => _client.Options.BaseUrl;
        }

        private readonly RestClient _client;

        public HttpClient(string baseUrl)
        {
            _client = new RestClient(baseUrl);
            //_client.CookieContainer = new System.Net.CookieContainer();
        }

        public HttpClient(string baseUrl, string personalAccessToken) 
        {
            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new TfsPatAuthenticator(personalAccessToken)
            };

            _client = new RestClient(options);
        }

        public HttpClient(string baseUrl, string userName, string userPassword)
        {
            // https://restsharp.dev/v107/#ntlm-authentication
            //var url = new Uri(baseUrl);

            var options = new RestClientOptions(baseUrl)
            {
                // Authenticator = new HttpBasicAuthenticator(userName, userPassword),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName, userPassword)//, url.Host)
            };

            _client = new RestClient(options);
        }

        public HttpClient(string baseUrl, ClaimsPrincipal claimsPrincipal)
            : this(baseUrl)
        {
            //_client.Options.Credentials = new NetworkCredential(
            //    claimsPrincipal.Identity!.Name, (string?) null, claimsPrincipal.Identity.AuthenticationType);
        }

        private RestRequest MakeRequest(string action,
            IReadOnlyDictionary<string, string>? customParams = null,
            IReadOnlyDictionary<string, string>? queryParams = null,
            IReadOnlyDictionary<string, string>? customHeaders = null)
        {
            var request = new RestRequest(action);

            if (customParams != null)
            {
                foreach (var param in customParams)
                {
                    request.AddParameter(param.Key, param.Value);
                }
            }

            if (queryParams != null)
            {
                foreach (var queryParam in queryParams)
                {
                    request.AddQueryParameter(queryParam.Key, queryParam.Value);
                }
            }

            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            return request;
        }

        public async Task<IHttpResponse?> GetAsync(string actionUrl, 
            IReadOnlyDictionary<string, string>? queryParams = null, 
            IReadOnlyDictionary<string, string>? customHeaders = null)
        {
            var request = MakeRequest(actionUrl,
                queryParams: queryParams,
                customHeaders: customHeaders);
            request.Method = Method.Get;

            var response = await _client.ExecuteAsync(request);
            return response != null
                ? new RestHttpResponse(response)
                : null;
        }

        public async Task<IHttpResponse?> PatchAsync(string actionUrl,
            IReadOnlyDictionary<string, string>? requestBody = null,
            IReadOnlyDictionary<string, string>? queryParams = null,
            IReadOnlyDictionary<string, string>? customHeaders = null)
        {
            var request = MakeRequest(actionUrl,
                customParams: requestBody,
                queryParams: queryParams,
                customHeaders: customHeaders);
            request.Method = Method.Patch;

            var response = await _client.ExecuteAsync(request);
            return response != null
                ? new RestHttpResponse(response)
                : null;
        }

        public async Task<IHttpResponse?> PatchJsonAsync(string actionUrl, object requestBody,
            IReadOnlyDictionary<string, string>? queryParams = null,
            IReadOnlyDictionary<string, string>? customHeaders = null)
        {
            var request = MakeRequest(actionUrl,
                queryParams: queryParams,
                customHeaders: customHeaders);
            request.Method = Method.Patch;

            if ((customHeaders != null) && (customHeaders.ContainsKey("Content-Type")))
            {
                request.AddJsonBody(requestBody, customHeaders["Content-Type"]);
            }
            else
            {
                request.AddJsonBody(requestBody);
            }

            var response = await _client.ExecuteAsync(request);
            return response != null
                ? new RestHttpResponse(response)
                : null;
        }

        public async Task<IHttpResponse?> PostAsync(string actionUrl,
            IReadOnlyDictionary<string, string>? requestBody = null,
            IReadOnlyDictionary<string, string>? queryParams = null,
            IReadOnlyDictionary<string, string>? customHeaders = null)
        {
            var request = MakeRequest(actionUrl,
                customParams: requestBody,
                queryParams: queryParams,
                customHeaders: customHeaders);
            request.Method = Method.Post;

            var response = await _client.ExecuteAsync(request);
            return response != null
                ? new RestHttpResponse(response)
                : null;
        }

        public async Task<IHttpResponse?> PostJsonAsync(string actionUrl, object requestBody,
            IReadOnlyDictionary<string, string>? queryParams = null,
            IReadOnlyDictionary<string, string>? customHeaders = null)
        {
            var request = MakeRequest(actionUrl,
                queryParams: queryParams,
                customHeaders: customHeaders);
            request.Method = Method.Post;

            if ((customHeaders != null) && (customHeaders.ContainsKey("Content-Type")))
            {
                request.AddJsonBody(requestBody, customHeaders["Content-Type"]);
            }
            else
            {
                request.AddJsonBody(requestBody);
            }

            var response = await _client.ExecuteAsync(request);
            return response != null
                ? new RestHttpResponse(response)
                : null;
        }
    }
}
