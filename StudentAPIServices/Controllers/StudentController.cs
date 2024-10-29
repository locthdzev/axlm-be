using System.Globalization;
using ClosedXML.Excel;
using Data.Models.FilterModel;
using Data.Models.StudentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.StudentRepositories;
using StudentAPIServices.Services;

namespace StudentAPIServices.Controllers
{
    [ApiController]
    [Route("api/student-management")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentServices _studentServices;
        public StudentController(IStudentServices studentServices, IStudentRepository studentRepository)
        {
            _studentServices = studentServices;
            _studentRepository = studentRepository;
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents(int page, [FromQuery] FilterModel reqModel)
        {
            Data.Models.ResultModel.ResultModel result = await _studentServices.GetAllStudents(page, reqModel);
            return result.IsSuccess ? Ok(result) : result.Code == 404 ? NotFound(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPost("students")]
        public async Task<IActionResult> ImportDataFromExcel(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (System.IO.Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                return BadRequest("Invalid file format. Please upload a .xlsx file.");
            }

            try
            {
                var studentCreateList = new List<StudentCreateReqModel>();
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);

                        var rows = worksheet.RowsUsed().Skip(1);
                        foreach (var row in rows)
                        {
                            DateTime dob;
                            if (DateTime.TryParseExact(row.Cell(3).Value.ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
                            {
                                var student = new StudentCreateReqModel
                                {
                                    FullName = row.Cell(1).Value.ToString(),
                                    Email = row.Cell(2).Value.ToString(),
                                    Dob = dob,
                                    Address = row.Cell(4).Value.ToString(),
                                    Gender = row.Cell(5).Value.ToString(),
                                    Phone = row.Cell(6).Value.ToString(),
                                };
                                studentCreateList.Add(student);
                            }
                            else
                            {
                                throw new Exception($"Wrong date format {row.Cell(3).Value.ToString()}. Please input the date follow format: M/d/yyyy");
                            }
                        }

                        var res = await _studentServices.CreateStudentList(studentCreateList);
                        return res.IsSuccess ? Ok(res) : BadRequest(res);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPost("students/student")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateReqModel Form)
        {
            Data.Models.ResultModel.ResultModel result = await _studentServices.CreateStudent(Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet]
        [Route("students/unassigned")]
        public async Task<IActionResult> GetUnassignedStudent(int page, [FromQuery] FilterModel reqModel)
        {
            var res = await _studentServices.GetUnassignedStudents(page, reqModel);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPut("students/status")]
        public async Task<IActionResult> EditStatusStudentInBatch([FromBody] List<StudentUpdateStatusResModel> Form)
        {
            Data.Models.ResultModel.ResultModel result = await _studentServices.UpdateStudentsByStatus(Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("students/export")]
        [Authorize(Roles = "SUAdmin, Admin")]
        public async Task<IActionResult> ExportStudentsToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Students");
                var currentRow = 1;
                var students = await _studentRepository.GetStudents();
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Full Name";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "DoB";
                worksheet.Cell(currentRow, 5).Value = "Address";
                worksheet.Cell(currentRow, 6).Value = "Gender";
                worksheet.Cell(currentRow, 7).Value = "Phone";
                worksheet.Cell(currentRow, 8).Value = "Status";

                foreach (var student in students)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = student.Id.ToString();
                    worksheet.Cell(currentRow, 2).Value = student.FullName;
                    worksheet.Cell(currentRow, 3).Value = student.Email;
                    worksheet.Cell(currentRow, 4).Value = student.Dob.ToString();
                    worksheet.Cell(currentRow, 5).Value = student.Address;
                    worksheet.Cell(currentRow, 6).Value = student.Gender;
                    worksheet.Cell(currentRow, 7).Value = student.Phone;
                    worksheet.Cell(currentRow, 8).Value = student.Status;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx");
                }
            }
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPut("students/{id}")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] StudentUpdateResModel Form)
        {
            Data.Models.ResultModel.ResultModel result = await _studentServices.UpdateStudent(Form, id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Trainer, Student")]
        [HttpGet]
        [Route("students/{id}/scores")]
        public async Task<IActionResult> GetFinalResult(Guid id)
        {
            var result = await _studentServices.GetScoreListByStudentId(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPut]
        [Route("students/{id}/{moduleId}/scores")]
        public async Task<IActionResult> UpdateStudentScoreList(Guid id, Guid moduleId, [FromBody] UpdateStudentScoreListReqModel reqModel)
        {
            var res = await _studentServices.UpdateStudentScoreListInModule(reqModel, id, moduleId);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet]
        [Route("students/{id}/admin-trainer")]
        public async Task<IActionResult> GetAdminAndTrainerOfStudent(Guid id)
        {
            var res = await _studentServices.GetAdminAndTrainerOfStudent(id);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
    }
}