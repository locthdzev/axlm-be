namespace Data.Models.ClassModel
{
    public class FilterDayModel
    {
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string? searchValue { get; set; }
    }

    public class ClassResModelDisplayInProgram
    {
        public Guid id { get; set; }
        public string Name { get; set; }
        public int numberOfStudent { get; set; }
    }

    public class ModuleLectureResModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuthorModuleResModel? CreatedBy { get; set; }
        public List<LectureOfModuleResModel> Lectures { get; set; } = new List<LectureOfModuleResModel>();
        public string? Status { get; set; }
    }

    public class AuthorModuleResModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
    }

    public class LectureOfModuleResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<LectureDocumentModel> Documents { get; set; } = new List<LectureDocumentModel>();
        public DateTime CreatedAt { get; set; }
        public AuthorModuleResModel? CreatedBy { get; set; }
    }

    public class LectureDocumentModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public AuthorModuleResModel? CreatedBy { get; set; }
    }
}