using NetTfsClient.Models;
using NetTfsClient.Services.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services
{
    /// <summary>
    /// Internal abstract base client class. Contains information about API versions and connection to TFS/Azure service.
    /// </summary>
    internal abstract class BaseClient
    {
        /// <summary>
        /// API Version of requests to TFS/Azure service.
        /// </summary>
        protected const string API_VERSION = "6.0";

        protected const string API_PREVIEW_VERSION = "6.0-preview.3";

        /// <summary>
        /// Instance of IClientConnection with information of connection to TFS/Azure service
        /// </summary>
        protected readonly IClientConnection clientConnection;

        /// <summary>
        /// IHttpClient intance of client connection
        /// </summary>
        protected readonly IHttpClient httpClient;

        public BaseClient(IClientConnection clientConnection)
        {
            this.clientConnection = clientConnection;
            this.httpClient = clientConnection.HttpClient;
        }
    }
}
