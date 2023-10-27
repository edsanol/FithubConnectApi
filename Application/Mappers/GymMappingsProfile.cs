using Application.Dtos.Request;
using Application.Dtos.Response;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Response;

namespace Application.Mappers
{
    public class GymMappingsProfile : Profile
    {
        public GymMappingsProfile()
        {
            CreateMap<Gym, GymResponseDto>()
                .ForMember(dest => dest.StateGym, opt => opt.MapFrom(src => src.Status.Equals(true) ? "Activo" : "Inactivo"))
                .ReverseMap();

            CreateMap<BaseEntityResponse<Gym>, BaseEntityResponse<GymResponseDto>>()
                .ReverseMap();

            CreateMap<GymRequestDto, Gym>();

            CreateMap<Gym, GymSelectResponseDto>()
                .ReverseMap();
        }
    }
}
