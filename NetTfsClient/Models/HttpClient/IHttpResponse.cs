using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.HttpClient
{
    /// <summary>
    /// Http Response object with properties
    /// </summary>
    public interface IHttpResponse
    {
        int StatusCode { get; }
        bool IsSuccess { get; }
        bool HasError { get; }

        Uri? RequestUrl { get; }
        string? Content { get; }

        IReadOnlyDictionary<string, string?>? Headers { get; }
        string? ContentType { get; }

        IReadOnlyDictionary<string, string>? Cookies { get; }
        bool IsEmptyCookies { get; }
    }
}
