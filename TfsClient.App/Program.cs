// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using NetTfsClient.Models.Project;
using NetTfsClient.Models.Workitems;
using NetTfsClient.Services;

Console.WriteLine("Hello to TFS Client test app!");


var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Get configuration
var serverUrl = configuration["ENV_SERVER_URL"];
var projetName = configuration["ENV_PROJECT_NAME"];
var userPat = configuration["ENV_PAT"];

// Create client connection with ClientFactory
var connection = ClientFactory.CreateClientConnection(serverUrl, projetName, userPat);

/* Get IProjectClient with extension method GetProjectClient() on connection */
var projectClient = connection.GetProjectClient();

Console.WriteLine("### SECTION PROJECTS ###");

// Get projects with GetProjectsAsync()
var projects = await projectClient.GetProjectsAsync();

foreach (var project in projects)
{
    Console.WriteLine($"Project: {project.Id} {project.Name}");
}
Console.WriteLine();

// Get all teams with GetAllTeamsAsync()
var teams = await projectClient.GetAllTeamsAsync();

Console.WriteLine("TFS Teams:");
foreach (var team in teams)
{
    Console.WriteLine($"Team: {team.Id} {team.Name}");
}
Console.WriteLine();

// Project teams
if (projects.Any())
{
    var project = projects.First();

    // Get project teams with GetProjectTeamsAsync(project)
    var projectTeams = await projectClient.GetProjectTeamsAsync(project);

    if (projectTeams.Any())
    {
        Console.WriteLine($"Teams of project {project.Name}");
        foreach (var projectTeam in projectTeams)
        {
            Console.WriteLine($"Team: {projectTeam.Id} {projectTeam.Name}");
        }
        Console.WriteLine();

        var team = projectTeams.First();
        
        // Get team members with GetProjectTeamMembersAsync(project, team)
        var members = await projectClient.GetProjectTeamMembersAsync(project, team);

        if (members.Any())
        {
            Console.WriteLine($"Members of team {team.Name} of project {project.Name}");
            foreach(var member in members)
            {
                Console.WriteLine($"Member: {member.Id} {member.UniqueName} {member.DisplayName}");
            }

            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"You don't have any members of team {team.Name} of project {project.Name}");
        }
    }
    else
    {
        Console.WriteLine($"You don't have any teams for project {project.Name} :(");
        Console.WriteLine();
    }
}
else
{
    Console.WriteLine("You don't have any projects :(");
}

Console.WriteLine("### END SECTION PROJECTS ###");

Console.WriteLine();

Console.WriteLine("### SECTION WORKITEMS ###");

/* Get IWorkitemClient with extension method GetProjectClient() on connection */
var workitemClient = connection.GetWorkitemClient();

var workitemId = 1;
// Get single workitem and properties with GetSingleWorkitemAsync()
var workitem = await workitemClient.GetSingleWorkitemAsync(workitemId);
if (workitem != null)
{
    Console.WriteLine($"--- {workitem.Id} {workitem.Title} {workitem.TypeName} ---");
    Console.WriteLine($"WI propery (System.Title): {workitem["System.Title"]}");
    Console.WriteLine($"Description: {workitem.Description} Revision: {workitem.Rev}");
}
else
{
    Console.WriteLine($"Can't get workitem with ID={workitemId}");
}
Console.WriteLine();

if (workitem != null)
{
    Console.WriteLine("Get workitem changes with GetWorkitemChangesAsync() on IWorkitem");

    var changes = await workitem.GetWorkitemChangesAsync();
    if (changes != null)
    {
        Console.WriteLine($"Workitem {workitem.Id} {workitem.Title} has {changes.Count()} changes");
        foreach (var change in changes)
        {
            Console.WriteLine($"Change {change.Id}: Rev: {change.Revision} Date: {change.RevisedDate} by {change.RevisedBy.DisplayName}");
            if (change.FieldsChanges.Any())
            {
                Console.WriteLine("Fields changes:");
                foreach (var fldChange in change.FieldsChanges)
                {
                    Console.WriteLine($"Field {fldChange.FieldName} change: Old value: {fldChange.OldValue} New value: {fldChange.NewValue}");
                }
            }
            else
            {
                Console.WriteLine("Change doesn't have any fields changes");
            }

            if (change.RelationsChanges.HasChanges)
            {
                var addedCount = change.RelationsChanges.Added.Count;
                var updatedCount = change.RelationsChanges.Updated.Count;
                var removedCount = change.RelationsChanges.Removed.Count;

                Console.WriteLine($"Relation changes: Added={addedCount} Updated={updatedCount} Removed={removedCount}");
            }
            else
            {
                Console.WriteLine("Change doesn't have any relation changes");
            }
        }
    }
    else
    {
        Console.WriteLine($"Can't get history changes of workitem {workitem.Id} {workitem.Title}");
    }
}
Console.WriteLine();

Console.WriteLine("### END SECTION WORKITEMS ###");