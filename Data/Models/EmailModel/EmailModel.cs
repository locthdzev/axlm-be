using Microsoft.AspNetCore.Http;

namespace Data.Models.EmailModel
{
    public class EmailCreateReqModel
    {
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public IFormFile? Files { get; set; }
    }

    public class EmailResModel
    {
        public Guid Id { get; set; }

        public RecipientModel Recipient { get; set; }

        public string Subject { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Attachment { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public CreatorEmailModel CreatedBy { get; set; }

        public string Status { get; set; } = null!;
    }

    public class EmailReplyResModel
    {
        public Guid Id { get; set; }

        public Guid RequestId { get; set; }

        public string ReplyContent { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public string Status { get; set; } = null!;
    }

    public class RecipientModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;
    }

    public class CreatorEmailModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class EmailGeneralResModel
    {
        public Guid Id { get; set; }

        public Guid RecipientId { get; set; }

        public string Subject { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Attachment { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public string Status { get; set; } = null!;

        public bool HasReply { get; set; }

        public DateTime LatestReply { get; set; }
    }

    public class ReplyResModel
    {
        public string ReplyContent { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public SenderEmailModel? Sender { get; set; }
    }

    public class EmailResReceivedModel
    {
        public Guid Id { get; set; }

        public string Subject { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Attachment { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public SenderEmailModel Sender { get; set; }

        public string Status { get; set; } = null!;
    }

    public class SenderEmailModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}