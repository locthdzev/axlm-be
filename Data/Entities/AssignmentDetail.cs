namespace Data.Entities
{
    public class AssignmentDetail
    {
        public Guid Id { get; set; }

        public Guid AssignmentId { get; set; }

        public string AttachmentUrl { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = null!;

        public virtual Assignment Assignment { get; set; } = null!;
    }
}