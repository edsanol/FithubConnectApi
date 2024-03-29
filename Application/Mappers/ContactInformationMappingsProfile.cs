using Application.Dtos.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers
{
    public class ContactInformationMappingsProfile : Profile
    {
        public ContactInformationMappingsProfile()
        {
            CreateMap<ContactInformation, ContactInformationResponseDto>()
                .ReverseMap();
        }
    }
}
