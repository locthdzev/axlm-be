using Data.Entities;

namespace Data.Models.SubmissionModel
{
    public class ListStudentWithSubmission
    {
        public AuthorOfSubmission AuthorOfSubmission { get; set; }
        public SubmissionSpecificResModel? SubmissionSpecificResModel { get; set; }
    }

    public class AuthorOfSubmission
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
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

    public class SubmissionCheckModel
    {
        public User? Student { get; set; }
        public bool isSubmit { get; set; }
        public string? FileName { get; set; }
        public decimal? Score { get; set; }
    }
}