using NetTfsClient.Models;
using NetTfsClient.Services.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services
{
    internal abstract class BaseClient
    {
        protected const string API_VERSION = "6.0";

        protected readonly IClientConnection clientConnection;
        protected readonly IHttpClient httpClient;

        public BaseClient(IClientConnection clientConnection)
        {
            this.clientConnection = clientConnection;
            this.httpClient = clientConnection.HttpClient;
        }
    }
}
