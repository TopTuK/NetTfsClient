using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services.HttpClient
{
    public static class HttpClientFactory
    {
        public static IHttpClient CreateHttpClient(string baseUrl, bool useDefaultCredentials = false)
        {
            return new HttpClient(baseUrl, useDefaultCredentials);
        }

        public static IHttpClient CreateHttpClient(string baseUrl, string userName, string userPassword)
        {
            return new HttpClient(baseUrl, userName, userPassword);
        }

        public static IHttpClient CreateHttpClient(string baseUrl, string personalAccessToken)
        {
            return new HttpClient(baseUrl, personalAccessToken);
        }

        public static IHttpClient CreateHttpClient(string baseUrl, ClaimsPrincipal claimsPrincipal)
        {
            return new HttpClient(baseUrl, claimsPrincipal);
        }
    }
}
