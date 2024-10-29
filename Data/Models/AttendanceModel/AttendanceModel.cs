namespace Data.Models.AttendanceModel
{
    public class AttendanceReqModel
    {
        public int AddMinute { get; set; }
        public int? AddLateTime { get; set; }
    }

    public class CheckAttendanceReqModel
    {
        public string Code { get; set; } = null!;
    }

    public class AttendanceResModel
    {
        public Guid AttendanceId { get; set; }
        public AttendanceClassModel Class { get; set; } = null!;
        public string AttendanceStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsCheckAttendance { get; set; }
    }

    public class AttendanceClassModel
    {
        public Guid Id { get; set; }
        public string ClassName { get; set; } = null!;
    }

    public class AttendanceListOfClassModel
    {
        public Guid AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = null!;
        public int numberOfStudents { get; set; }
        public int numberOfPresentStudent { get; set; }
    }
}