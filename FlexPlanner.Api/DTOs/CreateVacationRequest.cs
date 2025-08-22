namespace FlexPlanner.Api.DTOs
{
    public class CreateVacationRequest
    {
        public string VacationTypeCode { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }
    }
}
