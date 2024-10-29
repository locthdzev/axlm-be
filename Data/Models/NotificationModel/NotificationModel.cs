using Microsoft.AspNetCore.Http;

namespace Data.Models.NotificationModel
{
    public class NotifcationUpdateModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? File { get; set; }
    }

    public class NotificationCreateReqModel
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile? File { get; set; }
    }

    public class NotificationReqModel { }

    public class NotificationResModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } 

        public string Description { get; set; } 

        public AuthorNotificationResModel CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Attachment { get; set; }

        public string Status { get; set; } 
    }
    public class AuthorNotificationResModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}