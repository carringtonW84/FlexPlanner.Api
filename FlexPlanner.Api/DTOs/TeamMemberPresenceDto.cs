namespace FlexPlanner.Api.DTOs
{
    public class TeamMemberPresenceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
    }
}
