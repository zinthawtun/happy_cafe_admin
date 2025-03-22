﻿// <auto-generated />
using System;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Business.Entities.Cafe", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("Description");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("Location");

                    b.Property<string>("Logo")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Logo");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Location");

                    b.HasIndex("Name");

                    b.ToTable("Cafes");

                    b.HasAnnotation("ConstructorBinding", new[] { "id", "name", "description", "logo", "location" });

                    b.HasData(
                        new
                        {
                            Id = new Guid("e1c3c170-02a8-4367-8fe5-88697227fb27"),
                            Description = "Medium roated coffee beans and good coffee",
                            Location = "Tiong Baru Plaza",
                            Logo = "coffee-white-logo.png",
                            Name = "Coffee White"
                        },
                        new
                        {
                            Id = new Guid("0d0a50f4-6e0c-462a-9cdd-e4c1bd6b81cd"),
                            Description = "French coffee shop.",
                            Location = "Capital Green building.",
                            Logo = "la-cafe-logo.png",
                            Name = "La Cafe"
                        });
                });

            modelBuilder.Entity("Business.Entities.Employee", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("Id");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("EmailAddress");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("Gender");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("Name");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)")
                        .HasColumnName("Phone");

                    b.HasKey("Id");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Phone");

                    b.ToTable("Employees", t =>
                        {
                            t.HasCheckConstraint("CK_Employee_Phone", "\"Phone\" ~ '^[89]\\d{7}$'");
                        });

                    b.HasAnnotation("ConstructorBinding", new[] { "id", "name", "emailAddress", "phone", "gender" });

                    b.HasData(
                        new
                        {
                            Id = "UIPN5FP4O",
                            EmailAddress = "john.doe@example.com",
                            Gender = "Male",
                            Name = "John Doe",
                            Phone = "89123456"
                        },
                        new
                        {
                            Id = "UIY7AU2LA",
                            EmailAddress = "jane.smith@example.com",
                            Gender = "Female",
                            Name = "Jane Smith",
                            Phone = "98123456"
                        });
                });

            modelBuilder.Entity("Business.Entities.EmployeeCafe", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<DateTime>("AssignedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("AssignedDate");

                    b.Property<Guid>("CafeId")
                        .HasColumnType("uuid")
                        .HasColumnName("CafeId");

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("EmployeeId");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("IsActive");

                    b.HasKey("Id");

                    b.HasIndex("CafeId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("EmployeeCafes");

                    b.HasAnnotation("ConstructorBinding", new[] { "id", "cafeId", "employeeId", "assignedDate" });

                    b.HasData(
                        new
                        {
                            Id = new Guid("3320e8b9-f1d0-4e5a-ab64-e8a703b8a33d"),
                            AssignedDate = new DateTime(2025, 3, 19, 10, 37, 50, 833, DateTimeKind.Utc).AddTicks(6697),
                            CafeId = new Guid("e1c3c170-02a8-4367-8fe5-88697227fb27"),
                            EmployeeId = "UIPN5FP4O",
                            IsActive = true
                        },
                        new
                        {
                            Id = new Guid("1327364a-989c-4cd6-b34b-0866c0ff03e3"),
                            AssignedDate = new DateTime(2025, 3, 19, 10, 37, 50, 833, DateTimeKind.Utc).AddTicks(7922),
                            CafeId = new Guid("0d0a50f4-6e0c-462a-9cdd-e4c1bd6b81cd"),
                            EmployeeId = "UIY7AU2LA",
                            IsActive = true
                        });
                });

            modelBuilder.Entity("Business.Entities.EmployeeCafe", b =>
                {
                    b.HasOne("Business.Entities.Cafe", "Cafe")
                        .WithMany("EmployeeCafes")
                        .HasForeignKey("CafeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Business.Entities.Employee", "Employee")
                        .WithMany("EmployeeCafes")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cafe");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Business.Entities.Cafe", b =>
                {
                    b.Navigation("EmployeeCafes");
                });

            modelBuilder.Entity("Business.Entities.Employee", b =>
                {
                    b.Navigation("EmployeeCafes");
                });
#pragma warning restore 612, 618
        }
    }
}
