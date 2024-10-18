namespace Data.Entities
{
    public class Class
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Guid ProgramId { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public string Location { get; set; } = null!;

        public string Status { get; set; } = null!;

        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        public virtual ICollection<ClassManager> ClassManagers { get; set; } = new List<ClassManager>();

        public virtual ICollection<ClassTrainer> ClassTrainers { get; set; } = new List<ClassTrainer>();

        public virtual ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();

        public virtual TrainingProgram Program { get; set; } = null!;

        public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
    }
}