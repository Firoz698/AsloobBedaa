using Microsoft.EntityFrameworkCore;
using AsloobBedaa.Models;
using AsloobBedaa.Models.Letter;
using AsloobBedaa.Models.Permission;

namespace AsloobBedaa.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ===== Existing DbSets =====
        public DbSet<User>? Users { get; set; }
        public DbSet<LetterType>? LetterTypes { get; set; }
        public DbSet<LetterTemplate>? LetterTemplates { get; set; }
        public DbSet<LetterTemplateSection>? LetterTemplateSections { get; set; }


        public DbSet<DashboardKpi>? DashboardKpis { get; set; }
        public DbSet<Subcontractor>? Subcontractors { get; set; }
        public DbSet<AccountsTransaction>? AccountsTransactions { get; set; }
        public DbSet<PayrollMonthly> PayrollMonthlies { get; set; }
        public DbSet<FinalSettlement> FinalSettlements { get; set; }

        public DbSet<ActivityLog>? ActivityLogs { get; set; }
        public DbSet<Menu>? Menus { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<RoleMenuPermission>? RoleMenuPermissions { get; set; }

        // ===== Attendance System DbSets =====
        public DbSet<Project>? Projects { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<Attendance>? Attendances { get; set; }
        public DbSet<Shift>? Shifts { get; set; }
        public DbSet<Overtime>? Overtimes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicit Primary Keys
            modelBuilder.Entity<FinalSettlement>().HasKey(f => f.SettlementId);
            modelBuilder.Entity<PayrollMonthly>().HasKey(p => p.PayrollId);
            modelBuilder.Entity<DashboardKpi>().HasKey(d => d.Id);
            modelBuilder.Entity<Subcontractor>().HasKey(s => s.SubcontractorId);
            modelBuilder.Entity<AccountsTransaction>().HasKey(a => a.Id);
            modelBuilder.Entity<Project>().HasKey(p => p.ProjectID);  // Project primary key

            // Attendance Unique Constraint
            modelBuilder.Entity<Attendance>()
                        .HasIndex(a => new { a.EmployeeId, a.AttendanceDate, a.ShiftId })
                        .IsUnique();

            // Soft Delete Filters
            modelBuilder.Entity<Project>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Employee>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Attendance>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Shift>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Overtime>().HasQueryFilter(x => !x.IsDeleted);

            // Remove Default Active for Project (since IsActive does not exist)
        }


    }
}
