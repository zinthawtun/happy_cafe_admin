using AutoMapper;
using Business.Entities;
using Service.Commands.Cafes;
using Service.Commands.EmployeeCafes;
using Service.Commands.Employees;
using Service.Mappings;
using Service.Queries.Cafes;
using Service.Queries.EmployeeCafes;
using Service.Queries.Employees;
using System.Reflection;
using Utilities;

namespace Tests.Service.Mappings
{
    public class MappingProfileTests
    {
        private readonly IMapper mapper;

        public MappingProfileTests()
        {
            MapperConfiguration configuration = new MapperConfiguration(config =>
            {
                config.AddProfile<MappingProfile>();
            });

            configuration.AssertConfigurationIsValid();
            mapper = configuration.CreateMapper();
        }

        [Fact]
        public void Map_CreateCafeCommand_To_Cafe_Test()
        {
            CreateCafeCommand command = new CreateCafeCommand
            {
                Name = "Test Cafe",
                Description = "Test Description",
                Logo = "cafe-logo.png",
                Location = "Test Location"
            };

            Cafe cafe = mapper.Map<Cafe>(command);

            Assert.Equal(command.Name, cafe.Name);
            Assert.Equal(command.Description, cafe.Description);
            Assert.Equal(command.Logo, cafe.Logo);
            Assert.Equal(command.Location, cafe.Location);
        }

        [Fact]
        public void Map_CreateEmployeeCommand_To_Employee_Test()
        {
            CreateEmployeeCommand command = new CreateEmployeeCommand
            {
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                Phone = "89123456",
                Gender = Gender.Male
            };

            Employee employee = mapper.Map<Employee>(command);

            Assert.Equal(command.Name, employee.Name);
            Assert.Equal(command.EmailAddress, employee.EmailAddress);
            Assert.Equal(command.Phone, employee.Phone);
            Assert.Equal(command.Gender, employee.Gender);
            Assert.False(string.IsNullOrEmpty(employee.Id));
        }

        [Fact]
        public void Map_UpdateCafeCommand_To_Cafe_Test()
        {
            Guid cafeId = System.Guid.NewGuid();
            UpdateCafeCommand command = new UpdateCafeCommand
            {
                Id = cafeId,
                Name = "Updated Cafe",
                Description = "Updated Description",
                Logo = "cafe-logo.png",
                Location = "Updated Location"
            };

            Cafe cafe = mapper.Map<Cafe>(command);

            Assert.Equal(cafeId, cafe.Id);
            Assert.Equal(command.Name, cafe.Name);
            Assert.Equal(command.Description, cafe.Description);
            Assert.Equal(command.Logo, cafe.Logo);
            Assert.Equal(command.Location, cafe.Location);
        }

        [Fact]
        public void Map_Cafe_To_CafeDto_Test()
        {
            Cafe cafe = new Cafe(
                id: System.Guid.NewGuid(),
                name: "Test Cafe",
                description: "Test Description",
                logo: "Test Logo",
                location: "Test Location"
            );

            CafeDto dto = mapper.Map<CafeDto>(cafe);

            Assert.Equal(cafe.Id, dto.Id);
            Assert.Equal(cafe.Name, dto.Name);
            Assert.Equal(cafe.Description, dto.Description);
            Assert.Equal(cafe.Logo, dto.Logo);
            Assert.Equal(cafe.Location, dto.Location);
        }

        [Fact]
        public void Map_Employee_To_EmployeeDto_Test()
        {
            Employee employee = new Employee(
                "EMP123",
                "John Doe",
                "john.doe@example.com",
                "89123456",
                Gender.Male
            );

            EmployeeDto dto = mapper.Map<EmployeeDto>(employee);

            Assert.Equal(employee.Id, dto.Id);
            Assert.Equal(employee.Name, dto.Name);
            Assert.Equal(employee.EmailAddress, dto.EmailAddress);
            Assert.Equal(employee.Phone, dto.Phone);
            Assert.Equal(employee.Gender, dto.Gender);
        }

