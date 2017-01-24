using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Models.SubjectViewModels
{
    public class CreateSubjectViewModel
    {
        [Required]
        [Display(Name = "ชื่อวิชา")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "รหัสวิชา")]
        [StringLength(maximumLength: 7, MinimumLength = 7)]
        public string Id { get; set; }

        [Required]
        [Range(1, 127, ErrorMessage = "Section ต้องอยู่ระหว่าง 1 ถึง 127")]
        public byte Section { get; set; }

        [Required]
        [Display(Name = "เวลาก่อนเข้าสาย(นาที)")]
        public int LateTime { get; set; }

        [Display(Name = "เวลาก่อนขาดเรียน(นาที)")]
        [Required]
        public int AbsentTime { get; set; }

        [Display(Name = "เช็คชื่อแม้ขาดหรือไม่")]
        public bool UseComeAbsent { get; set; }

        [Display(Name = "ชื่ออาจารย์ผู้สอน")]
        public string LecturerId { get; set; }

    }
}
