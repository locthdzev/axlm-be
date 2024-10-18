namespace Data.Entities
{
    public class AttendanceDetail
    {
        public Guid Id { get; set; }

        public Guid AttendanceId { get; set; }

        public Guid StudentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = null!;

        public virtual Attendance Attendance { get; set; } = null!;

        public virtual User Student { get; set; } = null!;
    }
}