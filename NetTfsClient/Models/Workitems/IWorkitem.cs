using NetTfsClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    public enum UpdateFieldsResult : byte
    {
        UPDATE_FAIL = 0,
        UPDATE_SUCCESS,
        NOTHING_TO_UPDATE,
    }

    public enum UpdateRelationsResult : byte
    {
        UPDATE_FAIL = 0,
        UPDATE_SUCCESS
    }

    public interface IWorkitem
    {
        int Id { get; }
        int Rev { get; }

        string ItemUrl { get; }
        string TypeName { get; }

        string? Title { get; set; }
        string? Description { get; set; }

        string? State { get; }
        string? Reason { get; }
        string? AssignTo { get; }

        IReadOnlyCollection<string> FieldsNames { get; }
        string? this[string fieldName] { get; set; }
        bool IsFieldsModified { get; }

        Task<UpdateFieldsResult> SaveFieldsChangesAsync();

        IReadOnlyList<IWorkitemRelation> Relations { get; }
        IEnumerable<IWorkitemRelation> GetRelations(string relationTypeName);

        Task<UpdateRelationsResult> AddRelationLinkAsync(int destinationWorkitemId, string relationTypeName,
            IReadOnlyDictionary<string, string>? relationAttributes = null);
        Task<UpdateRelationsResult> RemoveRelationLinksAsync(int destinationWorkitemId);

        /// <summary>
        /// Get history changes of workitem.
        /// </summary>
        /// <returns>workitem changes history: <see cref="IWorkitemChange"/></returns>
        Task<IEnumerable<IWorkitemChange>> GetWorkitemChangesAsync();

        IWorkitemClient Client { get; }
    }
}
