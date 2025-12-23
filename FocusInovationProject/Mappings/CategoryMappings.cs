using AutoMapper;
using FocusInovationProject.DTOs.CategoryDtos;
using FocusInovationProject.Entities;

namespace FocusInovationProject.Mappings
{
    public class CategoryMappings : Profile
    {
        public CategoryMappings()
        {
            CreateMap<Category, ResultCategoryDto>().ReverseMap();

            CreateMap<Category, UpdateCategoryDto>().ReverseMap();

            CreateMap<Category, CreateCategoryDto>().ReverseMap(); 
        }
    }
}
