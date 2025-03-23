using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configuration
{
    public class CafeEntityConfiguration : IEntityTypeConfiguration<Cafe>
    {
        public void Configure(EntityTypeBuilder<Cafe> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.Id).IsUnique();
            
            builder.HasIndex(c => c.Location);
            
            builder.HasIndex(c => c.Name);

            builder.Property(c => c.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.Location)
                .IsRequired()
                .HasMaxLength(150);
                
            builder.Property(c => c.Id).HasColumnName("Id");
            builder.Property(c => c.Name).HasColumnName("Name");
            builder.Property(c => c.Description).HasColumnName("Description");
            builder.Property(c => c.Logo).HasColumnName("Logo");
            builder.Property(c => c.Location).HasColumnName("Location");

            builder.HasConstructor(
                "id",
                "name",
                "description",
                "logo",
                "location"
            );
        }
    }
}
