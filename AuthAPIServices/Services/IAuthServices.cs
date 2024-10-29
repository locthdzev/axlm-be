using Data.Models.ResultModel;
using Data.Models.UserModel;

namespace AuthAPIServices.Services
{
    public interface IAuthServices
    {
        Task<ResultModel> Login(UserLoginReqModel LoginForm);
        Task<ResultModel> ResetPassword(UserResetPasswordReqModel ResetPasswordReqModel);
        Task<ResultModel> ChangePassword(Guid userId, ChangePasswordReqModel changePasswordModel);
    }
}