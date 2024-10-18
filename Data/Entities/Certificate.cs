namespace Data.Entities
{
    public class Certificate
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        public Guid ProgramId { get; set; }

        public string CertificateAttachment { get; set; } = null!;

        public string Status { get; set; } = null!;

        public virtual TrainingProgram Program { get; set; } = null!;

        public virtual User Student { get; set; } = null!;
    }
}