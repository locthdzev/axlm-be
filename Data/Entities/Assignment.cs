using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Assignment
    {
        public Guid Id { get; set; }

        public Guid ModuleId { get; set; }

        public Guid ClassId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime? ExpiryDate { get; set; }

        [Column(TypeName = "ulong")]
        public bool IsOverTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public string Status { get; set; } = null!;

        public virtual ICollection<AssignmentDetail> AssignmentDetails { get; set; } = new List<AssignmentDetail>();

        public virtual Class Class { get; set; } = null!;

        public virtual User CreatedByNavigation { get; set; } = null!;

        public virtual Module Module { get; set; } = null!;

        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

        public virtual User? UpdatedByNavigation { get; set; }
    }
}