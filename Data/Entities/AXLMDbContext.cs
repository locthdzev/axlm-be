using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Entities
{
    public class AXLMDbContext : DbContext
    {
        public AXLMDbContext() { }

        public AXLMDbContext(DbContextOptions<AXLMDbContext> options)
        : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        public virtual DbSet<Assignment> Assignments { get; set; }

        public virtual DbSet<AssignmentDetail> AssignmentDetails { get; set; }

        public virtual DbSet<Attendance> Attendances { get; set; }

        public virtual DbSet<AttendanceDetail> AttendanceDetails { get; set; }

        public virtual DbSet<Certificate> Certificates { get; set; }

        public virtual DbSet<Class> Classes { get; set; }

        public virtual DbSet<ClassManager> ClassManagers { get; set; }

        public virtual DbSet<ClassTrainer> ClassTrainers { get; set; }

        public virtual DbSet<Document> Documents { get; set; }

        public virtual DbSet<EmailRequest> EmailRequests { get; set; }

        public virtual DbSet<Lecture> Lectures { get; set; }

        public virtual DbSet<Module> Modules { get; set; }

        public virtual DbSet<ModuleProgram> ModulePrograms { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<OtpVerify> OtpVerifies { get; set; }

        public virtual DbSet<RequestReply> RequestReplies { get; set; }

        public virtual DbSet<StudentClass> StudentClasses { get; set; }

        public virtual DbSet<Submission> Submissions { get; set; }

        public virtual DbSet<TrainingProgram> TrainingPrograms { get; set; }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Assignment");

                entity.ToTable("Assignment");

                entity.HasIndex(e => e.ClassId, "ClassId");

                entity.HasIndex(e => e.CreatedBy, "CreatedBy");

                entity.HasIndex(e => e.ModuleId, "ModuleId");

                entity.HasIndex(e => e.UpdatedBy, "UpdatedBy");

                entity.Property(e => e.Content).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
                entity.Property(e => e.IsOverTime).HasColumnType("bit");
                entity.Property(e => e.Status).HasMaxLength(30);
                entity.Property(e => e.Title).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Class).WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Assignment_ibfk_4");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AssignmentCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Assignment_ibfk_1");

                entity.HasOne(d => d.Module).WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Assignment_ibfk_3");

                entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AssignmentUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Assignment_ibfk_2");
            });

            modelBuilder.Entity<AssignmentDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_AssignmentDetail");

                entity.HasIndex(e => e.AssignmentId, "AssignmentId");

                entity.Property(e => e.AttachmentUrl)
                    .HasMaxLength(512)
                    .HasColumnName("AttachmentURL");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.Assignment).WithMany(p => p.AssignmentDetails)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("AssignmentDetails_ibfk_1");
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Attendance");

                entity.ToTable("Attendance");

                entity.HasIndex(e => e.ClassId, "ClassId");

                entity.Property(e => e.Code).HasMaxLength(4);
                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.DueTo).HasColumnType("datetime");
                entity.Property(e => e.LateTo).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.Class).WithMany(p => p.Attendances)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Attendance_ibfk_1");
            });

            modelBuilder.Entity<AttendanceDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_AttendanceDetail");

                entity.HasIndex(e => e.AttendanceId, "AttendanceId");

                entity.HasIndex(e => e.StudentId, "StudentId");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.Attendance).WithMany(p => p.AttendanceDetails)
                    .HasForeignKey(d => d.AttendanceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("AttendanceDetails_ibfk_1");

                entity.HasOne(d => d.Student).WithMany(p => p.AttendanceDetails)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("AttendanceDetails_ibfk_2");
            });

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Certificate");

                entity.ToTable("Certificate");

                entity.HasIndex(e => e.ProgramId, "ProgramId");

                entity.HasIndex(e => e.StudentId, "StudentId");

                entity.Property(e => e.CertificateAttachment).HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.Program).WithMany(p => p.Certificates)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Certificate_ibfk_2");

                entity.HasOne(d => d.Student).WithMany(p => p.Certificates)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Certificate_ibfk_1");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Class");

                entity.ToTable("Class");

                entity.HasIndex(e => e.ProgramId, "ProgramId");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.EndAt).HasColumnType("datetime");
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.StartAt).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(30);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Program).WithMany(p => p.Classes)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Class_ibfk_1");
            });

            modelBuilder.Entity<ClassManager>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_ClassManager");

                entity.ToTable("Class_Manager");

                entity.HasIndex(e => e.ClassId, "ClassId");

                entity.HasIndex(e => e.UserId, "UserId");

                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.Class).WithMany(p => p.ClassManagers)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Class_Manager_ibfk_1");

                entity.HasOne(d => d.User).WithMany(p => p.ClassManagers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Class_Manager_ibfk_2");
            });

            modelBuilder.Entity<ClassTrainer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_ClassTrainer");

                entity.ToTable("Class_Trainer");

                entity.HasIndex(e => e.ClassId, "ClassId");

                entity.HasIndex(e => e.UserId, "UserId");

                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.Class).WithMany(p => p.ClassTrainers)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Class_Trainer_ibfk_1");

                entity.HasOne(d => d.User).WithMany(p => p.ClassTrainers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Class_Trainer_ibfk_2");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Document");

                entity.HasIndex(e => e.CreatedBy, "CreatedBy");

                entity.HasIndex(e => e.LectureId, "LectureId");

                entity.HasIndex(e => e.UpdatedBy, "UpdatedBy");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.FileName).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(30);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DocumentCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Documents_ibfk_1");

                entity.HasOne(d => d.Lecture).WithMany(p => p.Documents)
                    .HasForeignKey(d => d.LectureId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Documents_ibfk_3");

                entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DocumentUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Documents_ibfk_2");
            });

            modelBuilder.Entity<EmailRequest>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_EmailRequest");

                entity.ToTable("Email_Request");

                entity.HasIndex(e => e.CreatedBy, "CreatedBy");

                entity.HasIndex(e => e.RecipientId, "RecipientId");

                entity.Property(e => e.Attachment).HasMaxLength(256);
                entity.Property(e => e.Content).HasMaxLength(4028);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(30);
                entity.Property(e => e.Subject).HasMaxLength(256);

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EmailRequestCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Email_Request_ibfk_1");

                entity.HasOne(d => d.Recipient).WithMany(p => p.EmailRequestRecipients)
                    .HasForeignKey(d => d.RecipientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Email_Request_ibfk_2");
            });

            modelBuilder.Entity<Lecture>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Lecture");

                entity.ToTable("Lecture");

                entity.HasIndex(e => e.ClassId, "ClassId");

                entity.HasIndex(e => e.CreatedBy, "CreatedBy");

                entity.HasIndex(e => e.ModuleId, "ModuleId");

                entity.HasIndex(e => e.UpdatedBy, "UpdatedBy");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Order).HasColumnType("int");
                entity.Property(e => e.Status).HasMaxLength(30);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Class).WithMany(p => p.Lectures)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Lecture_ibfk_2");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.LectureCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Lecture_ibfk_3");

                entity.HasOne(d => d.Module).WithMany(p => p.Lectures)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Lecture_ibfk_1");

                entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.LectureUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Lecture_ibfk_4");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Module");

                entity.ToTable("Module");

                entity.HasIndex(e => e.CreatedBy, "CreatedBy");

                entity.HasIndex(e => e.UpdatedBy, "UpdatedBy");

                entity.Property(e => e.Code).HasMaxLength(30);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(30);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModuleCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Module_ibfk_2");

                entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ModuleUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Module_ibfk_3");
            });

            modelBuilder.Entity<ModuleProgram>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_ModuleProgram");

                entity.ToTable("Module_Program");

                entity.HasIndex(e => e.ModuleId, "ModuleId");

                entity.HasIndex(e => e.ProgramId, "ProgramId");

                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.Module).WithMany(p => p.ModulePrograms)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Module_Program_ibfk_2");

                entity.HasOne(d => d.Program).WithMany(p => p.ModulePrograms)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Module_Program_ibfk_1");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Notification");

                entity.ToTable("Notification");

                entity.HasIndex(e => e.CreatedBy, "CreatedBy");

                entity.Property(e => e.Attachment).HasMaxLength(256);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(8192);
                entity.Property(e => e.Status).HasMaxLength(30);
                entity.Property(e => e.Title).HasMaxLength(256);

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Notification_ibfk_1");
            });

            modelBuilder.Entity<OtpVerify>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_OtpVerify");

                entity.ToTable("OTPVerify");

                entity.HasIndex(e => e.UserId, "UserId");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
                entity.Property(e => e.OtpCode).HasMaxLength(6);

                entity.HasOne(d => d.User).WithMany(p => p.Otpverifies)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("OTPVerify_ibfk_1");
            });

            modelBuilder.Entity<RequestReply>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_RequestReply");

                entity.ToTable("Request_Reply");

                entity.HasIndex(e => e.CreatedBy, "CreatedBy");

                entity.HasIndex(e => e.RequestId, "RequestId");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.ReplyContent).HasMaxLength(4028);
                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RequestReplies)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Request_Reply_ibfk_1");

                entity.HasOne(d => d.Request).WithMany(p => p.RequestReplies)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Request_Reply_ibfk_2");
            });

            modelBuilder.Entity<StudentClass>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_StudentClass");

                entity.ToTable("Student_Class");

                entity.HasIndex(e => e.ClassId, "ClassId");

                entity.HasIndex(e => e.StudentId, "StudentId");

                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.Class).WithMany(p => p.StudentClasses)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Student_Class_ibfk_1");

                entity.HasOne(d => d.Student).WithMany(p => p.StudentClasses)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Student_Class_ibfk_2");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Submission");

                entity.HasIndex(e => e.AssignmentId, "AssignmentId");

                entity.HasIndex(e => e.StudentId, "StudentId");

                entity.Property(e => e.AttachmentUrl)
                    .HasMaxLength(512)
                    .HasColumnName("AttachmentURL");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.IsGrade).HasColumnType("bit");
                entity.Property(e => e.Score).HasPrecision(5);
                entity.Property(e => e.Status).HasMaxLength(30);

                entity.HasOne(d => d.Assignment).WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Submissions_ibfk_2");

                entity.HasOne(d => d.Student).WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Submissions_ibfk_1");
            });

            modelBuilder.Entity<TrainingProgram>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_TrainingProgram");

                entity.ToTable("Training_Program");

                entity.HasIndex(e => e.CreatedBy, "CreatedBy");

                entity.HasIndex(e => e.UpdatedBy, "UpdatedBy");

                entity.Property(e => e.Code).HasMaxLength(30);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Duration).HasColumnType("int");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(30);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TrainingProgramCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Training_Program_ibfk_1");

                entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TrainingProgramUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Training_Program_ibfk_2");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_User");

                entity.ToTable("User");

                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.Dob)
                    .HasColumnType("datetime")
                    .HasColumnName("DOB");
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.Password).HasMaxLength(512);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Role).HasMaxLength(15);
                entity.Property(e => e.Salt).HasMaxLength(512);
                entity.Property(e => e.Status).HasMaxLength(30);
            });
        }
    }
}
