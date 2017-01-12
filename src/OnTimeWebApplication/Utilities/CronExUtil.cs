using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Utilities
{
    public class CronExUtil
    {
        /// <summary>
        /// Create cron expresstion from Datetime and DayOfWeek. THIS METHOD STILL WORK ONLY FOR HOUR, MINUTE AND DAYOFWEEK PORTION
        /// </summary>
        /// <param name="datetimes">datatime must be ordered the same as dayofweek</param>
        /// <param name="dayOfWeeks">system.dayofweek</param>
        /// <returns>cron expression string</returns>
        public static string CreateCron(DateTime[] datetimes, DayOfWeek[] dayOfWeeks)
        {
            if (datetimes == null || datetimes.Length < 1)
            {
                throw new ArgumentException("datetime array length is 0 or null");
            }

            var hourPortion = datetimes.Select(dt => dt.Hour).Aggregate("",(string ex, int hr) =>
            {
                return ex + hr.ToString() + ",";
            });

            var minutePortion = datetimes.Select(d => d.Minute).Aggregate("", (string ex, int min) =>
            {
                return ex + min.ToString() + ",";
            });

            var dayOfWeekPortion = dayOfWeeks.Select(d =>
            {
                switch (d)
                {
                    case DayOfWeek.Monday:
                        return "MON";
                    case DayOfWeek.Tuesday:
                        return "TUE";
                    case DayOfWeek.Wednesday:
                        return "WED";
                    case DayOfWeek.Thursday:
                        return "THU";
                    case DayOfWeek.Friday:
                        return "FRI";
                    case DayOfWeek.Saturday:
                        return "SAT";
                    case DayOfWeek.Sunday:
                        return "SUN";
                    default:
                        return "NEVER CALL";

                }
            }).Aggregate((f, s) =>
            {
                return f + "," + s;
            });

            hourPortion = hourPortion.Remove(hourPortion.Length - 1);
            minutePortion = minutePortion.Remove(hourPortion.Length - 1);


            string cron = $"{minutePortion} {hourPortion} * * {dayOfWeekPortion}";

            return "";
        }
    }
}
