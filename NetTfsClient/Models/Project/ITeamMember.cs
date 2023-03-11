using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
    public interface ITeamMember
    {
        /// <summary>
        /// Relevant Team
        /// </summary>
        ITeam Team { get; }

        /// <summary>
        /// Flag if member is team admin
        /// </summary>
        bool IsTeamAdmin { get; }

        /// <summary>
        /// Member ID.
        /// This ID is used for mentions
        /// </summary>
        string Id { get; }

        /// <summary>
        /// This is the non-unique display name of the graph subject.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// This url is the full route to the source resource of this graph subject.
        /// </summary>
        string Url { get; }
    }
}
