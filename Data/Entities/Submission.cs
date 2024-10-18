using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Submission
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid AssignmentId { get; set; }

        public string AttachmentUrl { get; set; } = null!;

        public decimal Score { get; set; }

        [Column(TypeName = "ulong")]
        public bool IsGrade { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = null!;

        public virtual Assignment Assignment { get; set; } = null!;

        public virtual User Student { get; set; } = null!;
    }
}