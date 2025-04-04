using AutoMapper;
using Business.Entities;
using Service.Commands.Cafes;
using Service.Commands.EmployeeCafes;
using Service.Commands.Employees;
using Service.Queries.Cafes;
using Service.Queries.EmployeeCafes;
using Service.Queries.Employees;
using Utilities;

namespace Service.Mappings
{    
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cafe, CafeDto>()
                .ForMember(dest => dest.EmployeeCount, opt => opt.Ignore());
            
            CreateMap<EmployeeCafe, EmployeeCafeDto>()
                .ForMember(dest => dest.CafeName, opt => opt.MapFrom(src => src.Cafe != null ? src.Cafe.Name : string.Empty))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : string.Empty));

            CreateMap<EmployeeCafeDto, EmployeeCafe>()
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Cafe, opt => opt.Ignore())
                .ConstructUsing(src => new EmployeeCafe(src.Id, src.CafeId, src.EmployeeId, src.AssignedDate));

            CreateMap<CreateCafeCommand, Cafe>()
                .ForMember(dest => dest.EmployeeCafes, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ConstructUsing(src => new Cafe(Guid.NewGuid(), src.Name, src.Description, src.Logo, src.Location));

            CreateMap<UpdateCafeCommand, Cafe>()
                .ForMember(dest => dest.EmployeeCafes, opt => opt.Ignore())
                .ConstructUsing(src => new Cafe(src.Id, src.Name, src.Description, src.Logo, src.Location));

            CreateMap<CreateEmployeeCommand, Employee>()
                .ForMember(dest => dest.EmployeeCafes, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ConstructUsing(src => new Employee(UniqueIdGenerator.GenerateUniqueId(), src.Name, src.EmailAddress, src.Phone, src.Gender));

            CreateMap<UpdateEmployeeCommand, Employee>()
                .ForMember(dest => dest.EmployeeCafes, opt => opt.Ignore())
                .ConstructUsing(src => new Employee(src.Id, src.Name, src.EmailAddress, src.Phone, src.Gender));

            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.AssignedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CafeId, opt => opt.MapFrom<EmployeeCafeIdResolver>())
                .ForMember(dest => dest.CafeName, opt => opt.MapFrom<EmployeeCafeNameResolver>())
                .ForMember(dest => dest.DaysWorked, opt => opt.MapFrom<CalculateDaysWorkedResolver>())
                .ForMember(dest => dest.IsAssignedToCafe, opt => opt.MapFrom(src => 
                    src.EmployeeCafes != null && src.EmployeeCafes.Any(ec => ec.IsActive)));

            CreateMap<AssignEmployeeToCafeCommand, EmployeeCafe>()
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Cafe, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ConstructUsing(src => new EmployeeCafe(Guid.NewGuid(), src.CafeId, src.EmployeeId, src.AssignedDate));

            CreateMap<UpdateEmployeeCafeAssignmentCommand, EmployeeCafe>()
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Cafe, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ConstructUsing(src => new EmployeeCafe(src.Id, src.CafeId, src.EmployeeId, src.AssignedDate));
        }
    }
} 