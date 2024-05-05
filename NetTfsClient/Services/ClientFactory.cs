using NetTfsClient.Models;
using NetTfsClient.Services.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services
{
    /// <summary>
    /// Main factory of library. Used to get an instance of a Client Connection.
    /// With this factory, you can get facades for managing workitems (WorkitemClient), projects (ProjectClient) and mentions (MentionFacade)
    /// </summary>
    public static class ClientFactory
    {
        // ServerUrl should ends with '/'
        private static string TranslateServerUrl(string serverUrl) => serverUrl.EndsWith("/")
            ? serverUrl
            : $"{serverUrl}/";

        private class ClientConnection : IClientConnection
        {
            public string ServerUrl { get; init; }

            public string CollectionName { get; }
            public string ProjectName { get; set; }

            // API responce only in Project, without subproject
            // Remove part after / in project-name, like DefaultCollection/MyProject => DefaultCollection
            public string ApiUrl => $"{CollectionName}/_apis/";

            // API response only for Collection/Project
            public string ProjectApiUrl => $"{CollectionName}/{ProjectName}/_apis/";

            public IHttpClient HttpClient { get; init; }

            public ClientConnection(IHttpClient httpClient, string serverUrl, string projectName)
            {
                HttpClient = httpClient;

                ServerUrl = serverUrl;
                // ServerUrl should ends with '/'
                /*ServerUrl = serverUrl.EndsWith("/")
                    ? serverUrl
                    : $"{serverUrl}/";*/
                // Changing base url is deprecated
                //HttpClient.BaseUrl = new Uri(ServerUrl);

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
        /// Creates IClientConnection instance with NTLM authorization for connection to TFS/Azure.
        /// Internally creates default HTTP client.
        /// </summary>
        /// <param name="userName">User name for NTLM auth</param>
        /// <param name="userPassword">User password for NTLM auth</param>
        /// <param name="serverUrl">URL of TFS/Azure service. Ex.: https://my-tfs/tfs</param>
        /// <param name="projectName">Project name with collection. Default is 'DefaultCollection' without project</param>
        /// <returns>Instance of IClientConnection with NTLM authorization for connection to TFS/Azure</returns>
        public static IClientConnection CreateClientConnection(string userName, string userPassword,
            string serverUrl, string projectName = "DefaultCollection")
        {
            serverUrl = TranslateServerUrl(serverUrl);
            var httpClient = HttpClientFactory.CreateHttpClient(serverUrl, userName, userPassword);

            return new ClientConnection(httpClient, serverUrl, projectName);
        }

        /// <summary>
        /// Creates IClientConnection instance with authorization with personal access token for connection to TFS/Azure.
        /// Internally creates default HTTP client.
        /// </summary>
        /// <param name="serverUrl">URL of TFS/Azure service</param>
        /// <param name="projectName">Project name with collection. Default is 'DefaultCollection' without project</param>
        /// <param name="personalAccessToken">Personal access token for connection to TFS/Azure service</param>
        /// <returns>Instance of IClientConnection with authorization with PAT for connection to TFS/Azure</returns>
        public static IClientConnection CreateClientConnection(string serverUrl, string projectName,
            string personalAccessToken)
        {
            serverUrl = TranslateServerUrl(serverUrl);
            var httpClient = HttpClientFactory.CreateHttpClient(serverUrl, personalAccessToken);

            return new ClientConnection(httpClient, serverUrl, projectName);
        }

        /// <summary>
        /// Creates IClientConnection instance with authorization with given claims principal
        /// </summary>
        /// <param name="serverUrl">URL of TFS/Azure service</param>
        /// <param name="projectName">Project name with collection. Default is 'DefaultCollection' without project</param>
        /// <param name="claimsPrincipal">Claims principal</param>
        /// <returns>Instance of IClientConnection with authorization with claims principal</returns>
        public static IClientConnection CreateClientConnection(string serverUrl, string projectName,
            ClaimsPrincipal claimsPrincipal)
        {
            serverUrl = TranslateServerUrl(serverUrl);
            var httpClient = HttpClientFactory.CreateHttpClient(serverUrl, claimsPrincipal);

            return new ClientConnection(httpClient, serverUrl, projectName);
        }

        /// <summary>
        /// Creates IClientConnection instance with given instance of IHttpClient for connection to TFS/Azure.
        /// Used for custom authorization realization.
        /// </summary>
        /// <param name="httpClient">Instance of IHttpClient with custom authorization</param>
        /// <param name="projectName">Project name with collection. Default is 'DefaultCollection' without project</param>
        /// <returns>Instance of IClientConnection with custom authorization</returns>
        /// <exception cref="ClientException">Raised if BaseUrl of instance of IHttpClient is null</exception>
        public static IClientConnection CreateClientConnection(IHttpClient httpClient, string projectName)
        {
            var serverUrl = httpClient.BaseUrl?.OriginalString;
            
            if (serverUrl == null)
            {
                throw new ClientException(
                    "Can't create conection. Server URL of given HTTP client is null",
                    new ArgumentNullException(nameof(httpClient.BaseUrl))
                );
            }

            return new ClientConnection(httpClient, serverUrl, projectName);
        }

        /// <summary>
        /// Returns instance of IWorkitemClient facade for managing workitems.
        /// </summary>
        /// <param name="clientConnection">Instance of IClientConnection with defined authorization</param>
        /// <returns>Instance of IWorkitemClient facade</returns>
        public static IWorkitemClient GetWorkitemClient(this IClientConnection clientConnection)
        {
            return new WorkitemClient.WorkitemClient(clientConnection);
        }

        /// <summary>
        /// Returns instance of IProjectClient facade for managing projects, teams and members of TFS/Azure service.
        /// </summary>
        /// <param name="clientConnection">Instance of IClientConnection with defined authorization</param>
        /// <returns>Instance of IProjectClient facade</returns>
        public static IProjectClient GetProjectClient(this IClientConnection clientConnection)
        {
            return new ProjectClient.ProjectClient(clientConnection);
        }

        /// <summary>
        /// Returns instance of IMentionClient facade for mention users
        /// </summary>
        /// <param name="clientConnection">Instance of IClientConnection with defined authorization</param>
        /// <returns>Instance of MentionClient facade for mention users in workitems</returns>
        public static IMentionClient GetMentionClient(this IClientConnection clientConnection)
        {
            return new MentionClient.MentionClient();
        }
    }
}