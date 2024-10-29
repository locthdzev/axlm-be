namespace Data.Models.ModuleModel
{
    public class ModuleReqModel
    {
        public string? Name { get; set; } = null!;
        public string? Code { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class ModuleInfoCopyModel
    {
        public Guid ProgramId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
    }

    public class ModuleReqCreateAndAddToTP
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }

    public class ModuleGeneralResModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuthorModuleResModel? CreatedBy { get; set; }
        public string? Status { get; set; }

    }

    public class ModuleDetailsResModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public AuthorModuleResModel? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuthorModuleResModel? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalLecture { get; set; }
        public string? Status { get; set; }
    }

    public class AuthorModuleResModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
    }

    public class OtherModuleListRes
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? numOfProgram { get; set; }
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

    public class ModuleUpdateModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}