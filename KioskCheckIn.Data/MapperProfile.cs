using AutoMapper;
using KioskCheckIn.API.DTO;
using KioskCheckIn.Data.Models;

namespace KioskCheckInAPI
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<VisitorDTO, Visitor>().ReverseMap();
        }
    }
}
