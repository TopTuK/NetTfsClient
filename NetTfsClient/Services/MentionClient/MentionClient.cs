using NetTfsClient.Models;
using NetTfsClient.Models.Project;
using NetTfsClient.Models.Workitems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Services.MentionClient
{
    internal class MentionClient : IMentionClient
    {
        private static string TFS_HISTORY_FIELD = "System.History";

        public async Task<MentionResult> SendMentionAsync(IWorkitem workitem, 
            IMember toUser, string title, string message, IMember? fromUser = null)
        {
            StringBuilder mention = new StringBuilder();

            // Add mention (no public API)
            mention.AppendLine($"<a href=\"#\" data-vss-mention=\"version:2.0,{toUser.Id}\">@{toUser.DisplayName}</a><br>");

            // Add comment/message
            mention.AppendLine(message);

            // Ignore if user sends mention himself
            if ((fromUser != null) && (fromUser.Id != toUser.Id))
            {
                mention.AppendLine($"<br>CC: <a href=\"#\" data-vss-mention=\"version:2.0,{fromUser.Id}\">@{fromUser.DisplayName}</a>");
            }

            try
            {
                workitem[TFS_HISTORY_FIELD] = mention.ToString();
                var updateResult = await workitem.SaveFieldsChangesAsync();

                return (updateResult == UpdateFieldsResult.UPDATE_SUCCESS)
                    ? MentionResult.SUCCESS
                    : MentionResult.ERROR;
            }
            catch (Exception ex)
            {
                throw new ClientException("IMentionClient::SendMentionAsync: exception raised", ex);
            }
        }
    }
}
