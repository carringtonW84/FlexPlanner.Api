namespace FlexPlanner.Api.DTOs
{
    public class UpdatePlanningRequest
    {
        public DateTime Date { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
