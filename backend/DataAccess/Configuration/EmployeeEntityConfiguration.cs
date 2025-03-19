using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configuration
{
    public class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.Id).IsUnique();

            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.EmailAddress)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(10);
                
            builder.Property(e => e.Id).HasColumnName("Id");
            builder.Property(e => e.Name).HasColumnName("Name");
            builder.Property(e => e.EmailAddress).HasColumnName("EmailAddress");
            builder.Property(e => e.Phone).HasColumnName("Phone");
            builder.Property(e => e.Gender).HasColumnName("Gender");

            builder.HasConstructor(
                "id",
                "name",
                "emailAddress",
                "phone",
                "gender"
            );
        }
    }
}
