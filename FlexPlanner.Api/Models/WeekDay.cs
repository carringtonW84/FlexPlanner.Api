using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexPlanner.Api.Models
{
    public class WeekDay
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public bool IsWorkingDay { get; set; } = true;

        public List<UserWeeklySchedule> UserWeeklySchedules { get; set; } = new();
    }
}
