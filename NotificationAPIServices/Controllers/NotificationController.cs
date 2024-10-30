using Data.Models.NotificationModel;
using Data.Models.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationAPIServices.Services;

namespace NotificationAPIServices.Controllers
{
    [ApiController]
    [Route("api/notification-management")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;

        public NotificationController(INotificationServices notificationServices)
        {
            _notificationServices = notificationServices;
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetAllNotifications(int page)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            ResultModel result = await _notificationServices.GetAllNotifications(token, page);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPost("notifications")]
        public async Task<IActionResult> CreateNotification([FromForm] NotificationCreateReqModel Form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _notificationServices.CreateNotification(token, Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPut("notifications/{id}")]
        public async Task<IActionResult> UpdateNotification(Guid id, [FromForm] NotifcationUpdateModel Form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            ResultModel result = await _notificationServices.UpdateNotification(id, token, Form);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }
        
        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpDelete("notifications/{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _notificationServices.DeleteNotification(id, token);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [HttpGet("notifications/{id}")]
        public async Task<IActionResult> NotificationDetails(Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            ResultModel result = await _notificationServices.GetNotificationDetails(id, token);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }
    }
}