using AutoMapper;
using Business.Entities;
using Service.Commands.Cafes;
using Service.Commands.EmployeeCafes;
using Service.Commands.Employees;
using Service.Queries.Cafes;
using Service.Queries.EmployeeCafes;
using Service.Queries.Employees;

namespace Service.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCafeCommand, Cafe>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateCafeCommand, Cafe>();

            CreateMap<Cafe, CafeDto>();

            CreateMap<CreateEmployeeCommand, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateEmployeeCommand, Employee>();

            CreateMap<Employee, EmployeeDto>();

            CreateMap<AssignEmployeeToCafeCommand, EmployeeCafe>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateEmployeeCafeAssignmentCommand, EmployeeCafe>();

            CreateMap<EmployeeCafe, EmployeeCafeDto>()
                .ForMember(dest => dest.CafeName, opt => opt.MapFrom(src => src.Cafe != null ? src.Cafe.Name : string.Empty))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : string.Empty));
        }
    }
} 