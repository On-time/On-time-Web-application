using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnTimeWebApplication.Utilities;
using Xunit;

namespace OntimeWebAppTest
{
    public class Class1
    {
        // 10:30, wed
        [Fact]
        public void CreateCronTest()
        {
            var testedDate = new DateTime(1970, 1, 1, 10, 30, 0);
            Assert.Equal("30 10 * * WED", 
                CronExUtil.CreateCron(new DateTime[] { testedDate }, new DayOfWeek[] { DayOfWeek.Wednesday }));
        }

        // 10:30, wed and fri
        [Fact]
        public void CreateCronTestMultipleDayOfWeeks()
        {
            var testedDate = new DateTime(1970, 1, 1, 10, 30, 0);
            Assert.Equal("30 10 * * WED,FRI",
                CronExUtil.CreateCron(new DateTime[] { testedDate }, new DayOfWeek[] { DayOfWeek.Wednesday, DayOfWeek.Friday }));
        }

        // 10:30 and 14.00, wed
        [Fact]
        public void CreateCronTestMultipleTime()
        {
            var testedDate = new DateTime(1970, 1, 1, 10, 30, 0);
            var testedDate2 = new DateTime(1970, 1, 1, 14, 0, 0);
            Assert.Equal("30,0 10,14 * * WED",
                CronExUtil.CreateCron(new DateTime[] { testedDate, testedDate2 }, new DayOfWeek[] { DayOfWeek.Wednesday }));
        }

        // 10:30 and 14.00, wed and fri
        [Fact]
        public void CreateCronTestMultipleTimeAndDayOfWeek()
        {
            var testedDate = new DateTime(1970, 1, 1, 10, 30, 0);
            var testedDate2 = new DateTime(1970, 1, 1, 14, 0, 0);
            Assert.Equal("30,0 10,14 * * WED,FRI",
                CronExUtil.CreateCron(new DateTime[] { testedDate, testedDate2 }, new DayOfWeek[] { DayOfWeek.Wednesday, DayOfWeek.Friday }));
        }
    }
}
