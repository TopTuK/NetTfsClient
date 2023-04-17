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

        public ProjectClientTests()
        {
            var configuration = new ConfigurationBuilder()
               .AddUserSecrets<WorkitemClientTests>()
               .Build();

            var serverUrl = configuration["ENV_SERVER_URL"];
            var projectName = configuration["ENV_PROJECT_NAME"];
            var pat = configuration["ENV_PAT"];

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
