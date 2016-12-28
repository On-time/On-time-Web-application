using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnTimeWebApplication.Models
{
    public class Subject
    {
        [Required]
        public string Name { get; set; }

        public string Id { get; set; }

        public byte Section { get; set; }

        public Teacher Teacher { get; set; }
        [Required]
        public string TeacherId { get; set; }

        public List<SubjectStudent> SubjectStudents { get; set; }
    }
}
