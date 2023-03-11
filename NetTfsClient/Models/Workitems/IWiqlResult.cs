using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    public interface IWiqlResult
    {
        IEnumerable<int> ItemIds { get; }

        bool IsEmpty { get; }
        int Count { get; }

        Task<IEnumerable<IWorkitem>> GetWorkitemsAsync();
    }
}
