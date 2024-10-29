using Data.Models.UserModel;

namespace Data.Utilities.Email
{
    public interface IEmail
    {
        Task<bool> SendEmail(string Email, string Subject, string Html);
        Task<bool> SendListEmail(string Subject, List<EmailSendingModel> sendingList);
    }
}