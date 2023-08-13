using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
    /// <summary>
    /// Internal project items factory
    /// </summary>
    internal static class ProjectItemsFactory
    {
        private class Project : IProject
        {
            public string Id { get; init; }
            public string Name { get; init; }
            public string Description { get; init; }

            public Project(JToken jsonItem)
            {
                Id = jsonItem["id"]?.Value<string>() ?? string.Empty;
                Name = jsonItem["name"]?.Value<string>() ?? string.Empty;
                Description = jsonItem["description"]?.Value<string>() ?? string.Empty;
            }
        }

        public static IProject ProjectFromJson(string jsonContent)
        {
            var jsonItem = JObject.Parse(jsonContent);
            return new Project(jsonItem);
        }

        public static IEnumerable<IProject> ProjectsFromJsonContent(string jsonContent)
        {
            var jsonItems = JObject.Parse(jsonContent);
            if (jsonItems["value"] != null)
            {
                return jsonItems["value"]!
                    .Select(jsonProject => new Project(jsonProject))
                    .ToList();
            }

            return Enumerable.Empty<IProject>();
        }

        private class Identity : IIdentity
        {
            public string IdentityType { get; init; }

            public string FriendlyName { get; init; }
            public string DisplayName { get; init; }

            public string FoundationId { get; init; }

            public bool IsGroup => IdentityType.Equals("group");
            public bool IsTeam => IdentityType.Equals("team");
            public bool IsUser => IdentityType.Equals("user");

            public Identity(JToken jsonItem)
            {
                // Must be field
                FoundationId = jsonItem["TeamFoundationId"]!.Value<string>()!;

                IdentityType = jsonItem["IdentityType"]?.Value<string>() ?? "Unknown";

                FriendlyName = jsonItem["FriendlyDisplayName"]?.Value<string>() ?? string.Empty;
                DisplayName = jsonItem["DisplayName"]?.Value<string>() ?? string.Empty;
            }
        }

        public static IEnumerable<IIdentity> IdentitiesFromJsonContent(string jsonContent)
        {
            var jsonItems = JObject.Parse(jsonContent);

            if (jsonItems["identities"] != null)
            {
                return jsonItems["identities"]!
                    .Select(jsonIdentity => new Identity(jsonIdentity))
                    .ToList();
            }

            return Enumerable.Empty<IIdentity>();
        }
    }
}
