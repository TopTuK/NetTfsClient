// See https://aka.ms/new-console-template for more information
using NetTfsClient.Services;

Console.WriteLine("Hello to TFS Client test app!");

var serverUrl = "";
var projetName = "";
var userPat = "";

var connection = ClientFactory.CreateClientConnection(serverUrl, projetName, userPat);