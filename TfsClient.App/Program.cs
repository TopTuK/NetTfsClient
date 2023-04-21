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

// Create client connection
var connection = ClientFactory.CreateClientConnection(serverUrl, projetName, userPat);

/* Manage Project client */
var projectClient = connection.GetProjectClient();

// Get projects
var projects = await projectClient.GetProjectsAsync();

Console.WriteLine("Projects:");
foreach (var project in projects)
{
    Console.WriteLine($"Project: {project.Id} {project.Name}");
}
Console.WriteLine();

// Get teams
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
    var projectTeams = await projectClient.GetProjectTeamsAsync(project);

    if (projectTeams.Any())
    {
        Console.WriteLine($"Teams of project {project.Name}");
        foreach (var projectTeam in projectTeams)
        {
            Console.WriteLine($"Team: {projectTeam.Id} {projectTeam.Name}");
        }
        Console.WriteLine();

        // Get team members
        var team = projectTeams.First();
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

Console.WriteLine();