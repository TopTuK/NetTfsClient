using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Boards
{
    /// <summary>
    /// Contains information about boards of given project and team
    /// </summary>
    public interface IBaseBoard
    {
        /// <summary>
        /// Board Id 
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Board name
        /// </summary>
        string Name { get; }
    }

    public enum BoardColumnType : byte
    {
        UNKNOWN,
        Incoming,
        Active,
        Outgoing
    }

    public interface IBoardStateMap
    {
        string WorkitemType { get; }
        string WorkitemState { get; }
    }

    public interface IBoardColumn
    {
        string Id { get; }
        string Name { get; }
        int WipLimit { get; }

        string ColumnType { get; }
        BoardColumnType ColumnRole { get; }

        IEnumerable<IBoardStateMap> StateMap { get; }
    }

    public interface IBoardRow
    {
        string Id { get; }
        string? Name { get; }
        string? Color { get; }
    }

    /// <summary>
    /// Azure board
    /// </summary>
    public interface IBoard : IBaseBoard
    {
        int Rev { get; }
        bool IsValid { get; }
        bool CanEdit { get; }

        IEnumerable<IBoardColumn> Columns { get; }
        IEnumerable<IBoardRow> Rows { get; }
    }
}
