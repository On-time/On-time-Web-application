using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnTimeWebApplication.Models;
using OnTimeWebApplication.Models.AccountViewModels;
using OnTimeWebApplication.Models.StudentViewModels;
using Microsoft.EntityFrameworkCore.Metadata;
using OnTimeWebApplication.Models.LecturerViewModels;
using OnTimeWebApplication.Models.AdminViewModels;

namespace OnTimeWebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<SubjectTime> SubjectTimes { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectStudent> SubjectStudents { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Student> Students { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Subject>()
                .HasKey(s => new { s.Id, s.Section });
            builder.Entity<SubjectStudent>()
                .HasKey(ss => new { ss.StudentId, ss.SubjectId });

            builder.Entity<SubjectStudent>()
                .HasOne<Subject>(ss => ss.Subject)
                .WithMany(s => s.SubjectStudents)
                .HasForeignKey(ss => new { ss.SubjectId, ss.SubjectSection });

            builder.Entity<SubjectStudent>()
                .HasOne<Student>(ss => ss.Student)
                .WithMany(s => s.SubjectStudents)
                .HasForeignKey(ss => ss.StudentId);

            builder.Entity<Subject>()
                .HasOne<Lecturer>(s => s.Lecturer)
                .WithMany(l => l.Subjects)
                .HasForeignKey(s => s.LecturerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Lecturer>()
                .HasIndex(l => l.AccountId);

            builder.Entity<Lecturer>()
                .HasOne<ApplicationUser>(l => l.Account)
                .WithOne(navigationName: null)
                .HasForeignKey<Lecturer>(l => l.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Entity<Student>()
                .HasIndex(s => s.AccountId);

            builder.Entity<Student>()
                .HasOne<ApplicationUser>(s => s.Account)
                .WithOne(navigationName: null)
                .HasForeignKey<Student>(s => s.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Entity<SubjectTime>()
                .HasKey(st => new { st.SubjectId, st.Section, st.DayOfWeek });

            builder.Entity<SubjectTime>()
                .HasOne<Subject>(st => st.Subject)
                .WithMany(s => s.SubjectTimes)
                .HasForeignKey(st => new { st.SubjectId, st.Section })
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }

        public DbSet<RegisterStudentViewModel> RegisterStudentViewModel { get; set; }
    }
}
