using Microsoft.Extensions.Configuration;
using NetTfsClient.Models.Boards;
using NetTfsClient.Models.Project;
using NetTfsClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsClient.Tests.WorkitemsTests;

namespace TfsClient.Tests.ProjectTests
{
    public class ProjectClientTests
    {
        private readonly IProjectClient _prjClient;

        private static void SetEnvironmentVariablesFromUserSecrets()
        {
            var configuration = new ConfigurationBuilder()
               .AddUserSecrets<WorkitemClientTests>()
               .Build();

            foreach (var child in configuration.GetChildren())
            {
                Environment.SetEnvironmentVariable(child.Key, child.Value);
            }
        }

        public ProjectClientTests()
        {
            SetEnvironmentVariablesFromUserSecrets();

            string serverUrl = Environment.GetEnvironmentVariable("ENV_SERVER_URL")!;
            string projectName = Environment.GetEnvironmentVariable("ENV_PROJECT_NAME")!;
            string pat = Environment.GetEnvironmentVariable("ENV_PAT")!;

            var clientConnection = ClientFactory.CreateClientConnection(serverUrl, projectName, pat);
            _prjClient = clientConnection.GetProjectClient();
        }

        [Fact(DisplayName = "Complex test of ProjectClient")]
        public async Task MakeProjectClientTests()
        {
            // Arrange
            IEnumerable<ITeam>? prjTeams = null;
            IEnumerable<ITeamMember>? members = null;

            // Act
            var projects = await _prjClient.GetProjectsAsync();
            var teams = await _prjClient.GetAllTeamsAsync();

            if ((projects != null) && (projects.Any()))
            {
                prjTeams = await _prjClient.GetProjectTeamsAsync(projects.First());

                if ((prjTeams != null) && (prjTeams.Any()))
                {
                    members = await _prjClient.GetProjectTeamMembersAsync(projects.First(), prjTeams.First());
                }
            }

            // Assert
            Assert.NotNull(projects);
            Assert.True(projects!.Any());

            Assert.NotNull(teams);
            Assert.True(teams.Any());

            Assert.NotNull(prjTeams);

            Assert.NotNull(members);
            Assert.True(members!.Any());
        }

        private async Task<IProject?> GetProject()
        {
            var projects = await _prjClient.GetProjectsAsync();
            return projects.FirstOrDefault();
        }

        private async Task<ITeam?> GetProjectTeam(IProject project)
        {
            var teams = await _prjClient.GetProjectTeamsAsync(project);
            return teams.FirstOrDefault();
        }


        [Fact(DisplayName = "Boards test for ProjectClient")]
        public async Task MakeBoardClientTests()
        {
            // Arrange
            IProject? project = null;
            ITeam? team = null;

            var boards = Enumerable.Empty<IBaseBoard>();
            IBoard? board = null;

            // Act
            project = await GetProject();
            if (project != null)
            {
                team = await GetProjectTeam(project);
                if (team != null)
                {
                    boards = await _prjClient.GetProjectTeamBoardsAsync(project, team);

                    if (boards.Any())
                    {
                        var baseBoard = boards.First();
                        board = await _prjClient.GetProjectTeamBoardAsync(project, team, baseBoard.Id);
                    }
                }
            }

            // Assert
            Assert.NotNull(project);
            Assert.NotNull(team);

            Assert.True(boards!.Any());

            Assert.NotNull(board);
            Assert.NotNull(board!.Name);
            Assert.True(board!.Columns.Any());
            Assert.True(board!.Rows.Any());
        }
    }
}
