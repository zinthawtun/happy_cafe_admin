using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configuration
{
    public class EmployeeCafeEntityConfiguration : IEntityTypeConfiguration<EmployeeCafe>
    {
        public void Configure(EntityTypeBuilder<EmployeeCafe> builder)
        {
            builder.HasKey(ec => ec.Id);

            builder.HasIndex(ec => ec.EmployeeId).IsUnique();

            builder.HasOne(ec => ec.Employee)
                .WithMany(e => e.EmployeeCafes)
                .HasForeignKey(ec => ec.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ec => ec.Cafe)
                .WithMany(c => c.EmployeeCafes)
                .HasForeignKey(ec => ec.CafeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ec => ec.AssignedDate).IsRequired();
            
            builder.Property(ec => ec.Id).HasColumnName("Id");
            builder.Property(ec => ec.CafeId).HasColumnName("CafeId");
            builder.Property(ec => ec.EmployeeId).HasColumnName("EmployeeId");
            builder.Property(ec => ec.AssignedDate).HasColumnName("AssignedDate");
            builder.Property(ec => ec.IsActive).HasColumnName("IsActive");

            builder.HasConstructor(
                "id", 
                "cafeId", 
                "employeeId", 
                "assignedDate"
            );
        }
    }
}
