using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using OnTimeWebApplication.Data;
using Microsoft.EntityFrameworkCore;
using OnTimeWebApplication.Controllers;
using OnTimeWebApplication.Models;
using OnTimeWebApplication.Websocket;
using Hangfire;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace OnTimeWebApplication.Services
{
    public class AttendanceCheckingService
    {
        private string _subjectId;
        private byte _section;
        private DateTime _startTime;
        private DateTime _endTime;
        private DateTime _lateTime;
        private DateTime _absentTime;
        private bool _useComeAbsent;
        private Task _initlizeAsync;
        private DbContextOptionsBuilder<AttendanceCheckingDbContext> _efCoreOptionBuilder;
        private UpdatingWebSocketHandler _websocketHandler;
        private List<string> _alreadyCheckedId = new List<string>();

        public static ConcurrentDictionary<Tuple<string, byte>, AttendanceCheckingService> Current { get; } = new ConcurrentDictionary<Tuple<string, byte>, AttendanceCheckingService>();

        public AttendanceCheckingService(IOptions<EFCoreOptions> options, UpdatingWebSocketHandler websocketHandle)
        {
            _efCoreOptionBuilder = new DbContextOptionsBuilder<AttendanceCheckingDbContext>();
            _efCoreOptionBuilder.UseSqlServer(options.Value.ConnectionString);
            _websocketHandler = websocketHandle;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task AddCurrentChecking(string subId, byte section, DayOfWeek dayOfWeek)
        {
            using (var context = new AttendanceCheckingDbContext(_efCoreOptionBuilder.Options))
            {
                var subjectTime = await context.SubjectTimes.Where(st => st.SubjectId == subId && st.Section == section && st.DayOfWeek == dayOfWeek).FirstOrDefaultAsync();
                var subject = await context.Subjects.Where(s => s.Id == subId && s.Section == section).FirstOrDefaultAsync();
                var now = DateTime.Now;
                _startTime = new DateTime(now.Year, now.Month, now.Day, subjectTime.Start.Hour, subjectTime.Start.Minute, 0);
                _endTime = new DateTime(now.Year, now.Month, now.Day, subjectTime.End.Hour, subjectTime.End.Minute, 0);
                _lateTime = _startTime + subject.LateTime;
                _absentTime = _startTime + subject.AbsentTime;
                _useComeAbsent = subject.UseComeAbsent;
                _subjectId = subId;
                _section = section;

                Current.AddOrUpdate(new Tuple<string, byte>(subId, section), this,
                    (key, oldValue) => this);
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public static bool RemoveCurrentChecking(string subId, byte section)
        {
            AttendanceCheckingService temp;
            return Current.TryRemove(new Tuple<string, byte>(subId, section), out temp);
        }

        public async Task CheckStudents(CheckingStudentData[] students)
        {
            var attendList = new List<UpdateMessage>(students.Length);

            using (var context = new AttendanceCheckingDbContext(_efCoreOptionBuilder.Options))
            {
                foreach (var student in students)
                {
                    if (_alreadyCheckedId.Contains(student.Id))
                    {
                        continue;
                    }

                    var attendance = new Attendance
                    {
                        StudentId = student.Id,
                        SubjectId = _subjectId,
                        SubjectSection = _section,
                        AttendedTime = student.AttendTime
                    };

                    if (student.AttendTime <= _lateTime)
                    {
                        // he came on time
                        attendance.AttendState = AttendState.InTime;
                    }
                    else if (student.AttendTime <= _absentTime)
                    {
                        // he came late
                        attendance.AttendState = AttendState.Late;
                    }
                    else
                    {
                        // he absent
                        attendance.AttendState = AttendState.Absent;

                        if (_useComeAbsent)
                        {
                            // he came but absent
                            attendance.AttendState = AttendState.AttendButAbsent;
                        }
                    }

                    attendList.Add(new UpdateMessage { StudentId = attendance.StudentId, AttendState = attendance.AttendState });
                    context.Attendance.Add(attendance);
                }

                await context.SaveChangesAsync();
            }

            var jsonString = JsonConvert.SerializeObject(attendList);
            await _websocketHandler.SendMessageAsync($"{_subjectId}{_section}", jsonString);
        }

        private class UpdateMessage
        {
            public string StudentId { get; set; }
            public AttendState AttendState { get; set; }
        }
    }
}
