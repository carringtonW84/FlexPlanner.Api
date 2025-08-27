namespace FlexPlanner.Api.DTOs
{
    public class DayStatusDto
    {
        public string StatusCode { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string? StatusEmoji { get; set; }
        public string? StatusColor { get; set; }
        public string? Notes { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsHoliday { get; set; }
    }
}
