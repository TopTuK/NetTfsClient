using NetTfsClient.Services.HttpClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsClient.Tests.HttpClientTests
{
    /// <summary>
    /// https://stackoverflow.com/questions/5725430/http-test-server-accepting-get-post-requests
    /// </summary>
    public class HttpClientTests
    {
        private const string _baseUrl = @"https://httpbin.org/";
        private const string _getUrl = "get";
        private const string _postUrl = "post";
        private const string _patchUrl = "patch";

        private readonly IHttpClient _httpService = HttpClientFactory.CreateHttpClient(_baseUrl);

        [Fact(DisplayName = "GET async Returns success response")]
        public async Task GetAsyncSuccess()
        {
            // Arrange

            // Act
            var response = await _httpService.GetAsync(_getUrl);

            // Assert
            Assert.NotNull(response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.True(response.IsSuccess);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.False(response.HasError);
            Assert.NotNull(response.Content);
        }

        [Fact(DisplayName = "GET Returns json response with args")]
        public async Task GetArgsSuccess()
        {
            // Arrange
            var args = new Dictionary<string, string>()
            {
                { "id", "1" },
                { "my_param", "my_value" }
            };

            // Act
            var response = await _httpService.GetAsync(_getUrl, args);

            // Assert
            Assert.NotNull(response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.True(response.IsSuccess);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.False(response.HasError);
            Assert.NotNull(response.Content);

            var jsonResponse = JObject.Parse(response.Content);
            Assert.NotNull(jsonResponse);
            Assert.NotNull(jsonResponse["args"]);

            Assert.Equal(1, jsonResponse["args"]["id"].Value<int>());
            Assert.Equal("my_value", jsonResponse["args"]["my_param"].Value<string>());
        }

        [Fact(DisplayName = "POST async Returns success response")]
        public async Task PostAsyncSuccess()
        {
            // Arrange

            // Act
            var response = await _httpService.PostAsync(_postUrl);

            // Assert
            Assert.NotNull(response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.True(response.IsSuccess);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.False(response.HasError);
            Assert.NotNull(response.Content);
        }

        [Fact(DisplayName = "POST Returns json response with args")]
        public async Task PostArgsSuccess()
        {
            // Arrange
            var args = new Dictionary<string, string>()
            {
                { "id", "1" },
                { "my_param", "my_value" }
            };

            // Act
            var response = await _httpService.PostAsync(_postUrl, args);

            // Assert
            Assert.NotNull(response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.True(response.IsSuccess);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.False(response.HasError);
            Assert.NotNull(response.Content);

            var jsonResponse = JObject.Parse(response.Content);
            Assert.NotNull(jsonResponse);
            Assert.NotNull(jsonResponse["form"]);

            Assert.Equal(1, jsonResponse["form"]["id"].Value<int>());
            Assert.Equal("my_value", jsonResponse["form"]["my_param"].Value<string>());
        }

        [Fact(DisplayName = "POST JSON async Returns json response with args")]
        public async Task PostJsonAsyncSuccess()
        {
            // Arrange
            var json = new List<object>()
            {
                new { A = "AA", B = "BB"},
                new { C = true, D = 1 }
            };

            // Act
            var response = await _httpService.PostJsonAsync(_postUrl, json);

            // Assert
            Assert.NotNull(response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.True(response.IsSuccess);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.False(response.HasError);
            Assert.NotNull(response.Content);

            var jsonResponse = JObject.Parse(response.Content);
            Assert.NotNull(jsonResponse);
            Assert.NotNull(jsonResponse["json"]);

            Assert.Equal(2, jsonResponse["json"].Count());
        }

        [Fact(DisplayName = "PATCH async Returns success response")]
        public async Task PatchAsyncSuccess()
        {
            // Arrange

            // Act
            var response = await _httpService.PatchAsync(_patchUrl);

            // Assert
            Assert.NotNull(response);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.True(response.IsSuccess);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.False(response.HasError);
            Assert.NotNull(response.Content);
        }
    }
}
