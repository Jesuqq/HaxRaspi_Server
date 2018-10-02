using AutoMapper;
using HaxRaspi_Server.Dtos;
using HaxRaspi_Server.Entities;

namespace HaxRaspi_Server.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Interface, InterfaceDto>();
            CreateMap<InterfaceDto, Interface>();
        }
    }
}