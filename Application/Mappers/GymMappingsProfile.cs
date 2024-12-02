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
            // Mapeo de Gym a GymResponseDto
            CreateMap<Gym, GymResponseDto>()
                .ForMember(dest => dest.StateGym, opt => opt.MapFrom(src => src.Status.Equals(true) ? "Activo" : "Inactivo"))
                .ForMember(dest => dest.AccessTypes, opt => opt.MapFrom(src => src.GymAccessTypes.Select(gat => new AccessType
                {
                    AccessTypeId = gat.IdAccessTypeNavigation.AccessTypeId,
                    AccessTypeName = gat.IdAccessTypeNavigation.AccessTypeName,
                    Status = gat.IdAccessTypeNavigation.Status
                }).ToList()))
                .ReverseMap();

            // Mapeo de BaseEntityResponse
            CreateMap<BaseEntityResponse<Gym>, BaseEntityResponse<GymResponseDto>>()
                .ReverseMap();

            // Mapeo de GymRequestDto a Gym
            CreateMap<GymRequestDto, Gym>();

            // Mapeo de Gym a GymSelectResponseDto
            CreateMap<Gym, GymSelectResponseDto>()
                .ReverseMap();
        }
    }
}
