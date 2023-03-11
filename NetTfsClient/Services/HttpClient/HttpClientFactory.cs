using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services.HttpClient
{
    public static class HttpClientFactory
    {
        public static IHttpClient CreateHttpClient(string baseUrl)
        {
            return new HttpClient(baseUrl);
        }

        public static IHttpClient CreateHttpClient(string baseUrl, string userName, string userPassword)
        {
            return new HttpClient(baseUrl, userName, userPassword);
        }

        public static IHttpClient CreateHttpClient(string baseUrl, string personalAccessToken)
        {
            return new HttpClient(baseUrl, personalAccessToken);
        }
    }
}
