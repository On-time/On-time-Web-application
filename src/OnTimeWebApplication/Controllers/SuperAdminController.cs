using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnTimeWebApplication.Data;
using Microsoft.AspNetCore.Authorization;

namespace OnTimeWebApplication.Controllers
{
    [Authorize(Roles = Constant.SuperAdminRoleName)]
    public class SuperAdminController : Controller
    {
        private ApplicationDbContext _context;

        public SuperAdminController(ApplicationDbContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<string> DeleteAllAttendance()
        {
            _context.Attendance.RemoveRange(_context.Attendance);
            await _context.SaveChangesAsync();

            return "all of the records were deleted";
        }
    }
}