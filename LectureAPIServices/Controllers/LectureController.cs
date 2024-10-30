using Data.Models.LectureModel;
using LectureAPIServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LectureAPIServices.Controllers
{
    [ApiController]
    [Route("api/lecture-management")]
    public class LectureController : ControllerBase
    {
        private readonly ILectureServices _lectureServices;

        public LectureController(ILectureServices lectureServices)
        {
            _lectureServices = lectureServices;
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpGet("lectures")]
        public async Task<IActionResult> GetLectureList(int page)
        {
            Data.Models.ResultModel.ResultModel result = await _lectureServices.GetListLecture(page);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpPost("lectures")]
        public async Task<IActionResult> CreateLecture([FromForm] LectureReqModel form)
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

            var createForm = new LectureReqModel
            {
                Order = form.Order,
                ModuleId = form.ModuleId,
                ClassId = form.ClassId,
                Files = form.Files,
                Name = form.Name,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            Data.Models.ResultModel.ResultModel result = await _lectureServices.CreateLecture(createForm);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpDelete("lectures")]
        public async Task<IActionResult> DeleteLecture([FromBody] List<Guid> listId)
        {
            var res = await _lectureServices.DeleteLecture(listId);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpGet("lectures/{id}")]
        public async Task<IActionResult> GetLectureDetail(Guid id)
        {
            Data.Models.ResultModel.ResultModel result = await _lectureServices.GetLectureDetail(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpPut("lectures/{id}")]
        public async Task<IActionResult> EditLecture(Guid id, [FromForm] LectureUpdateModel Form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _lectureServices.UpdateLecture(Form, id, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}