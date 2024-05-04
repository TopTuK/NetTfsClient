using NetTfsClient.Models.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services
{
    public interface IBoardClient
    {
        Task<IEnumerable<IBaseBoard>> GetBoardsAsync(string projectId, string teamId);
        Task<IBoard?> GetBoardAsync(string projectId, string teamId, string boardId);
    }
}
