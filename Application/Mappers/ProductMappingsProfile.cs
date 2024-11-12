using Application.Dtos.Request;
using Application.Dtos.Response;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Commons.Bases.Response;

namespace Application.Mappers
{
    public class ProductMappingsProfile : Profile
    {
        public ProductMappingsProfile()
        {
            CreateMap<ProductsRequestDto, Products>();
            CreateMap<Products, ProductsResponseDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductsVariants != null && src.ProductsVariants.Any() ? src.ProductsVariants.First().Price : 0))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.ProductsVariants != null && src.ProductsVariants.Any() ? src.ProductsVariants.First().SKU : string.Empty))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.ProductsVariants != null && src.ProductsVariants.Any() ? src.ProductsVariants.First().StockQuantity : 0))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.IdCategory))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.IdProductsCategoryNavigation != null ? src.IdProductsCategoryNavigation.CategoryName : string.Empty));

            CreateMap<BaseEntityResponse<Products>, BaseEntityResponse<ProductsResponseDto>>()
                .ReverseMap();
        }
    }
}
