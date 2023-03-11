using NetTfsClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsClient.Tests.ClientConnectionTests
{
    public class ClientConnectionBasicTests
    {
        private const string ServerUrl = @"http://localhost/";
        private const string Collection = @"DefaultCollection";
        private const string Project = @"TestProject";
        private const string ProjectName = $"{Collection}/{Project}";

        private const string FakePat = @"FakePAT";

        [Fact(DisplayName = "Basic client connection test with given params")]
        public void CreateClientConnectionTest()
        {
            // Arrange
            var apiUrl = $"{Collection}/_apis/";
            var projectApiUrl = $"{Collection}/{Project}/_apis/";

            // Act
            var clientConnection = ClientFactory.CreateClientConnection(ServerUrl, ProjectName, FakePat);

            // Assert
            Assert.Equal(ServerUrl, clientConnection.ServerUrl);
            Assert.Equal(Collection, clientConnection.CollectionName);
            Assert.Equal(Project, clientConnection.ProjectName);

            // Check server url
            Assert.NotNull(clientConnection.HttpClient.BaseUrl);
            Assert.Equal(ServerUrl, clientConnection.HttpClient.BaseUrl!.ToString());

            // Check API urls
            Assert.Equal(apiUrl, clientConnection.ApiUrl);
            Assert.Equal(projectApiUrl, clientConnection.ProjectApiUrl);
        }
    }
}
