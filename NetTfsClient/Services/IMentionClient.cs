using NetTfsClient.Models.Project;
using NetTfsClient.Models.Workitems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services
{
    /// <summary>
    /// Result of mention
    /// </summary>
    public enum MentionResult : byte
    {
        ERROR = 0,
        SUCCESS
    }

    /// <summary>
    /// Mention client for mention users of TFS/Azure service
    /// </summary>
    public interface IMentionClient
    {
        /// <summary>
        /// Sends mention to user. Writes mention to History of workitem
        /// WARNING: this function uses non-public API
        /// </summary>
        /// <param name="workitem">Workitem where mention will be saved</param>
        /// <param name="toUser">Mentioned user</param>
        /// <param name="message">Mention text</param>
        /// <param name="fromUser">copy user who will be mentioned. Default: null</param>
        /// <returns></returns>
        Task<MentionResult> SendMentionAsync(IWorkitem workitem, IMember toUser,
            string message, IMember? fromUser = null);
    }
}
