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
    [Authorize(Roles = Constant.SuperAdminRoleName)]
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
        public async Task<IActionResult> RegisterStudent(string username, string password, string tel)
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

                if (student.Tel == tel)
                {
                    return Json(new { status = "register student completed" });
                }
                else
                {
                    return Json(new { status = "register student fail, tel not match" });
                }
            }
            else
            {
                return Json(new { status = "username or password invalid" });
            }
        }

        [HttpPost("lecturer")]
        public async Task<IActionResult> RegisterLecturer(string username, string password, string tel)
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

                if (lecturer.Tel == tel)
                {
                    return Json(new { status = "register lecturer completed" });
                }
                else
                {
                    return Json(new { status = "register lecturer fail, tel not match" });
                }
            }
            else
            {
                return Json(new { status = "username or password invalid" });
            }
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