using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnTimeWebApplication.Models
{
    public class Subject
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [StringLength(maximumLength: 7, MinimumLength = 7)]
        public string Id { get; set; }

        public byte Section { get; set; }

        public Lecturer Lecturer { get; set; }
        [Required]
        public string TeacherId { get; set; }

        public List<SubjectStudent> SubjectStudents { get; set; }
    }
}
