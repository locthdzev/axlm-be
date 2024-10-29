using CertificateAPIServices.Services;
using Data.Models.CertificateModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertificateAPIServices.Controllers
{
    [ApiController]
    [Route("api/certificate-management")]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateServices _certificateServices;

        public CertificateController(ICertificateServices certificateServices)
        {
            _certificateServices = certificateServices;
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPost("certificates")]
        public async Task<IActionResult> AddCertificate([FromForm] CertificateModel reqModel)
        {
            var res = await _certificateServices.AddCertificate(reqModel);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpPut("certificates/{id}")]
        public async Task<IActionResult> UpdateCertificateStatus(Guid id, string newStatus)
        {
            var res = await _certificateServices.UpdateCertificateStatus(id, newStatus);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }

        [Authorize(Roles = "SUAdmin, Admin")]
        [HttpGet("certificates")]
        public async Task<IActionResult> GetStudentCertificateList()
        {
            Data.Models.ResultModel.ResultModel result = await _certificateServices.GetStudentCertificateList();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}