using NetTfsClient.Models.Project;
using NetTfsClient.Models.Workitems;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Boards
{
    internal static class BoardItemsFactory
    {
        private class BaseBoard : IBaseBoard
        {
            public string Id { get; init; }
            public string Name { get; init; }

            public BaseBoard(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public BaseBoard(JToken jsonItem)
            {
                Id = jsonItem["id"]?.Value<string>() ?? string.Empty;
                Name = jsonItem["name"]?.Value<string>() ?? string.Empty;
            }
        }

        internal static IEnumerable<IBaseBoard> BaseBoardsFromJsonContent(string jsonContent)
        {
            var jsonItems = JObject.Parse(jsonContent);
            if (jsonItems["value"] != null)
            {
                return jsonItems["value"]!
                    .Select(jsonProject => new BaseBoard(jsonProject))
                    .ToList();
            }

            return Enumerable.Empty<IBaseBoard>();
        }

        private class BoardStateMap : IBoardStateMap
        {
            public string WorkitemType { get; init; }
            public string WorkitemState { get; init; }

            public BoardStateMap(string workitemType, string workitemState)
            {
                WorkitemType = workitemType;
                WorkitemState = workitemState;
            }
        }

        private class BoardColumn : IBoardColumn
        {
            public string Id { get; init; }
            public string Name { get; init; }
            public int WipLimit { get; init; }
            
            public string ColumnType { get; init; }
            public BoardColumnType ColumnRole { get; init; }

            public IEnumerable<IBoardStateMap> StateMap { get; init; }

            public BoardColumn(JToken jsonItem)
            {
                Id = jsonItem["id"]?.Value<string>() ?? string.Empty;
                Name = jsonItem["name"]?.Value<string>() ?? string.Empty;
                WipLimit = jsonItem["itemLimit"]?.Value<int>() ?? -1;

                ColumnType = jsonItem["columnType"]?.Value<string>() ?? string.Empty;
                ColumnRole = ColumnType switch
                {
                    "inProgress" => BoardColumnType.Active,
                    "incoming" => BoardColumnType.Incoming,
                    "outgoing" => BoardColumnType.Outgoing,
                    _ => BoardColumnType.UNKNOWN
                };

                var stateItem = jsonItem["stateMappings"]?.Value<JObject>() ?? null;
                if (stateItem != null)
                {
                    var stateKeys = stateItem.Properties()
                        .Select(p => p.Name)
                        .ToList();
                    StateMap = stateKeys
                        .Select(k => new BoardStateMap(k, stateItem[k]?.Value<string>() ?? ""))
                        .ToList();
                }
                else
                {
                    StateMap = Enumerable.Empty<BoardStateMap>();
                }
            }
        }

        private class BoardRow : IBoardRow
        {
            public string Id { get; init; }
            public string? Name { get; init; }
            public string? Color { get; init; }

            public BoardRow(JToken jsonItem)
            {
                Id = jsonItem["id"]?.Value<string>() ?? string.Empty;
                Name = jsonItem["name"]?.Value<string>();
                Color = jsonItem["color"]?.Value<string>();
            }
        }

        private class Board : BaseBoard, IBoard
        {
            public int Rev { get; init; }
            public bool IsValid { get; init; }
            public bool CanEdit { get; init; }

            public IEnumerable<IBoardColumn> Columns { get; init; }
            public IEnumerable<IBoardRow> Rows { get; init; }

            public Board(JToken jsonItem)
                : base(jsonItem)
            {
                Rev = jsonItem["revision"]?.Value<int>() ?? 0;
                IsValid = jsonItem["isValid"]?.Value<bool>() ?? false;
                CanEdit = jsonItem["canEdit"]?.Value<bool>() ?? false;

                Columns = jsonItem["columns"]?.Select(jToken => new BoardColumn(jToken)).ToList()
                    ?? Enumerable.Empty<IBoardColumn>();
                Rows = jsonItem["rows"]?.Select(JToken => new BoardRow(JToken)).ToList()
                    ?? Enumerable.Empty<IBoardRow>();
            }
        }

        internal static IBoard BoardFromJsonContent(string jsonContent)
        {
            var jsonItem = JObject.Parse(jsonContent);
            return new Board(jsonItem);
        }
    }
}
