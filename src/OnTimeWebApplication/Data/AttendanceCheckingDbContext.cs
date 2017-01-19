using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnTimeWebApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OnTimeWebApplication.Data
{
    public class AttendanceCheckingDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Attendance> Attendance { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<SubjectTime> SubjectTimes { get; set; }

        public AttendanceCheckingDbContext(DbContextOptions<AttendanceCheckingDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Subject>()
                .HasKey(s => new { s.Id, s.Section });

            builder.Entity<Student>()
                .HasIndex(s => s.AccountId);

            builder.Entity<SubjectStudent>()
                .HasKey(ss => new { ss.StudentId, ss.SubjectId });

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

            builder.Entity<Attendance>()
                .HasKey(a => new { a.SubjectId, a.SubjectSection, a.StudentId, a.AttendedTime });

            builder.Entity<Attendance>()
                .HasOne<Subject>(a => a.Subject)
                .WithMany(navigationName: null)
                .HasForeignKey(a => new { a.SubjectId, a.SubjectSection });

            builder.Entity<Attendance>()
                .HasOne<Student>(a => a.Student)
                .WithMany(navigationName: null)
                .HasForeignKey(a => a.StudentId);
        }
    }

    public class EFCoreOptions
    {
        public string ConnectionString { get; set; }
    }
}
