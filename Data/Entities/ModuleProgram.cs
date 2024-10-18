namespace Data.Entities
{
    public class ModuleProgram
    {
        public Guid Id { get; set; }

        public Guid ModuleId { get; set; }

        public Guid ProgramId { get; set; }

        public string Status { get; set; } = null!;

        public virtual Module Module { get; set; } = null!;

        public virtual TrainingProgram Program { get; set; } = null!;
    }
}