namespace Data.Entities
{
    public class EmailRequest
    {
        public Guid Id { get; set; }

        public Guid RecipientId { get; set; }

        public string Subject { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Attachment { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public string Status { get; set; } = null!;

        public virtual User CreatedByNavigation { get; set; } = null!;

        public virtual User Recipient { get; set; } = null!;

        public virtual ICollection<RequestReply> RequestReplies { get; set; } = new List<RequestReply>();
    }
}