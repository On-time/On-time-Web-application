using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OnTimeWebApplication.Models;
using OnTimeWebApplication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OnTimeWebApplication.Models.StudentViewModels;
using Microsoft.Extensions.Logging;

namespace OnTimeWebApplication.Controllers
{
    public class StudentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly AppRoleManager _roleManager;
        private readonly ILogger _logger;

        public StudentController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext dbContext,
            AppRoleManager roleManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = dbContext;
            _roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        // GET: /Student/StudentList
        [HttpGet]
        [Authorize(Policy = Constant.AdministratorOnly)]
        public async Task<IActionResult> StudentList()
        {
            var lecturerList = await _context.Students.AsNoTracking().ToListAsync();
            var viewModel = lecturerList.Select(lec => new StudentViewModel { Id = lec.Id, FirstName = lec.FirstName, LastName = lec.LastName }).ToList();

            return View(viewModel);
        }

        //
        // GET: /Student/RegisterStudent
        [HttpGet]
        [Authorize(Policy = Constant.AdministratorOnly)]
        public IActionResult RegisterStudent(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        //
        // POST: /Student/RegisterStudent
        [HttpPost]
        [Authorize(Policy = Constant.AdministratorOnly)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudent(RegisterStudentViewModel viewModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = viewModel.Id, Email = viewModel.Email };
                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, Constant.StudentRoleName);
                    if (result.Succeeded)
                    {
                        var ss = await _context.Students.AddAsync(new Student { FirstName = viewModel.FirstName, LastName = viewModel.LastName, AccountId = user.Id, Id = viewModel.Id });
                        var re = await _context.SaveChangesAsync();
                        _logger.LogInformation(3, "User lecturer created a new account with password.");
                        return RedirectToLocal(returnUrl);
                    }
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }


        //
        // GET: /Student/Delete
        [HttpGet]
        [ActionName("Delete")]
        [Authorize(Policy = Constant.AdministratorOnly)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return StatusCode(404);
            }

            Student student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return StatusCode(404);
            }

            return View(student);
        }


        //
        // POST: /Student/Delete
        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Policy = Constant.AdministratorOnly)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _context.Students.Include(s => s.Account).Where(s => s.Id == id).FirstOrDefaultAsync();

            if (student == null)
            {
                return RedirectToAction(nameof(StudentController.StudentList));
            }

            _context.Users.Remove(student.Account);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(StudentController.StudentList));
        }

        #region helper method
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(StudentController.StudentList), "Student");
            }
        }

        #endregion
    }
}