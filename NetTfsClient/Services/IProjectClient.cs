using NetTfsClient.Models.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services
{
    /// <summary>
    /// IProjectClient for managing projects, teams and team members
    /// </summary>
    public interface IProjectClient
    {
        /// <summary>
        /// Async returns enumerable of Tfs/Azure projects.
        /// Docs: https://docs.microsoft.com/en-us/rest/api/azure/devops/core/projects/list?view=azure-devops-rest-6.0
        /// </summary>
        /// <param name="skip">number top project to skip. Default: 0</param>
        /// <returns>Enumerable of project <see cref="IProject"/></returns>
        Task<IEnumerable<IProject>> GetProjectsAsync(int skip = 0);

        /// <summary>
        /// Async returns enumerablet of TFS/Azure teams.
        /// Docs: https://docs.microsoft.com/en-us/rest/api/azure/devops/core/teams/get-teams?view=azure-devops-rest-6.0
        /// </summary>
        /// <param name="currentUser">If true return all the teams requesting user is member, otherwise return all the teams user has read access.</param>
        /// <returns>Enumerable of TFS/Azure teams <see cref="ITeam"/></returns>
        Task<IEnumerable<ITeam>> GetAllTeamsAsync(bool currentUser = false);

        /// <summary>
        /// Gets a list of teams of given project.
        /// Docs: https://docs.microsoft.com/en-us/rest/api/azure/devops/core/teams/get-teams?view=azure-devops-rest-6.0
        /// </summary>
        /// <param name="project"><see cref="IProject"/> instance</param>
        /// <param name="currentUser">If true return all the teams requesting user is member, otherwise return all the teams user has read access. Default: False</param>
        /// <returns>Enumerable of <see cref="ITeam"/></returns>
        Task<IEnumerable<ITeam>> GetProjectTeamsAsync(IProject project, bool currentUser = false);

        /// <summary>
        /// Gets a list of members for a specific team and a project.
        /// Docs: https://docs.microsoft.com/en-us/rest/api/azure/devops/core/teams/get-team-members-with-extended-properties?view=azure-devops-rest-6.0
        /// </summary>
        /// <param name="project"><see cref="IProject"/> instance of the team project the team belongs to</param>
        /// <param name="team"><see cref="ITeam"/> instance</param>
        /// <returns>Enumerable of <see cref="ITeamMember"/> members</returns>
        Task<IEnumerable<ITeamMember>> GetProjectTeamMembersAsync(IProject project, ITeam team);
    }
}
