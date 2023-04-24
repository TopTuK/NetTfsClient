using NetTfsClient.Models.Project;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// Internal Workitem changes factory
    /// </summary>
    internal static class WorkitemChangesFactory
    {
        private class FieldChange : IFieldChange
        {
            public string FieldName { get; init; }
            public string NewValue { get; init; }
            public string? OldValue { get; init; }

            public FieldChange(string fieldname, JToken jsonItem)
            {
                FieldName = fieldname;

                // Get new value
                if (jsonItem["newValue"] != null)
                {
                    if (jsonItem["newValue"]!["displayName"] != null)
                    {
                        var member = TeamItemsFactory.MemberFromJsonItem(jsonItem["newValue"]!["displayName"]!);
                        NewValue = member.DisplayName;
                    }
                    else
                    {
                        NewValue = jsonItem["newValue"]!.Value<string>() ?? string.Empty;
                    }
                }
                else
                {
                    NewValue = string.Empty;
                }

                // Get old value
                if (jsonItem["oldValue"] != null)
                {
                    if (jsonItem["oldValue"]!["displayName"] != null)
                    {
                        var member = TeamItemsFactory.MemberFromJsonItem(jsonItem["oldValue"]!["displayName"]!);
                        NewValue = member.DisplayName;
                    }
                    else
                    {
                        NewValue = jsonItem["oldValue"]!.Value<string>() ?? string.Empty;
                    }
                }
                else
                {
                    OldValue = null;
                }
            }
        }

        private class RelationChanges : IRelationChanges
        {
            public bool HasChanges { get; init; }
            public IReadOnlyList<IWorkitemRelation> Added { get; init; }
            public IReadOnlyList<IWorkitemRelation> Removed { get; init; }
            public IReadOnlyList<IWorkitemRelation> Updated { get; init; }

            public RelationChanges()
            {
                HasChanges = false;

                Added = new List<IWorkitemRelation>();
                Removed = new List<IWorkitemRelation>();
                Updated = new List<IWorkitemRelation>();
            }

            public RelationChanges(JToken jsonItem)
            {
                HasChanges = true;

                Removed = (jsonItem["removed"] != null)
                    ? jsonItem["removed"]!
                        .Select(jsonRelation => WorkitemRelationFactory.WorkitemRelationFromJson(jsonRelation))
                        .ToList()
                    : new List<IWorkitemRelation>();

                Updated = (jsonItem["updated"] != null)
                    ? jsonItem["updated"]!
                        .Select(jsonRelation => WorkitemRelationFactory.WorkitemRelationFromJson(jsonRelation))
                        .ToList()
                    : new List<IWorkitemRelation>();

                Added = (jsonItem["added"] != null)
                    ? jsonItem["added"]!
                        .Select(jsonRelation => WorkitemRelationFactory.WorkitemRelationFromJson(jsonRelation))
                        .ToList()
                    : new List<IWorkitemRelation>();
            }
        }

        private class WorkitemChange : IWorkitemChange
        {
            public int Id { get; init; }
            public int WorkitemId { get; init; }
            public int Revision { get; init; }
            public string Url { get; init; }

            public IMember RevisedBy { get; init; }
            public DateTime RevisedDate { get; init; }


            public IReadOnlyList<IFieldChange> FieldsChanges { get; init; }
            public IRelationChanges RelationsChanges { get; init; }

            public WorkitemChange(int id, int workitemId, int revision, string url,
                IMember revisedBy, DateTime revisedDate,
                IReadOnlyList<IFieldChange> fieldChanges, IRelationChanges relationChanges)
            {
                Id = id;
                WorkitemId = workitemId;
                Revision = revision;
                Url = url;

                RevisedBy = revisedBy;
                RevisedDate = revisedDate;

                FieldsChanges = fieldChanges;
                RelationsChanges = relationChanges;
            }
        }

        private static IWorkitemChange WorkitemChangeFromJson(JToken jsonItem)
        {
            var id = jsonItem["id"]?.Value<int>() ?? -1;
            var workitemId = jsonItem["workItemId"]?.Value<int>() ?? -1;
            var revision = jsonItem["rev"]?.Value<int>() ?? -1;
            var url = jsonItem["url"]?.Value<string>() ?? string.Empty;

            var revisedBy = TeamItemsFactory.MemberFromJsonItem(jsonItem["revisedBy"]!);
            var revisedDate = jsonItem["revisedDate"]?.Value<DateTime>() ?? DateTime.MinValue;

            IReadOnlyList<IFieldChange> fieldsChanges;
            if (jsonItem["fields"] != null)
            {
                // change contains information about changes in fields
                // fields is array
                fieldsChanges = jsonItem["fields"]!
                    .Children<JObject>()
                    .Select(jsonFieldChange => 
                    {
                        var obj = jsonFieldChange.Properties()
                            .Select(p => (p.Name, p.Value))
                            .First();
                        return new FieldChange(obj.Name, obj.Value);
                    })
                    .ToList();
            }
            else
            {
                fieldsChanges = new List<IFieldChange>();
            }

            var relationChanges = (jsonItem["relations"] != null)
                ? new RelationChanges(jsonItem["relations"]!)
                : new RelationChanges();

            return new WorkitemChange(id, workitemId, revision, url,
                revisedBy, revisedDate,
                fieldsChanges, relationChanges);
        }

        public static IEnumerable<IWorkitemChange> WorkitemChangeFromJsonContent(string jsonContent)
        {
            var jsonItems = JObject.Parse(jsonContent);
            if (jsonItems["value"] != null)
            {
                return jsonItems["value"]!
                    .Select(jsonChange => WorkitemChangeFromJson(jsonChange))
                    .ToList();
            }

            return Enumerable.Empty<IWorkitemChange>();
        }
    }
}
