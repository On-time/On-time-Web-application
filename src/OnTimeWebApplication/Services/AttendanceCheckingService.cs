using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnTimeWebApplication.Data;

namespace OnTimeWebApplication.Services
{
    public class AttendanceCheckingService
    {
        private DateTime StartTime;
        private DateTime EndTime;
        private DateTime LateTime;
        private DateTime AbsentTime;
        private bool UseComeAbsent;

        public static Dictionary<Tuple<string, byte>, AttendanceCheckingService> Current { get; } = new Dictionary<Tuple<string, byte>, AttendanceCheckingService>();

        public AttendanceCheckingService(AttendanceCheckingDbContext context)
        {

        }
    }
}
