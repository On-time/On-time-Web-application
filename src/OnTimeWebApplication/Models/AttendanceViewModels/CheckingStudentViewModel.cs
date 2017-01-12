using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Models.AttendanceViewModels
{
    public class CheckingStudentViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AttendState AttendState { get; set; }
    }
}
