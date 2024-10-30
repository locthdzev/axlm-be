using Data.Models.ModuleModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleAPIServices.Services;

namespace ModuleAPIServices.Controllers
{
    [ApiController]
    [Route("api/module-management")]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleServices _moduleServices;

        public ModuleController(IModuleServices moduleServices)
        {
            _moduleServices = moduleServices;
        }

        [Authorize(Roles = "SUAdmin,Admin")]
        [HttpGet("modules")]
        public async Task<IActionResult> GetModuleList(int page)
        {
            Data.Models.ResultModel.ResultModel result = await _moduleServices.GetListModule(page);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet("modules/{id}")]
        public async Task<IActionResult> GetModuleDetails(Guid id)
        {
            Data.Models.ResultModel.ResultModel result = await _moduleServices.GetDetailModule(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin,Admin")]
        [HttpPost("modules")]
        public async Task<IActionResult> CreateModule([FromBody] ModuleReqModel form)
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

            var createForm = new ModuleReqModel
            {
                Name = form.Name,
                Code = form.Code,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
            };

            Data.Models.ResultModel.ResultModel result = await _moduleServices.CreateModule(createForm);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPut("modules/{id}")]
        public async Task<IActionResult> EditModule(Guid id, [FromBody] ModuleUpdateModel Form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _moduleServices.UpdateModule(Form, id, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPut("modules/status")]
        public async Task<IActionResult> DeleteModule([FromBody] List<Guid> listId)
        {
            var res = await _moduleServices.DeleteModule(listId);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet]
        [Route("modules/{id}/{classId}/scores")]
        public async Task<IActionResult> ViewStudentResultRecord(Guid id, Guid classId)
        {
            var res = await _moduleServices.ViewStudentResultRecord(id, classId);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin,Admin")]
        [HttpPost("modules/{trainingProgramId}/clone")]
        public async Task<IActionResult> CreateModuleByCopy(Guid trainingProgramId, [FromBody] List<Guid> moduleIdList)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];

            Data.Models.ResultModel.ResultModel result = await _moduleServices.AddModuleByCopy(trainingProgramId, moduleIdList, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}