using AutoMapper;
using Business.Entities;
using Service.Commands.Cafes;
using Service.Commands.Employees;
using Service.Commands.EmployeeCafes;
using Service.Queries.Cafes;
using Service.Queries.Employees;
using Service.Queries.EmployeeCafes;
using Utilities;

namespace Service.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cafe, CafeDto>();
            CreateMap<EmployeeCafe, EmployeeCafeDto>();

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
                .ForMember(dest => dest.JoinedDate, opt => opt.Ignore());

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