        [Fact]
        public void Map_EmployeeCafe_To_EmployeeCafeDto_Test()
        {
            Cafe cafe = new Cafe(
                id: System.Guid.NewGuid(),
                name: "Test Cafe",
                description: "Test Description",
                logo: "Test Logo",
                location: "Test Location"
            );

            Employee employee = new Employee(
                "EMP123",
                "John Doe",
                "john.doe@example.com",
                "89123456",
                Gender.Male
            );

            EmployeeCafe employeeCafe = new EmployeeCafe(
                id: System.Guid.NewGuid(),
                cafeId: cafe.Id,
                employeeId: employee.Id,
                assignedDate: System.DateTime.Now.Date
            );
            
            typeof(EmployeeCafe).GetProperty("Cafe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employeeCafe, cafe);
            
            typeof(EmployeeCafe).GetProperty("Employee", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employeeCafe, employee);

            EmployeeCafeDto dto = mapper.Map<EmployeeCafeDto>(employeeCafe);

            Assert.Equal(employeeCafe.Id, dto.Id);
            Assert.Equal(employeeCafe.EmployeeId, dto.EmployeeId);
            Assert.Equal(employeeCafe.CafeId, dto.CafeId);
            Assert.Equal(employeeCafe.AssignedDate, dto.AssignedDate);
            Assert.Equal(employeeCafe.IsActive, dto.IsActive);
            
            Assert.Equal(cafe.Name, dto.CafeName);
            Assert.Equal(employee.Name, dto.EmployeeName);
        }

        [Fact]
        public void Map_EmployeeCafe_To_EmployeeCafeDto_WithNullNavigation_Test()
        {
            EmployeeCafe employeeCafe = new EmployeeCafe(
                id: System.Guid.NewGuid(),
                cafeId: System.Guid.NewGuid(),
                employeeId: "EMP123",
                assignedDate: System.DateTime.Now.Date
            );

            EmployeeCafeDto dto = mapper.Map<EmployeeCafeDto>(employeeCafe);

            Assert.Equal(employeeCafe.Id, dto.Id);
            Assert.Equal(employeeCafe.EmployeeId, dto.EmployeeId);
            Assert.Equal(employeeCafe.CafeId, dto.CafeId);
            Assert.Equal(employeeCafe.AssignedDate, dto.AssignedDate);
            Assert.Equal(employeeCafe.IsActive, dto.IsActive);
            
            Assert.Equal(string.Empty, dto.CafeName);
            Assert.Equal(string.Empty, dto.EmployeeName);
        }

        [Fact]
        public void Map_EmployeeCafeDto_To_EmployeeCafe_Test()
        {
            EmployeeCafeDto dto = new EmployeeCafeDto
            {
                Id = System.Guid.NewGuid(),
                CafeId = System.Guid.NewGuid(),
                EmployeeId = "EMP123",
                AssignedDate = System.DateTime.Now.Date,
                IsActive = true,
                CafeName = "Test Cafe",
                EmployeeName = "John Doe"
            };

            EmployeeCafe employeeCafe = mapper.Map<EmployeeCafe>(dto);

            Assert.Equal(dto.Id, employeeCafe.Id);
            Assert.Equal(dto.EmployeeId, employeeCafe.EmployeeId);
            Assert.Equal(dto.CafeId, employeeCafe.CafeId);
            Assert.Equal(dto.AssignedDate, employeeCafe.AssignedDate);
            Assert.Equal(dto.IsActive, employeeCafe.IsActive);
            
            Assert.Null(employeeCafe.Cafe);
            Assert.Null(employeeCafe.Employee);
        }

        [Fact]
        public void Map_AssignEmployeeToCafeCommand_To_EmployeeCafe()
        {
            AssignEmployeeToCafeCommand command = new AssignEmployeeToCafeCommand
            {
                EmployeeId = "EMP123",
                CafeId = System.Guid.NewGuid(),
                AssignedDate = System.DateTime.Now.Date
            };

            EmployeeCafe employeeCafe = mapper.Map<EmployeeCafe>(command);

            Assert.Equal(command.EmployeeId, employeeCafe.EmployeeId);
            Assert.Equal(command.CafeId, employeeCafe.CafeId);
            Assert.Equal(command.AssignedDate, employeeCafe.AssignedDate);
            Assert.True(employeeCafe.IsActive);
        }
        
        [Fact]
        public void Map_UpdateEmployeeCommand_To_Employee_Test()
        {
            string employeeId = "EMP123";
            UpdateEmployeeCommand command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                Name = "Jane Smith",
                EmailAddress = "jane.smith@example.com",
                Phone = "98765432",
                Gender = Gender.Female
            };

            Employee employee = mapper.Map<Employee>(command);

            Assert.Equal(employeeId, employee.Id);
            Assert.Equal(command.Name, employee.Name);
            Assert.Equal(command.EmailAddress, employee.EmailAddress);
            Assert.Equal(command.Phone, employee.Phone);
            Assert.Equal(command.Gender, employee.Gender);
        }
        
