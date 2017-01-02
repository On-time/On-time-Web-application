using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnTimeWebApplication.Models
{
    public class SubjectTime
    {
        [StringLength(maximumLength: 7, MinimumLength = 7)]
        public string SubjectId { get; set; }

        public byte Section { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime Start { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime End { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public Subject Subject { get; set; }
    }
}
