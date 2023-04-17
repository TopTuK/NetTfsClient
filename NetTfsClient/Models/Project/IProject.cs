using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
    public interface IProject
    {
        /// <summary>
        /// ID of TFS/Azure project
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Name of TFS/Azure project
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of TFS/Azure Project
        /// Can be Empty string
        /// </summary>
        string Description { get; }
    }
}
