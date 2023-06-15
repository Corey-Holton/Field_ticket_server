using AutoMapper;
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

            CreateMap<EmployeeDto, Employee>();
        }
    }
}
