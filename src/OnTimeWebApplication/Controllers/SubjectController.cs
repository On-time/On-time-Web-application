using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using OnTimeWebApplication.Data;
using OnTimeWebApplication.Models;
using OnTimeWebApplication.Models.SubjectViewModels;
using OnTimeWebApplication.Services;
using OnTimeWebApplication.Utilities;

namespace OnTimeWebApplication.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Subject
        public async Task<IActionResult> SubjectList()
        {
            var applicationDbContext = _context.Subjects.Include(s => s.Lecturer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(string id, byte? section)
        {
            if (id == null || section == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Lecturer)
                .Include(s => s.SubjectTimes)
                .FirstOrDefaultAsync(s => s.Id == id && s.Section == section);

            if (subject == null)
            {
                return NotFound();
            }

            var viewModel = new SubjectViewModel
            {
                Id = subject.Id,
                Section = subject.Section,
                Name = subject.Name,
                LateTime = subject.LateTime,
                AbsentTime = subject.AbsentTime,
                Lecturer = subject.Lecturer,
                SubjectTimes = subject.SubjectTimes,
                UseComeAbsent = subject.UseComeAbsent
            };

            var students = await _context.SubjectStudents
                .Where(ss => ss.SubjectId == id && ss.SubjectSection == section)
                .Select(ss => ss.Student)
                .ToListAsync();

            viewModel.Students = students;

            if (subject == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: Subject/Create
        public IActionResult CreateSubject()
        {
            ViewData["LecturerId"] = new SelectList(_context.Lecturers, "Id", "FullName");
            return View();
        }

        // POST: Subject/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubject(CreateSubjectViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var subject = new Subject
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                    Section = viewModel.Section,
                    UseComeAbsent = viewModel.UseComeAbsent,
                    LecturerId = viewModel.LecturerId,
                    LateTime = TimeSpan.FromMinutes(viewModel.LateTime),
                    AbsentTime = TimeSpan.FromMinutes(viewModel.AbsentTime)
                };

                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction("SubjectList");
            }
            ViewData["LecturerId"] = new SelectList(_context.Lecturers, "Id", "FullName", viewModel.LecturerId);
            return View(viewModel);
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(string id, byte? section)
        {
            if (id == null || section == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.SingleOrDefaultAsync(m => m.Id == id && m.Section == section);

            if (subject == null)
            {
                return NotFound();
            }

            ViewData["LecturerId"] = new SelectList(_context.Lecturers, "Id", "FullName", subject.LecturerId);

            return View(subject);
        }

        // POST: Subject/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Id,Section,LateTime,AbsentTime,UseComeAbsent,LecturerId")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Need 623 attention
                    if (!SubjectExists(subject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("SubjectList");
            }
            ViewData["LecturerId"] = new SelectList(_context.Lecturers, "Id", "Id", subject.LecturerId);
            return View(subject);
        }

        // GET: Subject/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Lecturer)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var subject = await _context.Subjects.SingleOrDefaultAsync(m => m.Id == id);
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return RedirectToAction("SubjectList");
        }

        [HttpGet]
        public async Task<IActionResult> AddSubjectTime(string id, byte? section)
        {
            if (id == null || section == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && s.Section == section);
            ViewData["HourClock"] = new SelectList(Enumerable.Range(1, 24));
            ViewData["MinuteClock"] = new SelectList(Enumerable.Range(0, 60));

            var viewModel = new AddSubjectTimeViewModel
            {
                Id = subject.Id,
                Name = subject.Name,
                Section = subject.Section
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSubjectTime(AddSubjectTimeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!await SubjectExistsAsync(viewModel.Id, viewModel.Section))
                {
                    return NotFound();
                }

                var subjectTime = new SubjectTime
                {
                    SubjectId = viewModel.Id,
                    Section = viewModel.Section,
                    Start = new DateTime(1970, 1, 1, viewModel.StartHr, viewModel.StartMin, 0),
                    End = new DateTime(1970, 1, 1, viewModel.EndHr, viewModel.EndMin, 0),
                    DayOfWeek = viewModel.DayOfWeek
                };

                _context.SubjectTimes.Add(subjectTime);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    RecurringJob.AddOrUpdate<AttendanceCheckingService>(a => a.AddCurrentChecking(subjectTime.SubjectId, subjectTime.Section, subjectTime.DayOfWeek),
                        CronExUtil.CreateCron(new DateTime[] { subjectTime.Start }, new DayOfWeek[] { subjectTime.DayOfWeek }), TimeZoneInfo.Local);
                    RecurringJob.AddOrUpdate(() => AttendanceCheckingService.RemoveCurrentChecking(subjectTime.SubjectId, subjectTime.Section),
                        CronExUtil.CreateCron(new DateTime[] { subjectTime.End }, new DayOfWeek[] { subjectTime.DayOfWeek }), TimeZoneInfo.Local);

                    return RedirectToAction(nameof(SubjectController.Details), new { id = viewModel.Id, section = viewModel.Section });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Can't add subject time to {viewModel.Name} subject.");

                    return View(viewModel);
                }
            }

            ViewData["HourClock"] = new SelectList(Enumerable.Range(1, 24));
            ViewData["MinuteClock"] = new SelectList(Enumerable.Range(0, 60));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubjectTime(string id, byte section, DayOfWeek dayOfWeek)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectTime = await _context.SubjectTimes
                .Where(st => st.SubjectId == id && st.Section == section && st.DayOfWeek == dayOfWeek).FirstOrDefaultAsync();

            if (subjectTime == null)
            {
                return NotFound();
            }

            _context.SubjectTimes.Remove(subjectTime);
            await _context.SaveChangesAsync();

            return Json(new { dayOfWeek = Enum.GetName(typeof(DayOfWeek), dayOfWeek) });
        }

        [HttpGet]
        public async Task<IActionResult> AddStudentToSubject(string id, byte? section)
        {
            var subject = await _context.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id && s.Section == section);            
            var studentInClasses = await _context.SubjectStudents.AsNoTracking()
                .Include(ss => ss.Student).Where(ss => ss.SubjectId == id && ss.SubjectSection == section)
                .Select(ss => ss.Student).ToListAsync();
            var students = await _context.Students.AsNoTracking().Where(s => !studentInClasses.Contains(s)).ToListAsync();

            var viewModel = new AddStudentToClassViewModel
            {
                Subject = subject,
                StudentInClass = studentInClasses,
                Students = students
            };

            ViewData["SelectableStudent"] = new SelectList(students, "Id", "FullName");

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudentToSubject(string id, byte? section, string studentId)
        {
            var exist = await _context.Subjects.Where(s => s.Id == id && s.Section == section).AnyAsync();

            if (!exist) return NotFound();

            var student = await _context.Students.Where(s => s.Id == studentId).FirstOrDefaultAsync();

            if (student == null) return NotFound();

            var alreadyAdd = _context.SubjectStudents.Where(ss => ss.StudentId == studentId && ss.SubjectId == id && ss.SubjectSection == section).Any();

            if (alreadyAdd)
            {
                return StatusCode(304);
                //return Json(new { id = student.Id, name = student.FullName });
            }

            var subjectStudent = new SubjectStudent
            {
                StudentId = studentId,
                SubjectId = id,
                SubjectSection = section.Value
            };

            await _context.SubjectStudents.AddAsync(subjectStudent);
            var addResult = await _context.SaveChangesAsync();

            if (addResult == 0) return StatusCode(500);

            return Json(new { id = student.Id, name = student.FullName });
        }

        private bool SubjectExists(string id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }

        private Task<bool> SubjectExistsAsync(string id, byte section)
        {
            return _context.Subjects.AnyAsync(s => s.Id == id && s.Section == section);
        }
    }
}
