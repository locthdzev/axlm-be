using Data.Models.OtpModel;
using Microsoft.AspNetCore.Mvc;
using OtpAPIServices.Services;

namespace OtpAPIServices.Controllers
{
    [ApiController]
    [Route("api/otp-management")]
    public class OtpController : ControllerBase
    {
        private readonly IVerifyServices _verifyServices;

        public OtpController(IVerifyServices verifyServices)
        {
            _verifyServices = verifyServices;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOtp([FromBody] string Email)
        {
            Data.Models.ResultModel.ResultModel result = await _verifyServices.SendOTPEmailRequest(Email);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] UserVerifyOTPResModel VerifyModel)
        {
            Data.Models.ResultModel.ResultModel result = await _verifyServices.VerifyOTPCode(VerifyModel.Email, VerifyModel.OTPCode);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}