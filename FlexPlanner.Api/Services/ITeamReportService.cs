using FlexPlanner.Api.DTOs;

namespace FlexPlanner.Api.Services
{
    public interface ITeamReportService
    {
        Task<TeamPlanningReportDto> GetTeamPlanningReportAsync(Guid teamId, DateTime startDate, DateTime endDate);
        Task<byte[]> ExportTeamPlanningToExcelAsync(Guid teamId, DateTime startDate, DateTime endDate);
        Task<string> GetTeamNameAsync(Guid teamId);
    }
}
