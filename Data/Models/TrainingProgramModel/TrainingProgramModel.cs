using System.Text.Json.Serialization;

namespace Data.Models.TrainingProgramModel
{
    public class TrainingProgramReqModel
    {
        public string? Name { get; set; } = null!;
        public string? Code { get; set; } = null!;
        public int Duration { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public Guid CreatedBy { get; set; }
    }

    public class TrainingProgramResModel
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Duration { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        public Guid UpdatedBy { get; set; }
        public string Status { get; set; }
    }

    public class TrainingProgramListResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Duration { get; set; }
        public AuthorResModel CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalClass { get; set; }
        public string Status { get; set; }
    }

    public class TrainingProgramDetailsResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Duration { get; set; }
        public AuthorResModel CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuthorResModel? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalClass { get; set; }
        public string Status { get; set; }
        public List<ClassResModelDisplayInProgram>? ClassList { get; set; }
    }

    public class ClassResModelDisplayInProgram
    {
        public Guid id { get; set; }
        public string Name { get; set; }
        public int numberOfStudent { get; set; }
    }

    public class AuthorResModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
    }

    public class TrainingProgramDropDownResModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
    }
}