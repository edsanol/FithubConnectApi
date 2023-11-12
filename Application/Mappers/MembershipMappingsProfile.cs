using Application.Dtos.Request;
using Application.Dtos.Response;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Response;

namespace Application.Mappers
{
    public class MembershipMappingsProfile : Profile
    {
        public MembershipMappingsProfile()
        {
            CreateMap<Membership, MembershipResponseDto>()
                .ReverseMap();

            CreateMap<BaseEntityResponse<Membership>, BaseEntityResponse<MembershipResponseDto>>()
                .ReverseMap();

            CreateMap<MembershipRequestDto, Membership>();

            CreateMap<Membership, MembershipSelectResponseDto>()
                .ReverseMap();
        }
    }
}
