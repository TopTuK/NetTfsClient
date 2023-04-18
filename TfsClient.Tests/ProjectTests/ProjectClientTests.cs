using Microsoft.Extensions.Configuration;
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
            Assert.True(projects.Any());

            Assert.NotNull(teams);
            Assert.True(teams.Any());

            Assert.NotNull(prjTeams);

            Assert.NotNull(members);
            Assert.True(members.Any());
        }
    }
}
