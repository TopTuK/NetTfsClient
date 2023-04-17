using Microsoft.Extensions.Configuration;
using NetTfsClient.Models.Workitems;
using NetTfsClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsClient.Tests.WorkitemsTests
{
    public class WorkitemClientTests
    {
        private readonly IWorkitemClient _workitemClient;

        public WorkitemClientTests()
        {
            var configuration = new ConfigurationBuilder()
               .AddUserSecrets<WorkitemClientTests>()
               .Build();

            var serverUrl = configuration["ENV_SERVER_URL"];
            var projectName = configuration["ENV_PROJECT_NAME"];
            var pat = configuration["ENV_PAT"];

            var clientConnection = ClientFactory.CreateClientConnection(serverUrl, projectName, pat);
            _workitemClient = clientConnection.GetWorkitemClient();
        }

        [Fact(DisplayName = "Async get workitem by Id")]
        public async Task GetSingleWorkitemTestSuccess()
        {
            // Arrange
            var workitem_title = "[BRQ] First test requirement";
            var workitem_id = 1;

            // Act
            var wi = await _workitemClient.GetSingleWorkitemAsync(workitem_id);

            // Assert
            Assert.NotNull(wi);
            Assert.Equal(workitem_id, wi.Id);
            Assert.Equal(workitem_title, wi.Title);
        }

        [Fact(DisplayName = "Create new workitem. Type: Requirement")]
        public async Task CreateWorkitemTestSuccess()
        {
            // Arrange
            var today = DateTime.Today;
            var wi_type_name = "Requirement";
            var wi_title = $"[BRQ] Created test requirement - {today}";
            var wi_description = $"This BRQ was created by GitHub at {today}";

            var wi_fields = new Dictionary<string, string>()
            {
                { "System.Title", wi_title },
                { "System.Description", wi_description },
            };

            // Act
            var wi = await _workitemClient.CreateWorkitemAsync(wi_type_name, wi_fields);

            // Assert
            Assert.NotNull(wi);
            Assert.Equal(wi_title, wi.Title);
        }

        [Fact(DisplayName = "Create child tasks for existing workitem")]
        public async Task CreateChildTasksTestSuccess()
        {
            // Arrange
            var today = DateTime.Today;

            var parent_id = 2;

            var wi_type_name = "Task";
            var wi_title = $"[Task] Created by integration test - {today}";
            var wi_description = $"This task was created by GitHub action at {today}";

            var wi_fields = new Dictionary<string, string>()
            {
                { "System.Title", wi_title },
                { "System.Description", wi_description },
            };

            // Act
            var parent = await _workitemClient.GetSingleWorkitemAsync(parent_id);
            var relations = new IWorkitemRelation[]
            {
                WorkitemRelationFactory.Create(RelationTypeNames.TypeName[WorkitemRelationType.Parent], parent!),
            };


            var wi = await _workitemClient.CreateWorkitemAsync(wi_type_name, wi_fields, relations);

            // Assert
            Assert.NotNull(wi);
            Assert.Equal(wi_title, wi.Title);

            Assert.Equal(relations.Length, wi.Relations.Count);

            Assert.Equal(relations[0].WorkitemId, wi.Relations[0].WorkitemId);
        }

        [Fact(DisplayName = "Update workitem fields")]
        public async Task UpdateWorkitemFieldsTestSuccess()
        {
            // Arrange
            var today = DateTime.Today;
            var workitem_id = 2;

            var title = $"[BRQ] Modified test requirement {today}";
            var description = $"This BRQ was modified by GitHub action. Last update: {today}";
            var history = $"GitHub action was starter {today}";

            // Act
            var wi = await _workitemClient.GetSingleWorkitemAsync(workitem_id);

            wi.Title = title;
            wi["System.Description"] = description;
            wi["System.History"] = history;

            var updateResult = await wi.SaveFieldsChangesAsync();

            var wiCopmare = await _workitemClient.GetSingleWorkitemAsync(workitem_id);

            // Assert
            Assert.Equal(UpdateFieldsResult.UPDATE_SUCCESS, updateResult);
            Assert.Equal(title, wi.Title);

            Assert.NotNull(wiCopmare);
            Assert.Equal(wiCopmare.Id, wi.Id);
            Assert.Equal(wiCopmare.Title, wi.Title);
            Assert.Equal(wi["System.Description"], wiCopmare.Description);
        }

        [Fact(DisplayName = "Get workitem changes success test")]
        public async Task GetWorkitemChangesTestSuccess()
        {
            // Arrange
            var workitem_id = 6; // [BRQ] Python requirement edited

            // Act
            var changes = await _workitemClient.GetWorkitemChangesAsync(workitem_id);

            // Assert
            Assert.NotNull(changes);
            Assert.True(changes.Any());

            foreach(var change in changes)
            {
                Assert.NotNull(change);
                Assert.True(change.Id > 0);
                Assert.True(change.WorkitemId == workitem_id);
                Assert.True(change.Revision > 0);

                if (change.FieldsChanges.Any())
                {
                    foreach(var fldChange in  change.FieldsChanges)
                    {
                        Assert.NotNull(fldChange);
                        Assert.False(fldChange.FieldName == string.Empty);
                        Assert.False(fldChange.NewValue == string.Empty);
                    }
                }

                if (change.RelationsChanges.HasChanges)
                {
                    var relChanges = change.RelationsChanges;
                    
                    if (relChanges.Added.Any())
                    {
                        foreach(var rel in relChanges.Added)
                        {
                            Assert.False(rel.TypeName == string.Empty);
                            Assert.True(rel.WorkitemId > 0);
                        }
                    }

                    if (relChanges.Removed.Any())
                    {
                        foreach (var rel in relChanges.Removed)
                        {
                            Assert.False(rel.TypeName == string.Empty);
                            Assert.True(rel.WorkitemId > 0);
                        }
                    }

                    if (relChanges.Updated.Any())
                    {
                        foreach (var rel in relChanges.Updated)
                        {
                            Assert.False(rel.TypeName == string.Empty);
                            Assert.True(rel.WorkitemId > 0);
                        }
                    }
                }
            }
        }

        [Fact(DisplayName = "Get workitem changes success test")]
        public async Task GetWorkitemChangesTestSuccess2()
        {
            // Arrange
            var workitem_id = 6; // [BRQ] Python requirement edited

            // Act
            var wi = await _workitemClient.GetSingleWorkitemAsync(workitem_id);
            var changes = await wi.GetWorkitemChangesAsync();

            // Assert
            Assert.NotNull(changes);
            Assert.True(changes.Any());

            foreach (var change in changes)
            {
                Assert.NotNull(change);
                Assert.True(change.Id > 0);
                Assert.True(change.WorkitemId == workitem_id);
                Assert.True(change.Revision > 0);

                if (change.FieldsChanges.Any())
                {
                    foreach (var fldChange in change.FieldsChanges)
                    {
                        Assert.NotNull(fldChange);
                        Assert.False(fldChange.FieldName == string.Empty);
                        Assert.False(fldChange.NewValue == string.Empty);
                    }
                }

                if (change.RelationsChanges.HasChanges)
                {
                    var relChanges = change.RelationsChanges;

                    if (relChanges.Added.Any())
                    {
                        foreach (var rel in relChanges.Added)
                        {
                            Assert.False(rel.TypeName == string.Empty);
                            Assert.True(rel.WorkitemId > 0);
                        }
                    }

                    if (relChanges.Removed.Any())
                    {
                        foreach (var rel in relChanges.Removed)
                        {
                            Assert.False(rel.TypeName == string.Empty);
                            Assert.True(rel.WorkitemId > 0);
                        }
                    }

                    if (relChanges.Updated.Any())
                    {
                        foreach (var rel in relChanges.Updated)
                        {
                            Assert.False(rel.TypeName == string.Empty);
                            Assert.True(rel.WorkitemId > 0);
                        }
                    }
                }
            }
        }
    }
}
