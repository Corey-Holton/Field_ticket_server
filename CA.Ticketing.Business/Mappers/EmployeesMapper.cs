using AutoMapper;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Business.Services.Employees.Dto;
using CA.Ticketing.Common.Enums;
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
                .ForMember(x => x.HasLogin, dest => dest.MapFrom(src => src.ApplicationUser != null));

            CreateMap<EmployeeDetailsDto, Employee>();

            CreateMap<(Employee Employee, AddEmployeeLoginDto AddEmployeeLoginModel), CreateEmployeeLoginDto>()
                .ForMember(x => x.EmployeeId, dest => dest.MapFrom(src => src.Employee.Id))
                .ForMember(x => x.FirstName, dest => dest.MapFrom(src => src.Employee.FirstName))
                .ForMember(x => x.LastName, dest => dest.MapFrom(src => src.Employee.LastName))
                .ForMember(x => x.UserName, dest => dest.MapFrom(src => src.AddEmployeeLoginModel.Username))
                .ForMember(x => x.Password, dest => dest.MapFrom(src => src.AddEmployeeLoginModel.Password))
                .ForMember(x => x.TicketIdentifier, dest => dest.MapFrom(src => src.AddEmployeeLoginModel.TicketIdentifier));

            CreateMap<Employee, EmployeeDateDto>();
        }
    }
}
