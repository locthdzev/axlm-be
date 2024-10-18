namespace Data.Entities
{
    public class RequestReply
    {
        public Guid Id { get; set; }

        public Guid RequestId { get; set; }

        public string ReplyContent { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public string Status { get; set; } = null!;

        public virtual User CreatedByNavigation { get; set; } = null!;

        public virtual EmailRequest Request { get; set; } = null!;
    }
}