using AccessData.Entities;
using AutoMapper;
using Common.Classes.BussinesLogic;

namespace App.Config.Dependencies
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<Vehicle, VehicleDTO>().ReverseMap();
        }
    }
}