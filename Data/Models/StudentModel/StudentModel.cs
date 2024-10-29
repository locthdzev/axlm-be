namespace Data.Models.StudentModel
{
    public class StudentCreateReqModel
    {
        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Password { get; set; } = null!;

        public DateTime Dob { get; set; }

        public string Address { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string Phone { get; set; } = null!;
    }

    public class StudentUpdateResModel
    {
        public DateTime Dob { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; } = null!;
    }

    public class StudentUpdateStatusResModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }

    public class UpdateStudentScoreListReqModel
    {
        public List<decimal> NewScoreList { get; set; }
    }
}