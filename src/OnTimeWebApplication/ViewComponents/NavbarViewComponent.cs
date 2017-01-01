using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OnTimeWebApplication.Data;
using OnTimeWebApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private UserManager<ApplicationUser> _userManager;
        private AppRoleManager _roleManager;

        public NavbarViewComponent(UserManager<ApplicationUser> userManager, AppRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync(UserClaimsPrincipal);

            if (currentUser == null)
            {
                return View(new NavbarViewModel());
            }

            IList<string> roleList = await _userManager.GetRolesAsync(currentUser);

            // if this user have no roles then return
            if (roleList.Count == 0)
            {
                return View(new NavbarViewModel());
            }

            return View(new NavbarViewModel { RoleNames = (List<string>)roleList });
        }
    }

    public class NavbarViewModel
    {
        public List<string> RoleNames { get; set; }
    }
}
