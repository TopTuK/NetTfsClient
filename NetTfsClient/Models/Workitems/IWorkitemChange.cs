using NetTfsClient.Models.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// Information of field change of workitem.
    /// </summary>
    public interface IFieldChange
    {
        /// <summary>
        /// Changed field name
        /// </summary>
        string FieldName { get; }

        /// <summary>
        /// New value of field
        /// </summary>
        string NewValue { get; }

        /// <summary>
        /// Old value of field. Can be null.
        /// </summary>
        string? OldValue { get; }
    }

    /// <summary>
    /// Information about relation changes
    /// </summary>
    public interface IRelationChanges
    {
        /// <summary>
        /// Flag indicating change has relation changes
        /// </summary>
        bool HasChanges { get; }

        /// <summary>
        /// List of added relations. Can be empty
        /// </summary>
        IReadOnlyList<IWorkitemRelation> Added { get; }

        /// <summary>
        /// List of removed relations. Can be empty
        /// </summary>
        IReadOnlyList<IWorkitemRelation> Removed { get; }

        /// <summary>
        /// List of updated relations. Can be empty
        /// </summary>
        IReadOnlyList<IWorkitemRelation> Updated { get; }
    }

    /// <summary>
    /// Information about workite change.
    /// </summary>
    public interface IWorkitemChange
    {
        /// <summary>
        /// Id of workitem change
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Id of workitem
        /// </summary>
        int WorkitemId { get; }

        /// <summary>
        /// Revision number
        /// </summary>
        int Revision { get; }

        /// <summary>
        /// Member who revised workitem
        /// </summary>
        IMember RevisedBy { get; }

        /// <summary>
        /// Date of revision
        /// </summary>
        DateTime RevisedDate { get; }

        /// <summary>
        /// URL of workitem change
        /// </summary>
        string Url { get; }

        /// <summary>
        /// List of changes of fields. Can be empty
        /// </summary>
        IReadOnlyList<IFieldChange> FieldsChanges { get; }

        /// <summary>
        /// Information of relations changes
        /// </summary>
        IRelationChanges RelationsChanges { get; }
    }
}
