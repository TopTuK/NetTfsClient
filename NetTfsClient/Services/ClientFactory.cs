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
    /// 
    /// </summary>
    public static class ClientFactory
    {
        private class ClientConnection : IClientConnection
        {
            public string ServerUrl { get; init; }

            public string CollectionName { get; }
            public string ProjectName { get; set; }

            // Remove part after / in project-name, like DefaultCollection/MyProject => DefaultCollection
            // API responce only in Project, without subproject
            public string ApiUrl => $"{CollectionName}/_apis/";

            // API response only for Collection/Project
            public string ProjectApiUrl => $"{CollectionName}/{ProjectName}/_apis/";

            public IHttpClient HttpClient { get; init; }

            public ClientConnection(IHttpClient httpClient, string serverUrl, string projectName)
            {
                HttpClient = httpClient;

                // ServerUrl should ends with '/'
                ServerUrl = serverUrl.EndsWith("/")
                    ? serverUrl
                    : $"{serverUrl}/";
                HttpClient.BaseUrl = new Uri(ServerUrl);

                // Closure function to get Collection and Project
                (string, string) GetCollectionAndProject()
                {
                    var splitPrj = projectName.Split('/');

                    string collection = splitPrj[0];
                    string project = "";

                    if ((splitPrj.Length > 1) && (splitPrj[1].Trim() != ""))
                    {
                        project = splitPrj[1];
                    }

                    return (collection, project);
                }

                // Assign Collection and Project
                (CollectionName, ProjectName) = GetCollectionAndProject();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="projectName"></param>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        public static IClientConnection CreateClientConnection(string serverUrl, string projectName,
            string userName, string userPassword)
        {
            var httpClient = HttpClientFactory.CreateHttpClient(serverUrl, userName, userPassword);
            return new ClientConnection(httpClient, serverUrl, projectName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="projectName"></param>
        /// <param name="personalAccessToken"></param>
        /// <returns></returns>
        public static IClientConnection CreateClientConnection(string serverUrl, string projectName,
            string personalAccessToken)
        {
            var httpClient = HttpClientFactory.CreateHttpClient(serverUrl, personalAccessToken);
            return new ClientConnection(httpClient, serverUrl, projectName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        /// <exception cref="TfsClientException"></exception>
        public static IClientConnection CreateClientConnection(IHttpClient httpClient, string projectName)
        {
            var serverUrl = httpClient.BaseUrl?.OriginalString;
            
            if (serverUrl == null)
            {
                throw new TfsClientException(
                    "Can't create conection. Server URL of given HTTP client is null",
                    new ArgumentNullException(nameof(httpClient.BaseUrl))
                );
            }

            return new ClientConnection(httpClient, serverUrl, projectName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientConnection"></param>
        /// <returns></returns>
        public static IWorkitemClient GetWorkitemClient(this IClientConnection clientConnection)
        {
            return new WorkitemClient.WorkitemClient(clientConnection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientConnection"></param>
        /// <returns></returns>
        public static IProjectClient GetProjectClient(this IClientConnection clientConnection)
        {
            return new ProjectClient.ProjectClient(clientConnection);
        }
    }
}
