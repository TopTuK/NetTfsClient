using NetTfsClient.Models;
using NetTfsClient.Models.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services.ProjectClient
{
    internal class ProjectClient : BaseClient, IProjectClient
    {
        private const string PROJECTS_URL = @"projects";
        private const string TEAMS_URL = @"teams";
        private const string PROJECT_TEAM_MEMBERS = @"members";

        public ProjectClient(IClientConnection clientConnection)
            : base(clientConnection)
        {
        }

        public async Task<IEnumerable<IProject>> GetProjectsAsync(int skip = 0)
        {
            var requestUrl = $"{clientConnection.ApiUrl}{PROJECTS_URL}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION },
                { "$skip", skip.ToString() }
            };

            try
            {
                List<IProject> projects = new();
                bool hasNext = false;

                do
                {
                    hasNext = false;

                    var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                    if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                    {
                        throw new TfsClientException("IProjectClient::GetProjectsAsync: HTTP response is empty or null");
                    }

                    var items = ProjectItemsFactory.FromJsonItems(httpResponse.Content);
                    if (items.Any())
                    {
                        projects.AddRange(items);
                        queryParams["$skip"] = projects
                            .Count
                            .ToString();
                        hasNext = true;
                    }

                    return projects;
                }
                while (hasNext);
            }
            catch (Exception ex)
            {
                throw new TfsClientException("IProjectClient::GetProjectsAsync: exception raised", ex);
            }
        }

        public async Task<IEnumerable<ITeam>> GetAllTeamsAsync(bool currentUser = false)
        {
            var requestUrl = $"{clientConnection.ApiUrl}{TEAMS_URL}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION },
                { "$mine", $"{currentUser}" }
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new TfsClientException("IProjectClient::GetAllTeamsAsync: HTTP response is empty or null");
                }

                return TeamItemsFactory.TeamsFromJsonItems(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new TfsClientException("IProjectClient::GetAllTeamsAsync: exception raised", ex);
            }
        }

        public async Task<IEnumerable<ITeam>> GetProjectTeamsAsync(IProject project, 
            bool currentUser = false)
        {
            var requestUrl = $"{clientConnection.ApiUrl}{PROJECTS_URL}/{project.Id}/{TEAMS_URL}";

            var queryParams = new Dictionary<string, string>()
            {
                { "api-version", API_VERSION },
                { "$mine", $"{currentUser}" }
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new TfsClientException("IProjectClient::GetProjectTeamsAsync: HTTP response is empty or null");
                }

                return TeamItemsFactory.TeamsFromJsonItems(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new TfsClientException("IProjectClient::GetProjectTeamsAsync: exception raised", ex);
            }
        }

        // GET {apiUrl}/{organization}/_apis/projects/{projectId}/teams/{teamId}/members?api-version=6.0
        public async Task<IEnumerable<ITeamMember>> GetProjectTeamMembersAsync(IProject project, ITeam team)
        {
            var requestUrl = $"{clientConnection.ApiUrl}" +
                $"{PROJECTS_URL}/{project.Id}/" +
                $"{TEAMS_URL}/{team.Id}/{PROJECT_TEAM_MEMBERS}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION }
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new TfsClientException("IProjectClient::GetProjectTeamMembersAsync: HTTP response is empty or null");
                }

                return TeamItemsFactory.MembersFromJsonItems(team, httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new TfsClientException("IProjectClient::GetProjectTeamMembersAsync: exception raised", ex);
            }
        }
    }
}
