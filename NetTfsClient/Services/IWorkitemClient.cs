using NetTfsClient.Models.Workitems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWorkitemClient
    {
        /// <summary>
        /// Get single workitem
        /// </summary>
        /// <param name="id">ID of workitem</param>
        /// <param name="fields">List of requested fields. Null - all fields</param>
        /// <param name="expand">The expand parameters for work item attributes. Possible options are { None, Relations, Fields, Links, All }</param>
        /// <returns>Return single wokitem with properties</returns>
        /// <exception cref="TfsClientException"></exception>
        Task<IWorkitem?> GetSingleWorkitemAsync(int id, 
            IEnumerable<string>? fields = null, string expand = "All");

        /// <summary>
        /// Get workitems with properties
        /// </summary>
        /// <param name="ids">List of id of workitems</param>
        /// <param name="fields">List of requested fields for each workitem</param>
        /// <param name="expand">The expand parameters for work item attributes. Possible options are { None, Relations, Fields, Links, All }</param>
        /// <param name="batchSize">Number of requested items per iteration</param>
        /// <returns>Enumerable list of workitems</returns>
        /// <exception cref="TfsClientException"></exception>
        Task<IEnumerable<IWorkitem>> GetWorkitemsAsync(IEnumerable<int> ids,
            IEnumerable<string>? fields = null, string expand = "All", int batchSize = 50);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="skip"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        Task<IEnumerable<IWorkitemChange>> GetWorkitemChangesAsync(int id, int skip = 0, int top = -1);

        /// <summary>
        /// Create new workitem
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemFields"></param>
        /// <param name="itemRelations"></param>
        /// <param name="expand"></param>
        /// <param name="bypassRules"></param>
        /// <param name="suppressNotifications"></param>
        /// <param name="validateOnly"></param>
        /// <returns></returns>
        Task<IWorkitem> CreateWorkitemAsync(string itemType,
            IReadOnlyDictionary<string, string>? itemFields = null, 
            IEnumerable<IWorkitemRelation>? itemRelations = null,
            string? projectName = null,
            string expand = "All", bool bypassRules = false,
            bool suppressNotifications = false, bool validateOnly = false);

        /// <summary>
        /// Create copy of given workitem
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="destinationItemFields"></param>
        /// <returns></returns>
        Task<IWorkitem> CopyWorkitemAsync(IWorkitem sourceItem,
            IReadOnlyDictionary<string, string>? destinationItemFields = null);

        /// <summary>
        /// Update workitem fields
        /// </summary>
        /// <param name="workitemId"></param>
        /// <param name="itemFields"></param>
        /// <param name="expand"></param>
        /// <param name="bypassRules"></param>
        /// <param name="suppressNotifications"></param>
        /// <param name="validateOnly"></param>
        /// <returns></returns>
        Task<IWorkitem> UpdateWorkitemFieldsAsync(int workitemId, IReadOnlyDictionary<string, string> itemFields,
            string expand = "All", bool bypassRules = false,
            bool suppressNotifications = false, bool validateOnly = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceWorkitemId"></param>
        /// <param name="destinationWorkitemId"></param>
        /// <param name="relationType"></param>
        /// <param name="relationAttributes"></param>
        /// <param name="expand"></param>
        /// <param name="bypassRules"></param>
        /// <param name="suppressNotifications"></param>
        /// <param name="validateOnly"></param>
        /// <returns></returns>
        Task<IWorkitem> AddRelationLinkAsync(int sourceWorkitemId, int destinationWorkitemId,
            string relationType, IReadOnlyDictionary<string, string>? relationAttributes = null,
            string expand = "All", bool bypassRules = false,
            bool suppressNotifications = false, bool validateOnly = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workitemId"></param>
        /// <param name="relationId"></param>
        /// <param name="expand"></param>
        /// <param name="bypassRules"></param>
        /// <param name="suppressNotifications"></param>
        /// <param name="validateOnly"></param>
        /// <returns></returns>
        Task<IWorkitem> RemoveRelationLinkAsync(
            int workitemId, int relationId,
            string expand = "All", bool bypassRules = false,
            bool suppressNotifications = false, bool validateOnly = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryId"></param>
        /// <returns></returns>
        Task<IWiqlResult> RunSavedQueryAsync(string queryId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="maxTop"></param>
        /// <returns></returns>
        Task<IWiqlResult> RunWiqlAsync(string query, int maxTop = -1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changesetId"></param>
        /// <returns></returns>
        Task<IChangeset> GetChangesetAsync(int changesetId);
    }
}
