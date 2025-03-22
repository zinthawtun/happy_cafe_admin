using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cafes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Logo = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cafes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.CheckConstraint("CK_Employee_EmailAddress_Format", "\"EmailAddress\" ~ '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$'");
                    table.CheckConstraint("CK_Employee_Phone", "\"Phone\" ~ '^[89]\\d{7}$'");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeCafes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CafeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCafes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeCafes_Cafes_CafeId",
                        column: x => x.CafeId,
                        principalTable: "Cafes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeCafes_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cafes",
                columns: new[] { "Id", "Description", "Location", "Logo", "Name" },
                values: new object[,]
                {
                    { new Guid("0d0a50f4-6e0c-462a-9cdd-e4c1bd6b81cd"), "French coffee shop.", "Capital Green building.", "la-cafe-logo.png", "La Cafe" },
                    { new Guid("e1c3c170-02a8-4367-8fe5-88697227fb27"), "Medium roated coffee beans and good coffee", "Tiong Baru Plaza", "coffee-white-logo.png", "Coffee White" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "EmailAddress", "Gender", "Name", "Phone" },
                values: new object[,]
                {
                    { "UIPN5FP4O", "john.doe@example.com", "Male", "John Doe", "89123456" },
                    { "UIY7AU2LA", "jane.smith@example.com", "Female", "Jane Smith", "98123456" }
                });

            migrationBuilder.InsertData(
                table: "EmployeeCafes",
                columns: new[] { "Id", "AssignedDate", "CafeId", "EmployeeId", "IsActive" },
                values: new object[,]
                {
                    { new Guid("1327364a-989c-4cd6-b34b-0866c0ff03e3"), new DateTime(2025, 3, 19, 10, 37, 50, 833, DateTimeKind.Utc).AddTicks(7922), new Guid("0d0a50f4-6e0c-462a-9cdd-e4c1bd6b81cd"), "UIY7AU2LA", true },
                    { new Guid("3320e8b9-f1d0-4e5a-ab64-e8a703b8a33d"), new DateTime(2025, 3, 19, 10, 37, 50, 833, DateTimeKind.Utc).AddTicks(6697), new Guid("e1c3c170-02a8-4367-8fe5-88697227fb27"), "UIPN5FP4O", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cafes_Id",
                table: "Cafes",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cafes_Location",
                table: "Cafes",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Cafes_Name",
                table: "Cafes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCafes_CafeId",
                table: "EmployeeCafes",
                column: "CafeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCafes_EmployeeId",
                table: "EmployeeCafes",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmailAddress",
                table: "Employees",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Id",
                table: "Employees",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Phone",
                table: "Employees",
                column: "Phone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeCafes");

            migrationBuilder.DropTable(
                name: "Cafes");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
