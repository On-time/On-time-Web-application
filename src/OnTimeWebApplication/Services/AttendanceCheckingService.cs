using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnTimeWebApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace OnTimeWebApplication.Services
{
    public class AttendanceCheckingService
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private DateTime _lateTime;
        private DateTime _absentTime;
        private bool _useComeAbsent;
        private Task _initlizeAsync;
        private AttendanceCheckingDbContext _context;

        public static Dictionary<Tuple<string, byte>, AttendanceCheckingService> Current { get; } = new Dictionary<Tuple<string, byte>, AttendanceCheckingService>();

        public AttendanceCheckingService(AttendanceCheckingDbContext context)
        {
            _context = context;
        }

        public async Task AddCurrentChecking(string subId, byte section, DayOfWeek dayOfWeek)
        {
            var subjectTime = await _context.SubjectTimes.Where(st => st.SubjectId == subId && st.Section == section && st.DayOfWeek == dayOfWeek).FirstOrDefaultAsync();
            var subject = await _context.Subjects.Where(s => s.Id == subId && s.Section == section).FirstOrDefaultAsync();
            var now = DateTime.Now;
            _startTime = new DateTime(now.Year, now.Month, now.Day, subjectTime.Start.Hour, subjectTime.Start.Minute, 0);
            _endTime = new DateTime(now.Year, now.Month, now.Day, subjectTime.End.Hour, subjectTime.End.Minute, 0);
            _lateTime = _startTime + subject.LateTime;
            _absentTime = _startTime + subject.AbsentTime;
            _useComeAbsent = subject.UseComeAbsent;
            Current.Add(new Tuple<string, byte>(subId, section), this);
        }

        public static void RemoveCurrentChecking(string subId, byte section)
        {
            Current.Remove(new Tuple<string, byte>(subId, section));
        }
    }
}
