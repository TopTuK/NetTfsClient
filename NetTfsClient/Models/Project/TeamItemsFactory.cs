using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
    internal static class TeamItemsFactory
    {
        private class Team : ITeam
        {
            public string Id { get; init; }
            public string Name { get; init; }
            public string Description { get; init; }

            public Team(JToken jTeam)
            {
                Id = jTeam["id"]?.Value<string>() ?? string.Empty;
                Name = jTeam["name"]?.Value<string>() ?? string.Empty;
                Description = jTeam["description"]?.Value<string>() ?? string.Empty;
            }
        }

        public static IEnumerable<ITeam> TeamsFromJsonItems(string jsonContent)
        {
            var jsonItems = JObject.Parse(jsonContent);
            if (jsonItems["value"] != null)
            {
                return jsonItems["value"]!
                    .Select(jTeam => new Team(jTeam))
                    .ToList();
            }

            return Enumerable.Empty<ITeam>();
        }

        private class TeamMember : ITeamMember
        {
            public ITeam Team { get; init; }
            public bool IsTeamAdmin { get; init; }

            public string Id { get; init; }
            public string DisplayName { get; init; }
            public string Url { get; init; }

            public TeamMember(ITeam team, JToken jMember)
            {
                Team = team;
                IsTeamAdmin = jMember["isTeamAdmin"]?.Value<bool>() ?? false;

                var jIdentity = jMember["identity"];
                if (jIdentity != null)
                {
                    Id = jIdentity["id"]?.Value<string>() ?? string.Empty;
                    DisplayName = jIdentity["displayName"]?.Value<string>() ?? string.Empty;
                    Url = jIdentity["url"]?.Value<string>() ?? string.Empty;
                }
                else
                {
                    Id = string.Empty;
                    DisplayName = string.Empty;
                    Url = string.Empty;
                }
            }
        }

        public static IEnumerable<ITeamMember> MembersFromJsonItems(ITeam team, string jsonContent)
        {
            var jsonItems = JObject.Parse(jsonContent);

            if (jsonItems["value"] != null)
            {
                return jsonItems["value"]!
                    .Select(jMember => new TeamMember(team, jMember))
                    .ToList();
            }

            return Enumerable.Empty<ITeamMember>();
        }
    }
}
