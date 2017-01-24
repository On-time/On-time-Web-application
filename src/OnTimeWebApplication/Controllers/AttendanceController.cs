using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnTimeWebApplication.Data;
using OnTimeWebApplication.Models;
using OnTimeWebApplication.Models.AttendanceViewModels;
using Microsoft.AspNetCore.Authorization;
using OnTimeWebApplication.Services;

namespace OnTimeWebApplication.Controllers
{
    [Authorize(Policy = Constant.StudentAndLecturer)]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Attendance
        public async Task<IActionResult> CurrentlyPerformCheckingList()
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var checkingServices = AttendanceCheckingService.Current.Select(d => d.Key).ToList();
            var subjects = new List<Subject>();

            foreach (var pair in checkingServices)
            {
                subjects.Add(await _context.Subjects.Where(s => s.Id == pair.Item1 && s.Section == pair.Item2).FirstOrDefaultAsync());
            }

            return View(subjects);
        }

        // GET: Attendance/Details/5
        public async Task<IActionResult> Details(string id, byte? section)
        {
            if (id == null || section == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.AsNoTracking().Where(s => s.Id == id && s.Section == section).FirstOrDefaultAsync();

            if (subject == null)
            {
                return NotFound();
            }

            var attendances = await _context.Attendance
                .Include(a => a.Student)
                .Where(a => a.SubjectId == id && a.SubjectSection == section)
                .ToListAsync();
            
            List<string> studentIds = attendances.Select(a => a.StudentId).ToList();

            var studentInClass = await _context.SubjectStudents.AsNoTracking().Include(ss => ss.Student)
                .Where(ss => ss.SubjectId == id && !studentIds.Contains(ss.StudentId)).Select(ss => ss.Student).ToListAsync();

            var allStudents = attendances.Select(a => new CheckingStudentViewModel { Id = a.StudentId, Name = a.Student.FullName, AttendState = a.AttendState }).ToList();
            allStudents.AddRange(studentInClass.Select(sic => new CheckingStudentViewModel { Id = sic.Id, Name = sic.FullName, AttendState = AttendState.NotComeYet }));

            return View(new CurrentCheckingViewModel { Subject = subject, Students = allStudents });
        }

        // GET: Attendance/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id");
            return View();
        }

        // POST: Attendance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectId,SubjectSection,StudentId,AttendedTime,AttendState")] Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", attendance.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", attendance.SubjectId);
            return View(attendance);
        }

        // GET: Attendance/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendance.SingleOrDefaultAsync(m => m.SubjectId == id);
            if (attendance == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", attendance.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", attendance.SubjectId);
            return View(attendance);
        }

        // POST: Attendance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SubjectId,SubjectSection,StudentId,AttendedTime,AttendState")] Attendance attendance)
        {
            if (id != attendance.SubjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceExists(attendance.SubjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", attendance.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", attendance.SubjectId);
            return View(attendance);
        }

        // GET: Attendance/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendance
                .Include(a => a.Student)
                .Include(a => a.Subject)
                .SingleOrDefaultAsync(m => m.SubjectId == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var attendance = await _context.Attendance.SingleOrDefaultAsync(m => m.SubjectId == id);
            _context.Attendance.Remove(attendance);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AttendanceExists(string id)
        {
            return _context.Attendance.Any(e => e.SubjectId == id);
        }
    }
}
