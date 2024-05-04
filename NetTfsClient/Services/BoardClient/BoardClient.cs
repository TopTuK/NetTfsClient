using NetTfsClient.Models;
using NetTfsClient.Models.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services.BoardClient
{
    internal class BoardClient : BaseClient, IBoardClient
    {
        private const string BOARDS_URL = @"boards";

        public BoardClient(IClientConnection connection)
            : base(connection)
        { 
        }

        // https://learn.microsoft.com/en-us/rest/api/azure/devops/work/boards/get?view=azure-devops-rest-6.0
        public async Task<IEnumerable<IBaseBoard>> GetBoardsAsync(string projectId, string teamId)
        {
            var requestUrl = $"{clientConnection.CollectionName}/{projectId}/{teamId}/_apis/work/{BOARDS_URL}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION },
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IBoardClient::GetBoardsAsync: HTTP response is empty or null");
                }

                return BoardItemsFactory.BoardsFromJsonContent(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IBoardClient::GetBoardsAsync: exception raised", ex);
            }
        }

        public async Task<IBoard?> GetBoardAsync(string projectId, string teamId, string boardId)
        {
            if (boardId == string.Empty)
            {
                throw new ArgumentException("Board Id can\'t be empty string", nameof(boardId));
            }

            var requestUrl = $"{clientConnection.CollectionName}/{projectId}/{teamId}/_apis/work/{BOARDS_URL}/{boardId}";

            var queryParams = new Dictionary<string, string>
            {
                { "api-version", API_VERSION },
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IBoardClient::GetBoardAsync: HTTP response is empty or null");
                }

                return BoardItemsFactory.BoardFromJsonContent(httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IBoardClient::GetBoardAsync: exception raised", ex);
            }
        }
    }
}
