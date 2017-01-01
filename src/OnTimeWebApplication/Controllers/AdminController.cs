using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OnTimeWebApplication.Models;
using OnTimeWebApplication.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using OnTimeWebApplication.Models.AdminViewModels;
using Microsoft.EntityFrameworkCore;

namespace OnTimeWebApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly AppRoleManager _roleManager;
        private readonly ILogger _logger;

        public AdminController(
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

        #region Admin manager
        // GET: /Admin/AdminList
        [HttpGet]
        [Authorize(Roles = Constant.SuperAdminRoleName)]
        public async Task<IActionResult> AdminList()
        {
            var adminList = await _userManager.GetUsersInRoleAsync(Constant.AdminRoleName);
            var viewModel = adminList.Select(a => new AdminViewModel { Id = a.Id, UserName = a.UserName }).ToList();

            return View(viewModel);
        }

        //
        // GET: /Admin/RegisterAdmin
        [HttpGet]
        [Authorize(Roles = Constant.SuperAdminRoleName)]
        public IActionResult RegisterAdmin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        //
        // POST: /Admin/RegisterAdmin
        [HttpPost]
        [Authorize(Roles = Constant.SuperAdminRoleName)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterAdminViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, Constant.AdminRoleName);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(3, "User admin created a new account with password.");
                        return RedirectToLocal(returnUrl);
                    }
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Admin/Delete
        [HttpGet]
        [ActionName("Delete")]
        [Authorize(Roles = Constant.SuperAdminRoleName)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return StatusCode(404);
            }

            AdminViewModel admin = await _context.Users.Where(u => u.Id == id).Select(u => new AdminViewModel { UserName = u.UserName, Id = u.Id }).FirstOrDefaultAsync();

            if (admin == null)
            {
                return StatusCode(404);
            }

            return View(admin);
        }


        //
        // POST: /Admin/Delete
        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = Constant.SuperAdminRoleName)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var admin = await _context.Users.FindAsync(id);

            if (admin == null)
            {
                return RedirectToAction(nameof(AdminController.AdminList));
            }

            _context.Users.Remove(admin);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AdminController.AdminList));
        }
        #endregion

        #region Helpers

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
                return RedirectToAction(nameof(AdminController.AdminList), "Admin");
            }
        }
        #endregion
    }
}