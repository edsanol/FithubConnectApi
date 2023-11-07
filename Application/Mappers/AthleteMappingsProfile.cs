using Application.Dtos.Request;
using Application.Dtos.Response;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Response;

namespace Application.Mappers
{
    public class AthleteMappingsProfile : Profile
    {
        public AthleteMappingsProfile()
        {
            CreateMap<Athlete, AthleteResponseDto>()
                .ForMember(dest => dest.StateAthlete, opt => opt.MapFrom(src => src.Status.Equals(true) ? "Activo" : "Inactivo"))
                .ReverseMap();

            CreateMap<BaseEntityResponse<Athlete>, BaseEntityResponse<AthleteResponseDto>>()
                .ReverseMap();

            CreateMap<AthleteRequestDto, Athlete>();
        }
    }
}
