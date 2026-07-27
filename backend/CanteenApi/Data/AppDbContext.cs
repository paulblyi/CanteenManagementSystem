using CanteenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CanteenApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MealTicket> MealTickets { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<RedemptionLog> RedemptionLogs { get; set; }
        public DbSet<Reconciliation> Reconciliations { get; set; }
        public DbSet<DailySummary> DailySummaries { get; set; }
        public DbSet<BillingRecord> BillingRecords { get; set; }
        public DbSet<Department> Departments { get; set; } // New DbSet for Departments
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global converter for all DateTime properties to UTC
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),                           // write: convert to UTC
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));   // read: mark as UTC
                    }
                }
            }

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.EmployeeCode)
                .IsUnique();

            modelBuilder.Entity<MealTicket>()
                .HasIndex(t => t.TicketNumber)
                .IsUnique();

            modelBuilder.Entity<Batch>()
                .HasIndex(b => b.BatchNumber)
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Users)
                .WithOne(u => u.Department)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull); // If department deleted, set null

            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "IT", Description = "Information Technology", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Department { Id = 2, Name = "HC", Description = "Human Capital", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Department { Id = 3, Name = "Finance", Description = "Finance Department", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Department { Id = 4, Name = "Operations", Description = "Operations & Logistics", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Department { Id = 5, Name = "Marketing", Description = "Marketing & Sales", IsActive = true, CreatedAt = DateTime.UtcNow }
            );

            // Seed default admin user (password: Admin@123)
            var adminUser = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Admin@123")),
                FullName = "System Administrator",
                Email = "admin@masimba.co.zw",
                Role = "Admin",
                DepartmentId = 1, // Assuming the IT department has an ID of 1
                EmployeeCode = "ADMIN001",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            modelBuilder.Entity<User>().HasData(adminUser);

        }
    }
}
