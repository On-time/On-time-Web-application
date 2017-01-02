using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        [StringLength(maximumLength: 10, MinimumLength = 10)]
        public string Tel { get; set; }

        public ApplicationUser Account { get; set; }

        public List<SubjectStudent> SubjectStudents { get; set; }
    }
}
