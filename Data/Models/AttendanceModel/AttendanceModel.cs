namespace Data.Models.AttendanceModel
{
    public class AttendanceListOfClassModel
    {
        public Guid AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = null!;
        public int numberOfStudents { get; set; }
        public int numberOfPresentStudent { get; set; }
    }
}