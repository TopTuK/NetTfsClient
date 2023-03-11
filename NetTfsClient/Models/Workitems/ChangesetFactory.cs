using NetTfsClient.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    internal static class ChangesetFactory
    {
        private class Changeset : IChangeset
        {
            private readonly IWorkitemClient _client;

            public int ChangesetId { get; init; }

            public IEnumerable<int> ItemIds { get; init; } = Enumerable.Empty<int>();
            public bool IsEmpty => (Count == 0);
            public int Count => ItemIds.Count();

            public Changeset(IWorkitemClient workitemClient, JToken jsonChangeset)
            {
                _client = workitemClient;

                ChangesetId = jsonChangeset["changesetId"]?.ToObject<int>() ?? -1;

                if (jsonChangeset["workItems"] != null)
                {
                    ItemIds = jsonChangeset["worktems"]!
                        .Select(jsonItem => jsonItem["id"]!.ToObject<int>())
                        .ToArray();
                }
            }

            public async Task<IEnumerable<IWorkitem>> GetAssociatedWorkitemsAsync()
            {
                return !IsEmpty
                    ? await _client.GetWorkitemsAsync(ItemIds)
                    : Enumerable.Empty<IWorkitem>();
            }
        }

        public static IChangeset FromJsonResponse(IWorkitemClient workitemClient, string jsonResponse)
        {
            var jsonObj = JObject.Parse(jsonResponse);
            return new Changeset(workitemClient, jsonObj);
        }
    }
}
