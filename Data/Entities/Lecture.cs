namespace Data.Entities
{
    public class Lecture
    {
        public Guid Id { get; set; }

        public int Order { get; set; }

        public Guid ModuleId { get; set; }

        public Guid ClassId { get; set; }

        public string Name { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public string Status { get; set; } = null!;

        public virtual Class Class { get; set; } = null!;

        public virtual User CreatedByNavigation { get; set; } = null!;

        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        public virtual Module Module { get; set; } = null!;

        public virtual User? UpdatedByNavigation { get; set; }
    }
}