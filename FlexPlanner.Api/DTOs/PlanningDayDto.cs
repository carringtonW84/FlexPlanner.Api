namespace FlexPlanner.Api.DTOs
{
    public class PlanningDayDto
    {
        public DateTime Date { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string? StatusEmoji { get; set; }
        public string? StatusColor { get; set; }
        public string? Notes { get; set; }
        public bool CanModify { get; set; }
    }
}
