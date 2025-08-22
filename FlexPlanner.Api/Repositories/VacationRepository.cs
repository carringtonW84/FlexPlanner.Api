using FlexPlanner.Api.Data;
using FlexPlanner.Api.DTOs;
using FlexPlanner.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlexPlanner.Api.Repositories
{
    public class VacationRepository : IVacationRepository
    {
        private readonly FlexPlannerDbContext _context;

        public VacationRepository(FlexPlannerDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserVacation>> GetUserVacationsAsync(Guid userId)
        {
            return await _context.UserVacations
                .Include(v => v.VacationType)
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.StartDate)
                .ToListAsync();
        }

        public async Task<UserVacation> CreateVacationAsync(Guid userId, CreateVacationRequest request)
        {
            var vacationType = await _context.VacationTypes
                .FirstOrDefaultAsync(vt => vt.Code == request.VacationTypeCode);

            if (vacationType == null)
                throw new ArgumentException("Invalid vacation type");

            var vacation = new UserVacation
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                VacationTypeId = vacationType.Id,
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
                Status = "pending",
                Notes = request.Notes
            };

            _context.UserVacations.Add(vacation);
            await _context.SaveChangesAsync();

            // Load the vacation type for return
            await _context.Entry(vacation)
                .Reference(v => v.VacationType)
                .LoadAsync();

            return vacation;
        }

        public async Task DeleteVacationAsync(Guid userId, Guid vacationId)
        {
            var vacation = await _context.UserVacations
                .FirstOrDefaultAsync(v => v.Id == vacationId && v.UserId == userId);

            if (vacation != null)
            {
                _context.UserVacations.Remove(vacation);
                await _context.SaveChangesAsync();
            }
        }
    }
}
