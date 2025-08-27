namespace FlexPlanner.Api.DTOs
{
    public class TeamPlanningReportDto
    {
        public string TeamName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<TeamMemberPlanningDto> Members { get; set; } = new();
        public List<DateTime> DateRange { get; set; } = new();
        public Dictionary<string, int> StatusSummary { get; set; } = new();
    }
}
