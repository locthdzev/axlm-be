namespace Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public byte[] Password { get; set; } = null!;

        public DateTime Dob { get; set; }

        public string Address { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public byte[] Salt { get; set; } = null!;

        public string Status { get; set; } = null!;

        public virtual ICollection<Assignment> AssignmentCreatedByNavigations { get; set; } = new List<Assignment>();

        public virtual ICollection<Assignment> AssignmentUpdatedByNavigations { get; set; } = new List<Assignment>();

        public virtual ICollection<AttendanceDetail> AttendanceDetails { get; set; } = new List<AttendanceDetail>();

        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

        public virtual ICollection<ClassManager> ClassManagers { get; set; } = new List<ClassManager>();

        public virtual ICollection<ClassTrainer> ClassTrainers { get; set; } = new List<ClassTrainer>();

        public virtual ICollection<Document> DocumentCreatedByNavigations { get; set; } = new List<Document>();

        public virtual ICollection<Document> DocumentUpdatedByNavigations { get; set; } = new List<Document>();

        public virtual ICollection<EmailRequest> EmailRequestCreatedByNavigations { get; set; } = new List<EmailRequest>();

        public virtual ICollection<EmailRequest> EmailRequestRecipients { get; set; } = new List<EmailRequest>();

        public virtual ICollection<Lecture> LectureCreatedByNavigations { get; set; } = new List<Lecture>();

        public virtual ICollection<Lecture> LectureUpdatedByNavigations { get; set; } = new List<Lecture>();

        public virtual ICollection<Module> ModuleCreatedByNavigations { get; set; } = new List<Module>();

        public virtual ICollection<Module> ModuleUpdatedByNavigations { get; set; } = new List<Module>();

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public virtual ICollection<OtpVerify> Otpverifies { get; set; } = new List<OtpVerify>();

        public virtual ICollection<RequestReply> RequestReplies { get; set; } = new List<RequestReply>();

        public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();

        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

        public virtual ICollection<TrainingProgram> TrainingProgramCreatedByNavigations { get; set; } = new List<TrainingProgram>();

        public virtual ICollection<TrainingProgram> TrainingProgramUpdatedByNavigations { get; set; } = new List<TrainingProgram>();
    }
}