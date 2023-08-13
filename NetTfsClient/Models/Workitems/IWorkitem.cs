using NetTfsClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// Result of updating workitem fields
    /// </summary>
    public enum UpdateFieldsResult : byte
    {
        UPDATE_FAIL = 0,
        UPDATE_SUCCESS,
        NOTHING_TO_UPDATE,
    }

    /// <summary>
    /// Result of updating workitem relations
    /// </summary>
    public enum UpdateRelationsResult : byte
    {
        UPDATE_FAIL = 0,
        UPDATE_SUCCESS
    }

    /// <summary>
    /// Contains information workitem of Azure/TFS service
    /// </summary>
    public interface IWorkitem
    {
        /// <summary>
        /// Workitem ID
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Workitem revision
        /// </summary>
        int Rev { get; }

        /// <summary>
        /// Workitem URL
        /// </summary>
        string ItemUrl { get; }

        /// <summary>
        /// Workitem type name
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Workitem title ("System.Title")
        /// </summary>
        string? Title { get; set; }
        /// <summary>
        /// Workitem description ("System.Description")
        /// </summary>
        string? Description { get; set; }

        /// <summary>
        /// Workitem state ("System.State")
        /// </summary>
        string? State { get; }
        /// <summary>
        /// Workitem reason ("System.Reason")
        /// </summary>
        string? Reason { get; }
        /// <summary>
        /// Workitem AssignedTo. Returns DisplayName
        /// </summary>
        string? AssignTo { get; }

        /// <summary>
        /// Return workitems names of fields
        /// </summary>
        IReadOnlyCollection<string> FieldsNames { get; }
        /// <summary>
        /// Returns given field value. Eg. ["System.Title"]
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Field value</returns>
        string? this[string fieldName] { get; set; }

        /// <summary>
        /// Flag indicates, that properties of workitem are modified
        /// </summary>
        bool IsFieldsModified { get; }

        /// <summary>
        /// Saves workitems properties
        /// </summary>
        /// <returns>Result <see cref="UpdateFieldsResult"/> of updating fields of workitem</returns>
        Task<UpdateFieldsResult> SaveFieldsChangesAsync();

        /// <summary>
        /// Returns relations (<see cref="IWorkitemRelation"/>) of workitem
        /// </summary>
        IReadOnlyList<IWorkitemRelation> Relations { get; }
        /// <summary>
        /// Gets relations of given relation type name
        /// </summary>
        /// <param name="relationTypeName">Relation type name</param>
        /// <returns>Enumerable of <see cref="IWorkitemRelation"/></returns>
        IEnumerable<IWorkitemRelation> GetRelations(string relationTypeName);

        /// <summary>
        /// Async adds relation link to workitem
        /// </summary>
        /// <param name="destinationWorkitemId">Destination workitem ID</param>
        /// <param name="relationTypeName">Relation type name</param>
        /// <param name="relationAttributes">Relation attributes</param>
        /// <returns>Result (<see cref="UpdateRelationsResult"/>) of updating relations</returns>
        Task<UpdateRelationsResult> AddRelationLinkAsync(int destinationWorkitemId, string relationTypeName,
            IReadOnlyDictionary<string, string>? relationAttributes = null);
        /// <summary>
        /// Async removes first relation link of wokitem
        /// </summary>
        /// <param name="destinationWorkitemId">Destination workitem ID</param>
        /// <returns>Result (<see cref="UpdateRelationsResult"/>) of updating relations</returns>
        Task<UpdateRelationsResult> RemoveRelationLinksAsync(int destinationWorkitemId);

        /// <summary>
        /// Get history changes of workitem.
        /// </summary>
        /// <returns>workitem changes history: <see cref="IWorkitemChange"/></returns>
        Task<IEnumerable<IWorkitemChange>> GetWorkitemChangesAsync();

        /// <summary>
        /// Return assosiated <see cref="IWorkitemClient"/> of workitem
        /// </summary>
        IWorkitemClient Client { get; }
    }
}
