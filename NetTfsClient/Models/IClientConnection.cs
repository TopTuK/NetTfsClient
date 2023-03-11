using NetTfsClient.Services.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models
{
    public interface IClientConnection
    {
        /// <summary>
        /// Get Server URL
        /// </summary>
        /// <example>For example: "https://my-tfs/tfs/"</example>
        string ServerUrl { get; }

        /// <summary>
        /// Get TFS Collection name
        /// </summary>
        /// <example>
        /// if you set project name as DefaultCollection/MyProject this property return DefaultCollection
        /// </example>
        string CollectionName { get; }

        /// <summary>
        /// Get or set current TFS project name
        /// </summary>
        /// <example>
        /// if you set project name as DefaultCollection/MyProject this property return MyProject
        /// </example>
        string ProjectName { get; set; }

        /// <summary>
        /// Ends with '/'
        /// </summary>
        string ApiUrl { get; }
        /// <summary>
        /// Ends with '/'
        /// </summary>
        string ProjectApiUrl { get; }

        IHttpClient HttpClient { get; }
    }
}
