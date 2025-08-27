namespace FlexPlanner.Api.DTOs
{
    public class TeamMemberPlanningDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public Dictionary<DateTime, DayStatusDto> DailyStatus { get; set; } = new();
    }
}
