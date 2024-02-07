// See https://aka.ms/new-console-template for more information
using NetTfsClient.Services;

Console.WriteLine("Hello, World!");

const string ServerUrl = "";
const string ProjectName = "";
const int WorkitemId = 0;

var connection = ClientFactory.CreateClientConnection(ServerUrl, ProjectName);
var client = connection.GetWorkitemClient();

try
{
    var wi = await client.GetSingleWorkitemAsync(WorkitemId);
    if (wi != null)
    {
        Console.WriteLine($"{wi.Id} {wi.Title}");
    }
    else
    {
        Console.WriteLine("WI is null");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Exception raised: {ex.Message} Inner: {ex.InnerException?.Message}");
}

Console.WriteLine("Meow!");