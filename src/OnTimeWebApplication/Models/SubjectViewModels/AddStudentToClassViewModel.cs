using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Models.SubjectViewModels
{
    public class AddStudentToClassViewModel
    {
        public List<Student> Students { get; set; }
        public List<Student> StudentInClass { get; set; }
        public Subject Subject { get; set; }
    }
}
