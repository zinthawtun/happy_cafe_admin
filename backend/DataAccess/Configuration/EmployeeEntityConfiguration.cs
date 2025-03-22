using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccess.Configuration
{
    public class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.Id).IsUnique();
            
            builder.HasIndex(e => e.EmailAddress).IsUnique();
            
            builder.HasIndex(e => e.Phone);

            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.EmailAddress)
                .IsRequired()
                .HasMaxLength(150);

            builder.ToTable(t => t.HasCheckConstraint("CK_Employee_EmailAddress_Format", "\"EmailAddress\" ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$'"));

            builder.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(8);
                
            builder.ToTable(t => t.HasCheckConstraint("CK_Employee_Phone", "\"Phone\" ~ '^[89]\\d{7}$'"));

            EnumToStringConverter<Gender> converter = new EnumToStringConverter<Gender>();

            builder.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(10)
                .HasConversion(converter);
                
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
