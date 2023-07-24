using AutoMapper;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class EmployeesMapper : Profile
    {
        public EmployeesMapper()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(x => x.JobTitleDisplay, dest => dest.MapFrom(src => src.JobTitle.GetJobTitle()));

            CreateMap<Employee, EmployeeDetailsDto>()
                .IncludeBase<Employee, EmployeeDto>()
                .ForMember(x => x.HasLogin, dest => dest.MapFrom(src => src.ApplicationUser != null))
                .ForMember(x => x.Username, dest => dest.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.UserName : string.Empty));

            CreateMap<EmployeeDto, Employee>()
                .ForMember(x => x.ApplicationUser, dest => dest.Ignore());

            CreateMap<EmployeeDetailsDto, Employee>()
                .IncludeBase<EmployeeDto, Employee>();

            CreateMap<(Employee Employee, AddEmployeeLoginDto AddEmployeeLoginModel), CreateEmployeeLoginDto>()
                .ForMember(x => x.Id, dest => dest.MapFrom(src => src.Employee.Id))
                .ForMember(x => x.FirstName, dest => dest.MapFrom(src => src.Employee.FirstName))
                .ForMember(x => x.LastName, dest => dest.MapFrom(src => src.Employee.LastName))
                .ForMember(x => x.Username, dest => dest.MapFrom(src => src.AddEmployeeLoginModel.Username))
                .ForMember(x => x.Password, dest => dest.MapFrom(src => src.AddEmployeeLoginModel.Password));

            CreateMap<Employee, EmployeeDateDto>();
        }
    }
}
