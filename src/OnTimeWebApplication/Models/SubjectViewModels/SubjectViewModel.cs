using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Models.SubjectViewModels
{
    public class SubjectViewModel
    {
        [Required]
        [Display(Name = "ชื่อวิชา")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Display(Name = "รหัสวิชา")]
        [StringLength(maximumLength: 7, MinimumLength = 7)]
        public string Id { get; set; }

        public byte Section { get; set; }

        [Required]
        [Display(Name = "เวลาก่อนเข้าสาย(นาที)")]
        public TimeSpan LateTime { get; set; }

        [Required]
        [Display(Name = "เวลาก่อนขาดเรียน(นาที)")]
        public TimeSpan AbsentTime { get; set; }

        [Display(Name = "เช็คชื่อแม้ขาดหรือไม่")]
        public bool UseComeAbsent { get; set; }

        public string LecturerId { get; set; }

        public Lecturer Lecturer { get; set; }

        public List<SubjectTime> SubjectTimes { get; set; }

        public List<Student> Students { get; set; }
    }
}
