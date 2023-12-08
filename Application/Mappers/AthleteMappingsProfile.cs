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
                .ForMember(dto => dto.StartDate, opt => opt.MapFrom(src => src.AthleteMemberships
                    .Where(m => m.EndDate > DateOnly.FromDateTime(DateTime.Today))
                    .OrderByDescending(m => m.StartDate)
                    .FirstOrDefault().StartDate))
                .ForMember(dto => dto.EndDate, opt => opt.MapFrom(src => src.AthleteMemberships
                    .Where(m => m.EndDate > DateOnly.FromDateTime(DateTime.Today))
                    .OrderByDescending(m => m.EndDate)
                    .FirstOrDefault().EndDate))
                .ForMember(dto => dto.MembershipName, opt => opt.MapFrom(src => src.AthleteMemberships
                    .Where(m => m.EndDate > DateOnly.FromDateTime(DateTime.Today))
                    .OrderByDescending(m => m.EndDate)
                    .FirstOrDefault().IdMembershipNavigation.MembershipName))
                .ForMember(dto => dto.MembershipId, opt => opt.MapFrom(src => src.AthleteMemberships
                    .Where(m => m.EndDate > DateOnly.FromDateTime(DateTime.Today))
                    .OrderByDescending(m => m.EndDate)
                    .FirstOrDefault().IdMembershipNavigation.MembershipId))
                .ReverseMap();

            CreateMap<BaseEntityResponse<Athlete>, BaseEntityResponse<AthleteResponseDto>>()
                .ReverseMap();

            CreateMap<AthleteRequestDto, Athlete>();
        }
    }
}
