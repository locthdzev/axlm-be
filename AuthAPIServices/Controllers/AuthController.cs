using AuthAPIServices.Services;
using Data.Models.AuthModel;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPIServices.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginReqModel LoginForm)
        {
            Data.Models.ResultModel.ResultModel result = await _authServices.Login(LoginForm);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordReqModel ResetPasswordReqModel)
        {
            Data.Models.ResultModel.ResultModel result = await _authServices.ResetPassword(ResetPasswordReqModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordReqModel changePasswordModel)
        {
            var userIdString = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }

            Data.Models.ResultModel.ResultModel result = await _authServices.ChangePassword(userId, changePasswordModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}