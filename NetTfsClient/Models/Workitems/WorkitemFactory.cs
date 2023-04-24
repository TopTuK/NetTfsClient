using NetTfsClient.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetTfsClient.Models.Workitems
{
    /// <summary>
    /// Internal workitem factory
    /// </summary>
    internal static class WorkitemFactory
    {
        private static readonly List<string> IGNORE_FIELDS = new()
        {
            "System.Id",
        };

        private class Workitem : IWorkitem
        {
            public string ItemUrl { get; init; }
            public int Id { get; init; }
            public int Rev { get; init; }

            public string TypeName { get; init; }

            public string? Title
            {
                get => GetStringField("System.Title");
                set
                {
                    this["System.Title"] = value;
                }
            }

            public string? Description
            {
                get => GetStringField("System.Description");
                set
                {
                    this["System.Description"] = value;
                }
            }

            public string? State => GetStringField("System.State");
            public string? Reason => GetStringField("System.Reason");

            public string? AssignTo
            {
                get
                {
                    if (_updatedFields.TryGetValue("System.AssignedTo", out var updatedFldValue))
                    {
                        return updatedFldValue;
                    }

                    if (_fields.TryGetValue("System.AssignedTo", out var fldValue))
                    {
                        if (fldValue == null) return null;

                        if (fldValue.Type == JTokenType.String) return fldValue.ToObject<string>();

                        return fldValue.Type == JTokenType.Object
                            ? fldValue["displayName"]?.ToObject<string>()
                            : fldValue.ToString();
                    }

                    return null;
                }
            }

            public IReadOnlyCollection<string> FieldsNames => _fields.Keys;
            public bool IsFieldsModified => _updatedFields.Count > 0;

            public IReadOnlyList<IWorkitemRelation> Relations { get; private set; }

            public string? this[string fieldName] 
            { 
                get
                {
                    if (_updatedFields.TryGetValue(fieldName, out var fieldValue))
                    {
                        return fieldValue;
                    }

                    if (_fields.TryGetValue(fieldName, out var jField))
                    {
                        if (jField == null) return null;

                        if (jField.Type == JTokenType.String)
                        {
                            return jField.ToObject<string>();
                        }
                        else
                        {
                            return jField.Type == JTokenType.Object
                                ? jField["displayName"]?.ToObject<string>()
                                : jField.ToString();
                        }
                    }

                    return null;
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("Can't add null field");
                    }

                    if (_updatedFields.ContainsKey(fieldName))
                    {
                        _updatedFields[fieldName] = value;
                    }
                    else
                    {
                        _updatedFields.Add(fieldName, value);
                    }
                }
            }
            
            private Dictionary<string, JToken?> _fields = new();
            private Dictionary<string, string> _updatedFields = new();

            public IWorkitemClient Client { get; init; }

            public Workitem(IWorkitemClient client, JToken jsonItem)
            {
                Client = client;

                string typeName = "Unknown";

                ItemUrl = jsonItem["url"]?.Value<string>() ?? "";
                Id = jsonItem["id"]?.Value<int>() ?? -1;
                Rev = jsonItem["rev"]?.Value<int>() ?? -1;

                var jsonFields = jsonItem["fields"]?.ToObject<JObject>();
                if ((jsonFields != null) && (jsonFields.HasValues))
                {
                    foreach (var jField in jsonFields)
                    {
                        var fldKey = jField.Key;
                        if (fldKey == "System.WorkItemType")
                        {
                            typeName = jField.Value?.Value<string>() ?? "Unknown";
                        }
                        else
                        {
                            if(!IGNORE_FIELDS.Contains(fldKey))
                            {
                                _fields.Add(fldKey, jField.Value);
                            }
                        }
                    }
                }

                var jsonRelations = jsonItem["relations"]?.ToObject<JArray>();
                if ((jsonRelations != null) && (jsonRelations.HasValues))
                {
                    Relations = jsonRelations
                        .Select(jsonRelation =>WorkitemRelationFactory.WorkitemRelationFromJson(jsonRelation))
                        .ToList();
                }
                else
                {
                    Relations = new List<IWorkitemRelation>();
                }

                TypeName = typeName;
            }

            private string? GetStringField(string fldName)
            {
                if (_updatedFields.TryGetValue(fldName, out var updValue))
                {
                    return updValue;
                }

                if (_fields.TryGetValue(fldName, out var fldValue))
                {
                    return fldValue?.Value<string>();
                }

                return null;
            }

            public IEnumerable<IWorkitemRelation> GetRelations(string relationTypeName)
            {
                return Relations
                    .Where(rel => rel.TypeName == relationTypeName)
                    .Select(rel => rel)
                    .ToList();
            }

            public async Task<UpdateRelationsResult> AddRelationLinkAsync(int destinationWorkitemId, string relationTypeName, IReadOnlyDictionary<string, string>? relationAttributes = null)
            {
                try
                {
                    var item = await Client.AddRelationLinkAsync(Id, destinationWorkitemId, relationTypeName, 
                        relationAttributes, expand: "relations");
                    Relations = item.Relations;

                    return UpdateRelationsResult.UPDATE_SUCCESS;
                }
                catch
                {
                    return UpdateRelationsResult.UPDATE_FAIL;
                }
            }

            public async Task<UpdateRelationsResult> RemoveRelationLinksAsync(int destinationWorkitemId)
            {
                var relIdx = (Relations as List<IWorkitemRelation>)
                    ?.FindIndex(rel => rel.WorkitemId == destinationWorkitemId) ?? -1;

                if (relIdx < 0)
                {
                    return UpdateRelationsResult.UPDATE_FAIL;
                }

                try
                {
                    var item = await Client.RemoveRelationLinkAsync(Id, relIdx, expand: "relations");
                    Relations = item.Relations;

                    return UpdateRelationsResult.UPDATE_SUCCESS;
                }
                catch
                {
                    return UpdateRelationsResult.UPDATE_FAIL;
                }
            }

            public async Task<UpdateFieldsResult> SaveFieldsChangesAsync()
            {
                if (!IsFieldsModified)
                {
                    return UpdateFieldsResult.NOTHING_TO_UPDATE;
                }

                try
                {
                    var item = await Client.UpdateWorkitemFieldsAsync(Id, _updatedFields, expand: "fields");
                    
                    if (item == null)
                    {
                        return UpdateFieldsResult.UPDATE_FAIL;
                    }

                    _updatedFields.Clear();
                    _fields = (item as Workitem)!._fields;

                    return UpdateFieldsResult.UPDATE_SUCCESS;
                }
                catch (Exception ex)
                {
                    throw new ClientException(
                        "IWorkitem::SaveFieldsChangesAsync: exception raised",
                        ex
                    );
                }
            }

            public async Task<IEnumerable<IWorkitemChange>> GetWorkitemChangesAsync()
            {
                return await Client.GetWorkitemChangesAsync(Id);
            }
        }

        public static IEnumerable<IWorkitem> FromJsonItems(IWorkitemClient client, string jsonItemsStr)
        {
            var jsonItems = JObject.Parse(jsonItemsStr);
            if (jsonItems["value"] == null)
            {
                throw new ClientException("Can't parse json items");
            }

            var items = jsonItems["value"]!.ToArray()
                .Select(jItem => new Workitem(client, jItem))
                .ToList();

            return items;
        }

        public static IWorkitem FromJson(IWorkitemClient client, string jsonItemStr)
        {
            var jsonItem = JObject.Parse(jsonItemStr);
            if (jsonItem == null)
            {
                throw new ClientException("Can't parse json item");
            }

            return new Workitem(client, jsonItem);
        }
    }
}
