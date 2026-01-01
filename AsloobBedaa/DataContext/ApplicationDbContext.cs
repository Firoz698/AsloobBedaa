using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using AsloobBedaa.Models;
using AsloobBedaa.Models.Permission;

namespace AsloobBedaa.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<User> Users { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Overtime> Overtimes { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }

    }
}
