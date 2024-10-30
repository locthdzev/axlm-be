using Data.Models.EmailModel;
using Data.Models.ResultModel;
using EmailAPIServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailAPIServices.Controllers
{
    [ApiController]
    [Route("api/email-management")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailServices _emailServices;

        public EmailController(IEmailServices emailServices)
        {
            _emailServices = emailServices;
        }

        [HttpGet("email-requests/sent")]
        public async Task<IActionResult> GetEmailRequestsList(int page)
        {
            var userIdString = User.FindFirst("userid")?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Unable to retrieve or invalid user ID");
            }

            ResultModel result = await _emailServices.GetEmailRequests(userId, page);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Student, Trainer")]
        [HttpPost("email-requests")]
        public async Task<IActionResult> SendEmail(string recipientEmail, [FromForm] EmailCreateReqModel Form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _emailServices.SendEmail(recipientEmail, token, Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Student, Trainer")]
        [HttpGet("email-requests/{id}/conversation")]
        public async Task<IActionResult> ViewEmailAndReplies(Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _emailServices.ViewEmailAndReplies(id, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Student, Trainer")]
        [HttpPost("email-requests/{id}/replies")]
        public async Task<IActionResult> SendReply([FromBody] string replyContent, Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _emailServices.SendReply(id, replyContent, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("email-requests/received")]
        public async Task<IActionResult> GetEmailReceivedList(int page)
        {
            var userIdString = User.FindFirst("userid")?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Unable to retrieve or invalid user ID");
            }

            ResultModel result = await _emailServices.GetEmailReceived(userId, page);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "Student, Trainer")]
        [HttpGet("email-requests/students/recipients")]
        public async Task<IActionResult> GetRecipientOfStudent()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _emailServices.GetRecipientOfStudent(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}