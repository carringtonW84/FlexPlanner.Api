namespace FlexPlanner.Api.Models
{
    public class UserTeam
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public Team Team { get; set; } = null!;
    }
}
