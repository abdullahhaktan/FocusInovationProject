using AutoMapper;
using FocusInovationProject.DTOs.ProductDtos;
using FocusInovationProject.Entities;

namespace FocusInovationProject.Mappings
{
    public class ProductMappings : Profile
    {
        public ProductMappings()
        {
            CreateMap<Product, ResultProductDto>()
                .ForMember(dest => dest.SRC, opt => opt.MapFrom(src => src.IMAGE_SRC))
                .ReverseMap();

            CreateMap<Product, UpdateProductDto>().ReverseMap();

            CreateMap<Product, CreateProductDto>().ReverseMap();
        }
    }
}
