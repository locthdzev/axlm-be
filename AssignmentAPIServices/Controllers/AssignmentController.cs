using AssignmentAPIServices.Services;
using Data.Models.AssignmentModel;
using Data.Models.FileModel;
using Data.Models.ResultModel;
using Data.Utilities.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentAPIServices.Controllers
{
    [ApiController]
    [Route("api/assignment-management")]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentServices _assignmentServices;
        private readonly IExcel _excel;

        public AssignmentController(IAssignmentServices assignmentServices, IExcel excel)
        {
            _assignmentServices = assignmentServices;
            _excel = excel;
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer, Student")]
        [HttpGet("assignments/classes/{classId}")]
        public async Task<IActionResult> GetASMList(Guid classId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _assignmentServices.GetAsmList(classId, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Trainer")]
        [HttpPost("assignments")]
        public async Task<IActionResult> CreateAssignment([FromForm] AssignmentCreateReqModel form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _assignmentServices.CreateAssignment(form, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Trainer")]
        [HttpPut("assignments/{id}")]
        public async Task<IActionResult> UpdateAssignment(Guid id, [FromForm] AssignmentUpdateReqModel form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _assignmentServices.UpdateAssignment(id, form, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Trainer")]
        [HttpPut("assignments/status")]
        public async Task<IActionResult> DeleteAssignment(List<Guid> assignmentId)
        {
            ResultModel result = await _assignmentServices.DeleteAssignment(assignmentId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Trainer, Student")]
        [HttpGet("assignments/{id}")]
        public async Task<IActionResult> GetAssignmentInformation(Guid id)
        {
            ResultModel result = await _assignmentServices.GetAssignmentInformation(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpGet]
        [Route("assignments/{id}/score-template")]
        public async Task<IActionResult> GetExcelTemplate(Guid id)
        {
            try
            {
                var excelModel = await _assignmentServices.GetExcelTemplateContent(id);
                var excelFile = _excel.GenerateExcelTemplateImportScore(excelModel, $"{excelModel.assignment.Title.Replace(".", "")}_{excelModel.classEntity.Name.Replace(".", "")}");
                return File(excelFile.FileContent, excelFile.ContentType, excelFile.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "An error occurred: " + ex.Message
                });
            }
        }

        [Authorize(Roles = "SUAdmin, Admin, Trainer")]
        [HttpPut]
        [Route("assignments/{id}/score-import")]
        public async Task<IActionResult> ImportScoreUsingExcel(IFormFile excelFile, Guid id)
        {

            if (!excelFile.FileName.EndsWith(".xls") && !excelFile.FileName.EndsWith(".xlsx"))
            {
                return BadRequest(new ResultModel
                {
                    IsSuccess = false,
                    Code = 400,
                    Message = "The provided file is not an Excel file."
                });
            }
            var excelFileDTO = new ExcelFile { FileStream = excelFile.OpenReadStream() };
            var scoreList = await _excel.ReadImportScoreExcelFile(excelFileDTO);
            var res = await _assignmentServices.ImportAssignmentScore(scoreList, id);
            return res.IsSuccess ? Ok(res) : res.Code == 404 ? NotFound(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Student")]
        [HttpPost]
        [Route("assignments/{id}/submission")]
        public async Task<IActionResult> SubmitAssignment(Guid id, [FromForm] SubmissionCreateResModel Form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _assignmentServices.SubmitAssignment(Form, id, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Trainer, Student")]
        [HttpGet]
        [Route("assignments/{id}/submission")]
        public async Task<IActionResult> GetSubmissonAssignment(Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _assignmentServices.GetSubmissionAssignment(id, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Trainer, Student")]
        [HttpPut]
        [Route("assignments/{id}/submission")]
        public async Task<IActionResult> UpdateSubmissionAssignment(Guid id, [FromForm] SubmissionCreateResModel Form)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _assignmentServices.UpdateSubmissionAssignment(Form, id, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Trainer")]
        [HttpPut]
        [Route("assignments/{id}/score")]
        public async Task<IActionResult> ScoreAssignment(Guid id, [FromBody] ScoreAssignmentResModel Form)
        {
            ResultModel result = await _assignmentServices.ScoreAssignment(Form, id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Trainer")]
        [HttpPut]
        [Route("assignments/{id}/students/scores")]
        public async Task<IActionResult> UpdateScoresInAssignment(Guid id, [FromBody] AssignmentUpdateScoreReqModel Form)
        {
            ResultModel result = await _assignmentServices.UpdateScoreForAssignment(Form, id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        
        [Authorize(Roles = "SUAdmin, Trainer")]
        [HttpGet]
        [Route("assignments/{id}/submission/list")]
        public async Task<IActionResult> GetSubmissionByAssignmentId(Guid id)
        {
            ResultModel result = await _assignmentServices.GetSubmissionByAssignmentId(id);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }
    }
}