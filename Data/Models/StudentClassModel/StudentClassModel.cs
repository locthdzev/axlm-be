namespace Data.Models.StudentClassModel
{
    public class UserResModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime Dob { get; set; }

        public string Address { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Status { get; set; } = null!;
    }

    public class ClassInformationOfStudent
    {
        public ClassInformationOfOthers ClassInformationOfOthers { get; set; } = null!;

        public AdminOfCLass AdminOfCLass { get; set; } = null!;
    }

    public class ClassInformationOfOthers
    {
        public Guid ClassId { get; set; }

        public string ClassName { get; set; } = null!;

        public Guid ProgramId { get; set; }

        public string Program { get; set; } = null!;

        public DateTime? StartAt { get; set; } = null!;

        public DateTime? EndAt { get; set; } = null!;

        public string Location { get; set; } = null!;

        public Guid? TrainerId { get; set; }

        public string? Trainer { get; set; } 

        public Guid? AdminId { get; set; }

        public string? Admin { get; set; } 
    }

    public class AdminOfCLass
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class ModuleAssignmentModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public List<BasicAssignmentModel>? Assignments { get; set; }
        public decimal? ModuleAvgScore { get; set; }
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

    public class NameAndEmailModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class StudentScoreModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ModuleAssignmentModel>? StudentScores { get; set; }
        public decimal? GPA { get; set; }
    }
}