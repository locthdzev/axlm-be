using Data.Models.UserModel;

namespace Data.Models.ClassModel
{
    public class ClassInformationResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ClassProgramDetail ProgramDetails { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public CreatorClassModel CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CreatorClassModel? UpdatedBy { get; set; }
        public int NumberOfStudents { get; set; }
        public List<UserResModel>? ListStudent { get; set; }
        public string Location { get; set; } = null!;
        public string Status { get; set; } = null!;
        public List<ClassManagerAndTrainer>? classManagerAndTrainers { get; set; }
    }

    public class ClassProgramDetail
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public CreatorClassModel CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CreatorClassModel? UpdatedBy { get; set; }
        public string Status { get; set; } = null!;
    }

    public class ClassManagerAndTrainer
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class ClassListResModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public CreatorClassModel CreatedBy { get; set; } = null!;

        public string Location { get; set; } = null!;

        public string Status { get; set; } = null!;

    }

    public class CreatorClassModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
    }

    public class ClassInformationOfStudent
    {
        public ClassInformationOfOthers ClassInformationOfOthers { get; set; } = null!;

        public AdminOfCLass AdminOfCLass { get; set; } = null!;
    }

    public class AdminOfCLass
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
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

    public class ClassOfStudentModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public CreatorClassOfStudentModel CreatedBy { get; set; } = null!;

        public DateTime? UpdatedAt { get; set; }

        public UpdatorClassOfStudentModel? UpdatedBy { get; set; }

        public int NumberOfStudents { get; set; }

        public List<UserResModel>? ListStudent { get; set; }

        public string Location { get; set; } = null!;

        public string Status { get; set; } = null!;

        public ClassOfStudentProgramDetails ProgramDetails { get; set; }

        public List<ClassManagerAndTrainer>? classManagerAndTrainers { get; set; }
    }

    public class CreatorClassOfStudentModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
    }

    public class UpdatorClassOfStudentModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class ClassOfStudentProgramDetails
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public CreatorClassOfStudentProgramModel CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CreatorClassOfStudentProgramModel? UpdatedBy { get; set; }
        public string Status { get; set; } = null!;
    }

    public class CreatorClassOfStudentProgramModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class NoneTrainerClassListModel
    {
        public Guid ClassId { get; set; }
        public string? ClassName { get; set; }
        public Guid AdminId { get; set; }
        public string? AdminName { get; set; }
        public string? AdminEmail { get; set; }
    }

    public class ClassReqModel
    {
        public string Name { get; set; } = null!;
        public Guid ProgramId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Location { get; set; } = null!;
    }

    public class FilterDayModel
    {
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string? searchValue { get; set; }
    }

    public class ClassUpdateReqModel
    {
        public string Name { get; set; } = null!;
        public Guid ProgramId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Location { get; set; } = null!;
        public string Status { get; set; } = null!;

        public DateTime UpdatedAt { get; } = DateTime.UtcNow;
        public Guid UpdatedBy { get; }

        public ClassUpdateReqModel() { }

        public ClassUpdateReqModel(Guid updatedBy)
        {
            UpdatedBy = updatedBy;
        }
    }
}