using Data.Models.EmailModel;
using Data.Models.ResultModel;

namespace EmailAPIServices.Services
{
    public interface IEmailServices
    {
        Task<ResultModel> GetEmailRequests(Guid userId, int page);
        Task<ResultModel> SendEmail(string recipientEmail, string token, EmailCreateReqModel form);
        Task<ResultModel> ViewEmailAndReplies(Guid emailId, string token);
        Task<ResultModel> SendReply(Guid emailId, string replyContent, string token);
        Task<ResultModel> GetEmailReceived(Guid userId, int page);
        Task<ResultModel> GetRecipientOfStudent(string token);
    }
}