using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexPlanner.Api.Models
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public List<User> Users { get; set; } = new(); // Ancienne relation 1-N (gardée pour compatibilité)

        // Nouvelles relations N-N
        public List<User> UsersMultiple { get; set; } = new(); // Collection des utilisateurs via N-N
        public List<UserTeam> UserTeams { get; set; } = new(); // Table de liaison
    }
}
