using NetTfsClient.Models.Project;
using NetTfsClient.Models.Workitems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services
{
    public enum MentionResult : byte
    {
        ERROR = 0,
        SUCCESS
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMentionClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workitem"></param>
        /// <param name="toUser"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="fromUser"></param>
        /// <returns></returns>
        Task<MentionResult> SendMentionAsync(IWorkitem workitem, IMember toUser,
            string message, IMember? fromUser = null);
    }
}
