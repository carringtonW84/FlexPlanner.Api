using FlexPlanner.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlexPlanner.Api.Data
{
    public class FlexPlannerDbContext : DbContext
    {
        public FlexPlannerDbContext(DbContextOptions<FlexPlannerDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<VacationType> VacationTypes { get; set; }
        public DbSet<PresenceStatus> PresenceStatuses { get; set; }
        public DbSet<WeekDay> WeekDays { get; set; }
        public DbSet<FrenchHoliday> FrenchHolidays { get; set; }
        public DbSet<UserWeeklySchedule> UserWeeklySchedules { get; set; }
        public DbSet<UserPlanning> UserPlanningEntries { get; set; }
        public DbSet<UserVacation> UserVacations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des tables
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users", schema: "flexplanner");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
                entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100);
                entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100);
                entity.Property(e => e.TeamId).HasColumnName("team_id");
                entity.Property(e => e.Velocity).HasColumnName("velocity");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.LastLogin).HasColumnName("last_login");

                entity.HasOne(e => e.Team).WithMany(t => t.Users).HasForeignKey(e => e.TeamId);
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("teams", schema: "flexplanner");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<VacationType>(entity =>
            {
                entity.ToTable("vacation_types", schema: "flexplanner");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
                entity.Property(e => e.Emoji).HasColumnName("emoji").HasMaxLength(10);
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<PresenceStatus>(entity =>
            {
                entity.ToTable("presence_statuses", schema: "flexplanner");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
                entity.Property(e => e.Emoji).HasColumnName("emoji").HasMaxLength(10);
                entity.Property(e => e.ColorClass).HasColumnName("color_class").HasMaxLength(100);
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<WeekDay>(entity =>
            {
                entity.ToTable("week_days", schema: "flexplanner");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(20);
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
                entity.Property(e => e.ShortName).HasColumnName("short_name").HasMaxLength(10);
                entity.Property(e => e.IsWorkingDay).HasColumnName("is_working_day");
            });

            modelBuilder.Entity<FrenchHoliday>(entity =>
            {
                entity.ToTable("french_holidays", schema: "flexplanner");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.HolidayKey).HasColumnName("holiday_key").HasMaxLength(10);
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<UserWeeklySchedule>(entity =>
            {
                entity.ToTable("user_weekly_schedules", schema: "flexplanner");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.WeekDayId).HasColumnName("week_day_id");
                entity.Property(e => e.IsOnsite).HasColumnName("is_onsite");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.User).WithMany(u => u.WeeklySchedules).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.WeekDay).WithMany(w => w.UserWeeklySchedules).HasForeignKey(e => e.WeekDayId);
                entity.HasIndex(e => new { e.UserId, e.WeekDayId }).IsUnique();
            });

            modelBuilder.Entity<UserPlanning>(entity =>
            {
                entity.ToTable("user_planning", schema: "flexplanner");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Date).HasColumnName("date").HasColumnType("date");
                entity.Property(e => e.StatusId).HasColumnName("status_id");
                entity.Property(e => e.Notes).HasColumnName("notes");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.User).WithMany(u => u.PlanningEntries).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Status).WithMany(s => s.UserPlanningEntries).HasForeignKey(e => e.StatusId);
                entity.HasIndex(e => new { e.UserId, e.Date }).IsUnique();
            });

            modelBuilder.Entity<UserVacation>(entity =>
            {
                entity.ToTable("user_vacations", schema: "flexplanner");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.VacationTypeId).HasColumnName("vacation_type_id");
                entity.Property(e => e.StartDate).HasColumnName("start_date").HasColumnType("date");
                entity.Property(e => e.EndDate).HasColumnName("end_date").HasColumnType("date");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
                entity.Property(e => e.Notes).HasColumnName("notes");
                entity.Property(e => e.RequestedAt).HasColumnName("requested_at");
                entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
                entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasOne(e => e.User).WithMany(u => u.Vacations).HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.VacationType).WithMany(v => v.UserVacations).HasForeignKey(e => e.VacationTypeId);
                entity.HasOne(e => e.ApprovedByUser).WithMany().HasForeignKey(e => e.ApprovedBy);
            });
        }
    }
}
