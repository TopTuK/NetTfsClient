using Microsoft.Extensions.Configuration;
using NetTfsClient.Models.Project;
using NetTfsClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TfsClient.Tests.WorkitemsTests;
using IIdentity = NetTfsClient.Models.Project.IIdentity;

namespace TfsClient.Tests.ProjectTests
{
    public class ProjectClientNonPublicTests
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

        public ProjectClientNonPublicTests()
        {
            SetEnvironmentVariablesFromUserSecrets();

            string serverUrl = Environment.GetEnvironmentVariable("ENV_SERVER_URL")!;
            string projectName = Environment.GetEnvironmentVariable("ENV_PROJECT_NAME")!;
            string pat = Environment.GetEnvironmentVariable("ENV_PAT")!;

            var clientConnection = ClientFactory.CreateClientConnection(serverUrl, projectName, pat);
            _prjClient = clientConnection.GetProjectClient();
        }

        [Fact(DisplayName = "Test of getting groups for project (Simple)")]
        public async Task GetProjectGroupsSuccessTest()
        {
            // Arrange
            IEnumerable<IIdentity>? identities = null;

            // Act
            var projects = await _prjClient.GetProjectsAsync();
            if (projects.Any())
            {
                var project = projects.First();
                identities = await _prjClient.GetProjectGroupsAsync(project);
            }

            // Assert
            Assert.NotNull(identities);
            Assert.True(identities!.Any());

            foreach (var identity in identities!)
            {
                Assert.True(identity.IsGroup || identity.IsUser || identity.IsTeam, $"Identity type is not user or group. Type={identity.IdentityType}");
            }
        }

        [Fact(DisplayName = "Test get project group members")]
        public async Task GetProjectGroupMembersSuccessTest()
        {
            // Arrange
            Dictionary<IIdentity, object>? groups = null;

            // Act
            var projects = await _prjClient.GetProjectsAsync();
            if (projects.Any())
            {
                var project = projects.First();
                var identities = await _prjClient.GetProjectGroupsAsync(project);

                if (identities.Any())
                {
                    groups = new Dictionary<IIdentity, object>(identities.Count());
                    foreach (var identity in identities)
                    {
                        if (identity.IsGroup)
                        {
                            var groupMembers = await _prjClient.GetProjectGroupMembersAsync(project, identity);
                            groups.Add(identity, groupMembers);
                        }
                        else if (identity.IsTeam)
                        {
                            var team = await _prjClient.GetTeamAsync(project.Id, identity.FoundationId, true);
                            var teamMembers = await _prjClient.GetProjectTeamMembersAsync(project, team);
                            
                            groups.Add(identity, teamMembers);
                        }
                        else // User?
                        {
                            groups.Add(identity, identity);
                        }
                    }
                }
            }

            // Assert
            Assert.NotNull(groups);
            Assert.True(groups!.Any());

            foreach(var group in groups!)
            {
                var identity = group.Key;

                Assert.True(identity.IsGroup || identity.IsUser || identity.IsTeam, $"Identity type is not user or group. Type={identity.IdentityType}");

                if (identity.IsGroup)
                {
                    foreach (var member in (IEnumerable<IIdentity>) group.Value)
                    {
                        Assert.True(member.IsGroup || member.IsUser || member.IsTeam, $"Identity member type is not user or group. Type={member.IdentityType}");
                    }
                }
                else if (identity.IsTeam)
                {
                    var teamMembers = (IEnumerable<ITeamMember>) group.Value;
                    Assert.True(teamMembers.Any());
                }
            }
        }
    }
}
