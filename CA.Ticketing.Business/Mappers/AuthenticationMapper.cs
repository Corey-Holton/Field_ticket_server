using AutoMapper;
using CA.Ticketing.Business.Services.Authentication.Dto;
using CA.Ticketing.Persistance.Models;

namespace CA.Ticketing.Business.Mappers
{
    public class AuthenticationMapper : Profile
    {
        public AuthenticationMapper()
        {
            CreateMap<CreateCustomerContactLoginDto, ApplicationUser>()
                .ForMember(x => x.UserName, dest => dest.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, UserDto>();

            CreateMap<UpdateUserDto, ApplicationUser>();

            CreateMap<CreateUserDto, ApplicationUser>();

            CreateMap<CreateEmployeeLoginDto, ApplicationUser>();

            CreateMap<ApplicationUser, ApplicationUser>()
                .ForMember(x => x.Id, dest => dest.Ignore());
        }
    }
}
