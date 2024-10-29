using ClassAPIServices.Services;
using Data.Models.ClassModel;
using Data.Models.ResultModel;
using Data.Models.StudentClassModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassAPIServices.Controllers
{
    [ApiController]
    [Route("api/class-management")]
    public class ClassController : ControllerBase
    {
        private readonly IClassServices _classServices;

        public ClassController(IClassServices classServices)
        {
            _classServices = classServices;
        }

        [Authorize(Roles = "SUAdmin,Admin")]
        [HttpGet("classes")]
        public async Task<IActionResult> GetClassList(int page, [FromQuery] FilterDayModel reqModel)
        {
            ResultModel result = await _classServices.GetClassList(page, reqModel);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin,Admin")]
        [HttpPost("classes")]
        public async Task<IActionResult> CreateClass([FromBody] ClassReqModel createForm)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _classServices.CreateClass(createForm, token);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpGet("classes/none-trainer")]
        public async Task<IActionResult> GetNoneTrainerClassList()
        {
            ResultModel result = await _classServices.GetNoneTrainerClassList();
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpGet("classes/{id}")]
        public async Task<IActionResult> GetClassInformation(Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _classServices.GetClassInformation(id, token);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("classes/me")]
        public async Task<IActionResult> GetClassByStudent()
        {
            var userIdString = User.FindFirst("userid")?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Unable to retrieve or invalid user ID");
            }

            ResultModel result = await _classServices.GetClassOfStudent(userId);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpGet("classes/managed")]
        public async Task<IActionResult> GetClassesOfManager()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _classServices.GetClassesOfManagerAndTrainer(token);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin,Admin")]
        [HttpPut("classes/{id}")]
        public async Task<IActionResult> UpdateClass(Guid id, [FromBody] ClassUpdateReqModel form)
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

            Guid currentUserId = userId;

            var updateForm = new ClassUpdateReqModel(currentUserId)
            {
                Name = form.Name,
                ProgramId = form.ProgramId,
                StartAt = form.StartAt,
                EndAt = form.EndAt,
                Location = form.Location,
                Status = form.Status,
            };

            try
            {
                ResultModel result = await _classServices.UpdateClass(updateForm, id);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPut]
        [Route("classes/status")]
        public async Task<IActionResult> DeleteClass([FromBody] DeleteClassReqModel reqModel)
        {
            var res = await _classServices.DeleteClass(reqModel);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet("classes/{id}/students")]
        public async Task<IActionResult> GetAllStudentsbyClassId(Guid id, int page)
        {
            ResultModel result = await _classServices.GetStudentsByClassId(id, page);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPost]
        [Route("classes/{id}/students")]
        public async Task<IActionResult> AddStudentToClass(Guid id, [FromBody] AddStudentToClassReqModel reqModel)
        {
            var res = await _classServices.AddStudentToClass(reqModel, id);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpDelete]
        [Route("classes/{id}/students")]
        public async Task<IActionResult> DeleteStudentsFromClass(Guid id, List<Guid> StudentsId)
        {
            var res = await _classServices.DeleteStudentsFromClass(id, StudentsId);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPost]
        [Route("classes/{id}/trainers")]
        public async Task<IActionResult> AddTrainerToClass(Guid id, [FromBody] AddTrainerModel reqModel)
        {
            var res = await _classServices.AddTrainerToClass(reqModel, id);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpGet]
        [Route("classes/{id}/modules/lectures")]
        public async Task<IActionResult> GetModuleLecture(Guid id)
        {
            var result = await _classServices.GetListModuleLectureByClassId(id);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        [Route("classes/modules/lectures")]
        public async Task<IActionResult> GetModuleLecture()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _classServices.GetListModuleLecture(token);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpGet]
        [Route("classes/{id}/modules/average")]
        public async Task<IActionResult> GetModuleListAvgScore(Guid id)
        {
            var result = await _classServices.GetModuleListAvgScoreOfClass(id);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }
    }
}