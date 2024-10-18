namespace Data.Entities
{
    public class ClassTrainer
    {
        public Guid Id { get; set; }

        public Guid ClassId { get; set; }

        public Guid UserId { get; set; }

        public string Status { get; set; } = null!;

        public virtual Class Class { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}