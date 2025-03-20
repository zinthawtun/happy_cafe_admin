using Business.Entities;
using DataAccess.Configuration;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppDbContext : DbContext, IDbContext
    {
        public AppDbContext(IDatabaseConnection connection)
        {
            DatabaseConnection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        private IDatabaseConnection? DatabaseConnection { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                if (DatabaseConnection == null)
                {
                    throw new InvalidOperationException("DbContext not properly configured.");
                }
                
                DatabaseConnection.ConfigureDbContext(builder);
            }
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Cafe> Cafes { get; set; } = null!;
        public DbSet<EmployeeCafe> EmployeeCafes { get; set; } = null!;

        public void EnsureCreated()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmployeeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CafeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeCafeEntityConfiguration());

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            Guid cafe1Id = new Guid("e1c3c170-02a8-4367-8fe5-88697227fb27");
            Guid cafe2Id = new Guid("0d0a50f4-6e0c-462a-9cdd-e4c1bd6b81cd");
            
            string employee1Id = "UIPN5FP4O";
            string employee2Id = "UIY7AU2LA";

            modelBuilder.Entity<Cafe>().HasData(
                new Cafe(
                        id: cafe1Id,
                        name: "Coffee White",
                        description: "Medium roated coffee beans and good coffee",
                        logo: "coffee-white-logo.png",
                        location: "Tiong Baru Plaza"
                    ),
                    new Cafe(
                        id: cafe2Id,
                        name: "La Cafe",
                        description: "French coffee shop.",
                        logo: "la-cafe-logo.png",
                        location: "Capital Green building."
                    )
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee(
                    id: employee1Id,
                    name: "John Doe",
                    emailAddress: "john.doe@example.com",
                    phone: "1234567890",
                    gender: Gender.Male
                ),
                new Employee(
                    id: employee2Id,
                    name: "Jane Smith",
                    emailAddress: "jane.smith@example.com",
                    phone: "0987654321",
                    gender: Gender.Female
                )
            );

            modelBuilder.Entity<EmployeeCafe>().HasData(
                new EmployeeCafe(
                    id: new Guid("3320e8b9-f1d0-4e5a-ab64-e8a703b8a33d"),
                    cafeId: cafe1Id,
                    employeeId: employee1Id,
                    assignedDate: new DateTime(2025, 3, 19, 10, 37, 50, 833, DateTimeKind.Utc).AddTicks(6697)
                ),
                new EmployeeCafe(
                    id: new Guid("1327364a-989c-4cd6-b34b-0866c0ff03e3"),
                    cafeId: cafe2Id,
                    employeeId: employee2Id,
                    assignedDate: new DateTime(2025, 3, 19, 10, 37, 50, 833, DateTimeKind.Utc).AddTicks(7922)
                )
            );
        }
    }
}
