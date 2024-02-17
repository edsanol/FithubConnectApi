using Application.Dtos.Request;
using Application.Dtos.Response;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Response;

namespace Application.Mappers
{
    public class MeasurementProgressMappingsProfile : Profile
    {
        public MeasurementProgressMappingsProfile()
        {
            CreateMap<MeasurementProgressRequestDto, MeasurementsProgress>()
                .ReverseMap();

            CreateMap<MeasurementsProgress, MeasurementProgressResponseDto>()
                .ReverseMap();

            CreateMap<BaseEntityResponse<MeasurementsProgress>, BaseEntityResponse<MeasurementProgressResponseDto>>()
                .ReverseMap();

            CreateMap<DashboardGraphicsResponse, DashboardGraphicsResponseDto>()
                .ReverseMap();
        }
    }
}
