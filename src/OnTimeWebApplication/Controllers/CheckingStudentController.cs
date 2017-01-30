using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnTimeWebApplication.Services;
using OnTimeWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using OnTimeWebApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace OnTimeWebApplication.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/checking")]
    public class CheckingStudentController : Controller
    {
        [HttpGet("testget")]
        public IActionResult sendTest()
        {
            return Json(new { status = "ok" });
        }

        [HttpPost("checkstudents")]
        public async Task<IActionResult> CheckStudents([FromBody]CheckingBatchData data)
        {
            if (data == null)
            {
                return Json(new { status = "no data sent" });
            }

            AttendanceCheckingService checkingService = null;
            var getResult = AttendanceCheckingService.Current.TryGetValue(new Tuple<string, byte>(data.SubjectId, data.SubjectSection), out checkingService);

            if (!getResult)
            {
                return Json(new { status = "not yet" });
            }

            await checkingService.CheckStudents(data.Students);

            return Json(new { status = "ok" });
        }
    }

    [Produces("application/json")]
    [Route("api/register")]
    public class OnPhoneRegisterController : Controller
    {
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _context;

        public OnPhoneRegisterController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("student")]
        public async Task<IActionResult> RegisterStudent(string username, string password, string androidId)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                await _signInManager.SignOutAsync();
                var studentAcc = await _userManager.FindByNameAsync(username);

                if (studentAcc == null)
                {
                    // normally, this shouldn't be null
                    return Json(new { status = "student not found" });
                }

                var student = await _context.Students.Where(s => s.AccountId == studentAcc.Id).FirstOrDefaultAsync();

                if (student == null)
                {
                    // normally, this shouldn't be null
                    return Json(new { status = "student not found" });
                    
                }
                else
                {
                    student.AndroidId = androidId;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { status = "register student completed" });
                }
            }
            else
            {
                return Json(new { status = "username or password invalid" });
            }
        }

        [HttpPost("lecturer")]
        public async Task<IActionResult> RegisterLecturer(string username, string password, string androidId)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                await _signInManager.SignOutAsync();
                var lecturerAcc = await _userManager.FindByNameAsync(username);

                if (lecturerAcc == null)
                {
                    // normally, this shouldn't be null
                    return Json(new { status = "lecturer not found" });
                }

                var lecturer = await _context.Lecturers.Where(s => s.AccountId == lecturerAcc.Id).FirstOrDefaultAsync();

                if (lecturer == null)
                {
                    // normally, this shouldn't be null
                    return Json(new { status = "lecturer not found" });
                }
                else
                {
                    lecturer.AndroidId = androidId;
                    await _context.SaveChangesAsync();
                    var studentDatas = await GetStudentDatas(lecturer.Id);
                    var subjectDatas = await GetSubjectDatas(lecturer.Id);
                    var subjectStudentDatas = await GetSubjectStudentDatas(studentDatas.Select(sd => sd.Id).ToList());

                    return Json(new { status = "register lecturer completed", Students = studentDatas, Subjects = subjectDatas, SubjectStudents = subjectStudentDatas });
                }
            }
            else
            {
                return Json(new { status = "username or password invalid" });
            }
        }

        private async Task<SimpleStudent[]> GetStudentDatas(string lecturerId)
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var subjectIds = await _context.Subjects.Where(s => s.LecturerId == lecturerId).Select(s => s.Id).ToListAsync();
            var students = await _context.SubjectStudents.Where(ss => subjectIds.Contains(ss.SubjectId)).Select(ss => new SimpleStudent(ss.StudentId, ss.Student.AndroidId)).ToArrayAsync();
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            return students;
        }

        private async Task<SimpleSubject[]> GetSubjectDatas(string lecturerId)
        {
            var subjects = await _context.Subjects.AsNoTracking().Where(s => s.LecturerId == lecturerId).Select(s => new SimpleSubject(s.Id, s.Section)).ToArrayAsync();

            return subjects;
        }

        private async Task<SimpleSubjectStudent[]> GetSubjectStudentDatas(List<string> studentIds)
        {
            var subjectStudents = await _context.SubjectStudents.AsNoTracking()
                .Where(ss => studentIds.Contains(ss.StudentId))
                .Select(ss => new SimpleSubjectStudent(ss.StudentId, ss.SubjectId, ss.SubjectSection))
                .ToArrayAsync();

            return subjectStudents;
        }

        private class SimpleStudent
        {
            public SimpleStudent(string id, string androidId)
            {
                Id = id;
                AndroidId = androidId;
            }

            public string Id { get; private set; }
            public string AndroidId { get; private set; }
        }

        private class SimpleSubject
        {
            public SimpleSubject(string id, byte section)
            {
                Id = id;
                Section = section;
            }

            public string Id { get; private set; }
            public byte Section { get; private set; }
        }

        private class SimpleSubjectStudent
        {
            public SimpleSubjectStudent(string studentId, string subjectId, byte subjectSection)
            {
                StudentId = studentId;
                SubjectId = subjectId;
                SubjectSection = subjectSection;
            }

            public string StudentId { get; private set; }
            public string SubjectId { get; private set; }
            public byte SubjectSection { get; private set; }
        }
    }

    public class CheckingBatchData
    {
        public string SubjectId { get; set; }

        public byte SubjectSection { get; set; }

        public CheckingStudentData[] Students { get; set; }
    }

    public class CheckingStudentData
    {
        public string Id { get; set; }
        public DateTime AttendTime { get; set; }
    }
}