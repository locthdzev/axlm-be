namespace Data.Entities
{
    public class Module
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public string Status { get; set; } = null!;

        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

        public virtual User CreatedByNavigation { get; set; } = null!;

        public virtual ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();

        public virtual ICollection<ModuleProgram> ModulePrograms { get; set; } = new List<ModuleProgram>();

        public virtual User? UpdatedByNavigation { get; set; }
    }
}