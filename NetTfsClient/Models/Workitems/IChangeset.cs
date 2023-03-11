using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChangeset
    {
        /// <summary>
        /// 
        /// </summary>
        int ChangesetId { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<int> ItemIds { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IWorkitem>> GetAssociatedWorkitemsAsync();
    }
}
