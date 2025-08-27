using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Data;
using FlexPlanner.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace FlexPlanner.Api.Services
{
    public class TeamReportService : ITeamReportService
    {
        private readonly FlexPlannerDbContext _context;
        private readonly IPlanningRepository _planningRepository;

        public TeamReportService(FlexPlannerDbContext context, IPlanningRepository planningRepository)
        {
            _context = context;
            _planningRepository = planningRepository;
        }

        // ... (méthodes GetTeamPlanningReportAsync et GetTeamNameAsync identiques) ...

        public async Task<byte[]> ExportTeamPlanningToExcelAsync(Guid teamId, DateTime startDate, DateTime endDate)
        {
            var report = await GetTeamPlanningReportAsync(teamId, startDate, endDate);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Planning Équipe");

            // Titre principal
            worksheet.Cells[1, 1].Value = $"Planning - {report.TeamName}";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;

            // Période
            worksheet.Cells[2, 1].Value = $"Période: {report.StartDate:dd/MM/yyyy} - {report.EndDate:dd/MM/yyyy}";

            // En-têtes des colonnes
            var headerRow = 4;
            worksheet.Cells[headerRow, 1].Value = "Nom";
            worksheet.Cells[headerRow, 2].Value = "Prénom";

            var colIndex = 3;
            foreach (var date in report.DateRange)
            {
                worksheet.Cells[headerRow, colIndex].Value = date.ToString("dd/MM");
                worksheet.Cells[headerRow + 1, colIndex].Value = date.ToString("ddd");
                colIndex++;
            }

            // Données
            var dataStartRow = 6;
            var rowIndex = dataStartRow;

            foreach (var member in report.Members)
            {
                var names = member.FullName.Split(' ');
                worksheet.Cells[rowIndex, 1].Value = names.LastOrDefault(); // Nom
                worksheet.Cells[rowIndex, 2].Value = string.Join(" ", names.Take(names.Length - 1)); // Prénom

                colIndex = 3;
                foreach (var date in report.DateRange)
                {
                    if (member.DailyStatus.TryGetValue(date, out var dayStatus))
                    {
                        var cell = worksheet.Cells[rowIndex, colIndex];
                        cell.Value = GetStatusText(dayStatus.StatusCode);

                        // Couleur de fond basique
                        try
                        {
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(GetStatusColor(dayStatus.StatusCode));
                        }
                        catch
                        {
                            // En cas d'erreur avec les couleurs, on continue sans
                        }
                    }
                    colIndex++;
                }

                rowIndex++;
            }

            // Auto-ajuster les colonnes
            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }

        private string GetStatusText(string statusCode)
        {
            return statusCode switch
            {
                "ONSITE" => "Sur site",
                "REMOTE" => "Télétravail",
                "VACATION" => "Congés",
                "HOLIDAY" => "Férié",
                "WEEKEND" => "Week-end",
                _ => statusCode
            };
        }

        private Color GetStatusColor(string statusCode)
        {
            return statusCode switch
            {
                "ONSITE" => Color.LightGreen,
                "REMOTE" => Color.LightBlue,
                "VACATION" => Color.LightYellow,
                "HOLIDAY" => Color.LightCoral,
                "WEEKEND" => Color.LightGray,
                _ => Color.White
            };
        }

        // Autres méthodes privées simplifiées...
        private async Task<DayStatusDto> GetUserDayStatusAsync(Guid userId, DateTime date)
        {
            var status = await _planningRepository.GetDayStatusAsync(userId, date);
            var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            var isHoliday = await _planningRepository.IsHolidayAsync(date);

            return new DayStatusDto
            {
                StatusCode = status.Code,
                StatusName = status.Name,
                StatusEmoji = status.Emoji,
                StatusColor = status.ColorClass,
                Notes = status.Notes,
                IsWeekend = isWeekend,
                IsHoliday = isHoliday
            };
        }

        private List<DateTime> GenerateDateRange(DateTime startDate, DateTime endDate)
        {
            var dates = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dates.Add(date);
            }
            return dates;
        }

        public async Task<TeamPlanningReportDto> GetTeamPlanningReportAsync(Guid teamId, DateTime startDate, DateTime endDate)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
                throw new ArgumentException("Équipe introuvable");

            var teamMembers = await _context.UserTeams
                .Where(ut => ut.TeamId == teamId)
                .Include(ut => ut.User)
                .Select(ut => ut.User)
                .Where(u => u.IsActive)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            var dateRange = GenerateDateRange(startDate, endDate);
            var members = new List<TeamMemberPlanningDto>();
            var statusSummary = new Dictionary<string, int>();

            foreach (var user in teamMembers)
            {
                var memberPlanning = new TeamMemberPlanningDto
                {
                    UserId = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    DailyStatus = new Dictionary<DateTime, DayStatusDto>()
                };

                foreach (var date in dateRange)
                {
                    var dayStatus = await GetUserDayStatusAsync(user.Id, date);
                    memberPlanning.DailyStatus[date] = dayStatus;

                    if (!statusSummary.ContainsKey(dayStatus.StatusName))
                        statusSummary[dayStatus.StatusName] = 0;
                    statusSummary[dayStatus.StatusName]++;
                }

                members.Add(memberPlanning);
            }

            return new TeamPlanningReportDto
            {
                TeamName = team.Name,
                StartDate = startDate,
                EndDate = endDate,
                Members = members,
                DateRange = dateRange,
                StatusSummary = statusSummary
            };
        }

        public async Task<string> GetTeamNameAsync(Guid teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            return team?.Name ?? "Équipe inconnue";
        }
    }
}