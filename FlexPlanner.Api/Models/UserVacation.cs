using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexPlanner.Api.Models
{
    public class UserVacation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid VacationTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public VacationType VacationType { get; set; } = null!;
    }
}
