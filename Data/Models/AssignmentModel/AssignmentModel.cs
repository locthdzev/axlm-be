using Data.Entities;
using Microsoft.AspNetCore.Http;

namespace Data.Models.AssignmentModel
{
    public class ModuleAssignmentResModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuthorAssignmentResModel? CreatedBy { get; set; }
        public List<AssignmentOfModuleResModel>? Assignments { get; set; } = new List<AssignmentOfModuleResModel>();
        public string? Status { get; set; }
    }

    public class AuthorAssignmentResModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }

    public class AssignmentOfModuleResModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        public bool IsOverTime { get; set; }
        public List<AssignmentDetailsResModel>? Details { get; set; } = new List<AssignmentDetailsResModel>();
        public DateTime CreatedAt { get; set; }
        public AuthorAssignmentResModel? CreatedBy { get; set; }
        public string? SubmissionStatus { get; set; }
        public int? NumberOfSubmissions { get; set; }
    }

    public class AssignmentDetailsResModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public string Status { get; set; } = null!;
    }

    public class AssignmentCreateReqModel
    {
        public Guid ModuleId { get; set; }
        public Guid ClassId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        public List<IFormFile>? Files { get; set; }
        public bool IsOverTime { get; set; }
    }

    public class AssignmentUpdateReqModel
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        public List<IFormFile>? Files { get; set; }
        public bool IsOverTime { get; set; }
    }

    public class AssignmentDetails
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class CreatorAssignmentModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
    }

    public class UpdatorAssignmentModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
    }

    public class AssignmentInformationResModel
    {
        public Guid Id { get; set; }

        public Guid ModuleId { get; set; }

        public Guid ClassId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime? ExpiryDate { get; set; }

        public bool IsOverTime { get; set; }

        public List<AssignmentDetails> AssignmentDetails { get; set; }

        public DateTime CreatedAt { get; set; }

        public CreatorAssignmentModel CreatedBy { get; set; } = null!;

        public DateTime? UpdatedAt { get; set; }

        public UpdatorAssignmentModel? UpdatedBy { get; set; } = null!;

        public string Status { get; set; } = null!;
    }

    // public class ExcelTemplateModel
    // {
    //     public Class classEntity { get; set; }
    //     public Assignment assignment { get; set; }
    //     public List<SubmissionCheckModel> checks { get; set; }
    // }

    // public class SubmissionCheckModel
    // {
    //     public User? Student { get; set; }
    //     public bool isSubmit { get; set; }
    //     public string? FileName { get; set; }
    //     public decimal? Score { get; set; }
    // }

    public class ScoreModel
    {
        public string email { get; set; }
        public decimal score { get; set; }
        public string isSubmit { get; set; }
    }

    public class IdScoreSubmitModel
    {
        public Guid studentId { get; set; }
        public decimal score { get; set; }
        public string IsSubmit { get; set; }
    }

    public class SubmissionCreateResModel
    {     
        public IFormFile Attachment { get; set; } = null!;
    }

    public class ScoreAssignmentResModel
    {
        public Guid StudentId { get; set; }

        public decimal Score { get; set; }
    }

    public class AssignmentUpdateScoreReqModel
    {
        public List<UpdateScoreReqModel> ScoresData { get; set; }
    }

    public class UpdateScoreReqModel
    {
        public Guid StudentId { get; set; }
        public decimal Score { get; set; }
    }

    // public class SubmissionResModel
    // {
    //     public Guid Id { get; set; }

    //     public AuthorOfSubmission? Student { get; set; }

    //     public Guid AssignmentId { get; set; }

    //     public string FileName { get; set; } = null!;

    //     public bool IsGrade { get; set; }

    //     public decimal Score { get; set; }

    //     public DateTime CreatedAt { get; set; }

    //     public string Status { get; set; } = null!;
    // }

    // public class AuthorOfSubmission
    // {
    //     public Guid Id { get; set; }
    //     public string Name { get; set; } = null!;
    // }
}