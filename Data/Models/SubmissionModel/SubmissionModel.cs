using Data.Entities;
using Microsoft.AspNetCore.Http;

namespace Data.Models.SubmissionModel
{
    public class ScoreAssignmentResModel
    {
        public Guid StudentId { get; set; }

        public decimal Score { get; set; }
    }

    public class UpdateStudentScoreListReqModel
    {
        public List<decimal> NewScoreList { get; set; }
    }

    public class SubmissionCreateResModel
    {     
        public IFormFile Attachment { get; set; } = null!;
    }

    public class SubmissionResModel
    {
        public Guid Id { get; set; }

        public AuthorOfSubmission? Student { get; set; }

        public Guid AssignmentId { get; set; }

        public string FileName { get; set; } = null!;

        public bool IsGrade { get; set; }

        public decimal Score { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = null!;
    }

    public class SubmissionSpecificResModel
    {
        public Guid Id { get; set; }

        public Guid AssignmentId { get; set; }

        public string FileName { get; set; } = null!;

        public bool IsGrade { get; set; }

        public decimal Score { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = null!;
    }

    public class AuthorOfSubmission
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class ListStudentWithSubmission
    {
        public AuthorOfSubmission AuthorOfSubmission { get; set; }
        public SubmissionSpecificResModel? SubmissionSpecificResModel { get; set; }
    }

    public class SubmissionCheckModel
    {
        public User? Student { get; set; }
        public bool isSubmit { get; set; }
        public string? FileName { get; set; }
        public decimal? Score { get; set; }
    }

    public class BasicAssignmentModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal? Score { get; set; }
        public string? Status { get; set; }
    }
    public class ModuleAssignmentModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public List<BasicAssignmentModel>? Assignments { get; set; }
        public decimal? ModuleAvgScore { get; set; }

    }
    public class StudentScoreModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ModuleAssignmentModel>? StudentScores { get; set; }
        public decimal? GPA { get; set; }

    }

    public class ModuleListAvgScore
    {
        public Guid ModuleId { get; set; }
        public string? Code { get; set; }
        public List<StudentListInfo>? studentListInfos { get; set; }
    }

    public class StudentListInfo
    {
        public Guid StudentId { get; set; }
        public string? Name { get; set; }
        public decimal? AvgScore { get; set; }
    }

    public class SubmissionUpdateResModel
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid AssignmentId { get; set; }

        public string FileName { get; set; } = null!;

        public bool IsGrade { get; set; }

        public decimal Score { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = null!;
    }
}