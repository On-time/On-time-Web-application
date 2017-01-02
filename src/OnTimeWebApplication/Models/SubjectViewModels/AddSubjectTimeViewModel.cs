using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Models.SubjectViewModels
{
    public class AddSubjectTimeViewModel
    {
        // subject section
        [Display(Name = "รหัสวิชา")]
        [StringLength(maximumLength: 7, MinimumLength = 7)]
        public string Id { get; set; }

        public byte Section { get; set; }

        public string Name { get; set; }

        // subjecttime section
        [Required]
        [Display(Name = "ชั่วโมง")]
        public byte StartHr { get; set; }
        [Required]
        [Display(Name = "นาที")]
        public byte StartMin { get; set; }

        [Required]
        [Display(Name = "ชั่วโมง")]
        public byte EndHr { get; set; }
        [Required]
        [Display(Name = "นาที")]
        public byte EndMin { get; set; }

        [Required]
        [Display(Name = "วัน")]
        public DayOfWeek DayOfWeek { get; set; }
    }
}
