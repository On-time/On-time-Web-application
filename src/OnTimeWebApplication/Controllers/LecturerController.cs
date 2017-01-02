using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnTimeWebApplication.Data;
using Microsoft.AspNetCore.Identity;
using OnTimeWebApplication.Models;
using OnTimeWebApplication.Models.LecturerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace OnTimeWebApplication.Controllers
{
    public class LecturerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly AppRoleManager _roleManager;
        private readonly ILogger _logger;

        public LecturerController(
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

        #region Lecturer list
        // GET: /Account/LecturerList
        [HttpGet]
        [Authorize(Policy = Constant.AdministratorOnly)]
        public async Task<IActionResult> LecturerList()
        {
            var lecturerList = await _context.Lecturers.AsNoTracking().ToListAsync();
            var viewModel = lecturerList.Select(lec => new LecturerViewModel { Id = lec.Id, FirstName = lec.FirstName, LastName = lec.LastName }).ToList();

            return View(viewModel);
        }

        //
        // GET: /Account/RegisterLecturer
        [HttpGet]
        [Authorize(Policy = Constant.AdministratorOnly)]
        public IActionResult RegisterLecturer(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        //
        // POST: /Account/RegisterLecturer
        [HttpPost]
        [Authorize(Policy = Constant.AdministratorOnly)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterLecturer(RegisterLecturerViewModel viewModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = viewModel.Username, Email = viewModel.Email };
                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, Constant.LecturerRoleName);
                    if (result.Succeeded)
                    {
                        var ss = await _context.Lecturers.AddAsync(new Lecturer { FirstName = viewModel.FirstName, LastName = viewModel.LastName, AccountId = user.Id });
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
        // GET: /Lecturer/Delete
        [HttpGet]
        [ActionName("Delete")]
        [Authorize(Policy = Constant.AdministratorOnly)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return StatusCode(404);
            }

            Lecturer lecturer = await _context.Lecturers.FindAsync(id);

            if (lecturer == null)
            {
                return StatusCode(404);
            }

            return View(lecturer);
        }


        //
        // POST: /Lecturer/Delete
        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Policy = Constant.AdministratorOnly)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lecturer = await _context.Lecturers.Include(s => s.Account).Where(l => l.Id == id).FirstOrDefaultAsync();

            if (lecturer == null)
            {
                return RedirectToAction(nameof(LecturerController.LecturerList));
            }

            _context.Users.Remove(lecturer.Account);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(LecturerController.LecturerList));
        }
        #endregion

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
                return RedirectToAction(nameof(LecturerController.LecturerList), "Lecturer");
            }
        }

        #endregion
    }
}