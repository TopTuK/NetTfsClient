using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// Contains results of query or WIQL request
    /// </summary>
    public interface IWiqlResult
    {
        /// <summary>
        /// Enumerable of IDs of items
        /// </summary>
        IEnumerable<int> ItemIds { get; }

        /// <summary>
        /// Flag: result is empty
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Count of result items
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Async gets associated workitems
        /// </summary>
        /// <returns>Associated workitems <see cref="IWorkitem"/></returns>
        Task<IEnumerable<IWorkitem>> GetWorkitemsAsync();
    }
}
