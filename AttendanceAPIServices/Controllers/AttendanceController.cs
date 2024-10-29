using AttendanceAPIServices.Services;
using Data.Models.AttendanceModel;
using Data.Models.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceAPIServices.Controllers
{
    [ApiController]
    [Route("api/attendance-management")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceServices _attendanceServices;

        public AttendanceController(IAttendanceServices attendanceServices)
        {
            _attendanceServices = attendanceServices;
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet("attendances/classes/{classId}")]
        public async Task<IActionResult> GetAttendanceListOfClass(Guid classId, int page)
        {
            var res = await _attendanceServices.GetAttendanceListOfClass(classId, page);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPost("attendances/{classId}")]
        public async Task<IActionResult> CreateAttendanceForClass(Guid classId, [FromBody] AttendanceReqModel reqModel)
        {
            ResultModel result = await _attendanceServices.CreateAttendance(classId, reqModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet("attendances/{id}")]
        public async Task<IActionResult> GetAttendance(Guid id)
        {
            var res = await _attendanceServices.GetAttendanceById(id);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("attendances/{id}/student")]
        public async Task<IActionResult> GetAttendanceForStudentClient(Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _attendanceServices.GetAttendanceForStudent(id, token);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("attendances/{id}/attendance-detail")]
        public async Task<IActionResult> GetAttendanceForStudentInDB(Guid id, [FromBody] CheckAttendanceReqModel req)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _attendanceServices.CheckAttendance(id, token, req);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}