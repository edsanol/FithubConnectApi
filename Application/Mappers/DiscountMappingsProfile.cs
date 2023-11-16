using Application.Dtos.Request;
using Application.Dtos.Response;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Response;

namespace Application.Mappers
{
    public class DiscountMappingsProfile : Profile
    {
        public DiscountMappingsProfile()
        {
            CreateMap<Discount, DiscountResponseDto>()
                .ReverseMap();

            CreateMap<BaseEntityResponse<Discount>, BaseEntityResponse<DiscountResponseDto>>()
                .ReverseMap();

            CreateMap<DiscountRequestDto, Discount>();

            CreateMap<Discount, DiscountSelectResponseDto>()
                .ReverseMap();
        }
    }
}
