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

            // Seed default admin user (password: Admin@123)
            var adminUser = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Admin@123")),
                FullName = "System Administrator",
                Email = "admin@canteen.com",
                Role = "Admin",
                Department = "IT",
                EmployeeCode = "ADMIN001",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            modelBuilder.Entity<User>().HasData(adminUser);
        }
    }
}
