using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
    public enum BugsBehaviorType : byte
    {
        UNKNOWN = 0,
        Off = 1,
        AsRequirements = 2,
        AsTasks = 3
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ITeamSettings
    {
        /// <summary>
        /// 
        /// </summary>
        BugsBehaviorType BugsBehavior { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<string> WorkingDays { get; }

        /// <summary>
        /// 
        /// </summary>
        string? DefaultIterationMacro { get; }

        /// <summary>
        /// 
        /// </summary>
        IAzureIteration BacklogIteration { get; }

        /// <summary>
        /// 
        /// </summary>
        IAzureIteration DefaultIteration { get; }
    }
}
