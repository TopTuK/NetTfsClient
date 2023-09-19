using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// public workitem relation factory
    /// </summary>
    public static class WorkitemRelationFactory
    {
        private class WorkitemRelation : IWorkitemRelation
        {
            private static readonly string WORKITEM_SUBSTR = "workItems/";
            private static readonly int WORKITEM_SUBSTR_LENGTH = WORKITEM_SUBSTR.Length;

            public string? Url { get; init; }

            public string? TypeName { get; init; }
            public WorkitemRelationType RelationType { get; init; }

            public int WorkitemId { get; init; }

            public WorkitemRelation(JToken jsonRelation)
            {
                Url = jsonRelation["url"]?.Value<string>();
                TypeName = jsonRelation["rel"]?.Value<string>();

                // 
                if ((TypeName != null) && (RelationTypeNames
                    .NameType.TryGetValue(TypeName, out var nameType)))
                {
                    RelationType = nameType;
                }
                else
                {
                    RelationType = WorkitemRelationType.Unknown;
                }

                int workitemId = -1;
                if (Url != null)
                {
                    var idStartIdx = Url.IndexOf(WORKITEM_SUBSTR);
                    if ((idStartIdx > 1)
                        && (int.TryParse(Url.Substring(idStartIdx + WORKITEM_SUBSTR_LENGTH), out var id)))
                    {
                        workitemId = id;
                    }
                }

                WorkitemId = workitemId;
            }

            public WorkitemRelation(string relationTypeName, int workitemId, string url)
            {
                TypeName = relationTypeName;
                if (RelationTypeNames.NameType.TryGetValue(relationTypeName, out var relationType))
                {
                    RelationType = relationType;
                }
                else
                {
                    RelationType = WorkitemRelationType.Unknown;
                }

                Url = url;
                WorkitemId = workitemId;
            }
        }

        internal static IWorkitemRelation WorkitemRelationFromJson(JToken jsonItem)
        {
            return new WorkitemRelation(jsonItem);
        }

        public static IWorkitemRelation Create(string relationName, IWorkitem workitem)
        {
            return new WorkitemRelation(relationName, workitem.Id, workitem.ItemUrl);
        }
    }
}
