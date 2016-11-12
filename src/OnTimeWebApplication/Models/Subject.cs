using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnTimeWebApplication.Models
{
    public class Subject
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Id { get; set; }

        [Required]
        public string Section { get; set; }

        public List<SubjectStudent> SubjectStudents { get; set; }
    }
}
