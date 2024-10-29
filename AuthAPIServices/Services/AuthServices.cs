using Data.Entities;
using AutoMapper;
using Data.Models.ResultModel;
using static Data.Enums.Status;
using Encoder = Data.Utilities.Encoder.Encoder;
using Repositories.UserRepositories;
using Data.Models.UserModel;

namespace AuthAPIServices.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IUserRepository _userRepository;

        public AuthServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResultModel> Login(UserLoginReqModel LoginForm)
        {
            ResultModel Result = new();
            try
            {
                var User = await _userRepository.GetUserByEmail(LoginForm.Email);
                if (User == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "Not found";
                    return Result;
                }
                else
                {
                    if (User.Status == UserStatus.INACTIVE)
                    {
                        Result.IsSuccess = false;
                        Result.Code = 400;
                        Result.Message = "User is inactive";
                        return Result;
                    }

                    var Salt = User.Salt;
                    var PasswordStored = User.Password;
                    var Verify = Encoder.VerifyPasswordHashed(LoginForm.Password, Salt, PasswordStored);
                    if (Verify)
                    {
                        if (User.Status == UserStatus.RESETPASSWORD)
                        {
                            User.Status = UserStatus.ACTIVE;
                            await _userRepository.Update(User);
                        }
                        UserLoginResModel LoginResData = new UserLoginResModel();

                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<User, UserResModel>();
                        });
                        IMapper mapper = config.CreateMapper();
                        UserResModel UserResModel = mapper.Map<User, UserResModel>(User);

                        LoginResData.User = UserResModel;
                        LoginResData.Token = Encoder.GenerateJWT(User);
                        Result.IsSuccess = true;
                        Result.Code = 200;
                        Result.Data = LoginResData;
                    }
                    else
                    {
                        Result.IsSuccess = false;
                        Result.Code = 400;
                        Result.Message = "Password is invalid";
                    }
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

        public async Task<ResultModel> ResetPassword(UserResetPasswordReqModel ResetPasswordReqModel)
        {
            ResultModel Result = new();
            try
            {
                var User = await _userRepository.GetUserByEmail(ResetPasswordReqModel.Email);
                if (User == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "The User cannot validate to reset password";
                    return Result;
                }
                if (User.Status != UserStatus.RESETPASSWORD)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "The request is denied!";
                    return Result;
                }
                var HashedPasswordModel = Encoder.CreateHashPassword(ResetPasswordReqModel.Password);
                User.Password = HashedPasswordModel.HashedPassword;
                User.Salt = HashedPasswordModel.Salt;
                User.Status = UserStatus.ACTIVE;
                 await _userRepository.Update(User);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Reset password successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> ChangePassword(Guid userId, ChangePasswordReqModel changePasswordModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.GetUserById(userId);

                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404; // Not Found
                    result.Message = "User not found";
                    return result;
                }

                // Verify the old password
                var oldPasswordHash = user.Password;
                var oldPasswordSalt = user.Salt;
                var isOldPasswordValid = Encoder.VerifyPasswordHashed(changePasswordModel.OldPassword, oldPasswordSalt, oldPasswordHash);

                if (!isOldPasswordValid)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Old password is incorrect";
                    return result;
                }

                // Generate new password hash and salt
                var newPasswordHashModel = Encoder.CreateHashPassword(changePasswordModel.NewPassword);
                user.Password = newPasswordHashModel.HashedPassword;
                user.Salt = newPasswordHashModel.Salt;

                // Update the user in the database
                await _userRepository.Update(user);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Password updated successfully";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500; // Internal Server Error
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}