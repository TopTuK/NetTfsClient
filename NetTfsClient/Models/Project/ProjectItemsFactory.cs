using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
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

        public static IEnumerable<IProject> FromJsonContent(string jsonContent)
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
    }
}
