using Microsoft.Extensions.Configuration;
using NetTfsClient.Models.Boards;
using NetTfsClient.Models.Project;
using NetTfsClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TfsClient.Tests.WorkitemsTests;

namespace TfsClient.Tests.BoardTests
{
    public class BoardClientTests
    {
        private readonly IBoardClient _boardClient;
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

        public BoardClientTests()
        {
            SetEnvironmentVariablesFromUserSecrets();

            string serverUrl = Environment.GetEnvironmentVariable("ENV_SERVER_URL")!;
            string projectName = Environment.GetEnvironmentVariable("ENV_PROJECT_NAME")!;
            string pat = Environment.GetEnvironmentVariable("ENV_PAT")!;

            var clientConnection = ClientFactory.CreateClientConnection(serverUrl, projectName, pat);
            _boardClient = clientConnection.GetBoardClient();
            _prjClient = clientConnection.GetProjectClient();
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


        [Fact(DisplayName = "Complex test of BoardClient")]
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
                    boards = await _boardClient.GetBoardsAsync(project.Id, team.Id);

                    if (boards.Any())
                    {
                        var baseBoard = boards.First();
                        board = await _boardClient.GetBoardAsync(project.Id, team.Id, baseBoard.Id);
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
