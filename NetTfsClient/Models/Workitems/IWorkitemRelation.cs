using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20item%20relation%20types/list?view=azure-devops-rest-6.0
    /// </summary>
    public enum WorkitemRelationType
    {
        Unknown = 0,
        Parent, // System.LinkTypes.Hierarchy-Reverse
        Child, // System.LinkTypes.Hierarchy-Forward
        Affects, // Microsoft.VSTS.Common.Affects-Forward
        AffectedBy, // Microsoft.VSTS.Common.Affects-Reverse
        Related // System.LinkTypes.Related
    }

    /// <summary>
    /// Helper static class for relation type names
    /// </summary>
    public static class RelationTypeNames
    {
        public static IReadOnlyDictionary<WorkitemRelationType, string> TypeName = new Dictionary<WorkitemRelationType, string>()
        {
            { WorkitemRelationType.Parent, "System.LinkTypes.Hierarchy-Reverse" },
            { WorkitemRelationType.Child, "System.LinkTypes.Hierarchy-Forward" },
            { WorkitemRelationType.Affects, "Microsoft.VSTS.Common.Affects-Forward" },
            { WorkitemRelationType.AffectedBy, "Microsoft.VSTS.Common.Affects-Reverse" },
            { WorkitemRelationType.Related, "System.LinkTypes.Related" },
        };

        public static IReadOnlyDictionary<string, WorkitemRelationType> NameType => TypeName
            .ToDictionary(x => x.Value, x => x.Key);
    }

    /// <summary>
    /// Information about workitem relation
    /// </summary>
    public interface IWorkitemRelation
    {
        /// <summary>
        /// Relation URL
        /// </summary>
        string? Url { get; }

        /// <summary>
        /// Relation type name
        /// </summary>
        string? TypeName { get; }
        /// <summary>
        /// Known type of relation type
        /// </summary>
        WorkitemRelationType RelationType { get; }

        /// <summary>
        /// Destination workitem ID
        /// </summary>
        int WorkitemId { get; }
    }
}
