using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
    /// <summary>
    /// Identity model contains infromation about TFS/Azure identity
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// Identity type
        /// </summary>
        string IdentityType { get; }

        /// <summary>
        /// Friendly display name of Identity
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Display name of Identity
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Team Foundation Id of Identity
        /// </summary>
        string FoundationId { get; }

        bool IsGroup { get; }
        bool IsTeam { get; }
        bool IsUser { get; }
    }
}
