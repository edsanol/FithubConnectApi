using Application.Dtos.Request;
using Application.Dtos.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers
{
    public class ProductsCategoryMappingsProfile : Profile
    {
        public ProductsCategoryMappingsProfile()
        {
            CreateMap<CategoryProductsRequestDto, ProductsCategory>();

            CreateMap<ProductsCategory, CategoryProductsResponseDto>();
        }
    }
}
