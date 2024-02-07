using NetTfsClient.Helpers;
using NetTfsClient.Models;
using NetTfsClient.Models.Project;
using NetTfsClient.Models.Workitems;
using NetTfsClient.Services.HttpClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services.WorkitemClient
{
    internal class WorkitemClient : BaseClient, IWorkitemClient
    {
        private const string WORKITEM_URL = @"wit/workitems";
        private const string QUERY_URL = @"wit/queries";
        private const string WIQL_URL = @"wit/wiql";
        private const string CHANGESET_URL = @"tfvc/changesets";

        public WorkitemClient(IClientConnection clientConnection)
            : base(clientConnection)
        {
        }

        public async Task<IWorkitem?> GetSingleWorkitemAsync(int id,
            IEnumerable<string>? fields = null,
            string expand = "All")
        {
            int[] ids = new int[]
            {
                id,
            };

            var items = await GetWorkitemsAsync(ids, fields, expand);

            return items.FirstOrDefault();
        }

        private static Dictionary<string, string> GetWorkitemsPrepareArgs(IEnumerable<string>? fields, string expand)
        {
            var defaultRequestParams = new Dictionary<string, string>
            {
                { "$expand", $"{expand}" },
                { "api-version", API_VERSION }
            };

            if (fields != null)
            {
                var flds = string.Join(",", fields);
                defaultRequestParams.Add("fields", flds);
            }

            return defaultRequestParams;
        }

        private async Task<IEnumerable<IWorkitem>> RequestWorkitemsAsync(
            IReadOnlyDictionary<string, string> requestParams,
            bool underProject = false)
        {
            var action = underProject
                ? clientConnection.ProjectApiUrl + WORKITEM_URL
                : clientConnection.ApiUrl + WORKITEM_URL;

            var httpResponse = await httpClient.GetAsync(action, requestParams);
            if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
            {
                throw new ClientException("IWorkitemClient::Exception: HTTP response is null or has error");
            }

            return WorkitemFactory.FromJsonItems(this, httpResponse.Content);
        }

        public async Task<IEnumerable<IWorkitem>> GetWorkitemsAsync(IEnumerable<int> ids,
            IEnumerable<string>? fields = null,
            string expand = "All", int batchSize = 50)
        {
            var idsCount = ids.Count();
            if (idsCount == 0)
            {
                throw new ClientException(
                    "Can't get workitems. Ids list is empty",
                    new ArgumentException(nameof(ids))
                );
            }

            var defaultRequestParams = GetWorkitemsPrepareArgs(fields, expand);

            var resultItems = new List<IWorkitem>(ids.Count());
            foreach (var items in ids.Batch(batchSize))
            {
                var requestParams = new Dictionary<string, string>(defaultRequestParams)
                {
                    { "ids", string.Join(",", items) }
                };

                try
                {
                    var workitems = await RequestWorkitemsAsync(requestParams);
                    resultItems.AddRange(workitems);
                }
                catch (Exception ex)
                {
                    throw new ClientException("IWorkitemClient: GetWorkitemsAsync exception", ex);
                }
            }

            return resultItems;
        }

        public async Task<IEnumerable<IWorkitemChange>> GetWorkitemChangesAsync(int id,
            int skip = 0, int top = -1)
        {
            var requestUrl = $"{clientConnection.ApiUrl}{WORKITEM_URL}/{id}/updates";

            var queryParams = new Dictionary<string, string>()
            {
                { "api-version", API_VERSION },
                { "$skip", $"{skip}" },
            };

            if (top > 0)
            {
                queryParams.Add("$top", $"{top}");
            }

            try
            {
                List<IWorkitemChange> changes = new();
                bool hasNext = true;

                do
                {
                    hasNext = false;

                    var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);
                    if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                    {
                        throw new ClientException("IWorkitemClient::GetWorkitemChangeAsync: http response is null or empty");
                    }

                    var items = WorkitemChangesFactory.WorkitemChangeFromJsonContent(httpResponse.Content);
                    if (items.Any())
                    {
                        changes.AddRange(items);
                        queryParams["$skip"] = changes
                            .Count
                            .ToString();
                        hasNext = true;
                    }
                }
                while (hasNext);

                return changes;
            }
            catch (Exception ex)
            {
                throw new ClientException("IWorkitemClient::GetWorkitemChangeAsync: exception raised", ex);
            }
        }

        private static Dictionary<string, string> MakeQueryParams(string expand, bool bypassRules,
            bool suppressNotifications, bool validateOnly) => new()
            {
                { "api-version", API_VERSION },
                { "$expand", expand },
                { "bypassRules", $"{bypassRules}" },
                { "suppressNotifications", $"{suppressNotifications}" },
                { "validateOnly", $"{validateOnly}" }
            };

        public async Task<IWorkitem> CreateWorkitemAsync(string itemType,
            IReadOnlyDictionary<string, string>? itemFields = null,
            IEnumerable<IWorkitemRelation>? itemRelations = null,
            string expand = "All", bool bypassRules = false,
            bool suppressNotifications = false, bool validateOnly = false)
        {
            var requestUrl = $"{clientConnection.ProjectApiUrl}{WORKITEM_URL}/${itemType}";

            var queryParams = MakeQueryParams(expand, bypassRules, suppressNotifications, validateOnly);

            var requestBody = (itemFields != null)
                ? itemFields
                    .Select(fld => new
                    {
                        op = "add",
                        path = $"/fields/{fld.Key}",
                        @from = (string?)null,
                        value = fld.Value
                    })
                    .ToList<object>()
                : new List<object>();

            if (itemRelations != null)
            {
                requestBody.AddRange(itemRelations
                    .Select(relation => new
                    {
                        op = "add",
                        path = $"/relations/-",
                        @from = (string?)null,
                        value = new
                        {
                            rel = relation.TypeName,
                            url = relation.Url,
                            attributes = (string?)null
                        }
                    })
                    .ToList());
            }

            // Media Types: "application/json-patch+json"
            var customHeaders = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json-patch+json" }
            };

            try
            {
                var httpResponse = await httpClient.PostJsonAsync(
                    requestUrl, requestBody,
                    queryParams: queryParams,
                    customHeaders: customHeaders);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IWorkitemClient::CreateWorkitemAsync: http response is null or empty");
                }

                return WorkitemFactory.FromJson(this, httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException("IWorkitemClient::CreateWorkitemAsync: exception raised", ex);
            }
        }

        private IReadOnlyDictionary<string, string> CopyWorkitemPrepareArgs(IWorkitem sourceItem,
            IReadOnlyDictionary<string, string>? destinationItemFields)
        {
            IReadOnlyList<string> ignoreFields = new List<string>
            {
                "System.TeamProject",
                "System.AreaPath",
                "System.AreaId",
                "System.AreaLevel1",
                "System.AreaLevel2",
                "System.AreaLevel3",
                "System.AreaLevel4",
                "System.Id",
                "System.NodeName",
                "System.Rev",
                "System.AutorizedDate",
                "System.RevisedDate",
                "System.IterationId",
                "System.IterationLevel1",
                "System.IterationLevel2",
                "System.IterationLevel3",
                "System.IterationLevel4",
                "System.CreatedBy",
                "System.ChangedDate",
                "System.ChangedBy",
                "System.AuthorizedAs",
                "System.AuthorizedDate",
                "System.Watermark"
            };

            var sourceItemFields = sourceItem.FieldsNames;

            var fields = new Dictionary<string, string>(sourceItemFields.Count);
            foreach (var fldName in sourceItemFields)
            {
                if (ignoreFields.Contains(fldName)) continue;

                var fldValue = sourceItem[fldName];
                if ((destinationItemFields != null) && (destinationItemFields.ContainsKey(fldName)))
                {
                    fldValue = destinationItemFields[fldName];
                }

                if (fldValue != null)
                {
                    fields.Add(fldName, fldValue);
                }
            }

            return fields;
        }

        public async Task<IWorkitem> CopyWorkitemAsync(IWorkitem sourceItem,
            IReadOnlyDictionary<string, string>? destinationItemFields = null)
        {
            var itemTypeName = sourceItem.TypeName;
            var fields = CopyWorkitemPrepareArgs(sourceItem, destinationItemFields);

            return await CreateWorkitemAsync(itemTypeName, itemFields: fields);
        }

        // https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-6.0
        public async Task<IWorkitem> UpdateWorkitemFieldsAsync(int workitemId,
            IReadOnlyDictionary<string, string> itemFields,
            string expand = "All", bool bypassRules = false,
            bool suppressNotifications = false, bool validateOnly = false)
        {
            if (itemFields.Count == 0)
            {
                throw new ClientException(
                    "Updated item fields count is 0",
                    new ArgumentNullException(nameof(itemFields))
                );
            }

            var requestUrl = $"{clientConnection.ProjectApiUrl}{WORKITEM_URL}/{workitemId}";

            var requestBody = itemFields
                .Select(fld => new {
                    op = "add",
                    path = $"/fields/{fld.Key}",
                    value = fld.Value
                })
                .ToList();

            var queryParams = MakeQueryParams(expand, bypassRules, suppressNotifications, validateOnly);

            var customHeaders = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json-patch+json" }
            };

            try
            {
                var httpResponse = await httpClient.PatchJsonAsync(requestUrl, requestBody,
                    queryParams: queryParams, customHeaders: customHeaders);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IWorkitemClient::UpdateWorkitemFieldsAsync: HTTP response is null or empty");
                }

                return WorkitemFactory.FromJson(this, httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException(
                    "IWorkitemClient::UpdateWorkitemFieldsAsync: exception raised while updating workitem fields",
                    ex
                );
            }
        }

        // https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work%20items/update?view=azure-devops-rest-6.0#add-a-link
        public async Task<IWorkitem> AddRelationLinkAsync(int sourceWorkitemId, int destinationWorkitemId,
            string relationType,
            IReadOnlyDictionary<string, string>? relationAttributes = null,
            string expand = "All", bool bypassRules = false,
            bool suppressNotifications = false, bool validateOnly = false)
        {
            if (relationType.Trim() == "")
            {
                throw new ClientException(
                    "Relationtype param is empty",
                    new ArgumentNullException(nameof(relationType))
                );
            }

            var requestUrl = $"{clientConnection.ProjectApiUrl}{WORKITEM_URL}/{sourceWorkitemId}";
            var queryParams = MakeQueryParams(expand, bypassRules, suppressNotifications, validateOnly);

            var requestBody = new[]
            {
                new
                {
                    op = "add",
                    path = @"/relations/-",
                    value = new
                    {
                        rel = relationType,
                        url = $"{clientConnection.ServerUrl}{clientConnection.ApiUrl}{WORKITEM_URL}/{destinationWorkitemId}",
                        attattributes = relationAttributes,
                    }
                }
            };

            var customHeaders = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json-patch+json" }
            };

            try
            {
                var httpResponse = await httpClient.PatchJsonAsync(requestUrl, requestBody,
                    queryParams: queryParams, customHeaders: customHeaders);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IWorkitemClient::AddRelationLinkAsync: HTTP response is null or empty");
                }

                return WorkitemFactory.FromJson(this, httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException(
                    "IWorkitemClient::AddRelationLinkAsync: exception raised while adding relation link",
                    ex
                );
            }
        }

        public async Task<IWorkitem> RemoveRelationLinkAsync(int workitemId, int relationId,
            string expand = "All", bool bypassRules = false,
            bool suppressNotifications = false, bool validateOnly = false)
        {
            var requestUrl = $"{clientConnection.ProjectApiUrl}{WORKITEM_URL}/{workitemId}";
            var queryParams = MakeQueryParams(expand, bypassRules, suppressNotifications, validateOnly);

            var requestBody = new[]
            {
                new
                {
                    op = "remove",
                    path = $"/relations/{relationId}"
                }
            };

            var customHeaders = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json-patch+json" }
            };

            try
            {
                var httpResponse = await httpClient.PatchJsonAsync(requestUrl, requestBody,
                    queryParams: queryParams, customHeaders: customHeaders);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IWorkitemClient::RemoveRelationLinkAsync: HTTP response is null or empty");
                }

                return WorkitemFactory.FromJson(this, httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException(
                    "TfsServiceClient: RemoveRelationLinkAsync exception",
                    ex);
            }
        }

        public async Task<IWiqlResult> RunSavedQueryAsync(string queryId)
        {
            if (queryId.Trim() == "")
            {
                throw new ClientException(
                    "IWorkItemClient::RunSavedQueryAsync: query id can't be empty",
                    new ArgumentException(nameof(queryId))
                );
            }

            var requestUrl = $"{clientConnection.ProjectApiUrl}{QUERY_URL}/{queryId}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api-version", API_VERSION },
                { "$expand", "clauses" }
            };

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IWorkitemClient::RunSavedQueryAsync: HTTP response is null or empty");
                }

                var jsonObj = JObject.Parse(httpResponse.Content);
                if ((jsonObj == null) || (jsonObj["wiql"] == null))
                {
                    throw new ClientException("IWorkitemClient::RunSavedQueryAsync: HTTP response does not have WIQL answer");
                }

                return await RunWiqlAsync(jsonObj["wiql"]!.ToObject<string>()!);
            }
            catch (Exception ex)
            {
                throw new ClientException(
                    "IWorkitemClient: RunSavedQueryAsync exception raised",
                    ex);
            }
        }

        public async Task<IWiqlResult> RunWiqlAsync(string query, int maxTop = -1)
        {
            if (query.Trim() == "")
            {
                throw new ClientException(
                    "IWorkItemClient::RunWiqlAsync: query string can't be empty",
                    new ArgumentException(nameof(query))
                );
            }

            var requestUrl = $"{clientConnection.ProjectApiUrl}{WIQL_URL}";

            var requestBody = new
            {
                query = query
            };

            var queryParams = new Dictionary<string, string>()
            {
                { "api-version", API_VERSION }
            };

            if (maxTop > 0)
            {
                queryParams.Add("$top", maxTop.ToString());
            }

            try
            {
                var httpResponse = await httpClient.PostJsonAsync(requestUrl, requestBody,
                    queryParams: queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IWorkitemClient::RunWiqlAsync: HTTP response is null or empty");
                }

                return WiqlFactory.FromContentResponse(this, httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException(
                    "IWorkitemClient: RunWiqlAsync exception raised",
                    ex);
            }
        }

        public async Task<IChangeset> GetChangesetAsync(int changesetId)
        {
            var queryParams = new Dictionary<string, string>()
            {
                { "api-version", API_VERSION },
                { "includeWorkItems", "true" }
            };

            // TODO: CHEK URL
            var requestUrl = $"{clientConnection.ProjectApiUrl}{WIQL_URL}/{changesetId}";

            try
            {
                var httpResponse = await httpClient.GetAsync(requestUrl, queryParams: queryParams);

                if ((httpResponse == null) || (httpResponse.HasError) || (httpResponse.Content == null))
                {
                    throw new ClientException("IWorkitemClient::GetChangesetAsync: HTTP response is empty or null");
                }

                return ChangesetFactory.FromJsonResponse(this, httpResponse.Content);
            }
            catch (Exception ex)
            {
                throw new ClientException(
                    "IWorkitemClient::GetChangesetAsync: exception raised", ex);
            }
        }
    }
}
