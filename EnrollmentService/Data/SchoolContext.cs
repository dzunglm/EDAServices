using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EnrollmentService.Data
{
    public partial class SchoolContext : DbContext
    {
        public SchoolContext()
        {
        }

        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Enrollment> Enrollments { get; set; } = null!;
        public virtual DbSet<Learner> Learners { get; set; } = null!;
        public virtual DbSet<Major> Majors { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=(local);Initial Catalog=School;Persist Security Info=True;User ID=sa;Password=haynho;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");

                entity.Property(e => e.CourseId)
                    .ValueGeneratedNever()
                    .HasColumnName("CourseID");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollment");

                entity.HasIndex(e => e.CourseId, "IX_Enrollment_CourseID");

                entity.HasIndex(e => e.LearnerId, "IX_Enrollment_LearnerID");

                entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.LearnerId).HasColumnName("LearnerID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId);

                entity.HasOne(d => d.Learner)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.LearnerId);
            });

            modelBuilder.Entity<Learner>(entity =>
            {
                entity.ToTable("Learner");

                entity.HasIndex(e => e.MajorId, "IX_Learner_MajorID");

                entity.Property(e => e.LearnerId).HasColumnName("LearnerID");

                entity.Property(e => e.MajorId).HasColumnName("MajorID");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.Learners)
                    .HasForeignKey(d => d.MajorId);
            });

            modelBuilder.Entity<Major>(entity =>
            {
                entity.ToTable("Major");

                entity.Property(e => e.MajorId).HasColumnName("MajorID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