        [Fact]
        public void Map_UpdateEmployeeCafeAssignmentCommand_To_EmployeeCafe_Test()
        {
            Guid assignmentId = System.Guid.NewGuid();
            Guid cafeId = System.Guid.NewGuid();
            string employeeId = "EMP123";
            DateTime assignedDate = System.DateTime.Now.Date;
            
            UpdateEmployeeCafeAssignmentCommand command = new UpdateEmployeeCafeAssignmentCommand
            {
                Id = assignmentId,
                CafeId = cafeId,
                EmployeeId = employeeId,
                AssignedDate = assignedDate,
                IsActive = true
            };

            EmployeeCafe employeeCafe = mapper.Map<EmployeeCafe>(command);

            Assert.Equal(assignmentId, employeeCafe.Id);
            Assert.Equal(cafeId, employeeCafe.CafeId);
            Assert.Equal(employeeId, employeeCafe.EmployeeId);
            Assert.Equal(assignedDate, employeeCafe.AssignedDate);
        }

        [Fact]
        public void Map_Employee_To_EmployeeDto_WithNullEmployeeCafes_Test()
        {
            Employee employee = new Employee(
                "EMP123",
                "John Doe",
                "john.doe@example.com",
                "89123456",
                Gender.Male
            );
            
            EmployeeDto dto = mapper.Map<EmployeeDto>(employee);
            
            Assert.Equal(employee.Id, dto.Id);
            Assert.Null(dto.CafeId);
            Assert.Equal(string.Empty, dto.CafeName);
            Assert.Equal(0, dto.DaysWorked);
            Assert.False(dto.IsAssignedToCafe);
        }
        
