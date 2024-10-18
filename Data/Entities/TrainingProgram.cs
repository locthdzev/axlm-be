namespace Data.Entities
{
    public class TrainingProgram
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public int Duration { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedBy { get; set; }

        public string Status { get; set; } = null!;

        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

        public virtual User CreatedByNavigation { get; set; } = null!;

        public virtual ICollection<ModuleProgram> ModulePrograms { get; set; } = new List<ModuleProgram>();

        public virtual User? UpdatedByNavigation { get; set; }
    }
}