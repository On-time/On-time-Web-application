using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OnTimeWebApplication.Models
{
    public class Lecturer
    {
        public string Id { get; set; }

        public string AccountId { get; set; }

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        public ApplicationUser Account { get; set; }

        public List<Subject> Subjects { get; set; }
    }
}
