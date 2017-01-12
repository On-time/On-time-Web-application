using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Models
{
    public class Attendance
    {
        [Display(Name = "รหัสวิชา")]
        [StringLength(maximumLength: 7, MinimumLength = 7)]
        public string SubjectId { get; set; }

        public byte SubjectSection { get; set; }

        [StringLength(maximumLength: 10, MinimumLength = 10)]
        public string StudentId { get; set; }

        public DateTime AttendedTime { get; set; }

        public AttendState AttendState { get; set; }

        public Student Student { get; set; }

        public Subject Subject { get; set; }
    }

    public enum AttendState
    {
        [Display(Name = "มาตรงเวลา")]
        InTime,
        [Display(Name = "มาสาย")]
        Late,
        [Display(Name = "ขาดเรียน")]
        Absent,
        [Display(Name = "ขาดเรียนแต่เข้าห้องเรียน")]
        AttendButAbsent,
        [Display(Name = "ยังไม่มา")]
        NotComeYet
    }
}
