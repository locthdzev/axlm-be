namespace Data.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public Guid CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Attachment { get; set; }

        public string Status { get; set; } = null!;

        public virtual User CreatedByNavigation { get; set; } = null!;
    }
}