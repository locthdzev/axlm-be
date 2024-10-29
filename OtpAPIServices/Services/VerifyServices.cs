using Data.Entities;
using Data.Models.ResultModel;
using Data.Utilities.Email;
using Repositories.OtpRepositories;
using Repositories.UserRepositories;
using static Data.Enums.Status;

namespace OtpAPIServices.Services
{
    public class VerifyServices : IVerifyServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpRepository _otpRepository;
        private readonly IEmail _email;

        public VerifyServices(IUserRepository userRepository, IOtpRepository otpRepository, IEmail email)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _email = email;
        }

        private string CreateOTPCode()
        {
            Random rnd = new();
            return rnd.Next(100000, 999999).ToString();
        }

        public async Task<ResultModel> SendOTPEmailRequest(string Email)
        {
            ResultModel Result = new ResultModel();
            try
            {
                var User = await _userRepository.GetUserByEmail(Email);
                if (User == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "The User with this email is invalid";
                    return Result;
                }
                var GetOTP = await _otpRepository.GetOTPByUserId(User.Id);
                if (GetOTP != null)
                {
                    if ((DateTime.Now - GetOTP.CreatedAt).TotalMinutes < 2)
                    {
                        Result.IsSuccess = false;
                        Result.Code = 400;
                        Result.Message = "Can not send OTP right now!";
                        return Result;
                    }
                }
                string OTPCode = CreateOTPCode();
                string FilePath = "../Data/Utilities/TemplateEmail/ResetPassword.html";
                string Html = File.ReadAllText(FilePath);
                Html = Html.Replace("{{OTPCode}}", OTPCode);
                Html = Html.Replace("{{toEmail}}", Email);
                bool check = await _email.SendEmail(Email, "Reset Password", Html);
                if (!check)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Send email is failed!";
                    return Result;
                }
                OtpVerify Otp = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = User.Id,
                    OtpCode = OTPCode,
                    CreatedAt = DateTime.Now,
                    ExpiredAt = DateTime.Now.AddMinutes(10),
                    IsUsed = false
                };
                  await _otpRepository.Insert(Otp);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "The OTP code has been sent to your email";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> VerifyOTPCode(string Email, string OTPCode)
        {
            ResultModel Result = new();
            try
            {
                var User = await _userRepository.GetUserByEmail(Email);
                if (User == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "The User cannot validate to verify this OTP";
                    return Result;
                }
                var GetOTP = await _otpRepository.GetOTPByUserId(User.Id);
                if (GetOTP != null)
                {
                    if ((DateTime.Now - GetOTP.CreatedAt).TotalMinutes > 10 || GetOTP.IsUsed)
                    {
                        Result.IsSuccess = false;
                        Result.Code = 400;
                        Result.Message = "The OTP is expired!";
                        return Result;
                    }
                    GetOTP.IsUsed = true;
                    await _otpRepository.Update(GetOTP);
                    User.Status = UserStatus.RESETPASSWORD;
                    await _userRepository.Update(User);
                    Result.IsSuccess = true;
                    Result.Code = 200;
                }
                else
                {
                    Result.IsSuccess = true;
                    Result.Code = 400;
                    Result.Message = "The OTP is invalid!";
                }
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }
    }
}