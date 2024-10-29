using Data.Models.ResultModel;

namespace OtpAPIServices.Services
{
    public interface IVerifyServices
    {
        Task<ResultModel> SendOTPEmailRequest(string Email);

        Task<ResultModel> VerifyOTPCode(string Email, string OTPCode);
    }
}