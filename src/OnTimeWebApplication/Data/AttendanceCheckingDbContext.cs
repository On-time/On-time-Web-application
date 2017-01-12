using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnTimeWebApplication.Models;

namespace OnTimeWebApplication.Data
{
    public class AttendanceCheckingDbContext : DbContext
    {
        public DbSet<Attendance> Attendance { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        public AttendanceCheckingDbContext(DbContextOptions<AttendanceCheckingDbContext> options) : base(options) { }
    }
}
