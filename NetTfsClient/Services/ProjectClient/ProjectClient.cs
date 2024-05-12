using NetTfsClient.Models;
using NetTfsClient.Models.Boards;
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
        private const string BOARDS_URL = @"boards";
        private const string TEAM_SETTINGS_URL = @"teamsettings";

        public ProjectClient(IClientConnection clientConnection)
            : base(clientConnection)
        {
        }

        public async Task<IProject> GetProjectAsync(string projectId, bool capabilities = false, bool history = false)
        {
            // request_url = f'{self.client_connection.api_url}{self._URL_PROJECTS}/{project_id}'
            var requestUrl = $"{clientConnection.ApiUrl}{PROJECTS_URL}/{projectId}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION },
                { "includeCapabilities", capabilities.ToString() },
                { "includeHistory", history.ToString() },
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IProjectClient::GetProjectAsync: HTTP response is empty or null");
                }

                return ProjectItemsFactory.ProjectFromJson(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetProjectAsync: exception raised", ex);
            }
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
                        throw new ClientException("IProjectClient::GetProjectsAsync: HTTP response is empty or null");
                    }

                    var items = ProjectItemsFactory.ProjectsFromJsonContent(httpResponse.Content);
                    if (items.Any())
                    {
                        projects.AddRange(items);
                        queryParams["$skip"] = projects
                            .Count
                            .ToString();
                        hasNext = true;
                    }
                }
                while (hasNext);

                return projects;
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetProjectsAsync: exception raised", ex);
            }
        }

        public async Task<ITeam> GetTeamAsync(string projectId, string teamId, bool expand = false)
        {
            // request_url = f'{self.client_connection.api_url}{self._URL_PROJECTS}/{project_id}/{self._URL_TEAMS}/{team_id}'
            var requestUrl = $"{clientConnection.ApiUrl}{PROJECTS_URL}/{projectId}/{TEAMS_URL}/{teamId}";
            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION },
                { "$expandIdentity", $"{expand}" }
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IProjectClient::GetTeamAsync: HTTP response is empty or null");
                }

                return TeamItemsFactory.TeamFromJson(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetTeamAsync: exception raised", ex);
            }
        }

        public async Task<IEnumerable<ITeam>> GetAllTeamsAsync(bool currentUser = false)
        {
            var requestUrl = $"{clientConnection.ApiUrl}{TEAMS_URL}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_PREVIEW_VERSION },
                { "$mine", $"{currentUser}" }
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IProjectClient::GetAllTeamsAsync: HTTP response is empty or null");
                }

                return TeamItemsFactory.TeamsFromJsonItems(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetAllTeamsAsync: exception raised", ex);
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
                    throw new ClientException("IProjectClient::GetProjectTeamsAsync: HTTP response is empty or null");
                }

                return TeamItemsFactory.TeamsFromJsonItems(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetProjectTeamsAsync: exception raised", ex);
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
                    throw new ClientException("IProjectClient::GetProjectTeamMembersAsync: HTTP response is empty or null");
                }

                return TeamItemsFactory.TeamMembersFromJsonItems(team, httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetProjectTeamMembersAsync: exception raised", ex);
            }
        }

        // {project_id}/_api/_identity/ReadScopedApplicationGroupsJson?__v=5
        public async Task<IEnumerable<IIdentity>> GetProjectGroupsAsync(IProject project)
        {
            var requestUrl = $"{clientConnection.CollectionName}/"
                + $"{project.Id}/_api/_identity/ReadScopedApplicationGroupsJson";

            var queryParams = new Dictionary<string, string>
            {
                { "__v", "5" },
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);
                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IProjectClient::GetProjectGroups: HTTP response is empty or null");
                }

                return ProjectItemsFactory.IdentitiesFromJsonContent(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetProjectGroups: exception raised", ex);
            }
        }

        // {project_id}/_api/_identity/ReadGroupMembers?__v=5&scope={group.foundation_id}&readMembers=true
        public async Task<IEnumerable<IIdentity>> GetProjectGroupMembersAsync(IProject project, IIdentity identity)
        {
            if (!identity.IsGroup)
            {
                throw new ArgumentException("IProjectClient::GetProjectGroupMembersAsync: identity should be groupd", nameof(identity));
            }

            // request_url = f'{self.client_connection.collection}/{project.id}/_api/_identity/ReadGroupMembers'
            var requestUrl = $"{clientConnection.CollectionName}/"
                + $"{project.Id}/_api/_identity/ReadGroupMembers";

            var queryParams = new Dictionary<string, string>
            {
                { "__v", "5" },
                { "scope", identity.FoundationId },
                { "readMembers", "true" },
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);
                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IProjectClient::GetProjectGroupMembersAsync: HTTP response is empty or null");
                }

                return ProjectItemsFactory.IdentitiesFromJsonContent(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetProjectGroupMembersAsync: exception raised", ex);
            }
        }

        public async Task<IBoard?> GetProjectTeamBoardAsync(IProject project, ITeam team, string boardId)
        {
            if (boardId == string.Empty)
            {
                throw new ArgumentException("Board Id can\'t be empty string", nameof(boardId));
            }

            var requestUrl = $"{clientConnection.CollectionName}/{project.Id}/{team.Id}/_apis/work/{BOARDS_URL}/{boardId}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION },
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IBoardClient::GetBoardAsync: HTTP response is empty or null");
                }

                return BoardItemsFactory.BoardFromJsonContent(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IBoardClient::GetBoardAsync: exception raised", ex);
            }
        }

        public async Task<IEnumerable<IBoard>> GetProjectTeamBoardsAsync(IProject project, ITeam team)
        {
            var requestUrl = $"{clientConnection.CollectionName}/{project.Id}/{team.Id}/_apis/work/{BOARDS_URL}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION },
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IProjectClient::GetProjectTeamBoardsAsync: HTTP response is empty or null");
                }

                var boardsInfo = BoardItemsFactory.BaseBoardsFromJsonContent(httpResponse.Content);
                
                var boards = new List<IBoard>(boardsInfo.Count());
                foreach (var boardInfo in boardsInfo)
                {
                    var board = await GetProjectTeamBoardAsync(project, team, boardInfo.Id);
                    if (board == null)
                    {
                        throw new ClientException($"IProjectClient::GetProjectTeamBoardsAsync: can\'t get board with Id={boardInfo.Id}");
                    }

                    boards.Add(board);
                }

                return boards;
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetProjectTeamBoardsAsync: exception raised", ex);
            }
        }

        public async Task<ITeamSettings?> GetProjectTeamSettingsAsync(IProject project, ITeam team)
        {
            var requestUrl = $"{clientConnection.CollectionName}/{project.Id}/{team.Id}/_apis/work/{TEAM_SETTINGS_URL}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION },
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IProjectClient::GetProjectTeamBoardsAsync: HTTP response is empty or null");
                }

                return TeamItemsFactory.TeamSettingsFromJson(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IProjectClient::GetProjectTeamSettingsAsync: exception raised", ex);
            }
        }
    }
}
