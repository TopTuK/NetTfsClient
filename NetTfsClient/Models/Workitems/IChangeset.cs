using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// information about changeset
    /// </summary>
    public interface IChangeset
    {
        /// <summary>
        /// Changeset ID
        /// </summary>
        int ChangesetId { get; }

        /// <summary>
        /// Related workitem IDs
        /// </summary>
        IEnumerable<int> ItemIds { get; }

        /// <summary>
        /// Flag is empty related items IDs
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Returns count of related items
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Async gets associated workitems
        /// </summary>
        /// <returns>Associated workitems <see cref="IWorkitem"/></returns>
        Task<IEnumerable<IWorkitem>> GetAssociatedWorkitemsAsync();
    }
}
