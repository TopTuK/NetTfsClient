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
        /// Returns TFS/Azure project instance.
        /// Docs: https://learn.microsoft.com/en-us/rest/api/azure/devops/core/projects/get?view=azure-devops-rest-6.0
        /// </summary>
        /// <param name="projectId">project id.</param>
        /// <param name="capabilities">Include capabilities (such as source control) in the team project result (default: false)</param>
        /// <param name="history">Search within renamed projects (that had such name in the past).</param>
        /// <returns></returns>
        Task<IProject> GetProjectAsync(string projectId, bool capabilities = false, bool history = false);

        /// <summary>
        /// Async returns enumerable of Tfs/Azure projects.
        /// Docs: https://docs.microsoft.com/en-us/rest/api/azure/devops/core/projects/list?view=azure-devops-rest-6.0
        /// </summary>
        /// <param name="skip">number top project to skip. Default: 0</param>
        /// <returns>Enumerable of project <see cref="IProject"/></returns>
        Task<IEnumerable<IProject>> GetProjectsAsync(int skip = 0);

        /// <summary>
        /// Returns TFS/Azure team instance.
        /// Docs: https://learn.microsoft.com/en-us/rest/api/azure/devops/core/teams/get?view=azure-devops-rest-6.0&tabs=HTTP
        /// </summary>
        /// <param name="projectId">project id. Can't be None.</param>
        /// <param name="teamId">team id. Can't be None</param>
        /// <param name="expand"></param>
        /// <returns>A value indicating whether or not to expand Identity information in the result WebApiTeam object</returns>
        Task<ITeam> GetTeamAsync(string projectId, string teamId, bool expand = false);

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

        /// <summary>
        /// Get a list of identities for the project
        /// Non-public api: {project_id}/_api/_identity/ReadScopedApplicationGroupsJson? __v = 5
        /// </summary>
        /// <param name="project"><see cref="IProject"/> instance of the team project the team belongs to</param>
        /// <returns>Enumerable of IIdentity</returns>
        Task<IEnumerable<IIdentity>> GetProjectGroupsAsync(IProject project);

        /// <summary>
        /// Get a list of project group members
        /// Non-public api: {project_id}/_api/_identity/ReadGroupMembers?__v=5&scope={group.foundation_id}&readMembers=true
        /// </summary>
        /// <param name="project"><see cref="IProject"/> instance of the team project the team belongs to</param>
        /// <param name="identity"><see cref="IIdentity"/> instance (group) </param>
        /// <returns>Enumerable of <see cref="IIdentity"/></returns>
        Task<IEnumerable<IIdentity>> GetProjectGroupMembersAsync(IProject project, IIdentity identity);
    }
}
