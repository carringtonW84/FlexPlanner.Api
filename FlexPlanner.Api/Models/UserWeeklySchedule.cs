using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexPlanner.Api.Models
{
    public class UserWeeklySchedule
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int WeekDayId { get; set; }
        public bool IsOnsite { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public WeekDay WeekDay { get; set; } = null!;
    }
}
