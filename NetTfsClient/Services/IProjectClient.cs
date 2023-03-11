using NetTfsClient.Models.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProjectClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        Task<IEnumerable<IProject>> GetProjectsAsync(int skip = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        Task<IEnumerable<ITeam>> GetAllTeamsAsync(bool currentUser = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        Task<IEnumerable<ITeam>> GetProjectTeamsAsync(IProject project, bool currentUser = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        Task<IEnumerable<ITeamMember>> GetProjectTeamMembersAsync(IProject project, ITeam team);
    }
}
