using Data.Models.FilterModel;
using Data.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPIServices.Services;

namespace UserAPIServices.Controllers
{
    [ApiController]
    [Route("api/user-management")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer, Student")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser(int page, [FromQuery] FilterModel reqModel)
        {
            Data.Models.ResultModel.ResultModel result = await _userServices.GetAllUser(page, reqModel);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin")]
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateReqModel Form)
        {
            Data.Models.ResultModel.ResultModel result = await _userServices.CreateAccount(Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("users/me")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userIdString = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }
            var result = await _userServices.GetUserProfile(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer, Student")]
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetAccountsInfo(Guid id)
        {
            var res = await _userServices.ViewAccountsInfo(id);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin")]
        [HttpPut("users/status")]
        public async Task<IActionResult> UpdateAccountsStatus(UpdateAccountsStatusModel model)
        {
            var res = await _userServices.UpdateAccountsStatus(model);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet("users/trainers")]
        public async Task<IActionResult> GetTrainerList(int page)
        {
            var res = await _userServices.GetTrainerList(page);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}