namespace Data.Entities
{
    public class Document
    {
        public Guid Id { get; set; }

        public Guid LectureId { get; set; }

        public string FileName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public string Status { get; set; } = null!;

        public virtual User CreatedByNavigation { get; set; } = null!;

        public virtual Lecture Lecture { get; set; } = null!;

        public virtual User? UpdatedByNavigation { get; set; }
    }
}