using Application.Dtos.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers
{
    public class AccessTypeMappingsProfile : Profile
    {
        public AccessTypeMappingsProfile()
        {
            CreateMap<AccessType, AccessTypeResponseDto>()
                .ReverseMap();
        }
    }
}
