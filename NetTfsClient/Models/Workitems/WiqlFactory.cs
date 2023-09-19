using NetTfsClient.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// Internal WIQL result factory
    /// </summary>
    internal static class WiqlFactory
    {
        private class WiqlResult : IWiqlResult
        {
            private readonly IWorkitemClient _workitemClient;

            private int[] _itemIds = Array.Empty<int>();

            public IEnumerable<int> ItemIds => _itemIds;
            public bool IsEmpty => _itemIds.Count() == 0;
            public int Count => _itemIds.Count();

            public WiqlResult(IWorkitemClient workitemClient, JToken jsonItems)
            {
                _workitemClient = workitemClient;

                var itemCount = jsonItems
                    .Children()
                    .Count();
                if (itemCount > 0)
                {
                    _itemIds = jsonItems.Children()
                        .Select(jItem => jItem["id"]!.ToObject<int>())
                        .ToArray();
                }
            }

            public async Task<IEnumerable<IWorkitem>> GetWorkitemsAsync()
            {
                return !IsEmpty
                    ? await _workitemClient.GetWorkitemsAsync(_itemIds)
                    : Enumerable.Empty<IWorkitem>();
            }
        }

        public static IWiqlResult FromContentResponse(IWorkitemClient workitemClient, string contentResponse)
        {
            try
            {
                var jsonObj = JObject.Parse(contentResponse);
                if (jsonObj["workItems"] == null)
                {
                    throw new ClientException("WiqlFactory::FromContentResponse: json content response doesn't contain \"workitems\" attribute");
                }

                return new WiqlResult(workitemClient, jsonObj["workItems"]!);
            }
            catch (Exception ex)
            {
                throw new ClientException(
                    "WiqlFactory::FromContentResponse: exception raised",
                    ex);
            }
        }
    }
}
