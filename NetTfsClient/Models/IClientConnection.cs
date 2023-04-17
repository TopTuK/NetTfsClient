using NetTfsClient.Services.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models
{
    /// <summary>
    /// IClientConnection interface contains information about connecting to the TFS/Azure service
    /// </summary>
    public interface IClientConnection
    {
        /// <summary>
        /// Returns TFS/Azure server URL
        /// </summary>
        /// <example>https://my-tfs/tfs/</example>
        string ServerUrl { get; }

        /// <summary>
        /// Returns TFS/Azure collection name
        /// </summary>
        /// <example>
        /// if you set project name as DefaultCollection/MyProject this property return DefaultCollection
        /// </example>
        string CollectionName { get; }

        /// <summary>
        /// Returns TFS/Azure project name.
        /// Sets project name for current connection.
        /// </summary>
        /// <example>if you set project name as DefaultCollection/MyProject this property return MyProject</example>
        string ProjectName { get; set; }

        /// <summary>
        /// Returns TFS/Azure API part of URL (collection without project). Ends with '/'.
        /// </summary>
        string ApiUrl { get; }

        /// <summary>
        /// Returns TFS/Azure project API part of URL. Ends with '/'.
        /// </summary>
        string ProjectApiUrl { get; }

        /// <summary>
        /// Returns instance of HttpClient with defined authorization
        /// </summary>
        IHttpClient HttpClient { get; }
    }
}
