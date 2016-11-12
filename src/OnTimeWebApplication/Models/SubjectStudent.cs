using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Models
{
    public class SubjectStudent
    {
        public string StudentId { get; set; }
        public Student Student { get; set; }

        public string SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
