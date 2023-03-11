using NetTfsClient.Services.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsClient.Tests.HttpClientTests
{
    public class HttpClientAuthTests
    {
        private const string _baseUrl = @"https://httpbin.org/";

        [Fact(DisplayName = "AUTH Returns successful auth")]
        public async Task AuthSuccess()
        {
            // Arrange
            var userName = "user";
            var userPwd = "pwd";
            var url = $"basic-auth/{userName}/{userPwd}";

            var client = HttpClientFactory.CreateHttpClient(_baseUrl, userName, userPwd);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.NotNull(response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.True(response.IsSuccess);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.False(response.HasError);
            Assert.NotNull(response.Content);
        }

        [Fact(DisplayName = "Auth bad Returns bad authorization")]
        public async Task AuthBad()
        {
            // Arrange
            var userName = "user";
            var userPwd = "pwd";
            var badUserPwd = "badpwd";
            var url = $"/basic-auth/{userName}/{userPwd}";

            var client = HttpClientFactory.CreateHttpClient(_baseUrl, userName, badUserPwd);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.NotNull(response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.False(response.IsSuccess);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.True(response.HasError);
            Assert.NotNull(response.Content);
        }

        [Fact(DisplayName = "COOKIE set get Returns success if set and get cookies")]
        public async Task CookieSetGet()
        {
            // Arrange
            const string cookieName = "myCookie";
            const string cookieValue = "myCookieValue";

            var setUrl = $"cookies/set/{cookieName}/{cookieValue}";
            var getUrl = $"cookies";

            var client = HttpClientFactory.CreateHttpClient(_baseUrl);

            // Act
            await client.GetAsync(setUrl);
            var response = await client.GetAsync(getUrl);

            // Assert
            Assert.NotNull(response);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.True(response.IsSuccess);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.False(response.IsEmptyCookies);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.True(response.Cookies.ContainsKey(cookieName));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.Equal(cookieValue, response.Cookies[cookieName]);
        }
    }
}
