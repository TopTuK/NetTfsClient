// See https://aka.ms/new-console-template for more information
using NetTfsClient.Services;

Console.WriteLine("Hello to TFS Client test app!");

var userName = @"";
var userPassword = @""; 

var serverUrl = @"";
var projetName = @"";

var clientConnection = ClientFactory.CreateClientConnection(userName, userPassword, serverUrl, projetName);
var workitemClient = clientConnection.GetWorkitemClient();