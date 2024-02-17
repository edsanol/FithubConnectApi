using Application.Dtos.Request;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers
{
    public class MeasurementProgressMappingsProfile : Profile
    {
        public MeasurementProgressMappingsProfile()
        {
            CreateMap<MeasurementProgressRequestDto, MeasurementsProgress>()
                .ReverseMap();
        }
    }
}
