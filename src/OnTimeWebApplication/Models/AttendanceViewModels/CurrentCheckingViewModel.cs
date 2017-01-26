using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Models.AttendanceViewModels
{
    public class CurrentCheckingViewModel
    {
        public List<CheckingStudentViewModel> Students { get; set; }

        public Subject Subject { get; set; }

        public SubjectTime SubjectTime { get; set; }
    }
}