        [Fact]
        public void Map_Employee_To_EmployeeDto_WithEmptyEmployeeCafes_Test()
        {
            Employee employee = new Employee(
                "EMP123",
                "Jane Smith",
                "jane.smith@example.com",
                "98765432",
                Gender.Female
            );
            
            typeof(Employee).GetProperty("EmployeeCafes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employee, new List<EmployeeCafe>());
            
            EmployeeDto dto = mapper.Map<EmployeeDto>(employee);
            
            Assert.Equal(employee.Id, dto.Id);
            Assert.Null(dto.CafeId);
            Assert.Equal(string.Empty, dto.CafeName);
            Assert.Equal(0, dto.DaysWorked);
            Assert.False(dto.IsAssignedToCafe);
        }
        
        [Fact]
        public void Map_Employee_To_EmployeeDto_WithActiveCafeAssignment_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string employeeId = "EMP123";
            DateTime assignedDate = DateTime.UtcNow.AddDays(-10);
            
            Cafe cafe = new Cafe(
                id: cafeId,
                name: "Test Cafe",
                description: "Test Description",
                logo: "test-logo.png",
                location: "Test Location"
            );
            
            Employee employee = new Employee(
                "EMP123",
                "Bob Johnson",
                "bob.johnson@example.com",
                "89123456",
                Gender.Male
            );
            
            EmployeeCafe employeeCafe = new EmployeeCafe(
                id: Guid.NewGuid(),
                cafeId: cafeId,
                employeeId: employeeId,
                assignedDate: assignedDate
            );
            
            typeof(EmployeeCafe).GetProperty("Cafe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employeeCafe, cafe);
                
            typeof(EmployeeCafe).GetProperty("IsActive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employeeCafe, true);
                
            typeof(Employee).GetProperty("EmployeeCafes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employee, new List<EmployeeCafe> { employeeCafe });
            
            EmployeeDto dto = mapper.Map<EmployeeDto>(employee);
            
            Assert.Equal(employee.Id, dto.Id);
            Assert.Equal(cafeId, dto.CafeId);
            Assert.Equal("Test Cafe", dto.CafeName);
            Assert.Equal((int)Math.Ceiling((DateTime.UtcNow - assignedDate).TotalDays), dto.DaysWorked);
            Assert.True(dto.IsAssignedToCafe);
        }
        
        [Fact]
        public void Map_Employee_To_EmployeeDto_WithActiveAssignmentButNullCafe_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string employeeId = "EMP123";
            DateTime assignedDate = DateTime.UtcNow.AddDays(-5);
            
            Employee employee = new Employee(
                "EMP123",
                "Alice Williams",
                "alice.williams@example.com",
                "89123456",
                Gender.Female
            );
            
            EmployeeCafe employeeCafe = new EmployeeCafe(
                id: Guid.NewGuid(),
                cafeId: cafeId,
                employeeId: employeeId,
                assignedDate: assignedDate
            );
            
            typeof(EmployeeCafe).GetProperty("IsActive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employeeCafe, true);
                
            typeof(Employee).GetProperty("EmployeeCafes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employee, new List<EmployeeCafe> { employeeCafe });
            
            EmployeeDto dto = mapper.Map<EmployeeDto>(employee);
            
            Assert.Equal(employee.Id, dto.Id);
            Assert.Equal(cafeId, dto.CafeId);
            Assert.Equal(string.Empty, dto.CafeName);
            Assert.Equal((int)Math.Ceiling((DateTime.UtcNow - assignedDate).TotalDays), dto.DaysWorked);
            Assert.True(dto.IsAssignedToCafe);
        }
        
        [Fact]
        public void Map_Employee_To_EmployeeDto_WithOnlyInactiveCafeAssignments_Test()
        {
            Guid cafeId = Guid.NewGuid();
            string employeeId = "EMP123";
            DateTime assignedDate = DateTime.UtcNow.AddDays(-30);
            
            Cafe cafe = new Cafe(
                id: cafeId,
                name: "Test Cafe",
                description: "Test Description",
                logo: "test-logo.png",
                location: "Test Location"
            );
            
            Employee employee = new Employee(
                "EMP123",
                "Charlie Brown",
                "charlie.brown@example.com",
                "89123456",
                Gender.Male
            );
            
            EmployeeCafe employeeCafe = new EmployeeCafe(
                id: Guid.NewGuid(),
                cafeId: cafeId,
                employeeId: employeeId,
                assignedDate: assignedDate
            );
            
            typeof(EmployeeCafe).GetProperty("Cafe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employeeCafe, cafe);
                
            typeof(EmployeeCafe).GetProperty("IsActive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employeeCafe, false);
                
            typeof(Employee).GetProperty("EmployeeCafes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employee, new List<EmployeeCafe> { employeeCafe });
            
            EmployeeDto dto = mapper.Map<EmployeeDto>(employee);
            
            Assert.Equal(employee.Id, dto.Id);
            Assert.Null(dto.CafeId);
            Assert.Equal(string.Empty, dto.CafeName);
            Assert.Equal(0, dto.DaysWorked);
            Assert.False(dto.IsAssignedToCafe);
        }

        [Fact]
        public void Map_Employee_To_EmployeeDto_WithMultipleCafeAssignments_Test()
        {
            Guid activeCafeId = Guid.NewGuid();
            Guid inactiveCafeId = Guid.NewGuid();
            string employeeId = "EMP123";
            DateTime activeAssignedDate = DateTime.UtcNow.AddDays(-3);
            DateTime inactiveAssignedDate = DateTime.UtcNow.AddDays(-15);
            
            Cafe activeCafe = new Cafe(
                id: activeCafeId,
                name: "Active Cafe",
                description: "Currently Active",
                logo: "active-logo.png",
                location: "Main Street"
            );
            
            Cafe inactiveCafe = new Cafe(
                id: inactiveCafeId,
                name: "Previous Cafe",
                description: "Previously Assigned",
                logo: "inactive-logo.png",
                location: "Side Street"
            );
            
            Employee employee = new Employee(
                "EMP123",
                "Multiple Assignment Employee",
                "multiple.assignments@example.com",
                "89123456",
                Gender.Male
            );
            
            EmployeeCafe activeEmployeeCafe = new EmployeeCafe(
                id: Guid.NewGuid(),
                cafeId: activeCafeId,
                employeeId: employeeId,
                assignedDate: activeAssignedDate
            );
            
            EmployeeCafe inactiveEmployeeCafe = new EmployeeCafe(
                id: Guid.NewGuid(),
                cafeId: inactiveCafeId,
                employeeId: employeeId,
                assignedDate: inactiveAssignedDate
            );
            
            typeof(EmployeeCafe).GetProperty("Cafe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(activeEmployeeCafe, activeCafe);
            
            typeof(EmployeeCafe).GetProperty("Cafe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(inactiveEmployeeCafe, inactiveCafe);
                
            typeof(EmployeeCafe).GetProperty("IsActive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(activeEmployeeCafe, true);
                
            typeof(EmployeeCafe).GetProperty("IsActive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(inactiveEmployeeCafe, false);
                
            typeof(Employee).GetProperty("EmployeeCafes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(employee, new List<EmployeeCafe> { activeEmployeeCafe, inactiveEmployeeCafe });
            
            EmployeeDto dto = mapper.Map<EmployeeDto>(employee);
            
            Assert.Equal(employee.Id, dto.Id);
            Assert.Equal(activeCafeId, dto.CafeId);
            Assert.Equal("Active Cafe", dto.CafeName);
            Assert.Equal((int)Math.Ceiling((DateTime.UtcNow - activeAssignedDate).TotalDays), dto.DaysWorked);
            Assert.True(dto.IsAssignedToCafe);
        }
    }
} 