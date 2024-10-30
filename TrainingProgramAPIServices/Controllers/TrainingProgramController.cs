using Data.Models.ModuleModel;
using Data.Models.TrainingProgramModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingProgramAPIServices.Services;

namespace TrainingProgramAPIServices.Controllers
{
    [ApiController]
    [Route("api/training-program-management")]
    public class TrainingProgramController : ControllerBase
    {
        private readonly ITrainingProgramServices _trainingProgramServices;

        public TrainingProgramController(ITrainingProgramServices trainingProgramServices)
        {
            _trainingProgramServices = trainingProgramServices;
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet]
        [Route("training-programs")]
        public async Task<IActionResult> GetListTrainingProgram()
        {
            var res = await _trainingProgramServices.GetListProgramTraining();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet]
        [Route("training-programs/{id}")]
        public async Task<IActionResult> GetTrainingProgram(Guid id)
        {
            var res = await _trainingProgramServices.GetProgramTraining(id);
            return res.IsSuccess ? Ok(res) : res.Code.Equals(404) ? NotFound(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin")]
        [HttpPost("training-programs")]
        public async Task<IActionResult> CreateTrainingProgram([FromBody] TrainingProgramReqModel form)
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
            var createForm = new TrainingProgramReqModel
            {
                Name = form.Name,
                Code = form.Code,
                Duration = form.Duration,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
            };

            Data.Models.ResultModel.ResultModel result = await _trainingProgramServices.CreateTrainingProgram(createForm);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin")]
        [HttpPut("training-programs/{id}")]
        public async Task<IActionResult> UpdateTrainingProgram(Guid id, [FromBody] TrainingProgramResModel form)
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
            var updateForm = new TrainingProgramResModel
            {
                Name = form.Name,
                Code = form.Code,
                Duration = form.Duration,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId,
                Status = form.Status,
            };
            Data.Models.ResultModel.ResultModel result = await _trainingProgramServices.UpdateTrainingProgram(updateForm, id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet]
        [Route("training-programs/preview")]
        public async Task<IActionResult> GetListDropDownTrainingProgram()
        {
            var res = await _trainingProgramServices.GetListDropDownProgramTraining();
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin")]
        [HttpPut]
        [Route("training-programs/status")]
        public async Task<IActionResult> DeleteTrainingProgram([FromBody] List<Guid> ListId)
        {
            var res = await _trainingProgramServices.DeleteTrainingProgram(ListId);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet("training-programs/{id}/modules")]
        public async Task<IActionResult> GetModulebyProgramId(Guid id, int page)
        {
            Data.Models.ResultModel.ResultModel result = await _trainingProgramServices.GetModuleByProgramId(id, page);
            return result.IsSuccess ? Ok(result) : result.Code.Equals(404) ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin")]
        [HttpPost("training-programs/{id}/modules")]
        public async Task<IActionResult> AddNewModuleIntoTrainingProgram(Guid id, List<ModuleReqCreateAndAddToTP> Form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _trainingProgramServices.CreateAndAddModuleToTP(token, id, Form);
            return result.IsSuccess ? Ok(result) : result.Code.Equals(404) ? NotFound(result) : BadRequest(result);
        }


        [Authorize(Roles = "SUAdmin")]
        [HttpDelete("training-programs/{id}/modules")]
        public async Task<IActionResult> DeleteModuleFromTrainingProgram(Guid id, List<Guid> ModulesId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _trainingProgramServices.DeleteModuleFromTP(token, id, ModulesId);
            return result.IsSuccess ? Ok(result) : result.Code.Equals(404) ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet("training-programs/{id}/other-modules")]
        public async Task<IActionResult> GetOtherModulebyProgramId(Guid id, int page)
        {
            Data.Models.ResultModel.ResultModel result = await _trainingProgramServices.GetModuleOfOthers(id, page);
            return result.IsSuccess ? Ok(result) : result.Code.Equals(404) ? NotFound(result) : BadRequest(result);
        }
    }
}