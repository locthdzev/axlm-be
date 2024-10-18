namespace Data.Entities
{
    public class StudentClass
    {
        public Guid Id { get; set; }

        public Guid ClassId { get; set; }

        public Guid StudentId { get; set; }

        public string Status { get; set; } = null!;

        public virtual Class Class { get; set; } = null!;

        public virtual User Student { get; set; } = null!;
    }
}