using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnTimeWebApplication.Models;

namespace OnTimeWebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectStudent> SubjectStudents { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
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
        }
    }
}
