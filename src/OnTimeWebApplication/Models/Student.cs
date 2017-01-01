using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnTimeWebApplication.Models
{
    public class Student
    {
        [StringLength(maximumLength: 10, MinimumLength = 10)]
        public string Id { get; set; }

        public string AccountId { get; set; }

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        [StringLength(maximumLength: 10, MinimumLength = 10)]
        public string Tel { get; set; }

        public ApplicationUser Account { get; set; }

        public List<SubjectStudent> SubjectStudents { get; set; }
    }
}
