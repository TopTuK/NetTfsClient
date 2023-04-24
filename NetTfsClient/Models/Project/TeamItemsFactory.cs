using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
    /// <summary>
    /// Internal teams items factory
    /// </summary>
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

        private class TeamMember : Member, ITeamMember
        {
            public ITeam Team { get; init; }
            public bool IsTeamAdmin { get; init; }


            public TeamMember(ITeam team, JToken jMember) 
                : base(jMember)
            {
                Team = team;
                IsTeamAdmin = jMember["isTeamAdmin"]?.Value<bool>() ?? false;
            }
        }

        private class Member : IMember
        {
            public string Id { get; init; }
            public string DisplayName { get; init; }
            public string UniqueName { get; init; }
            public string Url { get; init; }

            public Member(JToken jMember)
            {
                if (jMember["identity"] != null)
                {
                    var jIdentity = jMember["identity"]!;

                    Id = jIdentity["id"]?.Value<string>() ?? string.Empty;
                    DisplayName = jIdentity["displayName"]?.Value<string>() ?? string.Empty;
                    UniqueName = jIdentity["uniqueName"]?.Value<string>() ?? string.Empty;
                    Url = jIdentity["url"]?.Value<string>() ?? string.Empty;
                }
                else if (jMember["id"] != null)
                {
                    Id = jMember["id"]?.Value<string>() ?? string.Empty;
                    DisplayName = jMember["displayName"]?.Value<string>() ?? string.Empty;
                    UniqueName = jMember["uniqueName"]?.Value<string>() ?? string.Empty;
                    Url = jMember["url"]?.Value<string>() ?? string.Empty;
                }
                else
                {
                    Id = string.Empty;
                    DisplayName = string.Empty;
                    UniqueName = string.Empty;
                    Url = string.Empty;
                }
            }
        }

        public static IMember MemberFromJsonItem(JToken jsonItem)
        {
            return new Member(jsonItem);
        }

        public static IEnumerable<ITeamMember> TeamMembersFromJsonItems(ITeam team, string jsonContent)
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
