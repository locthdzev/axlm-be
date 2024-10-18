namespace Data.Entities
{
    public class Attendance
    {
        public Guid Id { get; set; }

        public Guid ClassId { get; set; }

        public DateTime Date { get; set; }

        public DateTime DueTo { get; set; }

        public DateTime LateTo { get; set; }

        public string Code { get; set; } = null!;

        public string Status { get; set; } = null!;

        public virtual ICollection<AttendanceDetail> AttendanceDetails { get; set; } = new List<AttendanceDetail>();

        public virtual Class Class { get; set; } = null!;
    }
}