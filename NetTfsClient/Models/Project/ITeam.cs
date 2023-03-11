using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
    public interface ITeam
    {
        /// <summary>
        /// Team (Identity) Guid. A Team Foundation ID. 
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Team name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Team description
        /// </summary>
        string Description { get; }
    }
}
