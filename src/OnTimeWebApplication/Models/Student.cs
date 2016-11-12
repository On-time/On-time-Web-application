using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnTimeWebApplication.Models
{
    public class Student
    {
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public List<SubjectStudent> SubjectStudents { get; set; }
    }
}
