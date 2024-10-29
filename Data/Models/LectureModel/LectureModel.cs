using Microsoft.AspNetCore.Http;

namespace Data.Models.LectureModel
{
    public class LectureReqModel
    {
        public int Order { get; set; }
        public Guid ModuleId { get; set; }
        public Guid ClassId { get; set; }
        public List<IFormFile>? Files { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class LectureResModel
    {
        public Guid? Id { get; set; }
        public int? Order { get; set; }
        public Guid? ModuleId { get; set; }
        public Guid? ClassId { get; set; }
        public string? Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public AuthorLectureResModel? CreatedBy { get; set; }
        public string? Status { get; set; }
    }

    public class LectureDetailResModel
    {
        public Guid? Id { get; set; }
        public int? Order { get; set; }
        public Guid? ModuleId { get; set; }
        public Guid? ClassId { get; set; }
        public string? Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public AuthorLectureResModel? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AuthorLectureResModel? UpdatedBy { get; set; }
        public string? Status { get; set; }
    }

    public class AuthorLectureResModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
    }

    public class LectureUpdateModel
    {
        public string Name { get; set; } = null!;
        public List<IFormFile>? Files { get; set; }
    }
}